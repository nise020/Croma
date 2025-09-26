using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class ItemTable : Table_Base
{
    [Serializable]
    public class Info
    {
        public int Id;
        public int Type;        // 아이템 타입
        public int SubType;     // 아이템 서브타입
        public string spriteName;
        public string PrefabPath;
        public int Name;
        public int Dec;
        public float Amount;
        public int MaxCount;    // 최대 개수
        public int BuffId;         // 스킬ID/버프ID 용도
    }

    public Dictionary<int, ItemBase> ItemTableData = new Dictionary<int, ItemBase>();
    public Dictionary<int, PotionItem> PotionData = new Dictionary<int, PotionItem>();
    public Dictionary<int, QuestItem> QuestData = new Dictionary<int, QuestItem>();



    #region Get Methods - Potion

    public PotionItem GetPotion(int _Id)
    {
        if (PotionData.TryGetValue(_Id, out var potion))
            return potion;
        return null;
    }

    public List<PotionItem> GetAllPotions()
    {
        return new List<PotionItem>(PotionData.Values);
    }

    public List<PotionItem> GetPotionsByType(SUBTYPE potionType)
    {
        var result = new List<PotionItem>();
        foreach (var potion in PotionData.Values)
        {
            if (potion.potionType == potionType)
                result.Add(potion);
        }
        return result;
    }

    #endregion

    #region Get Methods - Quest

    public QuestItem GetQuest(int _Id)
    {
        if (QuestData.TryGetValue(_Id, out var quest))
            return quest;
        return null;
    }

    public List<QuestItem> GetAllQuests()
    {
        return new List<QuestItem>(QuestData.Values);
    }

    #endregion

    #region Get Methods - Common

    public ItemBase Get(int _Id)
    {
        if (ItemTableData.TryGetValue(_Id, out var item))
            return item;
        return null;
    }

    #endregion

    #region Data Loading
    public void Init_Binary(string _Name)
    {
        Load_Binary<Dictionary<int, ItemBase>>(_Name, ref ItemTableData);
        RebuildTypeDictionaries();
    }

    public void Save_Binary(string _Name)
    {
        Save_Binary(_Name, ItemTableData);
    }

    public void Init_Csv(string _Name, int StartRow, int _StartCol)
    {
        CSVReader reader = GetCSVReader(_Name);
        if (reader == null) { return; }

        ItemTableData.Clear();
        PotionData.Clear();
        QuestData.Clear();

        for (int row = StartRow; row < reader.row; ++row)
        {
            Info info = new Info();

            if (!Read(reader, info, row, _StartCol))
                break;
             
            ITEMTYPE type = ConvertToItemType(info.Type);
            SUBTYPE subtype = ConvertToSubType(info.SubType, type);

            if (type == ITEMTYPE.None)
            {
                Debug.LogWarning($"[ItemTable] Invalid ITEMTYPE: {info.Type} at row {row}");
                continue;
            }

            ItemBase item = CreateAndStoreItem(info, type, subtype);
            if (item != null)
            {
                item.LoadResources();
            }
        }

        Debug.Log($"[ItemTable] Loaded {ItemTableData.Count} items " +
                 $"Potions: {PotionData.Count}, Quests: {QuestData.Count})");
    }

    #endregion

    #region Private Methods

    private ITEMTYPE ConvertToItemType(int typeNum)
    {
        switch (typeNum)
        {
            case 1: return ITEMTYPE.Potion;
            case 2: return ITEMTYPE.Quest;
            default:
                Debug.LogWarning($"[ItemTable] Unknown item type: {typeNum}");
                return ITEMTYPE.None;
        }
    }

    private SUBTYPE ConvertToSubType(int subtypeNum, ITEMTYPE itemType)
    {
        // 포션의 경우에만 서브타입 사용
        if (itemType == ITEMTYPE.Potion)
        {
            switch (subtypeNum)
            {
                case 1: return SUBTYPE.Heal;
                case 2: return SUBTYPE.Buff;
                default:
                    Debug.LogWarning($"[ItemTable] Unknown potion sub type: {subtypeNum}");
                    return SUBTYPE.None;
            }
        }

        // 다른 아이템 타입은 서브타입 없음
        return SUBTYPE.None;
    }

    private ItemBase CreateAndStoreItem(Info info, ITEMTYPE type, SUBTYPE subtype)
    {
        ItemBase item = null;

        switch (type)
        {
            case ITEMTYPE.Potion:
                var potion = new PotionItem
                {
                    itemId = info.Id,
                    type = type,
                    subType = subtype,
                    spriteName = info.spriteName,
                    prefabPath = info.PrefabPath,
                    itemName = info.Name,
                    itemDesc = info.Dec,
                    potionType = subtype,
                    buffId = info.BuffId,           // Id2를 버프ID로 사용
                    amount = info.Amount,        // Amount를 회복/버프량으로 사용
                    maxStackCount = info.MaxCount > 0 ? info.MaxCount : 10,
                    stackCount = 1,
                };
                PotionData.Add(potion.itemId, potion);
                item = potion;
                break;

            case ITEMTYPE.Quest:
                var quest = new QuestItem
                {
                    itemId = info.Id,
                    type = type,
                    subType = subtype,
                    spriteName = info.spriteName,
                    prefabPath = info.PrefabPath,
                    itemName = info.Name,
                    itemDesc = info.Dec
                };
                QuestData.Add(quest.itemId, quest);
                item = quest;
                break;
        }

        if (item != null)
        {
            ItemTableData.Add(item.itemId, item);
        }

        return item;
    }

    private void RebuildTypeDictionaries()
    {
        PotionData.Clear();
        QuestData.Clear();

        foreach (var item in ItemTableData.Values)
        {
            switch (item.type)
            {
                case ITEMTYPE.Potion:
                    if (item is PotionItem potion)
                        PotionData.Add(potion.itemId, potion);
                    break;
                case ITEMTYPE.Quest:
                    if (item is QuestItem quest)
                        QuestData.Add(quest.itemId, quest);
                    break;
            }
        }
    }
    #endregion

    #region CSV Reading

    protected bool Read(CSVReader _Reader, Info _info, int _Row, int _Col)
    {
        if (!_Reader.reset_row(_Row, _Col)) return false;

        _Reader.get(_Row, ref _info.Id);
        _Reader.get(_Row, ref _info.Type);
        _Reader.get(_Row, ref _info.SubType);
        _Reader.get(_Row, ref _info.spriteName);
        _Reader.get(_Row, ref _info.PrefabPath);
        _Reader.get(_Row, ref _info.Name);
        _Reader.get(_Row, ref _info.Dec);
        _Reader.get(_Row, ref _info.Amount);
        _Reader.get(_Row, ref _info.MaxCount);
        _Reader.get(_Row, ref _info.BuffId);

        return true;
    }

    #endregion
    public ItemBase CreateById(int itemId, int stack = 1)
    {
        var proto = Get(itemId);
        if (proto == null)
        {
            Debug.LogWarning($"[ItemTable] itemId {itemId} not found");
            return null;
        }

        var clone = CloneItem(proto);            
        clone.stackCount = Mathf.Clamp(stack, 1, Mathf.Max(1, clone.maxStackCount));
        clone.LoadResources();                
        return clone;
    }

    private ItemBase CloneItem(ItemBase proto)
    {
        ItemBase clone;
        switch (proto.type)
        {
            case Enums.ITEMTYPE.Potion:
                var p = proto as PotionItem;
                var np = new PotionItem
                {
                    potionType = p.potionType,
                    buffId = p.buffId,
                    duration = p.duration,
                };
                clone = np;
                break;

            default:
                clone = new ItemBase();
                break;
        }

        // 공통 필드 복사
        clone.itemId = proto.itemId;
        clone.type = proto.type;
        clone.subType = proto.subType;
        clone.spriteName = proto.spriteName;
        clone.prefabPath = proto.prefabPath;
        clone.itemName = proto.itemName;
        clone.itemDesc = proto.itemDesc;
        clone.amount = proto.amount;
        clone.maxStackCount = (proto.maxStackCount > 0 ? proto.maxStackCount : 1);
        clone.stackCount = 1;

        return clone;
    }

}