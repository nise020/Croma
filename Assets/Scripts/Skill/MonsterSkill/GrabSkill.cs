using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using UnityEngine;
using static Enums;
using UnityEditor;
using UnityEngine.TextCore.Text;

public class GrabSkill : IBasicSkill
{
    //private Monster_Base CHARECTER;
    //public SkillData skillData { get; set; }
    //public override SKILL_ID_TYPE SkILL_Id => SKILL_ID_TYPE.Grab;
    private bool isGrabbing = false;
    protected CancellationTokenSource AttackCTS { get; set; } = null;
   // GameObject WeaponObj;
    GameObject Hand;
    private Transform grabTarget;
    public override void Init(Character_Base _user)
    {
        CHARECTER = (Monster_Base)_user;
        base.Init(_user);
        //EffectAddData(CHARECTER.transform);
    }
    public override void OnUpdate() { }
    public override void OnTrigger(Character_Base _defender) 
    {
        //WeaponObj = CHARECTER.GetWeaponObj();
        Hand = CHARECTER.GetHand();

        Vector3 handToPlayer = Hand.transform.position -_defender.transform.position;// - Hand.transform.position;
        //handToPlayer.z = 0;

        Vector3 monsterToHand = Hand.transform.position - CHARECTER.transform.position;

        bool facingSameDirection = Vector3.Dot(handToPlayer.normalized, monsterToHand.normalized) > 0.95f;

        bool closerThanHand = handToPlayer.magnitude <= monsterToHand.magnitude;

        bool isBetween = facingSameDirection && closerThanHand;

        Vector3 targetPos = _defender.transform.position;
        targetPos.z = 0;

        Vector3 weaponPos = Hand.transform.position;
        weaponPos.z = 0;

        float distanceToPlayerFromHand = Vector3.Distance(weaponPos,targetPos);

        float grabRange = 3f;

        bool isInRange = distanceToPlayerFromHand <= grabRange;

        // 최종 조건
        if (isBetween || isInRange)
        {
            isGrabbing = true;
            grabTarget = Hand.transform;
            SkillOn(_defender);


            //if (AttackCTS != null)
            //{
            //    AttackCTS.Cancel();
            //    AttackCTS.Dispose();
            //}

            //if (_defender is Player) 
            //{
            //    Player player = (Player)_defender;
            //    _defender.IsPaused = true;
            //}

            //TimerOn();

            //AttackCTS = new CancellationTokenSource();
            //DistanseCheckAsync(AttackCTS.Token, _defender).Forget(); // 시작


            
        }

    }
    protected override async UniTask DistanseCheckAsync(CancellationTokenSource token,Character_Base _defender)
    {
        _defender.transform.position = Hand.transform.position;
        try
        {
            while (!token.IsCancellationRequested)
            {
                if (isGrabbing && _defender != null && grabTarget != null)
                {
                    float speed = 30f;
                    Vector3 targetPos = grabTarget.position;
                    targetPos.y = _defender.transform.position.y;

                    // Player를 GrabHand 쪽으로 끌어오기
                    _defender.transform.position = Vector3.MoveTowards(
                        _defender.transform.position,
                        targetPos,
                        Time.deltaTime * speed
                    );


                    if (RangeAttackDistanseCheck(WeaponObj.transform.position, _defender.transform.position, skillData.range))
                    {

                        _defender.IsPaused = false;
                        GameShard.Instance.BattleManager.DamageCheck(CHARECTER, _defender, skillData);
                        grabTarget = null;

                        Debug.Log("도착 완료");

                        AttackCTS.Cancel(); // 이 줄은 아래 finally로 충분히 대체 가능
                        break;
                    }
                }

                await UniTask.Yield(PlayerLoopTiming.Update, token.Token);
            }
        }
        catch (OperationCanceledException) { }
        finally
        {

            isGrabbing = false;
            BuffCheck();
            if (AttackCTS != null)
            {
                AttackCTS.Dispose();
                AttackCTS = null;
            }
        }
    }


    public override void TriggerOut()
    {
    }
}
