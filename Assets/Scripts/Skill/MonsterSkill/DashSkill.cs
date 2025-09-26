using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static Enums;

public class DashSkill : IBasicSkill
{
    //public SkillData skillData { get; set; }
    //public override SKILL_ID_TYPE SkILL_Id => SKILL_ID_TYPE.Dash;
    //bool IsActive { get; set; } = false;

    //protected CancellationTokenSource DashCTS { get; set; } = null;
    //protected GameObject WeaponObj;
    GameObject Hand;
    // private Transform grabTarget;
    protected float speed;
    public override void Init(Character_Base _user)
    {
        CHARECTER = _user;
        base.Init(_user);
        //WeaponObj = CHARECTER.GetWeaponObj();
        //EffectAddData(CHARECTER.transform);
    }
    public override void OnUpdate() { }
    public override void OnTrigger(Character_Base _defender)
    {
        SkillOn(_defender);
        //if (DashCTS == null)
        //{
        //    speed = (float)CHARECTER.StatusData[CHARACTER_STATUS.Speed];
        //    DashCTS = new CancellationTokenSource();
        //    TimerOn();
        //    DashForward(_defender, speed, 0.3f, DashCTS.Token).Forget();
        //}

    }
    protected override async UniTask DistanseCheckAsync(CancellationTokenSource _token, Character_Base _defender)
    {
        // 시작 지점과 방향 계산
        //Vector3 start = CHARECTER.transform.forward;
        //Vector3 targetPos = CHARECTER.transform.forward * 2;
        HashSet<Transform> damagedEnemies = new HashSet<Transform>();
        int count = skillData.hitCount;
        float elapsed = 0f;
        speed = (float)CHARECTER.StatusData[CHARACTER_STATUS.Speed];
        float dashDuration = 0.3f;


        try
        {
            while (elapsed < dashDuration)
            {
                //if (token.IsCancellationRequested) return;

                elapsed += Time.deltaTime;
                float moveStep = speed * Time.deltaTime;

                Vector3 dashDirection = CHARECTER.transform.forward * 1.5f;
                Vector3 move = dashDirection * moveStep;

                rd.MovePosition(rd.position + move);
                //CHARECTER.transform.position += move;

                //if (dist <= (float)State[MONSTER_STATE.Attack_Range])

                if (FowardAttackDistanseCheck(_defender.transform.position, skillData.range))
                {
                    GameShard.Instance.BattleManager.DamageCheck(CHARECTER, _defender, skillData);

                    break; // 한번만 공격하고 끝냄 (계속 공격하고 싶으면 break 제거)
                }
                //Vector3 toTargetNow = _target.transform.position - CHARECTER.transform.forward;
                //toTargetNow.y = 0; 

                //if (Vector3.Dot(dashDirection, toTargetNow.normalized) < 0f)
                //{
                //    break;
                //}

                await UniTask.Yield();
            }

        }

        catch 
        {
            BuffCheck();
        }

        

        //DashCTS = null;
    }
    //public override bool State()
    //{
    //    return IsActive;
    //}

    public override void TriggerOut()
    {
    }
}
