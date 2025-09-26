using System.Collections.Generic;
using UnityEngine;

public class QuestTab : MonoBehaviour 
{
    private List<QuestProgress > progress = new List<QuestProgress>();

    [SerializeField] private List<QuestSlot> questSlots;
    [SerializeField] private QuestPanel questPanel;
    

    private void OnEnable()
    {
        GameShard.Instance.QuestManager.OnQuestProgress += HandleQuestProgress;
        RefreshQuest();
        RefreshDetailIfSelected();
    }

    private void OnDisable()
    {
        if (GameShard.Instance != null && GameShard.Instance.QuestManager != null)
            GameShard.Instance.QuestManager.OnQuestProgress -= HandleQuestProgress;
    }

    private void HandleQuestProgress(QuestProgress changed)
    {
        if (questPanel.currentQuestId.HasValue &&
            questPanel.currentQuestId.Value == changed.questData.questId)
        {
            questPanel.RefreshQuest(changed);
        }       
    }

    private void RefreshDetailIfSelected()
    {
        // 패널이 이전에 보여주던 퀘스트가 있으면, 그 퀘스트를 찾아서 다시 패널 갱신
        if (questPanel.currentQuestId.HasValue)
        {
            int id = questPanel.currentQuestId.Value;
            var qp = progress.Find(p => p.questData.questId == id);
            if (qp != null)
                questPanel.RefreshQuest(qp);
        }
        else
        {
            // 선택 이력이 없다면 첫 슬롯 자동 선택(선호에 따라 끄거나 켜거나)
            if (progress.Count > 0)
                questPanel.RefreshQuest(progress[0]);
        }
    }

    public void RefreshQuest()
    {
        progress = GameShard.Instance.QuestManager.GetActiveQuests();

        for (int i = 0; i < questSlots.Count; i++)
        {
            if (i < progress.Count) 
                questSlots[i].Init(progress[i], questPanel);
            else 
                questSlots[i].Clear();
        }

        RefreshDetailIfSelected();      // ★ 리스트 갱신 후에도 한 번 더 싱크
    }


}
