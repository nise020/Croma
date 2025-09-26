public static partial class Enums
{
   // public enum PLAYER_COLOR_STATE { Empty, Less, Full }
    
    public enum MOUSE_INPUT_TYPE //예전에는 앞에 sScene을 붙여야 했다
    {
        None,
        Click,      // GetMouseButtonDown
        Hold,       // GetMouseButton
        Release     // GetMouseButtonUp
    }
    public enum ANIMATION_PATAMETERS_TYPE
    {
        None,
        Idle, // 1.0s ~ 1.2s
        Walk,
        Attack,
        Skill_1,//NomalAttack
        Skill_2,//Skill
        Skill_3,//Skill
        Death,
        KnockBack,
        Phase,
        Burst_1,
        Dash,
        //WalkAttack
    }

    //public enum PLAYER_MOVE_STATE
    //{
    //    Idle,
    //    Walking,
    //    Running, 
    //    Jump, 
    //    Dash,
    //    WallJump
    //}

    //public enum PLAYER_ATTACK_STATE
    //{
    //    FrontAttack,
    //    DownAttack,
    //    AirFrontAttack,
    //    DashAttack
    //}

    
    public enum PLAYER_DEBUFF
    {
        None,
        Bind,
        Grab,
        ColorLock,
        SkillLock,
        //SkillCopy,
    }

    public enum PLAYER_BUFF
    {
        None,
        AttackUP,
        HpHeal,
        SpeedUp
    }
}