using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickSlotBar : MonoBehaviour
{
    [SerializeField] private List<QuickSlot> slots;
    public IReadOnlyList<QuickSlot> Slots => slots;

    // ▼ 추가: 메뉴에서 쓸 임시 상태
    [Header("Picker (Menu)")]
    [SerializeField] private Canvas menuCanvas;

    private Transform parentCanvas;
    private Transform savedParent;
    private int savedSiblingIndex;
    private RectTransform rt;
    private ItemBase pending;

    public bool picking;
    public void CancelPick() => EndPick(false);

    private void Awake()
    {
        rt = (RectTransform)transform;
        menuCanvas = GameShard.Instance.GameUiManager.ReturnMenuCanvas();
    }

    private void OnEnable()
    {
        var sm = GameShard.Instance?.StageManager;
        if (sm != null)
        {
            // 씬 복귀/초기화 타이밍에도 바로 반영
            SetIntermissionVisual(sm.IsIntermission);
            sm.OnIntermissionChanged += SetIntermissionVisual;
        }
    }

    private void OnDisable()
    {
        var sm = GameShard.Instance?.StageManager;
        if (sm != null) sm.OnIntermissionChanged -= SetIntermissionVisual;
    }

    private void Reset()
    {
        slots = new List<QuickSlot>(GetComponentsInChildren<QuickSlot>(true));
    }

    public List<QuickSlot> ReturnSlots()
    {
        return slots;
    }

    public void RefreshAllUI()
    {
        if (slots == null) return;

        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].RefreshUI();
        }
    }

    public bool NotDuplicateItem(ItemBase item)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].boundItemId == item.itemId)
            {
                return false;
            }
        }
        return true;
    }

    public void AssignToIndex(int index, ItemBase item, bool clearDuplicates = true)
    {
        if (item == null) return;
        if (index < 0 || index >= slots.Count) return;

        if (clearDuplicates)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if (i == index)
                    continue;
                if (slots[i]?.boundItemId == item.itemId)
                    slots[i].Clear();
            }           
        }
        slots[index].Assign(item);
        RefreshAllUI();
    }

    public void BeginPickInMenu(ItemBase item)
    {
        if (picking || item == null || menuCanvas == null) return;
        pending = item;
        picking = true;

        // 1) 위치 저장 + MenuCanvas로 임시 이동 (Main/Popup이 꺼져도 살아있도록)
        savedParent = rt.parent;
        savedSiblingIndex = rt.GetSiblingIndex();
        rt.SetParent(menuCanvas.transform, false);
        rt.SetAsLastSibling(); // 딤 위에 보이게

        GameObject dim = GameShard.Instance.GameUiManager.DimmerImage;
        // 2) 화면 어둡게(입력 블록)
        if (GameShard.Instance.GameUiManager.DimmerImage)
        {
            GameShard.Instance.GameUiManager.DimmerImage.SetActive(true);
            GameShard.Instance.GameUiManager.DimmerImage.transform.SetAsLastSibling();
            rt.SetAsLastSibling(); // 슬롯이 딤 위에
        }

        // 3) 슬롯들을 "선택 모드"로 전환
        for (int i = 0; i < slots.Count; i++)
            if (slots[i]) slots[i].SetPickMode(true, OnPickIndex, i);
    }

    private void OnPickIndex(int index)
    {
        if (!picking || pending == null) { EndPick(false); return; }
        AssignToIndex(index, pending, clearDuplicates: true);
        EndPick(true);
    }

    public void EndPick(bool applied)
    {
        // 슬롯 원복
        for (int i = 0; i < slots.Count; i++)
            if (slots[i]) slots[i].SetPickMode(false, null, i);

        // 딤 내리기
        if (GameShard.Instance.GameUiManager.DimmerImage)
            GameShard.Instance.GameUiManager.DimmerImage.SetActive(false);

        // 원래 부모/순서 복귀
        if (savedParent)
        {
            rt.SetParent(savedParent, false);
            rt.SetSiblingIndex(savedSiblingIndex);
        }

        pending = null;
        picking = false;
    }
    public void SetIntermissionVisual(bool on)
    {
        if (slots == null) return;

        foreach (var s in slots)
        {
            if (!s) continue;

            // 슬롯 배경
            var bg = s.GetComponent<Image>();
            if (bg)
                bg.color = new Color(bg.color.r, bg.color.g, bg.color.b, on ? 0.45f : 1f);

            // 슬롯이 들고 있는 아이콘
            if (s.icon)  // QuickSlot.cs에 SerializeField 되어 있음
            {
                var c = s.icon.color;
                s.icon.color = new Color(c.r, c.g, c.b, on ? 0.45f : 1f);
            }
        }
    }
}
