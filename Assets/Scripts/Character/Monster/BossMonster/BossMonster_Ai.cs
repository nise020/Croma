using UnityEngine;
using static Enums;
using static Enums.ANIMATION_PATAMETERS_TYPE;

public partial class BossMonster : Monster_Base
{
    ANIMATION_PATAMETERS_TYPE skillPaturnType = ANIMATION_PATAMETERS_TYPE.Idle;

    public bool CoolTimerCheck(MONSTER_COOL_TIMER_TYPE _type)
    {
        if (_type == MONSTER_COOL_TIMER_TYPE.Skill_1)
        {
            if (SkillKeyWardData.TryGetValue(0, out SKILL_ID_TYPE skill))
            {
                return basicSkillSystem.CooltimeCheck(skill);
            }
            return false;
        }
        if (_type == MONSTER_COOL_TIMER_TYPE.Skill_2)
        {
            if (SkillKeyWardData.TryGetValue(1, out SKILL_ID_TYPE skill))
            {
                return basicSkillSystem.CooltimeCheck(skill);
            }
            return false;
        }
        if (_type == MONSTER_COOL_TIMER_TYPE.Skill_3)
        {
            if (SkillKeyWardData.TryGetValue(2, out SKILL_ID_TYPE skill))
            {
                return basicSkillSystem.CooltimeCheck(skill);
            }
            return false;
        }
        else
        {
            Debug.LogError($"_type = {_type}");
            return false;
        }
    }

    public override bool RangeCheck(float _value, CHARACTER_DATA _type)
    {
        float range = (float)(InfoData[_type]);
        if (_value < range)
        {
            return true;
        }
        return false;
    }
    public override void AI_TargetChase(Vector3 _pos, float _distance)
    {

        AnimationParameterUpdate(ANIMATION_PATAMETERS_TYPE.Walk, true);

        Agent.nextPosition = rg.position;
        Agent.SetDestination(_pos);
        //Agent<- navMesh Agent

        Vector3 velocity = Agent.desiredVelocity;
        //rg.MovePosition(rg.position + velocity * Time.deltaTime);
        rg.MovePosition(rg.position + velocity * Time.deltaTime);

        if (velocity.sqrMagnitude > 0.01f)
        {
            Vector3 direction = velocity;
            direction.y = 0f;

            Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
            //targetRotation.y = 0;

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * RotateSpeed);
        }

        //direction.y = 0.0f;
        //transform.rotation = Quaternion.LookRotation(direction);
        StateCheckData[Attack.ToString()] = false;

        if (CharacterSoundPlayer.isPlaying) return;
        if (SoundDatas.TryGetValue((int)InfoData[CHARACTER_DATA.WalkSoundId], out AudioClip clip))
        {
            CharacterSoundPlayer.PlayOneShot(clip);
        }

        //Debug.Log($"{gameObject.transform.position},{MoveSpeed},{_pos}");
        //npcRunStateAnimation(_distance);
    }

    public override void Ai_Attack(Transform _transform, ANIMATION_PATAMETERS_TYPE _type)
    {
        //if (StateCheckData[Walk.ToString()])//Move Reset
        //{
        //    StateCheckData[Walk.ToString()] = false;
        //    AnimationParameterUpdate(ANIMATION_PATAMETERS_TYPE.Walk, StateCheckData[Walk.ToString()]);
        //    //WalkAnimation(monster_Info.WALKSTATE);
        //}
        AnimationParameterUpdate(ANIMATION_PATAMETERS_TYPE.Walk, false);
        if (CharacterSoundPlayer.isPlaying)
        {
            CharacterSoundPlayer.Stop();
        }
        if (StateCheckData[Attack.ToString()] || _transform == null) //On
        {
            //if (AttackDelay)//AttackDelay Type Only
            //{
            //    AttackDelayTimer += Time.deltaTime;
            //    if (AttackDelayTimer > AttackDelrayTime)
            //    {
            //        //AttackDelay = false;
            //        AttackDelayTimer = 0.0f;
            //        StateCheckData[Attack.ToString()] = false;
            //    }
            //}
            return;//overlap Return
        }
        else
        {
            StateCheckData[Attack.ToString()] = true;
            Vector3 direction = _transform.position - transform.position;
            direction.y = 0f;
            transform.rotation = Quaternion.LookRotation(direction);
            PlayerTrans = _transform;
            AttackOn(_type);

           // Vector3 start = transform.position;
           // Vector3 targetPosition = _transform.position;
           // targetPosition.y = start.y;
           // targetPosition.z = start.z;

           // Vector3 dashDirection = (targetPosition - start).normalized;
           // dashDirection.z = 0f;
           // dashDirection.y = 0f;

           // if (dashDirection.x != 0f)
           // {
           //     float angleY = dashDirection.x > 0f ? 90f : 140f;
           //     transform.rotation = Quaternion.Euler(0f, angleY, 0f);
           // }

           //// transform.LookAt(targetPosition);

        }
    }

    protected override void AttackOn(ANIMATION_PATAMETERS_TYPE _type)
    {
        AnimationParameterUpdate(_type, true);

    }
    

}
