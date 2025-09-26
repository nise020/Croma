using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : SlotBase
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI stackText;

    private ItemBase currentItem;

    [SerializeField] private Canvas canvas;
    private ItemTab itemTab;

    private Color IconVisible = new Color(1f, 1f, 1f, 1f);
    private Color IconInvisible = new Color(1f, 1f, 1f, 0f);


    protected new void Awake()
    {
        base.Awake();

        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();

        if (MenuSystem.Instance.itemTab != null)
        {
            itemTab = MenuSystem.Instance.itemTab;
        }

        UpdateSlot();
    }

    protected new void OnDisable()
    {
        base.OnDisable();
        itemTab.HideItemInfo();
    }

    public void SetItem(ItemBase item)
    {
        currentItem = item;
        UpdateSlot();
    }

    public ItemBase GetItem() => currentItem;

    public void UpdateSlot()
    {
        if (currentItem != null)
        {
            itemIcon.sprite = currentItem.icon;
            itemIcon.enabled = true;
            itemIcon.color = IconVisible;

            if (currentItem.IsStackable)
            {
                stackText.text = currentItem.stackCount.ToString();
            }
            else
            {
                stackText.text = "";
            }
        }
        else
        {
            itemIcon.sprite = null;
            itemIcon.enabled = false;
            itemIcon.color = IconInvisible;
            stackText.text = "";
        }
    }

    public void Clear()
    {
        currentItem = null;
        UpdateSlot();
    }

    public int AddStack(int amount)
    {
        if (currentItem == null) return amount;
        return currentItem.AddStack(amount);
    }

    #region Pointer
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);

        buttonManager?.HandleSelection(this);

        if (currentItem == null)
        {
            MenuSystem.Instance.itemTab.HideItemInfo();
        }
        
        // Doble Click : Use Item
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (eventData.clickCount == 1)
            {
                MenuSystem.Instance.itemTab.ShowItemInfo(currentItem, this);
            }
            else if (eventData.clickCount == 2)
            {
                MenuSystem.Instance.itemTab.UseItem(currentItem, this);
            }
        }
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (!isSelected)
            buttonManager?.StartFade(this, 0f, 1f);

        if (currentItem == null)
            return;
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (!isSelected)
            buttonManager?.StartFade(this, 1f, 0f);

    }
    #endregion

    #region Drag & Drop
    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (currentItem == null) return;
        MenuSystem.Instance.ShowDragIcon(currentItem.icon);
        itemIcon.color = new Color(1f, 1f, 1f, 0.5f);
        itemIcon.raycastTarget = false;
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (currentItem == null) return;
        MenuSystem.Instance.UpdateDragIcon(eventData.position);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        MenuSystem.Instance.HideDragIcon();
        itemIcon.raycastTarget = true;
        itemIcon.color = new Color(1f, 1f, 1f, 1f);
    }

    public override void OnDrop(PointerEventData eventData)
    {
        var draggedSlot = eventData.pointerDrag?.GetComponent<ItemSlot>();
        if (draggedSlot != null && draggedSlot != this) 
            SwapItems(draggedSlot);
    }

    #endregion
   
    #region Helpers


    private void SwapItems(ItemSlot otherSlot)
    {
        var temp = currentItem;
        SetItem(otherSlot.GetItem());
        otherSlot.SetItem(temp);
    }

    private void CloseAnyMenusInCanvas() 
    {
/*        if (canvas == null) return;
        var menus = canvas.GetComponentsInChildren<ItemMenu>(true);
        for (int i = 0; i < menus.Length; i++)
            menus[i].Close();

        // ���� ĳ�õ� �ʱ�ȭ
        if (spawnedMenu != null) spawnedMenu = null;*/
    }
    #endregion
}
