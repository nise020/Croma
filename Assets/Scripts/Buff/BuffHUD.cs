using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class BuffHUD : MonoBehaviour
{
    [SerializeField] private GameObject buffIconPrefab;
    [SerializeField] private Transform container;

    private readonly Dictionary<BUFFTYPE, Sprite> spriteByType = new();
    private readonly Dictionary<BUFFTYPE, EntryBuff> entries = new();

    private class EntryBuff
    {
        public BuffIcon ui;
        public float duration; // 총 지속시간(초)
    }

    private void Awake()
    {
        GetSpirteByType(Shared.Instance.DataManager.Buff_Table);
    }

    public void GetSpirteByType(BuffTable buffTable)
    {
        spriteByType.Clear();
        if (buffTable == null) return;

        foreach (var kv in buffTable.BuffTableData)
        {
            var data = kv.Value;

            if (data == null || string.IsNullOrEmpty(data.spriteName)) continue;
            if (spriteByType.ContainsKey(data.type)) continue;

            var sprite = Shared.Instance.AtlasManager.Get(CONFIG_ATLAS_TYPE.Buff_Icon, data.spriteName);
            if (sprite != null) 
                spriteByType[data.type] = sprite;
        }
    }

    public void ShowBuff(BuffData data)
    {
        if (data == null) return;

        EndBuff(data.type);

        var go = Instantiate(buffIconPrefab, container);
        var ui = go.GetComponent<BuffIcon>() ?? go.AddComponent<BuffIcon>();

        if (spriteByType.TryGetValue(data.type, out var sprite))
        {
            ui.SetSprite(sprite);
        }
        else
        {
            Debug.LogWarning($"[BuffHUD] No cached sprite for {data.type}. Did you call PrewarmFromTable()?");
        }
           
        ui.SetRemain(1f);
        entries[data.type] = new EntryBuff { ui = ui, duration = Mathf.Max(0f, data.time) };
    }

    public void UpdateBuff(BUFFTYPE type, float elapsedSec, float durationSec)
    {
        if (!entries.TryGetValue(type, out var e) || e.ui == null) return;
        e.ui.SetRemain(durationSec > 0f ? 1f - (elapsedSec / durationSec) : 0f);
    }

    public void EndBuff(BUFFTYPE type)
    {
        if (!entries.TryGetValue(type, out var e)) 
            return;

        if (e.ui != null)
            Destroy(e.ui.gameObject);
        entries.Remove(type);
    }

    public void ClearAllHud()
    {
        var toRemove = new List<BUFFTYPE>(entries.Keys);
        for (int i = 0; i < toRemove.Count; i++)
        {
            var type = toRemove[i];
            if (entries.TryGetValue(type, out var e) && e.ui != null)
                Destroy(e.ui.gameObject);
            entries.Remove(type);
        }
    }
}
