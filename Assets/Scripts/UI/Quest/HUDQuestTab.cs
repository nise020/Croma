using System.Collections.Generic;
using UnityEngine;
using static Enums;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class HUDQuestTab : MonoBehaviour
{
    [Header("Layout")]
    [SerializeField] private RectTransform content;   // VerticalLayoutGroup �޸� �θ�
    [SerializeField] private HUDQuestItem itemPrefab; // quest_ ������ ������
    [SerializeField] private int maxLines = 4;        // �� ȭ�� �ִ� ���� ��
    [SerializeField] private bool hideCompleted = false;

    private readonly List<HUDQuestItem> pool = new List<HUDQuestItem>();
    private readonly Dictionary<int, HUDQuestItem> map = new Dictionary<int, HUDQuestItem>();
    private bool subscribed;

    private void Awake()
    {
        if (!content)
            content = (RectTransform)transform;

        var cg = GetComponent<CanvasGroup>();
        if (cg != null) 
        { 
            cg.blocksRaycasts = false;  
            cg.interactable = false; 
        }
    }

    private async void Start()
    {
        await UniTask.WaitUntil(() => GameShard.Instance?.QuestManager != null);

        var hud = GetComponent<HUDQuestTab>();
        hud.gameObject.SetActive(true);  // �ʿ��ϸ� ���⼭ ��
        hud.RebuildAll();                // ���� ���������� Ȱ�� ����Ʈ�� ä��
    }

    private async void OnEnable()
    {
        await UniTask.WaitUntil(() => GameShard.Instance?.QuestManager != null);

        if (!subscribed)
        {
            GameShard.Instance.QuestManager.OnQuestProgress += HandleProgress;
            subscribed = true;
        }

        RebuildAll();
    }

    private void OnDisable()
    {
        if (subscribed && GameShard.Instance?.QuestManager != null)
        {
            GameShard.Instance.QuestManager.OnQuestProgress -= HandleProgress;
            subscribed = false;
        }
    }

    public void RebuildAll()
    {
        // ��� ����
        for (int i = 0; i < pool.Count; i++) pool[i].gameObject.SetActive(false);
        map.Clear();

        var list = GameShard.Instance.QuestManager.GetActiveQuests();
        int shown = 0;
        for (int i = 0; i < list.Count && shown < maxLines; i++)
        {
            var q = list[i];
            if (hideCompleted && q.state == QUEST_STATE.Completed) continue;

            var line = GetOrCreate(shown);
            line.Bind(q);
            map[q.questData.questId] = line;
            shown++;
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
    }

    private void HandleProgress(QuestProgress q)
    {
        // �Ϸ� ���� �ɼ�
        if (hideCompleted && q.state == QUEST_STATE.Completed)
        {
            if (map.TryGetValue(q.questData.questId, out var line))
            {
                line.gameObject.SetActive(false);
                map.Remove(q.questData.questId);
                FillVacancy();
            }
            return;
        }

        if (map.TryGetValue(q.questData.questId, out var target))
        {
            target.Refresh(q);
            return;
        }

        if (map.Count < maxLines)
        {
            var line = GetOrCreate(map.Count);
            line.Bind(q);
            map[q.questData.questId] = line;
        }
    }

    private HUDQuestItem GetOrCreate(int index)
    {
        while (pool.Count <= index)
        {
            var it = Instantiate(itemPrefab, content);
            pool.Add(it);
        }
        var item = pool[index];
        item.gameObject.SetActive(true);
        return item;
    }

    private void FillVacancy()
    {
        var list = GameShard.Instance.QuestManager.GetActiveQuests();

        for (int i = 0; i < list.Count && map.Count < maxLines; i++)
        {
            var q = list[i];
            if (hideCompleted && q.state == QUEST_STATE.Completed) continue;
            if (map.ContainsKey(q.questData.questId)) continue;

            var line = GetOrCreate(map.Count);
            line.Bind(q);
            map[q.questData.questId] = line;
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
    }

    public void ClearItems(bool destroy = false)
    {
        map.Clear();

        for (int i = 0; i < pool.Count; i++)
        {
            var it = pool[i];
            if (!it)
                continue;

            if (destroy)
                Destroy(it.gameObject);
            else
                it.gameObject.SetActive(false);
        }
        if (destroy) pool.Clear();

        // ���̾ƿ� ����
        if (content) 
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(content);
    }
}
