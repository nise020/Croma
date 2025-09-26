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
        // �г��� ������ �����ִ� ����Ʈ�� ������, �� ����Ʈ�� ã�Ƽ� �ٽ� �г� ����
        if (questPanel.currentQuestId.HasValue)
        {
            int id = questPanel.currentQuestId.Value;
            var qp = progress.Find(p => p.questData.questId == id);
            if (qp != null)
                questPanel.RefreshQuest(qp);
        }
        else
        {
            // ���� �̷��� ���ٸ� ù ���� �ڵ� ����(��ȣ�� ���� ���ų� �Ѱų�)
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

        RefreshDetailIfSelected();      // �� ����Ʈ ���� �Ŀ��� �� �� �� ��ũ
    }


}
