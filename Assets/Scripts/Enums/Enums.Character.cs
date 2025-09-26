using UnityEngine;

public static partial class Enums
{
    public enum CHARACTER_CONDITION_STATE { Default, Health, Sick, Death }
    public enum CHARACTER_DATA 
    {
        Id,
        Type,
        StateId,
        BookId,
        FOVLength,
        AttackLength,
        Icon,
        Prefab,
        Name,
        Dec,
        Exp, 
        WalkSoundId,
        AttackSoundId,
    }
    //public enum CHARACTER_TYPE
    //{
    //    None,
    //    Player,
    //    NomalMonster,
    //    BossMonster,
    //}
    public enum CHARACTER_STATUS
    {
        Hp,
        MaxHp,
        Atk,
        Def,
        Speed,
        SkillPoint,
        //TotalExp,
        //StatPoint,
    }
    //public enum LEVELUP_STAT
    //{
    //    SkillPoint,
    //    TotalExp,
    //    StatPoint,
    //}   


    public enum PLAYER_MOVEMENT_STATUS { Default, Idle, Run, Dash, Jump }
    public enum PLAYER_ACTION_STATUS { Default, Attack, Defense, Skill } // Temporary data
    //public enum PLAYER_ANIMATION_STATUS
    //{
    //    Default,
    //    Idle,
    //    Idle1,
    //    Attack1,
    //    Attack2,
    //    Attack3,
    //    KnockBack,
    //    Walk,
    //    Run,
    //    Jump,
    //    Dash,
    //    Dead
    //}
}