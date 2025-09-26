using UnityEngine;
using static AI_Monster;
using static Enums;

public class AI_Base 
{

    protected Transform TargetTrans { get; set; } = null;
    public virtual void Initialize(Character_Base _Character) { }
    public void InitTrans(Transform _Trans) => TargetTrans = _Trans;
    protected MONSTER_AI_STATE AIState { get; set; } = MONSTER_AI_STATE.Default;
    public enum AI_STATE { Idle, Move, Attack, }
    public AI_STATE MyAIState { get; set; } = AI_STATE.Idle;
    public virtual void State()
    {
        switch (AIState)
        {
            case MONSTER_AI_STATE.Creat:
                Idel();
                break;
            case MONSTER_AI_STATE.Search:
                Search();
                break;
            case MONSTER_AI_STATE.Move:
                Move(Vector3.zero);
                break;
            case MONSTER_AI_STATE.Attack:
                Attack(Vector3.zero);
                break;
            case MONSTER_AI_STATE.Reset:
                Reset();
                break;
        }
    }
    public virtual void PlayerDataInit(Player _player)
    {

    }
    protected virtual void Idel()
    {
        AIState = MONSTER_AI_STATE.Search;
    }

    protected virtual void Search()
    {
        AIState = MONSTER_AI_STATE.Search;
    }

    protected virtual void Move(Vector3 _pos)
    {
        AIState = MONSTER_AI_STATE.Search;
    }

    protected virtual void Attack(Vector3 _pos)
    {
        AIState = MONSTER_AI_STATE.Search;
    }

    protected virtual void Reset()
    {
        AIState = MONSTER_AI_STATE.Search;
    }
    public virtual void HpUpdate(float _hp, float _maxHp)
    {

    }
    public virtual void DefenderState(bool isDefenderDead)//Reset
    {

    }
}
