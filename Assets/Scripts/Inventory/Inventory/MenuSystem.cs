using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Enums;
using static UiManager;
public enum UiTab_Type 
{
    None,
    ItemTab,
    StateTab,
    ArchiveTab,
    QuestTab,
    Toggel,
    EscapeMenu,
}

public partial class MenuSystem : MonoBehaviour
{
    public static MenuSystem Instance;

    [Header("Tabs")]
    [SerializeField] GameObject GamePauseTab;
    [SerializeField] Transform MenuUiObj;
    public ItemTab itemTab;
    public StatTab statTab;
    public QuestTab questTab;
    public SkillTab skillTab;

    //[SerializeField] GameObject archiveTab;


    [Header("QuickSlot")]
    public QuickSlotBar quickSlotBar;

    [Header("Drag UI")]
    [SerializeField] private DragItem dragIcon;

    Player PLAYER;
    public bool isInvenOpen = false;
    private GameObject currentSubUI = null;
    //Stack<GameObject> UiActiveSatckData = new();
    AudioSource UiPlayer;
    GameObject beUiPopObj;
    Dictionary<GameObject, Toggle> UiTabData = new Dictionary<GameObject, Toggle>();
    public enum UITYPE 
    {
        Inventory,
        Quest,
        Skill,
        Stat,
    }

    private void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
        }

        else
        {
            Destroy(gameObject);
            return;
        }

        MenuUiObj = gameObject.transform.GetChild(0);
        MenuUiObj.gameObject.SetActive(false);

        if (quickSlotBar == null)
            quickSlotBar = FindFirstObjectByType<QuickSlotBar>(FindObjectsInactive.Include);

        UiPlayer = gameObject.AddComponent<AudioSource>();

        for (int i = 0; i < panels.Length; i++)
        {
            UiTabData.Add(panels[i], toggles[i]);
        }

    }

    public void PlayUI(int id)
    {
        if (UiPlayer == null)
            UiPlayer = gameObject.AddComponent<AudioSource>();
        UiPlayer.outputAudioMixerGroup = Shared.Instance.SoundManager.GetMixser(OptionHandler.SOUND.Effect);
        var src = Shared.Instance.SoundManager.ClipGet(id);
        UiPlayer.PlayOneShot(src);
    }
    public void MenuExit() 
    {
        MenuUiObj.gameObject.SetActive(false);
        ToggleTab.SetActive(false);
        GameShard.Instance.GameUiManager.UiActiveSatckData.Clear();
        UiActive(false);
    }
    public void PopUi(KeyCode _input) 
    {
        GameObject go = getUiType(_input);
        if (go == null) return;

        // 토글탭이 열려있지 않으면 강제로 열기
        //if (!UiActiveSatckData.Contains(ToggleTab))
        //{
        //    ToggleTab.SetActive(true);
        //    UiActiveSatckData.Push(ToggleTab);
        //    UiActive(true);
        //}

        // 이미 다른 보조 UI가 켜져있으면 닫기
        if (currentSubUI != null && currentSubUI != go)
        {
            currentSubUI.SetActive(false);
        }

        // 현재 UI 켜기
        go.SetActive(true);
        currentSubUI = go;
        //GameObject go = getUiType(_input);
        //if (go == null) return;

        //if (UiActiveSatckData.Contains(MenuUiObj.gameObject))//ToggelOn 
        //{
        //    go.SetActive(true);
        //    UiActive(true);
        //}
        //else 
        //{
        //    GameObject uiTab = UiActiveSatckData.Pop();
        //    uiTab.SetActive(false);
        //    go.SetActive(false);
        //    UiActive(false);
        //}

        //if (!go.activeSelf)
        //{
        //    go.SetActive(true);
        //    UiActive(true);
        //}
        //else
        //{
        //    go.SetActive(false);
        //    // 자유 UI 닫았다고 해서 스택 건드리진 않음
        //    if (UiActiveSatckData.Count == 0)
        //        UiActive(false);
        //}
    }
    private GameObject getUiType(KeyCode _input)
    {
        switch (_input)
        {
            case KeyCode.I: return itemTab.gameObject;
            default: return null;
        }
    }
    //private void MenuToggleOn() 
    //{
    //    if (currentSubUI != null)
    //    {
    //        currentSubUI.SetActive(false);
    //        currentSubUI = null;
    //        return;
    //    }

    //    // 2️⃣ 보조 UI가 없으면 → 토글탭 켜기/끄기
    //    if (!UiActiveSatckData.Contains(ToggleTab)) // 토글이 안 켜져있을 때
    //    {
    //        ToggleTab.SetActive(true);
    //        UiActiveSatckData.Push(ToggleTab);
    //        UiActive(true);
    //    }

    //    else // 토글 닫기
    //    {
    //        Stack<GameObject> temp = new Stack<GameObject>();
    //        while (UiActiveSatckData.Count > 0)
    //        {
    //            GameObject top = UiActiveSatckData.Pop();
    //            if (top == ToggleTab)
    //            {
    //                top.SetActive(false);
    //                break;
    //            }
    //            temp.Push(top);
    //        }
    //        while (temp.Count > 0) UiActiveSatckData.Push(temp.Pop());

    //        if (UiActiveSatckData.Count == 0)
    //            UiActive(false);
    //    }
    //    //if (!UiActiveSatckData.Contains(ToggleTab))//Toggel Not Open 
    //    //{
    //    //    ToggleTab.SetActive(true);
    //    //    UiActiveSatckData.Push(ToggleTab);
    //    //    UiActive(true);
    //    //}
    //    //else
    //    //{
    //    //    //UiActiveSatckData.Push(ToggleTab);
    //    //    GameObject uiTab = UiActiveSatckData.Pop();
    //    //    uiTab.SetActive(false);
    //    //    //ToggleTab.SetActive(false);
    //    //    UiActive(false);
            
    //    //}

    //    //if (UiActiveSatckData.Contains(itemTab.gameObject) ||
    //    //        UiActiveSatckData.Contains(questTab.gameObject) ||
    //    //         UiActiveSatckData.Contains(statTab.gameObject) ||
    //    //        UiActiveSatckData.Contains(skillTab.gameObject))
    //    //{
    //    //    return;
    //    //}

    //    //GameStopMenuOff();

    //}
    public void UiActive(bool _Check) 
    {
        bool state = GameShard.Instance.InputManager.isUIOpen;
        if (_Check == state) return;

        if (_Check != state)
        {
            GameShard.Instance.InputManager.isUIOpen = _Check;
        }

        GameShard.Instance.MonsterManager.UiStateUpdate(_Check);

        if (GameShard.Instance?.GameUiManager != null)
            GameShard.Instance.GameUiManager.SetMenuMode(_Check);

        //Time.timeScale = _Check ? 0f : 1f;
    }

    public void EscMenuOpen() 
    {
        GamePauseTab.gameObject.SetActive(true);
        GameShard.Instance.GameUiManager.UiActiveSatckData.Push(GamePauseTab);
        //if (quickSlotBar != null && quickSlotBar.picking)
        //{
        //    quickSlotBar.CancelPick();
        //    return;
        //}

        //if (currentSubUI != null)
        //{
        //    currentSubUI.SetActive(false);
        //    currentSubUI = null;


        //    //Toggle Close
        //    CloseToggleTab();
        //    UiActive(false);

        //    return;
        //}

        //// 2️⃣ 스택이 비어있지 않으면 → 맨 위 UI 닫기
        //if (UiActiveSatckData.Count > 0)
        //{
        //    GameObject uiObj = UiActiveSatckData.Pop();
        //    uiObj.SetActive(false);
        //    if (UiActiveSatckData.Count == 0) 
        //    {
        //        UiActive(false);
        //    }
        //    return;
        //}

        // 3️⃣ 아무 것도 없으면 → 게임 메뉴(ExitMenu) 열기
        //MenuUiObj.SetActive(true);
        //UiActiveSatckData.Push(GameMenuTab);
        UiActive(true);
    }

    //private void CloseToggleTab()
    //{
    //    if (ToggleTab == null || !ToggleTab.activeSelf) return;

    //    ToggleTab.SetActive(false);

    //    var temp = new Stack<GameObject>();
    //    while (UiActiveSatckData.Count > 0)
    //    {
    //        var top = UiActiveSatckData.Pop();
    //        if (top == ToggleTab) break;
    //        temp.Push(top);
    //    }
    //    while (temp.Count > 0) UiActiveSatckData.Push(temp.Pop());
    //}
    public bool UiOut() 
    {
        if (!itemTab.gameObject.activeSelf &&
            !questTab.gameObject.activeSelf &&
            !statTab.gameObject.activeSelf &&
            !skillTab.gameObject.activeSelf) 
        {
            MenuUiObj.gameObject.SetActive(false);
            ToggleTab.SetActive(false);
            return true;
        }
        return false;
    }
    public void InputUiOpen(UITYPE _type) 
    {
        if (!MenuUiObj.gameObject.activeSelf) 
        {
            MenuUiObj.gameObject.SetActive(true);
            ToggleTab.SetActive(true);
        }
        switch (_type)
        {
            case UITYPE.Inventory:
                Uicheck(itemTab.gameObject);
                break;
            case UITYPE.Quest:
                Uicheck(questTab.gameObject);
                break;
            case UITYPE.Stat:
                Uicheck(statTab.gameObject);
                break;
            case UITYPE.Skill:
                Uicheck(skillTab.gameObject);
                break;
        }
        UiActive(true);
    }
    private void Uicheck(GameObject _obj) 
    {
        if (GameShard.Instance.GameUiManager.UiActiveSatckData.Contains(_obj))
        {
            GameObject go = GameShard.Instance.GameUiManager.UiActiveSatckData.Pop();
            go.SetActive(false);
            ToggleTab.SetActive(false);
            MenuUiObj.gameObject.SetActive(false);
            return;
        }
        else 
        {
            if (GameShard.Instance.GameUiManager.UiActiveSatckData.Count != 0) 
            {
                GameObject top = GameShard.Instance.GameUiManager.UiActiveSatckData.Pop();
                Toggle beforeToggle = UiTabData[top];
                beforeToggle.isOn = false;
                top.SetActive(false);
            }

            GameShard.Instance.GameUiManager.UiActiveSatckData.Push(_obj.gameObject);
            _obj.SetActive(true);
            Toggle afterToggle = UiTabData[_obj];
            afterToggle.isOn = true;
        }
        
    }
    public void invenOpen()
    {
        if (itemTab == null) 
        { 
            Debug.Log("InvenObj is null");
            return; 
        }


        if (!isInvenOpen)
        {
            isInvenOpen = true;
            ToggleTab.SetActive(true);
            toggles[lastTabIndex].SetIsOnWithoutNotify(true);
            ShowOnlyPanel(lastTabIndex);
            UiActive(true);
            return;
        }

        if (statTab != null && currentSubUI == statTab.gameObject)
        {
            bool canClose = statTab.BeforeLeave(() =>
            {
                CloseInventoryImmediate();
            });
            if (!canClose) return; 
        }

        CloseInventoryImmediate();
    }

    public void ApplyPlayer(Player _player) 
    {
        PLAYER = _player;
    }

    public void init(Player _player)
    {
        PLAYER = _player;

        itemTab = GetComponentInChildren<ItemTab>(true);
        questTab = GetComponentInChildren<QuestTab>(true);
        statTab = GetComponentInChildren<StatTab>(true);
        skillTab = GetComponentInChildren<SkillTab>(true);

        if (itemTab.gameObject.activeSelf) { itemTab.gameObject.SetActive(false); }
        if (questTab.gameObject.activeSelf) { questTab.gameObject.SetActive(false); }
        if (statTab.gameObject.activeSelf) { statTab.gameObject.SetActive(false); }
        if (skillTab.gameObject.activeSelf) { skillTab.gameObject.SetActive(false); }

        statTab.Initialize(PLAYER, skillTab);
        skillTab.Initialize(PLAYER);
        itemTab.Init();
        ToggelAdd();
        LoadTestItemToInventory();
    }

    public void ShowDragIcon(Sprite icon)
    {
        dragIcon?.Show(icon);
    }

    public void UpdateDragIcon(Vector2 pos)
    {
        dragIcon?.Follow(pos);
    }

    public void HideDragIcon()
    {
        dragIcon?.Hide();
    }

    // ITEM Test
    private void LoadTestItemToInventory()
    {
        if (itemTab != null && itemTab.GetItemSlots().Count == 0)
            itemTab.Init();

        var potions = Shared.Instance.DataManager.Item_Table.GetAllPotions();
        foreach (var proto in potions)
        {
            var clone = Shared.Instance.DataManager.Item_Table.CreateById(proto.itemId); 
            clone.stackCount = 4;                                                     
            itemTab.AddItem(clone, AcsqtType.Drop);                                                
        }
    }

    private void CloseInventoryImmediate()
    {
        if (quickSlotBar != null && quickSlotBar.picking)
            quickSlotBar.CancelPick();

        isInvenOpen = false;

        ToggleTab.SetActive(false);
        if (panels != null)
            for (int i = 0; i < panels.Length; i++)
                if (panels[i]) panels[i].SetActive(false);

        currentSubUI = null;
        UiActive(false);
    }

    public void CloseInventory()
    {
        if (!ToggleTab || !ToggleTab.activeSelf) return;

        if (statTab != null && currentSubUI == statTab.gameObject)
        {
            bool canClose = statTab.BeforeLeave(() =>
            {
                CloseInventoryImmediate();
            });
            if (!canClose) return;
        }

        CloseInventoryImmediate();
    }

    #region Quick Slot
    public bool AssignEmptyQuickSlot(ItemBase item)
    {
        if (item == null || quickSlotBar == null) return false;
        if (item.type != ITEMTYPE.Potion) return false;

        var slots = quickSlotBar.Slots;
        if (slots == null) return false;

        // (정책) 중복 금지 유지
        if (!quickSlotBar.NotDuplicateItem(item))
        {
            itemTab.instructionText.ShowInstruction();
            return false;
        }

        quickSlotBar.BeginPickInMenu(item);
        return true;
    }

    public void UseQuickSlot(int slotNumber) // 1..N
    {
        if (quickSlotBar == null) return;
        if (quickSlotBar.Slots[slotNumber - 1] != null)
            quickSlotBar.Slots[slotNumber - 1].Use();

    }
    #endregion
}