using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Enums;

namespace Game.InGame
{
    public partial class UIManager : MonoBehaviour
    {
        Player PLAYER;
        public PlayerStateBar PlayerStateBar;
        public MenuSystem MenuSystem;
        public HUDQuestTab QuestHUD;
        public AreaName AreaName;
        public ReminderTab ReminderTab;
        public ClearTab ClearTab;
        public FailedTab FailedTab;
        public ItemAcqstTab ItemAcqstTab;
        public QuickSlotBar QuickSlotBar;
        private FadeEffect FadeEffect;
        public GameObject DimmerImage;
        public GameInfo GameInfo;

        public Stack<GameObject> UiActiveSatckData = new();

        Canvas MainUICanvas;
        Canvas PopupCanvas;
        Canvas MenuCanvas;

        public ButtonUIManager buttonMgr;

        public void PlayerInit(Player _player)
        {
            PLAYER = _player;
            if (PlayerStateBar != null) 
            {
                PlayerStateBar.InitializeCharacter(PLAYER);
                PLAYER.HpBarInit(PlayerStateBar);
                PLAYER.BuffHUDInit(PlayerStateBar.buffHUD);
            }
        }
        public void CanvasDelet() 
        {
            Destroy(MainUICanvas.gameObject);
            Destroy(PopupCanvas.gameObject);
            Destroy(MenuCanvas.gameObject);
        }
        //private void OnDestroy()
        //{
        //    Destroy(MainUICanvas);
        //    Destroy(PopupCanvas);
        //    Destroy(MenuCanvas);
        //}

        public async UniTask InitAsync()
        {
            GameShard.Instance.GameUiManager = this;
            if (buttonMgr != null)
            {
                buttonMgr = gameObject.AddComponent<ButtonUIManager>();
            }

            GameObject menuUiObj        = Shared.Instance.ResourcesManager.MenuObj;
            GameObject questHudObj      = Shared.Instance.ResourcesManager.QuestHUD;
            GameObject areaNameObj      = Shared.Instance.ResourcesManager.AreaName;
            GameObject StatebarObj      = Shared.Instance.ResourcesManager.PlayerStatePrefab;
            GameObject FadeObj          = Shared.Instance.ResourcesManager.FadePrefab;
            GameObject bossStateObj     = Shared.Instance.ResourcesManager.BossStatePrefab;
            GameObject reminderTabObj   = Shared.Instance.ResourcesManager.PointRemindPrefab;
            GameObject clearTabObj      = Shared.Instance.ResourcesManager.ClearTabPrefab;
            GameObject failedTabObj     = Shared.Instance.ResourcesManager.FailedTabPrefab;
            GameObject itemAcqstTabObj  = Shared.Instance.ResourcesManager.ItemAcqstTabPrefab;
            GameObject gameInfoObj      = Shared.Instance.ResourcesManager.GameInfoPrefab;
            GameObject quickSlotBarObj  = Shared.Instance.ResourcesManager.QuickSlotPrefab;
            GameObject dimmerImageObj   = Shared.Instance.ResourcesManager.DimmerImagePrefab;

            if (!MainUICanvas)
            {
                var go = GameObject.Find("MainUICanvas");
                if (!go || !go.TryGetComponent(out MainUICanvas))
                    Debug.LogError("[GameUIManager] MainUICanvas not found");
            }

            if (!PopupCanvas)
            {
                var go = GameObject.Find("PopupCanvas");
                if (!go || !go.TryGetComponent(out PopupCanvas))
                    Debug.LogError("[GameUIManager] PopupCanvas not found");
            }

            if (!MenuCanvas)
            {
                var go = GameObject.Find("MenuCanvas");
                if (!go || !go.TryGetComponent(out MenuCanvas))
                    Debug.LogError("[GameUIManager] MenuCanvas not found");
            }

            EnsureScaler(MainUICanvas, 1920, 1080);
            MainUICanvas.sortingOrder = 1;
            EnsureScaler(PopupCanvas, 1920, 1080);
            PopupCanvas.sortingOrder = 2;
            EnsureScaler(MenuCanvas, 1920, 1080);
            MenuCanvas.sortingOrder = 3;


            DontDestroyOnLoad(MainUICanvas);
            DontDestroyOnLoad(PopupCanvas);
            DontDestroyOnLoad(MenuCanvas);

            PopupCanvas.sortingOrder = 1;

            // Main Canvas
            GameObject questHudGo       = Instantiate(questHudObj, MainUICanvas.transform);
            GameObject statebarGo       = Instantiate(StatebarObj, MainUICanvas.transform);
            GameObject gameInfoGo       = Instantiate(gameInfoObj, MainUICanvas.transform);
            GameObject fadeGo           = Instantiate(FadeObj, MainUICanvas.transform);
            GameObject quickslotbarGo   = Instantiate(quickSlotBarObj, MainUICanvas.transform);

            // Popup Canvas
            GameObject areaNameGo       = Instantiate(areaNameObj, PopupCanvas.transform);            
            GameObject bossStateGo      = Instantiate(bossStateObj, PopupCanvas.transform);
            GameObject reminderTabGo    = Instantiate(reminderTabObj, PopupCanvas.transform);
            GameObject clearTabGo       = Instantiate(clearTabObj, PopupCanvas.transform);
            GameObject failedTabGo      = Instantiate(failedTabObj, PopupCanvas.transform);
            GameObject itemAcqstGo      = Instantiate(itemAcqstTabObj, PopupCanvas.transform);


            // Menu Canvas
            GameObject menuGo           = Instantiate(menuUiObj, MenuCanvas.transform);
            DimmerImage                 = Instantiate(dimmerImageObj, MenuCanvas.transform);


            MenuSystem                  = menuGo.GetComponent<MenuSystem>();
            QuestHUD                    = questHudGo.GetComponent<HUDQuestTab>();
            AreaName                    = areaNameGo.GetComponent<AreaName>();
            PlayerStateBar              = statebarGo.GetComponent<PlayerStateBar>();
            FadeEffect                  = fadeGo.GetComponent<FadeEffect>();
            ReminderTab                 = reminderTabGo.GetComponent<ReminderTab>();
            ClearTab                    = clearTabGo.GetComponent<ClearTab>();
            FailedTab                   = failedTabGo.GetComponent<FailedTab>();
            ItemAcqstTab                = itemAcqstGo.GetComponent<ItemAcqstTab>();
            GameInfo                    = gameInfoGo.GetComponent<GameInfo>(); 
            QuickSlotBar                = quickslotbarGo.GetComponent<QuickSlotBar>();

            //BossStateBar bossUi = obj6.GetComponent<BossStateBar>();

            GameShard.Instance.MonsterManager.ApplyCanvas(PopupCanvas.gameObject);//BossUi Parent Canvas

            PlayerStateBar.InitializeCharacter(PLAYER);
            PLAYER.HpBarInit(PlayerStateBar);

            PLAYER.BuffHUDInit(PlayerStateBar.buffHUD);
            PLAYER.BuffSystem?.Init(PLAYER);

            MenuSystem.itemTab.OnItemAcqst += ItemAcqstTab.HandleItemAcqst;
            MenuSystem.init(PLAYER);

            MenuSystem.quickSlotBar = QuickSlotBar;
            DimmerImage.SetActive(false);

            GameShard.Instance.InputManager.KeyinPutUiEventData += inPutUiEvent;

            await UniTask.Yield(); // 필요시 프레임 분산
        }
        protected void inPutUiEvent(KeyCode type)
        {
            //if (Shared.Instance.UIManager.ActivePopupState(UI_TITLE_POPUP_LIST.Option)) 
            //{
            //    return;
            //}

            switch (type)
            {
                case KeyCode.I:
                    //MenuSystem.invenOpen();
                    MenuSystem.InputUiOpen(MenuSystem.UITYPE.Inventory);
                    break;
                case KeyCode.K:
                    MenuSystem.InputUiOpen(MenuSystem.UITYPE.Skill);
                    break;
                case KeyCode.P:
                    MenuSystem.InputUiOpen(MenuSystem.UITYPE.Quest);
                    break;
                case KeyCode.L:
                    MenuSystem.InputUiOpen(MenuSystem.UITYPE.Stat);
                    break;
                case KeyCode.Escape:

                    if (UiActiveSatckData.Count != 0)
                    {
                        GameObject top = UiActiveSatckData.Pop();
                        if (!top.activeSelf)
                        {
                            UiActiveSatckData.Clear();
                            MenuSystem.UiOut();
                        }
                        else 
                        {
                            top.SetActive(false);
                            MenuSystem.UiOut();
                        }
                    }
                    else 
                    {
                        MenuSystem.UiOut();
                        MenuSystem.EscMenuOpen();
                        MenuSystem.UiActive(true);
                    }

                    //if (MenuSystem.quickSlotBar != null && MenuSystem.quickSlotBar.picking)
                    //{
                    //    MenuSystem.quickSlotBar.CancelPick();
                    //    break;
                    //}

                    //if (MenuSystem.ToggleTab != null && MenuSystem.ToggleTab.activeSelf)
                    //{
                    //    MenuSystem.CloseInventory();        // ← 인벤만 닫기
                    //}

                    //else
                    //{
                    //    if (UiActiveSatckData.Count != 0)
                    //    {
                    //        GameObject top = UiActiveSatckData.Pop();
                    //        top.SetActive(false);
                    //        return;
                    //    }
                    //    else 
                    //    {
                    //        GameShard.Instance.InputManager.isUIOpen = false;
                    //        GameShard.Instance.MonsterManager.UiStateUpdate(false);
                    //    }
                    //        //MenuSystem.EscMenuOpen();           // 다른 UI/게임 메뉴 처리 기존대로
                    //}
                    break;

                default:
                    //MenuSystem.PopUi(type);                 // (필요 시) 다른 단축키 처리
                    break;
            }
            if (UiActiveSatckData.Count == 0) 
            {
                MenuSystem.UiActive(false);
            }

        }
        public void ShowSkillCooldown(SKILL_ID_TYPE type, float cooldown)
        {
            Debug.Log($"{type} {cooldown}");
        }
            
        public IEnumerator FadeEvent(bool _state) 
        {
            FadeEffect.transform.SetAsLastSibling();
            yield return StartCoroutine(FadeEffect.Fade(_state));
        }

        private void EnsureScaler(Canvas c, int w, int h)
        {
            if (!c)
                return;

            var scaler = c.GetComponent<CanvasScaler>();
            if (!scaler)
                scaler = c.gameObject.AddComponent<CanvasScaler>();

            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(w, h);
        }

        public void SetMenuMode(bool isMenuOpen)
        {
            // 메뉴가 열리면: HUD/팝업 감춤, 메뉴 캔버스만 표시
            if (MainUICanvas) 
                MainUICanvas.enabled = !isMenuOpen;

            if (PopupCanvas) 
                PopupCanvas.enabled = !isMenuOpen;
        }

        //public IEnumerator FadeViewEvent(bool _state) 
        //{
        //    //await StartCoroutine(FadeEffect.Fade(_state));
        //    yield return StartCoroutine(FadeEffect.Fade(_state));
        //}

        public Canvas ReturnMenuCanvas()
        {
            return MenuCanvas;
        }

        private void OnDestroy()
        {
            if (MainUICanvas) Destroy(MainUICanvas.gameObject);
            if (PopupCanvas) Destroy(PopupCanvas.gameObject);
            if (MenuCanvas) Destroy(MenuCanvas.gameObject);
        }
    }
}
