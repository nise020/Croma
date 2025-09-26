using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using static Enums;
using static Enums.ANIMATION_PATAMETERS_TYPE;

public partial class BossMonster : Monster_Base
{
    
    bool PhaseState = false;
    //[SerializeField] protected GameObject WeaponObj;
    public override void SkillEventOn(int value) //Event, 1 == skill1
    {
        if (SkillKeyWardData.TryGetValue(value, out SKILL_ID_TYPE skill))
        {
            basicSkillSystem.SkillActive(skill, Player);

        }
        else
        {
            Debug.LogError($"{skill} Skill = null");
        }
    }

    //public override void SkillEventEnd(int _skillnumber) //Event <- Attack End Event
    //{
    //    ANIMATION_PATAMETERS_TYPE type = SkillAniamtiontype(_skillnumber);
    //    if (SkillKeyWardData.TryGetValue(_skillnumber, out SKILL_ID_TYPE skill))
    //    {
    //        AnimationParameterUpdate(type, false);
    //        //basicSkillSystem.SkillOff(skill);
    //    }
    //    else
    //    {
    //        Debug.LogError($"{skill} Skill = null");
    //    }

    //}
    public void PhaseAnimation(int value)//Event,Ai
    {
        
        if (value != 0 && PhaseState == false)//on
        {
            CharacterAnimator.SetInteger(ANIMATION_PATAMETERS_TYPE.Phase.ToString(), value);

            AI.IsBurserker = true;
            PhaseState = true;
        }
        else if (value == 0 && PhaseState == true)//Off
        {
            CharacterAnimator.SetInteger(ANIMATION_PATAMETERS_TYPE.Phase.ToString(), value);

            AI.IsBurserker = false;
            PhaseState = false;
        }
 
    }


    public override void DeathEvent()//AnimationEvent
    {
        AnimationParameterUpdate(Death, false);

        //ITEMLists.Count
        int key = UnityEngine.Random.Range(0, 10);

        if (DropItemData.TryGetValue(ITEMLists[key], out GameObject itemObj))
        {
            itemObj.transform.position = transform.position;
            itemObj.SetActive(true);
        }

        StateCheckData[KnockBack.ToString()] = false;

        //Resurrection
        GameShard.Instance.MonsterManager.MonsterAcquired(this);
        GameShard.Instance.MonsterManager.Resurrection(this);
    }

    protected override async UniTaskVoid DistanseCheckAsync(CancellationToken token)
    {
        while (true)
        {
            token.ThrowIfCancellationRequested(); // 중단 요청 처리

            // 거리 계산
            //float dist = Vector3.Distance(WeaponObject.transform.position,PlayerTrans.position);

            Vector3 checkPos = new Vector3();

            if (WeaponObj != null)
            {
                checkPos = WeaponObj.transform.position;
            }
            else 
            {
                checkPos = transform.position;
            }

            if (AttackDistanseCheck(checkPos, PlayerTrans.position, 1.0f))
            {
                //GameShard.Instance.BattleManager.DamageCheck(this, Player);
                //AttackEventReset();
                break; // 한번만 공격하고 끝냄 (계속 공격하고 싶으면 break 제거)
            }
            else 
            {
                Debug.LogError($"공격이 닺지 않는 거리 입니다");
            }

            // 다음 프레임까지 대기
            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }

    }
    protected bool AttackDistanseCheck(Vector3 _weapon,Vector3 target,float _dist) 
    {
        float dist = Vector3.Distance(_weapon, target);

        //if (dist <= (float)State[MONSTER_STATE.Attack_Range])
        if (dist <= _dist)
        {
            return true;
        }
        else 
        {
            return false;
        }
    }
    protected virtual void AttackEventReset()
    {
        
    }
}
    


