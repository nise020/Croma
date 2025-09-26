using static Enums;

public class QuestProgress
{
    public QuestData questData;
    public int currentProgress;
    public QUEST_STATE state;

    public bool rewardGiven;

    public QuestProgress(QuestData data)
    {
        this.questData = data;
        state = QUEST_STATE.InProgress;
        currentProgress = 0;
        rewardGiven = false;
    }

    public bool IsComplete()
    {
        return currentProgress >= questData.targetCount;
    }
}
