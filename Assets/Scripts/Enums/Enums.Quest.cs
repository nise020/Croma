public static partial class Enums
{
    public enum QUEST_DATA
    {
        ID,
        Name,
        Type,
        Description,
        Objectives,
    }

    public enum QUEST_STATE
    {
        NonStarted,
        InProgress,
        Completed,
        Failed,
    }

    public enum QUEST_TYPE
    {
        Kill,
        Collect,
        Survive
    }

    public enum Quest_Type
    {
        Kill = 1,
        NoHitClear = 2,
        NoPotionClear = 3,
        Hmmm = 4,
    }
}
