using UnityEngine;
using static Enums;

public class ItemBase
{
    public int itemId;
    public ITEMTYPE type;
    public SUBTYPE subType;
    public string spriteName;
    public string prefabPath;
    public int itemName;
    public int itemDesc;
    public float amount;
    public int maxStackCount = 1; // 기본은 스택 불가
    public int stackCount;

    public GameObject prefab;
    public Sprite icon;


    public virtual bool IsStackable => maxStackCount > 1;

    public virtual bool CanStackWith(ItemBase other)
    {
        return other != null
            && this.itemId == other.itemId
            && this.type == other.type
            && IsStackable;
    }

    public virtual int AddStack(int amount)
    {
        if (!IsStackable) return amount;
        int canAdd = Mathf.Min(amount, maxStackCount - stackCount);
        stackCount += canAdd;
        return amount - canAdd;
    }

    public void LoadResources()
    {
        if (type == ITEMTYPE.Potion)
        {
            prefab = Resources.Load<GameObject>(prefabPath);
            icon = Shared.Instance.AtlasManager.Get(CONFIG_ATLAS_TYPE.Item, spriteName);
        }
        else
        {
            Debug.Log("Not Potion Icon");
        }
    }

    public bool IsQuestItem() => type == ITEMTYPE.Quest;
}

