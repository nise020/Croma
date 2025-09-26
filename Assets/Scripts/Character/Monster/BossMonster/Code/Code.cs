using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static Enums;

public class Code : BossMonster
{
    [SerializeField] GameObject GrabHand;
    //private bool isGrabbing = false;
    //private Transform grabTarget;

    protected override void AttackEventReset()
    {
        PlayerTrans.SetParent(null);
    }

    //public void thunderbolt() 
    //{
    //    basicSkillSystem.SkillActive(MONSTER_SKILL.Thunderbolt, Player);
    //}

    //public override void SkillEventOn(string _skillType)
    //{
    //    basicSkillSystem.SkillActive(MONSTER_SKILL.Grab, Player);

    //}

    //protected override async UniTaskVoid DistanseCheckAsync(CancellationToken token) 
    //{
    //    PlayerTrans.position = GrabHand.transform.position;
    //    try
    //    {
    //        while (!token.IsCancellationRequested)
    //        {
    //            if (isGrabbing && PlayerTrans != null && grabTarget != null)
    //            {
    //                float speed = 30f;
    //                Vector3 targetPos = grabTarget.position;
    //                targetPos.y = PlayerTrans.position.y;

    //                // Player�� GrabHand ������ �������
    //                PlayerTrans.position = Vector3.MoveTowards(
    //                    PlayerTrans.position,
    //                    targetPos,
    //                    Time.deltaTime * speed
    //                );


    //                if (AttackDistanseCheck(WeaponObj.transform.position, PlayerTrans.position,2.0f))
    //                {
    //                    GameShard.Instance.BattleManager.DamageCheck(this, Player);
                        

    //                    Debug.Log("���� �Ϸ�");

    //                    AttackCTS.Cancel(); // �� ���� �Ʒ� finally�� ����� ��ü ����
    //                    break;
    //                }
    //            }

    //            await UniTask.Yield(PlayerLoopTiming.Update, token);
    //        }
    //    }
    //    catch (OperationCanceledException) { }
    //    finally
    //    {

    //        isGrabbing = false;

    //        if (AttackCTS != null)
    //        {
    //            AttackCTS.Dispose();
    //            AttackCTS = null;
    //        }
    //    }
    //}
    //public void DashAttack() //Event
    //{
    //    basicSkillSystem.SkillActive(MONSTER_SKILL.Dash, Player);
    //    //if (DashCTS == null)
    //    //{
    //    //    DashCTS = new CancellationTokenSource();
    //    //    DashForward(Player, (float)State[MONSTER_STATE.Move_Speed], 1.0f, DashCTS.Token).Forget();
    //    //}
    //}
    //private async UniTaskVoid DashForward(Player _target, float dashSpeed, float dashDuration, CancellationToken token)
    //{
       
    //    // ���� ������ ���� ���
    //    Vector3 start = transform.position;
    //    Vector3 targetPos = _target.transform.position;
    //    targetPos.y = start.y; // y ���� (���� �̵���)

    //    Vector3 dashDirection = (targetPos - start).normalized;

    //    if (dashDirection != Vector3.zero)
    //        transform.rotation = Quaternion.LookRotation(dashDirection);

    //    float elapsed = 0f;

    //    while (elapsed < dashDuration)
    //    {
    //        if (token.IsCancellationRequested) return;

    //        elapsed += Time.deltaTime;
    //        float moveStep = dashSpeed * Time.deltaTime;

    //        transform.position += dashDirection * moveStep;

    //        //if (dist <= (float)State[MONSTER_STATE.Attack_Range])

    //        if (AttackDistanseCheck(WeaponObj.transform.position, PlayerTrans.position, 3.0f))
    //        {
    //            GameShard.Instance.BattleManager.DamageCheck(this, Player);

    //            break; // �ѹ��� �����ϰ� ���� (��� �����ϰ� ������ break ����)
    //        }

    //        await UniTask.Yield();
    //    }

    //    DashCTS = null;
    //}
    public override GameObject GetWeaponObj()
    {
        return WeaponObj;
    }
    public override GameObject GetHand()
    {
        return GrabHand;
    }
}
