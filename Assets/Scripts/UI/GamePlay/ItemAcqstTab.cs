using UnityEngine;
using static Enums;

public class ItemAcqstTab : MonoBehaviour
{
    [SerializeField] private GameObject dropItemAcqst;
    [SerializeField] private GameObject rewardItemAcqst;

    public void HandleItemAcqst(ItemBase item, int count, AcsqtType type)
    {
        ShowItemAcqst(item, count, 1.2f, type); 
    }

    private void ShowItemAcqst(ItemBase item, int count, float showSec, AcsqtType type)
    {
        GameObject prefab = (type == AcsqtType.Drop) ? dropItemAcqst : rewardItemAcqst;

        if (prefab == null)
        {
            Debug.LogWarning($"[ItemAcqstTab] Prefab missing for {type}");
            return;
        }
        
        var go = Instantiate(prefab, transform, false);
        var comp = go.GetComponent<ItemAcqst>();
        comp.Init(item, count);        
    }

    public void ClearAll()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            var child = transform.GetChild(i);
            var c = child.GetComponent<ItemAcqst>();
            if (c) c.Clear();
            Destroy(child.gameObject);
        }
    }
}
