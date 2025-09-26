using UnityEngine;
using static Enums;
using static Enums.ANIMATION_PATAMETERS_TYPE;

public partial class NomalMonster : Monster_Base
{
    //public override void StatusUpdate(float _hp)
    //{
    //    base.StatusUpdate(_hp);
    //    monster_Info.HIT_STATE = MONSTER_HIT_STATE.Hit;
    //}

    //public override bool HitStateCheck()
    //{
    //    if (monster_Info.HIT_STATE == MONSTER_HIT_STATE.Hit)
    //    {
    //        monster_Info.HIT_STATE = MONSTER_HIT_STATE.NONE;
    //        return true;
    //    }
    //    return false;
    //}

    public override void AiTagetUpdate(bool _check)//Taget State update
    {
        AI.DefenderState(_check);
    }

    //public void AiUpdate(Player _player)
    //{

    //    //AI.PlayerDataInit(_player);
    //}

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
        StateCheckData[Attack.ToString()] = false;
        //Debug.Log($"{name} onNavMesh:{Agent.isOnNavMesh} pos:{transform.position}");
        if (CharacterSoundPlayer.isPlaying) return;

        if (SoundDatas.TryGetValue((int)InfoData[CHARACTER_DATA.WalkSoundId], out AudioClip clip))
        {
            CharacterSoundPlayer.PlayOneShot(clip);
        }


        //Vector3 direction = _pos - transform.position;
        //direction.y = 0;

        //Quaternion targetRotation = Quaternion.LookRotation(direction);
        //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * RotateSpeed);

        //Vector3 start = transform.position;
        //Vector3 targetPosition = _pos;
        //targetPosition.y = start.y;
        //targetPosition.z = start.z;

        //Vector3 dashDirection = (targetPosition - start).normalized;
        //dashDirection.z = 0f;
        //dashDirection.y = 0f;

        //if (dashDirection.x != 0f)
        //{
        //    float angleY = dashDirection.x > 0f ? 90f : 280f;
        //    transform.rotation = Quaternion.Euler(0f, angleY, 0f);
        //}
        //Vector3 direction = _pos - transform.position;
        //direction.y = 0f;
        //transform.rotation = Quaternion.LookRotation(direction);

        //Vector3 moveDir = (direction + WeightPos * 0.5f).normalized;
        ////gameObject.transform.position += moveDir * (float)StatusData[CHARACTER_STATUS.Speed] * Time.deltaTime;

        //float speed = StatusData[CHARACTER_STATUS.Speed];
        //float speedMultiplier = 1.0f;

        //Debug.Log($"{gameObject.transform.position},{MoveSpeed},{_pos}");
        //npcRunStateAnimation(_distance);
    }

    public override void Ai_Attack(Transform _transform,ANIMATION_PATAMETERS_TYPE _type)
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
            if (AttackDelay)//AttackDelay Type Only
            {
                AttackDelayTimer += Time.deltaTime;
                if (AttackDelayTimer > AttackDelrayTime)
                {
                    //AttackDelay = false;
                    AttackDelayTimer = 0.0f;
                    StateCheckData[Attack.ToString()] = false;
                }
            }
            return;//overlap Return
        }
        else
        {
            Vector3 direction = _transform.position - transform.position;
            direction.y = 0f;
            transform.rotation = Quaternion.LookRotation(direction);
            PlayerTrans = _transform;
            AttackOn(_type);


        }
    }


    protected override void AttackOn(ANIMATION_PATAMETERS_TYPE _type)
    {
        if (SkillKeyWardData.TryGetValue(0, out SKILL_ID_TYPE skill))
        {
            if (basicSkillSystem.CooltimeCheck(skill))
            {
                AnimationParameterUpdate(_type, true);
                StateCheckData[Attack.ToString()] = true;
            }
            else
            {
                return;
            }
        }
    }


    public override bool StartPointMove()
    {
        float dist = Vector3.Distance(StartPoint, transform.position);

        if (dist < 0.1f)
        {
            //StateCheckData[Walk.ToString()] = false;
            AnimationParameterUpdate(ANIMATION_PATAMETERS_TYPE.Walk, false);
            //WalkAnimation(monster_Info.WALKSTATE);
            return true;
        }
        else
        {
            Vector3 disTance = (StartPoint - transform.position);

            disTance.y = 0.0f;

            Quaternion rotation = Quaternion.LookRotation(disTance.normalized);

            transform.position += disTance.normalized * (float)StatusData[CHARACTER_STATUS.Speed] * Time.deltaTime;

            transform.rotation = Quaternion.Slerp(transform.rotation,
                rotation, Time.deltaTime * RotateSpeed);

            //StateCheckData[Walk.ToString()] = true;
            AnimationParameterUpdate(ANIMATION_PATAMETERS_TYPE.Walk, true);
            //WalkAnimation(monster_Info.WALKSTATE);    
            return false;
        }
    }
    public override bool RangeCheck(float _value, CHARACTER_DATA _type)
    {
        float range = (float)(InfoData[_type]);
        if (_value <= range) 
        {
            return true;
        }
        return false;
    }

    public void NextMovePoint()
    {
        SlotCount = (SlotCount + 1) % MovePositionList.Count;
    }

    public override void Ai_SearchMove()
    {
        if (MovePosition == Vector3.zero)
        {
            MovePosition = MovePositionList[SlotCount];
        }

        MovePoint(MovePosition);
    }


    public override void MovePoint(Vector3 _pos)//Search
    {
        float dist = TargetDistanseCheck(_pos);

        if (dist < 0.1f)
        {
            //transform.position = _pos;
            //Timer
            StopDelrayTimer += Time.deltaTime;
            if (StopDelrayTimer < StopDelrayTime)
            {
                AnimationParameterUpdate(ANIMATION_PATAMETERS_TYPE.Walk, false);
                //WalkAnimation(WALK_STATE.eWALK_OFF);
                MoveDelrayTimer = 0.0f;
                return;
            }
            //StateCheckData[Walk.ToString()] = false;
            AnimationParameterUpdate(ANIMATION_PATAMETERS_TYPE.Walk, false);

            MovePosition = Vector3.zero;
            NextMovePoint();
            StopDelrayTimer = 0.0f;
        }
        else
        {
            Vector3 disTance = (_pos - transform.position);

            disTance.y = 0.0f;
            //disTance.z = 0.0f;

            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, 10f))
            {
                if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer(LAYER_TYPE.Wall.ToString()))
                {
                    MoveDelrayTimer += Time.deltaTime;
                    if (MoveDelrayTimer < MoveDelrayTime)
                    {
                        AnimationParameterUpdate(ANIMATION_PATAMETERS_TYPE.Walk, false);
                        StopDelrayTimer = 0.0f;
                        return;
                    }
                    MovePosition = Vector3.zero;
                    NextMovePoint();
                    MoveDelrayTimer = 0.0f;
                }
                else
                {
                    autoMove(disTance);

                }
            }
            else
            {
                autoMove(disTance);
            }
        }
        AnimationParameterUpdate(ANIMATION_PATAMETERS_TYPE.Walk, true);
    }

    private void autoMove(Vector3 _disTance)
    {
        //StateCheckData[Walk.ToString()] = true;

        transform.position += _disTance.normalized * (float)StatusData[CHARACTER_STATUS.Speed] * Time.deltaTime;

        //Debug.Log($"Speed = {(float)State[MONSTER_STATE.Move_Speed]}");
        
        //Quaternion rotation = Quaternion.LookRotation(_disTance.normalized);
        
        //transform.rotation = Quaternion.Slerp(transform.rotation,
        //    rotation, Time.deltaTime * RotateSpeed);

        //Vector3 start = transform.position;
        //Vector3 targetPosition = _disTance;
        //targetPosition.y = start.y;
        //targetPosition.z = start.z;

        //Vector3 dashDirection = (targetPosition - start).normalized;
        //dashDirection.z = 0f;
        //dashDirection.y = 0f;

        if (_disTance.x != 0f)
        {
            float angleY = _disTance.x > 0f ? 90f : 280f;
            transform.rotation = Quaternion.Euler(0f, angleY, 0f);
        }
    }


    //public override bool SearchCheck(Character_Base _target)
    //{
    //    //float radius = 8f;
    //    //float fieldOfView = 90f;
    //    _target = Shared.MonsterManager.monsterSearch(gameObject, radius);

    //    if (_target == null)
    //    {
    //        Debug.LogError($"_monster = {_target}");
    //        return false;
    //    }
    //    else
    //    {
    //        Vector3 targetPos = _target.BodyObjectLoad().position;
    //        Vector3 myPos = gameObject.transform.position;

    //        float distance = Vector3.Distance(targetPos, myPos);

    //        if (radius >= distance)
    //        {
    //            return true;
    //        }
    //        else
    //        {
    //            _target = null;
    //            return false;
    //        }
    //    }
    //}
}
