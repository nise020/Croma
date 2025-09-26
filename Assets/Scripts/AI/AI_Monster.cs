using System;
using UnityEngine;
using static Enums;

public class AI_Monster : AI_Base
{
    protected NomalMonster Monster { get; set; }
    public override void Initialize(Character_Base _Character) => Monster = _Character as NomalMonster;
    
    private Player TargetPlayer { get; set; } = null;
    public Action<bool> AttackEvent { get; set; } = null;
    //private bool TargetAlive { get; set; } = false;
    private float ChaseTimer { get; set; } = 0.0f;
    private float ChaseTime { get; set; } = 0.5f;

    public enum AiState //Npc AI ป๓ลย
    {
        Idel,
        Search,
        Move,
        Attack,
        Reset,
    }

    private MONSTER_ATTACK_TYPE ATTACK_TYPE;

    public void AI_Attack_Type(MONSTER_ATTACK_TYPE _type) => ATTACK_TYPE = _type;

    protected AiState monster_Ai = AiState.Idel;



    public override void State()
    {
        if (TargetTrans == null) { return; }

        //if (!TagetAive && monster_Ai != AiState.Search)
        //{
        //    monster_Ai = AiState.Idel;
        //}
        //if (ATTACK_TYPE == MONSTER_ATTACK_TYPE.None)
        //{
        //    Debug.LogError($"preemptive_Type = {ATTACK_TYPE}");
        //}

        switch (monster_Ai)
        {
            case AiState.Idel:
                Idel();
                break;
            case AiState.Search:
                Search();
                break;
            case AiState.Move:
                Move(TargetTrans.position);
                break;
            case AiState.Attack:
                Attack(TargetTrans.position);
                break;
            case AiState.Reset:
                Reset();
                break;
        }
    }
    protected override void Idel()
    {
        float value = Monster.TargetDistanseCheck(TargetTrans.position);

        if (Monster.RangeCheck(value, CHARACTER_DATA.FOVLength) == true)
        {
            if (ATTACK_TYPE == MONSTER_ATTACK_TYPE.Preemptive)
            {
                monster_Ai = AiState.Move;
            }
            else if (ATTACK_TYPE == MONSTER_ATTACK_TYPE.Non_Preemptive)//Non Preemptive_Strike
            {
                //if (Monster.HitStateCheck())
                //{
                //    monster_Ai = AiState.Move;
                //}
                //else
                //{
                //    monster_Ai = AiState.Search;
                //}
                monster_Ai = AiState.Move;
            }
            else 
            {
                monster_Ai = AiState.Move;
            }
            //else if (ATTACK_TYPE == MONSTER_ATTACK_TYPE.C_Preemptive)//Non Preemptive_Strike
            //{
            //    monster_Ai = AiState.Attack;
            //}
        }
        else
        {
            monster_Ai = AiState.Move;
            //if (ATTACK_TYPE == MONSTER_ATTACK_TYPE.C_Preemptive)//Non Preemptive_Strike
            //{
            //    monster_Ai = AiState.Search;
            //}
            return;
        }

    }

    protected override void Search()
    {
        float value = Monster.TargetDistanseCheck(TargetTrans.position);

        if (Monster.RangeCheck(value, CHARACTER_DATA.AttackLength) == true)
        {
            monster_Ai = AiState.Move;
            //if (ATTACK_TYPE == MONSTER_ATTACK_TYPE.Non_Preemptive)//Non Preemptive_Strike
            //{
            //    if (Monster.HitStateCheck())
            //    {
            //        monster_Ai = AiState.Move;
            //    }
            //    else
            //    {
            //        //Monster.Ai_SearchMove();
            //    }
            //}
            //else
            //{
            //    monster_Ai = AiState.Move;
            //}
        }
        //else
        //{
        //    if (ATTACK_TYPE == MONSTER_ATTACK_TYPE.C_Preemptive)
        //    {
        //        Debug.Log($"Not_Move");
        //        return;
        //    }
        //    else
        //    {
        //        //Monster.Ai_SearchMove();
        //        Debug.Log($"npcAi={monster_Ai}");
        //    }
        //}
    }

    protected override void Move(Vector3 _pos)
    {
        float value = Monster.TargetDistanseCheck(TargetTrans.position);

        if (Monster.RangeCheck(value, CHARACTER_DATA.AttackLength) == true)//Preemptive_Strike
        {
            monster_Ai = AiState.Attack;

            //if (Monster.RangeCheck(value, CHARACTER_DATA.AttackLength) == true)//Preemptive_Strike
            //{
            //    monster_Ai = AiState.Attack;
            //    ChaseTimer = 0.0f;
            //}
            //else
            //{
            //    Monster.AI_TargetChase(_pos, value);
            //    Debug.Log($"npcAi={monster_Ai}");
            //}

        }
        else
        {
            Monster.AI_TargetChase(_pos, value);
            //Debug.Log($"npcAi={monster_Ai}");

            //ChaseTimer += Time.deltaTime;

            //if (ChaseTimer >= ChaseTime)
            //{
            //    monster_Ai = AiState.Reset;
            //    ChaseTimer = 0.0f;
            //    return;
            //}
        }
    }

    protected override void Attack(Vector3 _pos)
    {
        if (TargetTrans == null)
        {
            monster_Ai = AiState.Idel;
            return;
        }

        float value = Monster.TargetDistanseCheck(TargetTrans.position);

        if (Monster.RangeCheck(value, CHARACTER_DATA.AttackLength) == true)//Preemptive_Strike                                                                             
        {
            Monster.Ai_Attack(TargetTrans, ANIMATION_PATAMETERS_TYPE.Skill_1);
            monster_Ai = AiState.Move;
            //Debug.Log($"npcAi={monster_Ai}");

            //if (ATTACK_TYPE == MONSTER_ATTACK_TYPE.C_Preemptive)//CPreemptive
            //{
            //    if (Monster.RangeCheck(value, CHARACTER_DATA.AttackLength) == true)//Preemptive_Strike
            //    {
            //        Monster.Ai_Attack(TargetTrans, ANIMATION_PATAMETERS_TYPE.Skill_1, false);
            //        Debug.Log($"npcAi={monster_Ai}");
            //        return;
            //        //npcAi = NpcAiState.Idel;
            //    }
            //    else
            //    {
            //        Monster.Ai_Attack(TargetTrans, ANIMATION_PATAMETERS_TYPE.Skill_2, false);
            //        Debug.Log($"Trap_Attack");
            //        //Character.Ai_TrapAttack(tagetTrs);
            //        return;
            //    }
            //}
            //else 
            //{
            //    if (Monster.RangeCheck(value, CHARACTER_DATA.AttackLength) == true)//Preemptive_Strike
            //    {
            //        Monster.Ai_Attack(TargetTrans, ANIMATION_PATAMETERS_TYPE.Skill_1, false);
            //        monster_Ai = AiState.Move;
            //        Debug.Log($"npcAi={monster_Ai}");
            //    }
            //    else
            //    {
            //        monster_Ai = AiState.Move;
            //    }
            //}


        }
        else
        {
            monster_Ai = AiState.Move;
            //monster_Ai = AiState.Search;
        }
    }

    protected override void Reset()
    {
        if (Monster.StartPointMove())
        {
            monster_Ai = AiState.Idel;
        }

        float value = Monster.TargetDistanseCheck(TargetTrans.position);

        if (Monster.RangeCheck(value, CHARACTER_DATA.AttackLength) == true)//Preemptive_Strike
        {
            monster_Ai = AiState.Move;
        }
        else
        {
            if (Monster.StartPointMove())
            {
                monster_Ai = AiState.Idel;
            }
        }

    }

    public override void DefenderState(bool isDefenderDead)//Reset
    {
        if (isDefenderDead)
        {
            monster_Ai = AiState.Search;

            //TagetAive = false;
            TargetPlayer = null;
            TargetTrans = null;
            Debug.Log($"Player = {TargetPlayer}");
        }

    }
    public bool TargetAlive()
    {
        if (TargetPlayer == null)
        {
            return true;
        }
        return false;
    }
    public override void PlayerDataInit(Player _player)
    {
        TargetPlayer = _player;
        TargetTrans = _player.transform;
    }
}
