using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class RewardSlot : SlotBase
{
    [Header("Icon")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI countText;

    private ItemBase currentItem;

    private static readonly Color IconVisible = new Color(1f, 1f, 1f, 1f);
    private static readonly Color IconInvisible = new Color(1f, 1f, 1f, 0f);

    private void Reset()
    {
        if (!itemIcon) itemIcon = GetComponentInChildren<Image>(true);
    }

    private void Awake()
    {
        UpdateView();
    }

    public void SetItem(ItemBase item, int count)
    {
        currentItem = item;

        if (item != null) //&& item.icon != null)
        {
            SetIcon(item.icon);

            if (countText)
            {
                bool showCount = count > 1;
                countText.gameObject.SetActive(showCount);
                countText.text = showCount ? $"x{count}" : "";
            }
        }
        else
        {
            Clear();
        }
    }

    public void SetIcon(Sprite icon)
    {
        currentItem = null;
        itemIcon.sprite = icon;
        itemIcon.enabled = icon != null;
        itemIcon.color = icon != null ? IconVisible : IconInvisible;
    }

    /// <summary>����</summary>
    public void Clear()
    {
        currentItem = null;
        if (itemIcon)
        {
            itemIcon.sprite = null;
            itemIcon.enabled = false;
            itemIcon.color = IconInvisible;
        }

        if (countText)
            countText.gameObject.SetActive(false);
    }

    private void UpdateView()
    {
        if (!itemIcon) return;
        if (itemIcon.sprite == null)
        {
            itemIcon.enabled = false;
            itemIcon.color = IconInvisible;
        }
        else
        {
            itemIcon.enabled = true;
            itemIcon.color = IconVisible;
        }
    }

    #region Pointer (Highlight�� ����)
    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (!isSelected)
            buttonManager?.StartFade(this, 0f, 1f); // ���� ���̶���Ʈ ȿ��
        // ToolTip
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (!isSelected)
            buttonManager?.StartFade(this, 1f, 0f);
        // Close ToolTip
    }

    // Ŭ��/�巡�� ���� ������ ���� �̻�� �� �⺻ ���� �״�� (�ƹ� �͵� �� ��)
    #endregion
}