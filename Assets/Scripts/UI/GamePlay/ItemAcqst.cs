using TMPro;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.UI;
using static LanguageTable;
using static UnityEngine.Rendering.DebugUI;

public class ItemAcqst : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI acqstText;
    private Animator animator;
    ItemBase itemBase;
    int itemCount = 0;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        Shared.Instance.LanguageManager.LanguageChangeEvent += TaxtChange;
    }

    public void Init(ItemBase item, int count)
    {
        itemBase = item;
        TaxtChange();
        itemIcon.sprite = item.icon;
        itemCount = count;
    }

    public void TaxtChange()
    {
        if (acqstText == null) return;

        var table1 = Shared.Instance.DataManager.Language_Table.Get(itemBase.itemName);

        if (Shared.Instance.LanguageManager.lANGUEGE_TYPE == LANGUEGE_TYPE.Ko)
        {
            acqstText.text = $"{table1.Ko} +{itemCount}";
        }
        else if (Shared.Instance.LanguageManager.lANGUEGE_TYPE == LANGUEGE_TYPE.En)
        {
            acqstText.text = $"{table1.Ko} +{itemCount}";
        }
    }
    public void EndShow()
    {
        Destroy(this.gameObject);
    }

    public void Clear()
    {
        itemIcon.sprite = null;
        acqstText.text = null;
    }
}
