using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickSlotBar : MonoBehaviour
{
    [SerializeField] private List<QuickSlot> slots;
    public IReadOnlyList<QuickSlot> Slots => slots;

    // �� �߰�: �޴����� �� �ӽ� ����
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
            // �� ����/�ʱ�ȭ Ÿ�ֿ̹��� �ٷ� �ݿ�
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

        // 1) ��ġ ���� + MenuCanvas�� �ӽ� �̵� (Main/Popup�� ������ ����ֵ���)
        savedParent = rt.parent;
        savedSiblingIndex = rt.GetSiblingIndex();
        rt.SetParent(menuCanvas.transform, false);
        rt.SetAsLastSibling(); // �� ���� ���̰�

        GameObject dim = GameShard.Instance.GameUiManager.DimmerImage;
        // 2) ȭ�� ��Ӱ�(�Է� ���)
        if (GameShard.Instance.GameUiManager.DimmerImage)
        {
            GameShard.Instance.GameUiManager.DimmerImage.SetActive(true);
            GameShard.Instance.GameUiManager.DimmerImage.transform.SetAsLastSibling();
            rt.SetAsLastSibling(); // ������ �� ����
        }

        // 3) ���Ե��� "���� ���"�� ��ȯ
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
        // ���� ����
        for (int i = 0; i < slots.Count; i++)
            if (slots[i]) slots[i].SetPickMode(false, null, i);

        // �� ������
        if (GameShard.Instance.GameUiManager.DimmerImage)
            GameShard.Instance.GameUiManager.DimmerImage.SetActive(false);

        // ���� �θ�/���� ����
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

            // ���� ���
            var bg = s.GetComponent<Image>();
            if (bg)
                bg.color = new Color(bg.color.r, bg.color.g, bg.color.b, on ? 0.45f : 1f);

            // ������ ��� �ִ� ������
            if (s.icon)  // QuickSlot.cs�� SerializeField �Ǿ� ����
            {
                var c = s.icon.color;
                s.icon.color = new Color(c.r, c.g, c.b, on ? 0.45f : 1f);
            }
        }
    }
}
