using static Enums;


public class QuestData
{
    public int questId;
    public int stage;
    public Quest_Type type; // -> Kill(Repeat) / NoHitClear / NoPotionClear / hmmm... 
    public int targetId; // MonsterID or ItemID?
    public int targetCount;
    public int rewardId;
    public int questName;
    public int description; // UI
}
