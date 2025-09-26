using TMPro;
using UnityEngine;
using static LanguageTable;
using static UnityEngine.Rendering.DebugUI;

public class QuestPanel : MonoBehaviour
{
    public int? currentQuestId { get; private set; }

    [SerializeField] private TextMeshProUGUI questName;
    [SerializeField] private TextMeshProUGUI questDescription;
    [SerializeField] private TextMeshProUGUI questObjective;

    [Header("Reward")]
    [SerializeField] private TextMeshProUGUI rewardExp;
    [SerializeField] private TextMeshProUGUI rewardScore;
    [SerializeField] private RewardSlot rewardSlot;
    QuestProgress questProgress;
    private void Awake()
    {
        Shared.Instance.LanguageManager.LanguageChangeEvent += TaxtChange;
    }
    public void TaxtChange()
    {
        var table1 = Shared.Instance.DataManager.Language_Table.Get(questProgress.questData.questName);

        var table2 = Shared.Instance.DataManager.Language_Table.Get(questProgress.questData.description);

        if (Shared.Instance.LanguageManager.lANGUEGE_TYPE == LANGUEGE_TYPE.Ko)
        {
            questName.text = table1.Ko;
            questDescription.text = table2.Ko;
        }
        else if (Shared.Instance.LanguageManager.lANGUEGE_TYPE == LANGUEGE_TYPE.En)
        {
            questName.text = table1.En;
            questDescription.text = table2.En;
        }
    }
    public void RefreshQuest(QuestProgress quest)
    {
        questProgress = quest;

        currentQuestId = quest.questData.questId;
        var data = quest.questData;
        TaxtChange();
        questObjective.text = $"{quest.currentProgress} / {data.targetCount}";


        var reward = Shared.Instance.DataManager.Reward_Table.Get(data.rewardId);
        if (reward == null)
        {
            rewardExp.text = "Exp : 0";
            rewardScore.text = "Score : 0";
            if (rewardSlot) 
            { 
                rewardSlot.Clear(); 
                rewardSlot.gameObject.SetActive(false); 
            }
            return;
        }

        rewardExp.text = $"Exp : {reward.Exp}";
        rewardScore.text = $"Score : {reward.Score}";

        if (reward.ItemId > 0 && reward.ItemCount > 0)
        {
            var rewardItem = Shared.Instance.DataManager.Item_Table.CreateById(reward.ItemId, reward.ItemCount);
            rewardSlot.gameObject.SetActive(true);
            rewardSlot.SetItem(rewardItem, reward.ItemCount);
        }
        else 
        { 
            rewardSlot.Clear(); 
            rewardSlot.gameObject.SetActive(false); 
        }
    }
}
