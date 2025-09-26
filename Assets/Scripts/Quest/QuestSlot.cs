using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using static LanguageTable;

public class QuestSlot : SlotBase
{
    [SerializeField] private TextMeshProUGUI questNameTMP;
    private QuestProgress currentQuest;
    private QuestPanel currentPanel;

    public void Init(QuestProgress quest, QuestPanel panel)
    {
        currentQuest = quest;
        currentPanel = panel;
        TaxtChange();
        Shared.Instance.LanguageManager.LanguageChangeEvent += TaxtChange;
        gameObject.SetActive(true);
    }
    public void TaxtChange()
    {
        var table1 = Shared.Instance.DataManager.Language_Table.Get(currentQuest.questData.questName);

        if (Shared.Instance.LanguageManager.lANGUEGE_TYPE == LANGUEGE_TYPE.Ko)
        {
            questNameTMP.text = table1.Ko;
        }
        else if (Shared.Instance.LanguageManager.lANGUEGE_TYPE == LANGUEGE_TYPE.En)
        {
            questNameTMP.text = table1.En;
        }
    }
    public void Clear()
    {
        currentQuest = null;
        if (questNameTMP) 
            questNameTMP.text = "";

        gameObject.SetActive(false);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);

        buttonManager?.HandleSelection(this);

        if (currentQuest != null)
        {   
            currentPanel.RefreshQuest(currentQuest); // To be implemented
        }
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (!isSelected)
            buttonManager?.StartFade(this, 0f, 1f);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (!isSelected)
            buttonManager?.StartFade(this, 1f, 0f);
    }
}