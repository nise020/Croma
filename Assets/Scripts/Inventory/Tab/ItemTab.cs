using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using static Enums;

public class ItemTab : MonoBehaviour
{
    [Header("Inventory Slots")]
    [SerializeField] private List<ItemSlot> itemSlots = new List<ItemSlot>();
    private List<ItemBase> inventoryItems = new List<ItemBase>();

    [Header("UI")]
    [SerializeField] private ItemInfo info;
    private bool isInfoActive = false;
    private ItemSlot infoContextSlot;
    [SerializeField] public InstructionText instructionText;


    private Camera previewCam;
    private LobbyPlayer lobbyRef;

    public event Action<ItemBase, int, AcsqtType> OnItemAcqst;

    #region Lifecycle
    public void Init()
    {
        itemSlots.Clear();
        itemSlots.AddRange(GetComponentsInChildren<ItemSlot>(includeInactive: true));
        gameObject.SetActive(false);
        info.gameObject.SetActive(false);
        //itemDuplicationText.SetActive(false);
    }

    private void OnEnable()
    {

    }

    private void OnDisable()
    {
    }
    #endregion

    #region Inventory
    public bool AddItem(ItemBase item, AcsqtType type = AcsqtType.Drop) // Acsqt Default -> Drop
    {
        if (item == null) { Debug.LogWarning("[ItemTab] AddItem() : item == null"); return false; }

        int requested = Mathf.Max(1, (item.stackCount > 0 ? item.stackCount : 1));
        int remaining = requested;


        for (int i = 0; i < itemSlots.Count && remaining > 0; i++)            
        {
            var ex = itemSlots[i].GetItem();                                 
            if (ex != null && ex.CanStackWith(item) && ex.stackCount < ex.maxStackCount)
            {
                remaining = ex.AddStack(remaining);
                itemSlots[i].UpdateSlot();
                if (remaining <= 0) break;
            }
        }

        while (remaining > 0)
        {
            int emptyIdx = -1;
            for (int i = 0; i < itemSlots.Count; i++)
            {
                if (itemSlots[i].GetItem() == null) { emptyIdx = i; break; }
            }
            if (emptyIdx == -1)
            {
                Debug.Log("Inventory is Full");
                return false;
            }

            var clone = Shared.Instance.DataManager.Item_Table.CreateById(item.itemId); 
            int put = Mathf.Min(remaining, Mathf.Max(1, clone.maxStackCount));         
            clone.stackCount = put;                                                     
            itemSlots[emptyIdx].SetItem(clone);                                        
            inventoryItems.Add(clone);                                             
            remaining -= put;                                                   
        }

        int totalAdded = requested - remaining;
        if (totalAdded > 0)
        {
            OnItemAcqst?.Invoke(item, totalAdded, type);
        }

        return true;
    }

    public bool RemoveItem(ItemBase item)
    {
        if (item == null) return false;

        for (int i = 0; i < itemSlots.Count; i++)
        {
            if (itemSlots[i].GetItem() == item)
            {
                if (item is PotionItem potion)
                {
                     
                }
                itemSlots[i].SetItem(null);
                inventoryItems.Remove(item);
                return true;
            }
        }

        return false;
    }

    public bool HasItem(ItemBase item) => inventoryItems.Contains(item);

    public int GetItemCount(ItemBase item)
    {
        if (item is PotionItem potion)
        {
            foreach (var invItem in inventoryItems)
            {
                if (invItem is PotionItem invPotion && invPotion.itemId == potion.itemId)
                    return invPotion.stackCount;
            }
        }

        else
        {
            int count = 0;
            foreach (var invItem in inventoryItems)
                if (invItem.itemId == item.itemId) count++;
            return count;
        }
        return 0;
    }

    private bool TryStackPotion(PotionItem newPotion)
    {
        for (int i = 0; i < itemSlots.Count; i++)
        {
            if (itemSlots[i].GetItem() is PotionItem existingPotion &&
                existingPotion.itemId == newPotion.itemId &&
                existingPotion.CanStackWith(existingPotion))
            {
                existingPotion.AddStack(newPotion.stackCount);
                return true;
            }
        }
        return false;
    }
    #endregion

    #region  UI Helpers
    public void RefreshInventory()
    {
        for (int i = 0; i < itemSlots.Count; i++)
        {
            itemSlots[i].UpdateSlot();
        }
    }

    public void UseItem(ItemBase item, ItemSlot fromSlot)
    {
        if (item == null) return;

        switch (item)
        {
            case PotionItem potion:
                potion.Use();

                if (potion.stackCount <= 0)
                {
                    if (fromSlot != null) fromSlot.SetItem(null);
                    inventoryItems.Remove(potion);
                }
                else
                {
                    if (fromSlot != null) fromSlot.UpdateSlot();
                }

                MenuSystem.Instance.quickSlotBar.RefreshAllUI();
                RefreshInventory();
                break;

            case QuestItem:
                // 퀘스트 아이템은 즉시 사용/소비 로직이 없을 수 있음
                break;
        }
    }

    public void ShowItemInfo(ItemBase item, ItemSlot fromSlot)
    {
        if (info == null) return;

        infoContextSlot = fromSlot;
        info.SetData(item);

        info.Init(
        onUse: () =>
        {
            // 실제 사용 로직
            UseItem(item, infoContextSlot);
        },
        onQuick: () =>
        {
            // 퀵슬롯 배정(빈칸 우선, 꽉 찼으면 픽모드)
            MenuSystem.Instance.AssignEmptyQuickSlot(item);
        });

        isInfoActive = true;

    }

    public void HideItemInfo()
    {
        if (info != null && isInfoActive)
        {
            info.SetData(null);
            info.gameObject.SetActive(false);
            isInfoActive = false;
        }
    }



    #endregion

    #region Data / QuickSlot helpers
    public List<ItemBase> GetInventoryItems() => new List<ItemBase>(inventoryItems);

    public List<ItemSlot> GetItemSlots() => itemSlots;

    public int CountPotionsByItemId(int itemId)
    {
        int total = 0;

        foreach (var slot in itemSlots)
        {
            var item = slot.GetItem();

            if (item is PotionItem p && p.itemId == itemId)
                total += Mathf.Max(0, p.stackCount);
        }
        return total;
    }

    public bool TryConsumePotionByItemId(int itemId)
    {
        for (int i = 0; i < itemSlots.Count; i++)
        {
            var slot = itemSlots[i];
            var it = slot.GetItem();
            if (it is PotionItem p && p.itemId == itemId && p.stackCount > 0)
            {
                p.Use();                       // 내부에서 stackCount--

                if (p.stackCount <= 0)
                { 
                    slot.SetItem(null);
                    inventoryItems.Remove(p);  // ← 직접 제거 또는
                    //RemoveItem(p);
                }

                else
                    slot.UpdateSlot();         // 남아있으면 텍스트만 갱신

                // 퀵슬롯/다른 슬롯들 UI 싱크
                MenuSystem.Instance.quickSlotBar.RefreshAllUI();
                return true;
            }
        }
        return false; // 소비할 스택 없음
    }
    #endregion

    #region  Lobby Preview Internals

    #endregion
}
