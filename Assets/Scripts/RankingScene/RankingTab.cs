using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RankingTab : MonoBehaviour
{
    public static RankingTab Instance;

    [Header("Title")]
    public TextMeshProUGUI title_Text;
    public RankRowView myRank;

    [Header("List")]
    public ScrollRect scroll;
    public Transform content;
    public RankRowView rowTemplate;

    [Header("Bottom")]
    public Button first_Btn;
    public Button prev_Btn;
    public Transform pageButtons;
    public Button pageButtonPrefab;
    public Button next_Btn;
    public Button last_Btn;

    [Header("Pages")]
    [SerializeField] private int windowSize = 5; 
    private int windowStart = 1;                
    [SerializeField] private Color currentPageTextColor = new Color32(52, 120, 246, 255);
    [SerializeField] private Color normalPageTextColor = Color.white;

    private List<RankEntry> data = new();
    private int currentPage;
    private int totalPages;

    [Header("Dummy Settings")]
    public int pageDataSize = 5;
    public int dummyCount = 137;
    public string myId = "Player001";
    private void Awake()
    {
        Instance = this;
    }
    public async UniTask RankSetting()
    {
        //Server.instanse.OnBtnConnect();

        pageDataSize = 5;

        if (rowTemplate)
            rowTemplate.gameObject.SetActive(false);

        BuildDummy(); //Create DummyData <- Not server

        var me = data.Find(e => e.id == myId) ?? data[0];
        if (myRank) 
            myRank.Set(me);

        totalPages = Mathf.Max(1, Mathf.CeilToInt(data.Count / (float)pageDataSize));
        Debug.Log($"{data.Count} / {pageDataSize} / {totalPages}");

        HookNav();
        RenderPage(1);

        await UniTask.CompletedTask;
    }

    void BuildDummy()
    {
        data.Clear();

        PlayerDataList datalist = Shared.Instance.Load();

        if (datalist != null)
        {
            List<PlayerData> playerDatas = datalist.players;

            for (int i = 0; i < playerDatas.Count || i == 10; i++)//10 <- test
            {
                data.Add(new RankEntry
                {
                    rank = i + 1,
                    id = $"Player{playerDatas[i].playerId:000}",
                    score = 20000 - playerDatas[i].Score,
                });
                dummyCount = dummyCount - 1;
            }

            for (int i = 0; i < dummyCount; i++)
            {
                data.Add(new RankEntry
                {
                    rank = i + 1,
                    id = $"Player{i:000}",
                    score = 20000 - i,
                });
            }

            if (data.Find(e => e.id == myId) == null)
            {
                data.Add(new RankEntry { rank = data.Count + 1, id = myId, score = 15000 });
            }

            data.Sort((a, b) => b.score.CompareTo(a.score));
            for (int i = 0; i < data.Count; i++)
            {
                data[i].rank = i + 1;
            }

        }
        else
        {
            for (int i = 0; i < dummyCount; i++)
            {
                data.Add(new RankEntry
                {
                    rank = i + 1,
                    id = $"Player{i:000}",
                    score = 20000 - i,
                });
            }
            if (data.Find(e => e.id == myId) == null)
            {
                data.Add(new RankEntry { rank = data.Count + 1, id = myId, score = 15000 });
            }

            data.Sort((a, b) => b.score.CompareTo(a.score));
            for (int i = 0; i < data.Count; i++)
            {
                data[i].rank = i + 1;
            }
        }            
    }

    private void HookNav()
    {
        if (first_Btn)
            first_Btn.onClick.AddListener(() => { windowStart = 1; RenderPage(1); });

        if (prev_Btn)
            prev_Btn.onClick.AddListener(() => MoveWindow(-1));

        if (next_Btn)
            next_Btn.onClick.AddListener(() => MoveWindow(+1));

        if (last_Btn)
        {
            last_Btn.onClick.AddListener(() =>
            {
                int lastStart = Mathf.Max(1, totalPages - windowSize + 1);
                windowStart = lastStart;
                RenderPage(windowStart);
            });
        }
    }

    private void MoveWindow(int dir)
    {
        int lastStart = Mathf.Max(1, totalPages - windowSize + 1);
        windowStart = Mathf.Clamp(windowStart + dir * windowSize, 1, lastStart);
        RenderPage(windowStart);
    }

    public void RenderPage(int page)
    {
        currentPage = Mathf.Clamp(page, 1, totalPages);

        int desiredStart = ((currentPage - 1) / windowSize) * windowSize + 1;
        if (windowStart != desiredStart) windowStart = desiredStart;

        for (int i = content.childCount - 1; i >= 0; i--)
        {
            var child = content.GetChild(i).gameObject;
            if (rowTemplate && child == rowTemplate.gameObject) continue;
            Destroy(child);
        }

        int start = (currentPage - 1) * pageDataSize;
        int end = Mathf.Min(start + pageDataSize, data.Count);
        for (int i = start; i < end; i++)
        {
            var item = CreateRow();
            item.Set(data[i]);
        }

        // 페이지 버튼은 windowStart 기준으로만 재구성(숫자 버튼을 눌러도 묶음은 그대로)
        BuildPagination();

        if (scroll) scroll.verticalNormalizedPosition = 1f;

        // 묶음 이동 버튼의 가능 여부
        int lastStart = Mathf.Max(1, totalPages - windowSize + 1);
        if (prev_Btn) prev_Btn.interactable = windowStart > 1;
        if (first_Btn) first_Btn.interactable = windowStart > 1;
        if (next_Btn) next_Btn.interactable = windowStart < lastStart;
        if (last_Btn) last_Btn.interactable = windowStart < lastStart;
    }

    private RankRowView CreateRow()
    {
        RankRowView row = null;
        if (rowTemplate)
        {
            var go = Instantiate(rowTemplate.gameObject, content);
            go.SetActive(true);
            row = go.GetComponent<RankRowView>();
        }
        else
        {
            var holder = new GameObject("RowItem", typeof(RectTransform), typeof(RankRowView));
            holder.transform.SetParent(content, false);
            row = holder.GetComponent<RankRowView>();
        }
        return row;
    }

    private void BuildPagination()
    {
        for (int i = pageButtons.childCount -1; i >= 0; i--)
        {
            Destroy(pageButtons.GetChild(i).gameObject);
        }

        int lastStart = Mathf.Max(1, totalPages - windowSize + 1);
        int start = windowStart;
        int count = Mathf.Min(windowSize, totalPages - start + 1);

        Button MakeBtn(string label, System.Action onClick, bool interactable = true, bool isCurrent = false)
        {
            Button proto = pageButtonPrefab ? pageButtonPrefab : prev_Btn;
            var btn = Instantiate(proto, pageButtons);
            btn.onClick.RemoveAllListeners();
            btn.interactable = interactable;

            var text = btn.GetComponentInChildren<TMP_Text>(true);
            if (text)
            {
                text.text = label;
                text.color = isCurrent ? currentPageTextColor : normalPageTextColor;
            }

            if (interactable && onClick != null && !isCurrent)
                btn.onClick.AddListener(() => onClick());

            return btn;
        }

        for (int i = 0; i < count; i++)
        {
            int pageIndex = start + i;
            bool isCurrent = (pageIndex == currentPage);
            MakeBtn(pageIndex.ToString(), () =>
            {
                RenderPage(pageIndex);
            },
            interactable: !isCurrent,
            isCurrent: isCurrent);
        }
    }

    public void JumpToMyRank()
    {
        var me = data.Find(e => e.id == myId);
        if (me == null) return;
        int page = Mathf.Clamp((me.rank - 1) / pageDataSize + 1, 1, totalPages);
        RenderPage(page);
    }
}

