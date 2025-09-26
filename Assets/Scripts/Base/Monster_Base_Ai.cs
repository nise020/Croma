using UnityEngine;
using static Enums;
using static Enums.ANIMATION_PATAMETERS_TYPE;

public partial class Monster_Base : Character_Base
{
    public virtual void StateReset() 
    {
        
    }



    //public bool HitCheck()
    //{
    //    //if (monster_Info.HIT_STATE == MONSTER_HIT_STATE.Hit)
    //    //{
    //    //    monster_Info.HIT_STATE = MONSTER_HIT_STATE.NONE;
    //    //    return true;
    //    //}
    //    return false;
    //}
    public void AutoMoveInit(Spawn _spown)
    {
        Spawn = _spown;
        MovePositionList = Spawn.LoadMovePos();
        StartPoint = Spawn.gameObject.transform.position;
    }
    public void PlayerInit(Player _player,Transform _trs)
    {
        Player = _player;
        Creatab = _trs;
    }
    public Transform BodyObjectLoad()
    {
        return gameObject.transform;
    }
    public bool AttackStateCheck()
    {
        return StateCheckData[Attack.ToString()] == false;
    }
    public virtual void AiTagetUpdate(bool _check) { }
    public virtual bool SearchCheck(out UnityEngine.TextCore.Text.Character _target) 
    {
        _target = null;
        return false;
    }

    public virtual float TargetDistanseCheck(Vector3 _pos)
    {
        Vector3 direction = _pos - transform.position;
        direction.y = 0f;

        float distance = direction.magnitude;

        return distance;

    }
    public virtual bool RangeCheck(float _value, CHARACTER_DATA _type)
    {
        return false;
    }
    public virtual void AI_TargetChase(Vector3 _pos, float _distance)
    {

    }

    public virtual void Ai_Attack(Transform _transform,ANIMATION_PATAMETERS_TYPE _type)
    {

    }
    public virtual void Ai_TrapAttack(Transform _transform)
    {

    }
    protected virtual void AutoAttack()
    {

    }
    public virtual void MovePoint(Vector3 _pos)
    {

    }
    public virtual void Ai_SearchMove()
    {

    }
    public virtual bool StartPointMove()
    {
        return false;
    }
}
