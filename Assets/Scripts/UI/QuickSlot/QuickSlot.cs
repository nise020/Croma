using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Enums;
public class QuickSlot : MonoBehaviour, IPointerClickHandler
{
    public enum SlotType { None, Potion }

    [Header("UI")]
    [SerializeField] public Image icon;
    [SerializeField] private TextMeshProUGUI stackText;

    public ItemTab inventory;

    [Header("Ref")]
    public SlotType boundType = SlotType.None;
    public int boundItemId;

    public bool IsPickMode { get; private set; }
    private System.Action<int> onPicked;
    private int indexForPick;

    private const int soundID = 1;

    void Start()
    {
        if (inventory == null)
            inventory = MenuSystem.Instance?.itemTab;

        RefreshUI();
    }

    public void Assign(ItemBase item)
    {
        if (item == null)
        {
            Clear();
            return;
        }

        if (item.type != ITEMTYPE.Potion)
        {
            Debug.Log("[QuickSlot] Item is not Potion.");
            return;
        }

        boundItemId = item.itemId;
        boundType = SlotType.Potion;

        if (item.icon == null)
            item.LoadResources();

        icon.enabled = true;
        icon.sprite = item.icon;

        RefreshUI();
    }

    public void Use()
    {
        if (!inventory || boundType == SlotType.None) return;

        var sm = GameShard.Instance?.StageManager;
        if (sm != null && !sm.CanUseConsumables())
        {
            Debug.Log("[QuickSlot] Intermission: consumables are disabled.");
            return;
        }
        bool ok = inventory.TryConsumePotionByItemId(boundItemId); // 내부에서 item.Use()까지 보장되게

        if (!ok)
        {
            Debug.LogWarning("[QuickSlot] TryConsumePotionByItemId failed.");
            return;
        }
        Shared.Instance.SoundManager.PlaySFXOneShot(soundID);
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (!inventory || boundType != SlotType.Potion)
        {
            Clear();
            return;
        }

        int cnt = inventory.CountPotionsByItemId(boundItemId);
        stackText.text = cnt.ToString();

        if (cnt <= 0) 
            Clear();
    }
    
    public void Clear()
    {
        boundType = SlotType.None;
        boundItemId = -1;
        icon.enabled = false;
        icon.sprite = null;
        stackText.text = "";
    }

    public void SetPickMode(bool on, System.Action<int> pickedCb, int myIndex)
    {
        IsPickMode = on;
        onPicked = pickedCb;
        indexForPick = myIndex;
    }

    public void OnPointerClick(PointerEventData e)
    {
        if (IsPickMode) 
            onPicked?.Invoke(indexForPick);
        else
            Use(); // 기존 클릭 동작은 그대로
    }
}