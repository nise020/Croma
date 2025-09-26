using UnityEngine;
using static AI_Monster;
using static Enums;

public class AI_Boss : AI_Base
{

    BossMonster BOSSMONSTER { get; set; }
    public override void Initialize(Character_Base _Character) => BOSSMONSTER = _Character as BossMonster;

    private Player TargetPlayer { get; set; } = null;
    float HpValue = 0f;
    

    public bool IsBurserker = false;

    CHARACTER_ID Type;//= MONSTER_TYPE.None;


    public override void PlayerDataInit(Player _player)
    {
        TargetPlayer = _player;
        TargetTrans = _player.transform;
    }


    public void BossTypeInit(CHARACTER_ID _type)
    {
        Type = _type;
    }
    public enum BossPattern_State //Npc AI ป๓ลย
    {
        Pattern_1,
        Pattern_2,
        Pattern_3,
        Another,
    }

    public BossPattern_State BossFase = BossPattern_State.Pattern_1;
    public override void State()
    {
        if (TargetTrans == null) { return; }

        switch (MyAIState)
        {
            case AI_STATE.Idle:
                Idel();
                break;
            case AI_STATE.Move:
                Move(TargetTrans.position);
                break;
            case AI_STATE.Attack:
                Attack(TargetTrans.position);
                break;

        }
        //if (PhaseChange)
        //{
        //    if (HpValue <= 100 && HpValue > 70)
        //    {
        //        BOSSMONSTER.PhaseAnimation(1);
        //    }
        //    else if (HpValue <= 70 && HpValue > 40)
        //    {
        //        BOSSMONSTER.PhaseAnimation(2);
        //    }
        //    else if (HpValue <= 40)
        //    {
        //        //BOSSMONSTER.PhaseAnimation(3);
        //        PhaseChange = false;
        //    }
        //    return;
        //}
        //else 
        //{
        //    switch (MyAIState)
        //    {
        //        case AI_STATE.Idle:
        //            Idel();
        //            break;
        //        case AI_STATE.Move:
        //            Move(TargetTrans.position);
        //            break;
        //        case AI_STATE.Attack:
        //            Attack(TargetTrans.position);
        //            break;

        //    }
        //}

    }

    public override void HpUpdate(float _hp, float _maxHp)
    {
        float Hp = (_hp / _maxHp) * 100;
        if (Hp <= 40) 
        {
            //IsBurserker = true;
            BOSSMONSTER.BurserKer();
        }
        HpValue = Hp;
    }

    private void PattenCheck(float _Hp)
    {
        if (_Hp <= 100 && _Hp > 70)
        {
            BossFase = BossPattern_State.Pattern_1;
        }
        else if (_Hp <= 70 && _Hp > 40)
        {
            BossFase = BossPattern_State.Pattern_2;
        }
        else if (_Hp <= 40)
        {
            BossFase = BossPattern_State.Pattern_3;
        }
    }
    protected override void Idel()
    {
        PattenCheck(HpValue);
        //if (Type == CHARACTER_ID.Bear_Boss)
        //{
        //    PattenCheck(HpValue);
        //}
        //else if (Type == CHARACTER_ID.Loop_Boss)
        //{
        //    if (TargetPlayer.Debuff == PLAYER_DEBUFF.Bind) //Debuff State check
        //    {
        //        BossFase = BossPattern_State.Another;
        //    }
        //    else
        //    {
        //        PattenCheck(HpValue);
        //    }
        //}
        //else if (Type == CHARACTER_ID.Code_Boss)
        //{
        //    PattenCheck(HpValue);
        //}
        //else if (Type == CHARACTER_ID.Rustdrone_Boss)
        //{
        //    PattenCheck(HpValue);
        //}

        MyAIState = AI_STATE.Move;
    }



    protected override void Move(Vector3 _pos)
    {
        float value = BOSSMONSTER.TargetDistanseCheck(_pos);

        if (BOSSMONSTER.RangeCheck(value, CHARACTER_DATA.AttackLength) == true)
        {
            MyAIState = AI_STATE.Attack;
        }
        else
        {
            BOSSMONSTER.AI_TargetChase(_pos, 0.0f);
        }
    }

    protected override void Attack(Vector3 _pos)
    {
        //if (BOSSMONSTER.AttackStateCheck() == false) 
        //{
        //    MyAIState = AI_STATE.Idle;
        //    return; 
        //}

        float value = BOSSMONSTER.TargetDistanseCheck(_pos);

        if (BossFase == BossPattern_State.Pattern_1)//100%
        {
            Patten1(value);
        }
        else if (BossFase == BossPattern_State.Pattern_2)//70%
        {
            Patten2(value);
        }
        else if (BossFase == BossPattern_State.Pattern_3)//40%
        {
            Patten3(value);
        }
        //else if (BossFase == BossPattern_State.Another)
        //{
        //    Patten_Another(value);
        //}


    }
    private void Patten1(float value)
    {
        if (BOSSMONSTER.RangeCheck(value, CHARACTER_DATA.AttackLength) == true)
        {
            if (BOSSMONSTER.CoolTimerCheck(MONSTER_COOL_TIMER_TYPE.Skill_1))
            {
                BOSSMONSTER.Ai_Attack(TargetTrans, ANIMATION_PATAMETERS_TYPE.Skill_1);
                MyAIState = AI_STATE.Idle;
            }
        }
        else
        {
            MyAIState = AI_STATE.Move;
        }
    }
    private void Patten2(float value)
    {
        if (BOSSMONSTER.RangeCheck(value, CHARACTER_DATA.AttackLength) == true)
        {
            if (BOSSMONSTER.CoolTimerCheck(MONSTER_COOL_TIMER_TYPE.Skill_2))
            {
                BOSSMONSTER.Ai_Attack(TargetTrans, ANIMATION_PATAMETERS_TYPE.Skill_2);

                //false = Apply Skill Cool Timer
                //true = Not Apply Skill Cool Timer
            }
            else
            {
                BOSSMONSTER.Ai_Attack(TargetTrans, ANIMATION_PATAMETERS_TYPE.Skill_1);
            }
            MyAIState = AI_STATE.Idle;
        }
        else
        {
            MyAIState = AI_STATE.Move;
        }
    }
    private void Patten3(float value)
    {
        if (Type == CHARACTER_ID.Code)
        {
            if (BOSSMONSTER.RangeCheck(value, CHARACTER_DATA.AttackLength) == true)
            {
                if (BOSSMONSTER.CoolTimerCheck(MONSTER_COOL_TIMER_TYPE.Skill_3))
                {
                    //Skill_3 Count Check
                    //if
                    BOSSMONSTER.Ai_Attack(TargetTrans, ANIMATION_PATAMETERS_TYPE.Skill_3);
                    //else
                    
                    MyAIState = AI_STATE.Idle;
                }
                else
                {
                    if (BOSSMONSTER.CoolTimerCheck(MONSTER_COOL_TIMER_TYPE.Skill_2))
                    {
                        BOSSMONSTER.Ai_Attack(TargetTrans, ANIMATION_PATAMETERS_TYPE.Skill_2);
                        //false = Apply Skill Cool Timer
                        //true = Not Apply Skill Cool Timer
                    }
                    else
                    {
                        BOSSMONSTER.Ai_Attack(TargetTrans, ANIMATION_PATAMETERS_TYPE.Skill_1);
                    }

                    //if (TargetPlayer.HitStateCheck(Type)) //+ Boss type
                    //{
                    //    //Boss type

                    //    //Debuff
                    //}
                    MyAIState = AI_STATE.Idle;
                }

            }
            else
            {
                MyAIState = AI_STATE.Move;
            }
        }
        else if (Type == CHARACTER_ID.Rustdrone_Boss)
        {
            if (BOSSMONSTER.CoolTimerCheck(MONSTER_COOL_TIMER_TYPE.Skill_3) && TargetPlayer.colorSlotUI.ColorSlotState())//TargetPlayer Color
            {
                BOSSMONSTER.Ai_Attack(TargetTrans, ANIMATION_PATAMETERS_TYPE.Skill_3);

                MyAIState = AI_STATE.Idle;
            }
            else
            {
                if (BOSSMONSTER.RangeCheck(value, CHARACTER_DATA.AttackLength) == true)
                {
                    if (BOSSMONSTER.CoolTimerCheck(MONSTER_COOL_TIMER_TYPE.Skill_2))
                    {
                        BOSSMONSTER.Ai_Attack(TargetTrans, ANIMATION_PATAMETERS_TYPE.Skill_2);
                        //false = Apply Skill Cool Timer
                        //true = Not Apply Skill Cool Timer

                    }
                    else
                    {
                        BOSSMONSTER.Ai_Attack(TargetTrans, ANIMATION_PATAMETERS_TYPE.Skill_1);
                    }
                    MyAIState = AI_STATE.Idle;
                }
                else
                {
                    MyAIState = AI_STATE.Move;
                }
            }
        }
        else if (Type == CHARACTER_ID.Loop_Boss)
        {
            if (BOSSMONSTER.RangeCheck(value, CHARACTER_DATA.AttackLength) == true)
            {
                if (BOSSMONSTER.CoolTimerCheck(MONSTER_COOL_TIMER_TYPE.Skill_3) && TargetPlayer.colorSlotUI.ColorSlotState())
                {
                    //PlayerColor Check
                    BOSSMONSTER.Ai_Attack(TargetTrans, ANIMATION_PATAMETERS_TYPE.Skill_3);
                    //Color Drain
                    MyAIState = AI_STATE.Idle;
                }
                else
                {
                    if (BOSSMONSTER.CoolTimerCheck(MONSTER_COOL_TIMER_TYPE.Skill_2))
                    {
                        BOSSMONSTER.Ai_Attack(TargetTrans, ANIMATION_PATAMETERS_TYPE.Skill_2);
                    }
                    else
                    {
                        BOSSMONSTER.Ai_Attack(TargetTrans, ANIMATION_PATAMETERS_TYPE.Skill_1);
                    }
                    MyAIState = AI_STATE.Idle;
                }
            }
            else
            {
                MyAIState = AI_STATE.Move;
            }
        }
        else
        {
            if (BOSSMONSTER.CoolTimerCheck(MONSTER_COOL_TIMER_TYPE.Skill_3))
            {
                BOSSMONSTER.Ai_Attack(TargetTrans, ANIMATION_PATAMETERS_TYPE.Skill_3);
                //false = Apply Skill Cool Timer
                //true = Not Apply Skill Cool Timer

                //Player Color Drain <- if

                MyAIState = AI_STATE.Idle;
            }
            else
            {
                if (BOSSMONSTER.RangeCheck(value, CHARACTER_DATA.AttackLength) == true)
                {
                    if (BOSSMONSTER.CoolTimerCheck(MONSTER_COOL_TIMER_TYPE.Skill_2))
                    {
                        BOSSMONSTER.Ai_Attack(TargetTrans, ANIMATION_PATAMETERS_TYPE.Skill_2);
                        //false = Apply Skill Cool Timer
                        //true = Not Apply Skill Cool Timer
                    }
                    else
                    {
                        BOSSMONSTER.Ai_Attack(TargetTrans, ANIMATION_PATAMETERS_TYPE.Skill_1);
                    }
                    MyAIState = AI_STATE.Idle;
                }
                else
                {
                    MyAIState = AI_STATE.Move;
                }
            }
        }

    }
    //private void Patten_Another(float value)
    //{
    //    if (BOSSMONSTER.CoolTimerCheck(MONSTER_COOL_TIMER_TYPE.Skill_3))
    //    {

    //    }
    //    else
    //    {
    //        if (BOSSMONSTER.RangeCheck(value, CHARACTER_DATA.AttackLength) == true)
    //        {
    //            BOSSMONSTER.Ai_Attack(TargetTrans, ANIMATION_PATAMETERS_TYPE.Skill_1);
                
    //            MyAIState = AI_STATE.Idle;
    //        }
    //        else
    //        {
    //            MyAIState = AI_STATE.Move;
    //        }
    //    }
    //}
    protected override void Reset()
    {

    }

    public override void DefenderState(bool isDefenderDead)//Reset
    {
        if (isDefenderDead)
        {
            MyAIState = AI_STATE.Idle;

            //TagetAive = false;
            TargetPlayer = null;
            TargetTrans = null;
            Debug.Log($"Player = {TargetPlayer}");
        }

    }
}
