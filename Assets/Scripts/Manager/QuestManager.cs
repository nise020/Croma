using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Enums;

public class QuestManager : MonoBehaviour
{
    private readonly List<QuestProgress> activeQuests = new();

    public Action OnAllQuestsCompleted;
    public event Action<QuestProgress> OnQuestProgress;

    // Test
    private bool hitThisStage = false;
    private bool usePotionThisStage = false;

    private int questClearSound = 20016;

    public void CheckAllCompleted()
    {
        if (AllQuestCompleted())
        {
            Debug.Log("Quest All Clear!");
            OnAllQuestsCompleted?.Invoke();
        }
    }


    public void OnStageStarted()
    {
        hitThisStage = false;
        usePotionThisStage = false;
    }

    public void OnStageCleared()
    {
        for (int i = 0;  i < activeQuests.Count; i++)
        {
            var q = activeQuests[i];
            if (q.state != QUEST_STATE.InProgress) continue;

            switch (q.questData.type)
            {
                case Quest_Type.NoHitClear:
                    if (!hitThisStage)
                        CompleteAndReward(q);
                    break;
                case Quest_Type.NoPotionClear:
                    if (!usePotionThisStage)
                        CompleteAndReward(q);
                    break;
            }
        }
        CheckAllCompleted();
    }

    public void NotifyPlayerDamaged()
    {
        hitThisStage = true;

        for (int i = 0; i < activeQuests.Count; i++)
        {
            var q = activeQuests[i];
            if (q.state != QUEST_STATE.InProgress) continue;
            if (q.questData.type == Quest_Type.NoHitClear)
            {
                q.state = QUEST_STATE.Failed;
                OnQuestProgress?.Invoke(q);
            }
        }
    }

    public void NotifyPotionUsed()
    {
        usePotionThisStage = true;

        for (int i = 0; i < activeQuests.Count; i++)
        {
            var q = activeQuests[i];
            if (q.state != QUEST_STATE.InProgress) continue;
            if (q.questData.type == Quest_Type.NoPotionClear)
            {
                q.state = QUEST_STATE.Failed;
                OnQuestProgress?.Invoke(q);
            }
        }
    }

    private void CompleteAndReward(QuestProgress quest)
    {
        quest.currentProgress = Mathf.Max(1, quest.questData.targetCount);
        quest.state = QUEST_STATE.Completed;
        OnQuestProgress?.Invoke(quest);
        Shared.Instance.SoundManager.PlaySFXOneShot(questClearSound);
        GiveRewardItem(quest);
    }


    public void AcceptQuest(QuestData data)
    {
        if (activeQuests.Exists(q => q.questData.questId == data.questId))
            return;

        activeQuests.Add(new QuestProgress(data));
        Debug.Log($"{data.questName} Quest Accept");
    }

    public void OnObjective(Quest_Type type, int targetId, int value = 1)
    {
        for (int i = 0; i < activeQuests.Count; i++)
        {
            var quest = activeQuests[i];

            if (quest.questData.type != Quest_Type.Kill)
                continue;
            if (quest.state != QUEST_STATE.InProgress)
                continue;
            if (quest.questData.type != type)
                continue;

            bool valid = quest.questData.targetId == targetId;

            if (!valid)
                continue;

            quest.currentProgress += value;
            OnQuestProgress?.Invoke(quest);

            if (quest.IsComplete())
            {
                Shared.Instance.SoundManager.PlaySFXOneShot(questClearSound);
                int target = Mathf.Max(1, quest.questData.targetCount);

                if (IsRepeat(quest.questData))
                {
                    int clears = quest.currentProgress / target;
                    int overflow = quest.currentProgress % target;

                    for (int c = 0; c < clears; c++)
                    {
                        quest.state = QUEST_STATE.Completed;
                        OnQuestProgress?.Invoke(quest);

                        quest.rewardGiven = false;
                        GiveRewardItem(quest);
                    }

                    quest.state = QUEST_STATE.InProgress;
                    quest.currentProgress = overflow;
                    OnQuestProgress?.Invoke(quest);
                }

                else
                {
                    quest.state = QUEST_STATE.Completed;
                    Debug.Log($"[{quest.questData.questName}] Quest Clear");
                    OnQuestProgress?.Invoke(quest);
                    GiveRewardItem(quest);
                }

            }
        }
        CheckAllCompleted();
    }

    public bool AllQuestCompleted()
    {
        for (int i = 0; i < activeQuests.Count; i++)
        {
            if (activeQuests[i].state != QUEST_STATE.Completed)
                return false;
        }
        Debug.Log("Quest All Clear");
        return true;
    }
    

    public List<QuestProgress> GetActiveQuests()
    {
        return new List<QuestProgress>(activeQuests);
    }

    public int GetActiveQuestCount()
    {   
        return activeQuests.Count;
    }

    public int GetCompletedQuestCount()
    {
        return activeQuests.Count(q => q.state == QUEST_STATE.Completed);
    }


    private void GiveRewardItem(QuestProgress quest)
    {
        if (quest == null || quest.rewardGiven) return;

        int rewardId = quest.questData.rewardId;

        var reward = Shared.Instance.DataManager.Reward_Table.Get(rewardId);

        if (reward == null)
        {
            Debug.LogWarning($"[Reward] Reward ID {rewardId} not found");
            return;
        }

        if (reward.ItemId > 0 && reward.ItemCount > 0)
        {
            var item = Shared.Instance.DataManager.Item_Table.CreateById(reward.ItemId, reward.ItemCount);
            if (item == null)
            {
                Debug.LogWarning($"[Reward] Item ID {reward.ItemId} not found");
            }

            else
            {
                var tab = MenuSystem.Instance.itemTab;
                if (tab != null && tab.AddItem(item, AcsqtType.QuestReward))
                {
                    tab.RefreshInventory();
                }
                else
                {
                    Debug.LogWarning("[Reward] Inventory full or ItemTab null");
                }
            }
            MenuSystem.Instance.quickSlotBar.RefreshAllUI();
        }
    
        
        // Exp
        if (reward.Exp > 0)
        {
            //GameShard.Instance.GameManager.Player?.AddExp(reward.Exp);
            GameShard.Instance.GameUiManager.PlayerStateBar.SetExp(reward.Exp);
            Debug.Log($"[Reward] EXP +{reward.Exp}");
        }

        // Score
        if (reward.Score > 0)
        {
            GameShard.Instance?.GameManager.PlusGameScore(reward.Score);
            Debug.Log($"[Reward] SCORE +{reward.Score}");
        }

        quest.rewardGiven = true;
    }

    public void Clear() => activeQuests.Clear();
    public bool ForceCompleteAllActiveQuests()
    {
        bool touched = false;

        for (int i = 0; i < activeQuests.Count; i++)
        {
            var q = activeQuests[i];
            if (q == null) continue;

            if (q.state == QUEST_STATE.Completed)
                continue;

            int target = q.questData.targetCount;

            if (q.currentProgress < target)
                q.currentProgress = target;

            OnQuestProgress?.Invoke(q);

            if (q.IsComplete())
            {
                q.state = QUEST_STATE.Completed;
                Debug.Log($"[CHEAT] Force complete: {q.questData.questName}");
                OnQuestProgress?.Invoke(q);

                GiveRewardItem(q);
                touched = true;
            }
        }

        if (touched)
        {
            CheckAllCompleted();
            return true;
        }
        return false;
    }

    private bool IsRepeat(QuestData data) => data.type == Quest_Type.Kill;
}
