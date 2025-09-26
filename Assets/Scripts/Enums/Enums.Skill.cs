using UnityEngine;

public static partial class Enums
{
    public enum SKILL_DATA 
    {
        Id,
        Type,
        Value,
        ValueMax,
        Time,
        BuffId,
        Target,
        Count,
        CountMax,
        Icon,
        Prefab,
        Name,
        Dec,
    }

    public enum SKILL_ID_TYPE //: int
    {
        //Player
        None = 0,
        Shot_Type_1 = 1,
        Shot_Type_2 = 2,
        Shot_Type_3 = 3,
        Shot_Type_4 = 4,

        Dash_Type_1 = 11,
        Dash_Type_2 = 12,
        Dash_Type_3 = 13,
        Dash_Type_4 = 14,

        Area_Type_1 = 21,
        Area_Type_2 = 22,
        Area_Type_3 = 23,
        Area_Type_4 = 24,

        Auto_Type_1 = 31,
        Auto_Type_2 = 32,
        Auto_Type_3 = 33,
        Auto_Type_4 = 34,

        Burst_Type_1 = 41,
        Burst_Type_2 = 42,
        Burst_Type_3 = 43,
        Burst_Type_4 = 44,
        //Monster
        //Long,
        //Shoot_Type_2 = 1,

        Dash_Monster_nomal1 = 200001,
        Dash_Monster_nomal2 = 200002,
        Shot_Monster_nomal1 = 200003,
        Jump_Nomal = 200004,
        Shoot_Type_1 = 200005,

        Shot_Monster_Boss1 = 300001,
        Shot_Monster_Boss2 = 300002,
        Jump_Boss = 300003,
        Grab_Nomal = 300004,

        Trow,
        //Charge,

        MinianSummon,
        TentacleSummon,
        Thunderbolt,
        ColorLock,
        ColorSteal,
        SaturationEye,
        PurifyBlade,
        InvisibleShield,
        BrokenLight,
    }

    //public enum MONSTER_SKILL : int
    //{
    //    None,
    //    Shot,
    //    Long,
    //    Grab,
    //    Dash,
    //    Charge,
    //    MinianSummon,
    //    TentacleSummon,
    //    Trow,
    //    Jump,
    //    Thunderbolt,
    //    ColorLock,
    //    ColorSteal,

    //}
}
