public static partial class Enums
{
    public enum MONSTER_INFO_SIZE_DATA { Small, Medium, Large, ExtraLarge }
    //public enum MONSTER_DATA
    //{
    //    ID,
    //    Name,
    //    AssetPath,
    //    MapPath,
    //    Size,
    //    AttackType,
    //    State,
    //    MoveType,
    //    ColorType,
    //    Color,
    //    Description,
    //    ModelDescription,
    //    BattleDescription,
    //}

    public enum MONSTER_MOVE_TYPE { Ground, Flying, Fixed }
    //public enum MONSTER_STATE
    //{
    //    HP,
    //    Max_HP,
    //    Attack_Damage,
    //    Attack_Range,
    //    Defence,
    //    Aggro,
    //    Move_Speed,
    //    Projectile_Size,
    //    Projectile_Speed,
    //    KnockBack_Range,
    //    Skill_Cool_1,
    //    Skill_Cool_2,
    //    Buff_Effect,
    //}
    
    public enum MONSTER_ATTACK_TYPE
    {
        None,
        Preemptive,
        Non_Preemptive,
        //C_Preemptive,
    }
    //public enum MONSTER_ATTACK_TYPE 
    //{
    //    None,
    //    Preemptive,
    //    NonPreemptive,
    //    CPreemptive,
    //}
    public enum CHARACTER_ID : int
    {
        Default = 0,
        Player = 100001,
        Bear = 200001,
        Rustdrone = 200002,
        Loop = 200003,
        Code = 200004,
        Bear_Boss = 300001,
        Rustdrone_Boss = 300002,
        Loop_Boss = 300003,
        Code_Boss = 300004,


        //Default,
        //Firerat = 140101,
        //Bluelava = 140201,
        //Redlava = 140301,
        //Firelizard = 140401,
        //Ashslime = 140501,
        //Magumana = 140601,
        //Firewing = 140701,
        //Burntknight = 140801,
        //Blazewraith = 140901,
        //Shardbug_1 = 141001,
        //Moss = 141101,
        //Paint = 141201,
        //Rustdrone_1 = 141301,
        //Venus = 141401,
        //Scrapteddy_1 = 141501,
        //Loop_1 = 141601,
        //code = 141701,
        //Shardbug_2 = 141801,
        //Rustdrone_2 = 141901,
        //Metaball = 142001,
        //Nano = 142101,
        //Scrapteddy_2 = 142201,
        //Loop_2 = 142301,
        //Error_53 = 142401,
        //Reverse = 142501,
        //Echo = 142601,
        //Restleech = 142701,
        //Fadedglowworm = 142801,
        //ForgottenrockStealer = 142901,
        //Canyonrat = 143001,
        //Sil = 143101,
        //Amberscorpion = 143201,
        //Stonebird = 143301,
        //Harvegolem = 143401,
    }
    public enum MONSTER_AI_STATE
    {
        Default,
        Creat,
        Search,
        Move,
        Attack,
        Reset,
    }
    public enum DEBUFF_TYPE
    {
        None,
        Bind,
        ColorLock,
        Move,
    }

    public enum MONSTER_MOVE_STATE
    {
        Idle,
        Move,
        Run,
    }

    //public enum ANIMATION_PATAMETERS_TYPE
    //{
    //    Idle,
    //    Walk,
    //    Skill_1,//NomalAttack
    //    Skill_2,//Skill
    //    Skill_3,//Skill
    //    Death,
    //    Hit,
    //    Phase,
    //}
    public enum MONSTER_COOL_TIMER_TYPE 
    {
        Skill_1,
        Skill_2,
        Skill_3,
    }
    public enum MONSTER_WEAPON_TYPE
    {
        Short,
        Trow,
        Trap,
    }
}