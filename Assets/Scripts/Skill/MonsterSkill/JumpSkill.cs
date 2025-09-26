using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static Enums;

public class JumpSkill : IBasicSkill
{
    //public SkillData skillData { get; set; }
    GameObject weaponObj;
    MonsterWeapon Weapon;
    //protected CancellationTokenSource JumpCTS;

    //public override SKILL_ID_TYPE SkILL_Id => SKILL_ID_TYPE.Jump;
    float speed;
    public override void Init(Character_Base _user) 
    {
        CHARECTER = (Monster_Base)_user;
        speed = (float)CHARECTER.StatusData[Enums.CHARACTER_STATUS.Speed];
        //EffectAddData(CHARECTER.transform);
        base.Init(_user);
    }
    public override void OnTrigger(Character_Base _defender) 
    {
        AttackOn(_defender).Forget();
    }

    private async UniTask AttackOn(Character_Base _defender)
    {
        Vector3 start = HitObject.transform.position;
        float elapsed = 0f;

        Vector3 _targetPos = _defender.transform.position;

        Vector3 horizontal = _targetPos - start;
        float distance = horizontal.magnitude;
        Vector3 direction = horizontal.normalized;

        float Throuheight = 5.0f;
        float moveSpeed = speed;
        float moveTime = Mathf.Max(distance / moveSpeed, 0.2f);

        CHARECTER.isJump = true;

        while (elapsed < moveTime)
        {
            elapsed += Time.deltaTime;
            float time = elapsed / moveTime;

            Vector3 currentPos = start + direction * distance * time;

            float parabola = 4 * Throuheight * time * (1 - time);
            currentPos.y = Mathf.Lerp(start.y, _targetPos.y, time) + parabola;
            //currentPos.y = start.y + parabola;
            //rd.MovePosition(rd.position + currentPos);
            //rd.MovePosition(currentPos);
            CHARECTER.transform.position = currentPos;

            await UniTask.Yield();
        }

        CHARECTER.isJump = false;
        SkillOn(_defender);
    }

    protected override async UniTask DistanseCheckAsync(CancellationTokenSource _token, Character_Base _defender)
    {

        try
        {
            if (RangeAttackDistanseCheck(HitObject.transform.position, _defender.transform.position, skillData.range))
            {
                GameShard.Instance.BattleManager.DamageCheck(CHARECTER, _defender, skillData);
            }
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f), cancellationToken: _token.Token);//, cancellationToken: _token.Token
        }
        catch (OperationCanceledException)
        {
            BuffCheck();

        }

       


        //Vector3 start = monster.transform.position;
        //Vector3 targetPos = _target.transform.position;
        ////targetPos.y = start.y; 
        ////targetPos.z = start.z; 

        //Vector3 dashDirection = (targetPos - start).normalized;
        //dashDirection.z = 0f;
        //dashDirection.y = 0f;

        ////if (dashDirection.x != 0f)
        ////{
        ////    float angleY = dashDirection.x > 0f ? 90f : 180f;
        ////    monster.transform.rotation = Quaternion.Euler(0f, angleY, 0f);
        ////}

        //float elapsed = 0f;

        //while (elapsed < dashDuration)
        //{
        //    if (token.IsCancellationRequested) return;

        //    elapsed += Time.deltaTime;
        //    float moveStep = dashSpeed * Time.deltaTime;

        //    Vector3 move = dashDirection * moveStep;
        //    move.z = 0f;

        //    monster.transform.position += move;

        //    //if (dist <= (float)State[MONSTER_STATE.Attack_Range])

        //    if (AttackDistanseCheck(monster.transform.position, _target.transform.position, 2.0f))
        //    {
        //        GameShard.Instance.BattleManager.DamageCheck(monster, _target);

        //        break; // 한번만 공격하고 끝냄 (계속 공격하고 싶으면 break 제거)
        //    }

        //    Vector3 toTargetNow = _target.transform.position - monster.transform.position;
        //    toTargetNow.y = 0; // 높이 무시

        //    // 방향이 반대가 되었다면 break (dot product < 0)
        //    if (Vector3.Dot(dashDirection, toTargetNow.normalized) < 0f)
        //    {
        //        break;
        //    }
        //    await UniTask.Yield();
        //}


    }

   


    public override void OnUpdate() { }
    public override void TriggerOut() { }
}
