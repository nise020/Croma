using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static LanguageTable;

public class ItemInfo : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemDesc;

    [SerializeField] private ItemMenuBtn useBtn;
    [SerializeField] private ItemMenuBtn quickSlotBtn;

    private Action onUse;
    private Action onQuick;

    ItemBase itemBase;

    private void Start()
    {
        Shared.Instance.LanguageManager.LanguageChangeEvent += TaxtChange;
    }

    public void SetData(ItemBase item)
    {
        if (item == null)
        {
            this.gameObject.SetActive(false);
            iconImage.sprite = null;
            itemName.text = null;
            itemDesc.text = null;
            return;
        }
        itemBase = item;
        this.gameObject.SetActive(true);
        iconImage.sprite = item.icon;

        TaxtChange();

    }
    public void TaxtChange() 
    {
        var table1 = Shared.Instance.DataManager.Language_Table.Get(itemBase.itemName);

        var table2 = Shared.Instance.DataManager.Language_Table.Get(itemBase.itemDesc);
        if (Shared.Instance.LanguageManager.lANGUEGE_TYPE == LANGUEGE_TYPE.Ko) 
        {
            itemName.text = table1.Ko;
            itemDesc.text = table2.Ko;
        }
        else if (Shared.Instance.LanguageManager.lANGUEGE_TYPE == LANGUEGE_TYPE.En) 
        {
            itemName.text = table1.En;
            itemDesc.text = table2.En;
        }
    }
    public void Init(Action onUse, Action onQuick)
    {

        this.onUse = onUse;
        this.onQuick = onQuick;

        useBtn.btn.onClick.RemoveAllListeners();
        quickSlotBtn.btn.onClick.RemoveAllListeners();

        useBtn.Init(
         onUse: () => { this.onUse?.Invoke(); },
         onQuick: () => { this.onQuick?.Invoke(); });

        quickSlotBtn.Init(
            onUse: () => { this.onUse?.Invoke(); },
            onQuick: () => { this.onQuick?.Invoke(); });
    }
}
