using TMPro;
using UnityEngine;
using static Enums;
using static LanguageTable;

public class HUDQuestItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questNameTMP;
    [SerializeField] private TextMeshProUGUI progressTMP;

    private int lastCur = -1;
    private int lastTar = -1;
    private bool lastDone = false;
    private int questId = -1;

    private bool lastCompleted = false;
    private bool lastFailed = false;

    public int QuestId => questId;
    QuestProgress questProgress;
    public void Bind(QuestProgress q)
    {
        questProgress = q;
        questId = q.questData.questId;
        TaxtChange();
        Shared.Instance.LanguageManager.LanguageChangeEvent += TaxtChange;
        Apply(q, true);
    }
    public void TaxtChange()
    {
        var table = Shared.Instance.DataManager.Language_Table.Get(questProgress.questData.questName);

        if (Shared.Instance.LanguageManager.lANGUEGE_TYPE == LANGUEGE_TYPE.Ko)
        {
            questNameTMP.text = table.Ko;
        }
        else if (Shared.Instance.LanguageManager.lANGUEGE_TYPE == LANGUEGE_TYPE.En)
        {
            questNameTMP.text = table.En;
        }
    }
    public void Refresh(QuestProgress q) => Apply(q, false);

    private void Apply(QuestProgress q, bool force)
    {
        int cur = q.currentProgress < 0 ? 0 : q.currentProgress;
        int tar = q.questData.targetCount <= 0 ? 1 : q.questData.targetCount;

        if (force || cur != lastCur || tar != lastTar)
        {
            if (progressTMP) progressTMP.text = $"{cur} / {tar}";
            lastCur = cur; lastTar = tar;
        }

        bool isCompleted = q.state == QUEST_STATE.Completed; // or q.IsComplete()
        bool isFailed = q.state == QUEST_STATE.Failed;

        if (force || isCompleted != lastCompleted)
        {
            float a = isCompleted ? 0.6f : 1f; // 필요 시 인스펙터로 뺄 수 있음
            if (questNameTMP) { var c = questNameTMP.color; c.a = a; questNameTMP.color = c; }
            if (progressTMP) { var c = progressTMP.color; c.a = a; progressTMP.color = c; }
            lastCompleted = isCompleted;
        }

        // 2) 실패면 취소선만 켜고 끔
        if (force || isFailed != lastFailed)
        {
            TextStrike(questNameTMP, isFailed);
            TextStrike(progressTMP, isFailed);
            lastFailed = isFailed;
        }

        // 진행 중일 때(둘 다 false) → 원상복귀(알파 1, 취소선 off)
        if (!isCompleted && !isFailed)
        {
            if (questNameTMP) { var c = questNameTMP.color; c.a = 1f; questNameTMP.color = c; }
            if (progressTMP) { var c = progressTMP.color; c.a = 1f; progressTMP.color = c; }
            TextStrike(questNameTMP, false);
            TextStrike(progressTMP, false);
        }
    }

    private void TextStrike(TextMeshProUGUI tmp, bool on)
    {
        if (!tmp) return;

        if (on)
            tmp.fontStyle |= FontStyles.Strikethrough;
        else
            tmp.fontStyle &= ~FontStyles.Strikethrough;
    }
}
