using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using static Enums;
using static LanguageTable;

public class UiManager : Manager_Base
{
    public struct PopupInfo
    {
        public GameObject PopupCanvas { get; set; }
        public GameObject PopupDim { get; set; }
        public GameObject Popup { get; set; }

        public Dictionary<UI_TITLE_POPUP_LIST, GameObject> PopupList { get; set; }
        public UI_POPUP_STATE PopupActivation { get; set; }
    }

    public PopupInfo popupInfo = new();
    public GameObject MainCanvas { get; private set; }
    public GameObject PopupCanvas { get; private set; }

    // Inventory
    public GameObject inventory;

    public Canvas FadeCanvas { get; private set; }

    public FadeEffect FadeObj;
    public Image lodingImag;
    public Transform lodingImagParent;
    public Button NextStageBtn;
    private int Stageid = (int)SCENE_SCENES.BamserMap_Stage_1;
    private TMP_Text lodingTxt;
    public bool isInvenOpen = false;

    [Header("TextEvent")]
    private bool textFinished = false;
    private bool textStarted = false;
    [Header("LodingEvent")]
    private bool gaugeInitialized = false;


    public async override UniTask Initialize(string _str)
    {
        await base.Initialize(_str);

        Initialized(_str);
    }
    public void StageStart() 
    {
        lodingTxt.text = "";
        lodingTxt.gameObject.SetActive(false);

        lodingImag.fillAmount = 0.0f;

        textStarted = false;
        textFinished = false;

        lodingImagParent.gameObject.SetActive(false);
        NextStageBtn.gameObject.SetActive(false);
        Shared.Instance.UIManager.FadeEvent(false).Forget();
        if (GameShard.Instance != null&& GameShard.Instance.StageManager!= null) 
        {
            GameShard.Instance.StageManager.StageSetting().Forget();
        }
       
    }
    private void fadeCanvasCreat()
    {
        GameObject go = Shared.Instance.ResourcesManager.FadeCanvasPrefab;
        go = Instantiate(go);

        //GameObject go = GameObject.Find("Fade_Canvas");
        FadeCanvas = go.GetComponent<Canvas>();
        DontDestroyOnLoad(FadeCanvas);

        FadeObj = FadeCanvas.GetComponentInChildren<FadeEffect>(true);

        GameObject gos = GameObject.Find("BossMonsterHpGauge1_Image");

        if (gos != null) 
        {
            lodingImag = gos.GetComponent<Image>();
            //lodingImag.fillAmount = 0;
            lodingImagParent = lodingImag.gameObject.transform.parent.parent;
            lodingImagParent.gameObject.SetActive(false);
        }

        GameObject btn = GameObject.Find("Next_Btn");
        NextStageBtn = btn.GetComponent<Button>();
        NextStageBtn.onClick.AddListener(StageStart);
        NextStageBtn.gameObject.SetActive(false);

        GameObject txt = GameObject.Find("Load_Text");
        lodingTxt = txt.GetComponent<TMP_Text>();
        lodingTxt.gameObject.SetActive(false);
    }

    public void GetMainCanvas(GameObject[] _Objs) => MainCanvas = FindObject(_Objs, "Title_Canvas");

    public void GetPopupCanvas(GameObject[] _Objs) => PopupCanvas = FindObject(_Objs, "Popup_Canvas");
    public void InitPopup()
    {
        fadeCanvasCreat();

        var objs = Resources.FindObjectsOfTypeAll<GameObject>();

        GetMainCanvas(objs);

        popupInfo.PopupCanvas = FindObject(objs, "Popup_Canvas");
        popupInfo.Popup = FindObject(objs, "Option_Panel");
        DontDestroyOnLoad(popupInfo.PopupCanvas);
        UpdatePopupActivation(false);

        popupInfo.PopupList = new();
        foreach (UI_TITLE_POPUP_LIST list in Enum.GetValues(typeof(UI_TITLE_POPUP_LIST)))
        {
            var popup = popupInfo.Popup.transform.Find(Enums.EnumToCustomString(list))?.gameObject;
            if (popup != null) popupInfo.PopupList.Add(list, popup);
        }

        popupInfo.PopupDim = popupInfo.Popup.transform.GetChild(0).gameObject;
        popupInfo.PopupDim.transform.SetAsLastSibling();
        PopupDimActivation(false);

    }

    private async UniTask Textevent(int _id)
    {
        if (textStarted) return;         
        textStarted = true;

        string fullText = "";

        var table = Shared.Instance.DataManager.Language_Table.Get(_id);

        if (table == null) 
        {
            textFinished = true;
            return;
        }


        if (Shared.Instance.LanguageManager.lANGUEGE_TYPE == LANGUEGE_TYPE.Ko)
        {
            fullText = table.Ko;//Test
        }
        else if (Shared.Instance.LanguageManager.lANGUEGE_TYPE == LANGUEGE_TYPE.En)
        {
            fullText = table.En;//Test
        }
 
        lodingTxt.gameObject.SetActive(true);

        int index = 0;
        int length = fullText.Length;

        while (index <= length)
        {
            lodingTxt.text = fullText.Substring(0, index);
            index++;
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
        }

        await UniTask.Delay(TimeSpan.FromSeconds(1.0f));

        textFinished = true;
    }
    public async UniTask<bool> UpdateLoadingBar(float _check,int _id)
    {
        if (lodingImag == null)
        {
            GameObject gos = GameObject.Find("BossMonsterHpGauge1_Image");
            lodingImag = gos.GetComponent<Image>();
            lodingImagParent = lodingImag.transform.parent.parent;

            if (!gaugeInitialized)
            {
                lodingImag.fillAmount = 0f;
                gaugeInitialized = true;
            }
        }
        else
        {
            lodingImagParent.gameObject.SetActive(true);
        }
        Stageid = _id;
        await Textevent(_id);

        while (lodingImag.fillAmount < _check - 0.001f)
        {
            lodingImag.fillAmount = Mathf.MoveTowards(
                lodingImag.fillAmount,
                _check,
                Time.deltaTime * 0.5f
            );
            await UniTask.Yield();
        }

        if (Mathf.Approximately(_check, 1f))
        {
            await UniTask.WaitUntil(() => textFinished == true);

            StageStart();
            //NextStageBtn.gameObject.SetActive(true);
            return true;
        }

        return false;
    }
    public async UniTask FadeEvent(bool _value) 
    {
       await FadeObj.Fade(_value);
    }
    public void ActivePopup(bool _Active, UI_TITLE_POPUP_LIST _State)
    {
        UpdatePopupActivation(_Active);

        //if (Shared.Instance.SceneManager.CurrentScene == SCENE_SCENES.Title) MainCanvas.SetActive(!_Active);

        if (_State != UI_TITLE_POPUP_LIST.None) popupInfo.PopupList[_State].SetActive(_Active);
        else foreach (var go in popupInfo.PopupList) go.Value.SetActive(false);
        if (GameShard.Instance != null) 
        {
            GameShard.Instance.GameUiManager.UiActiveSatckData.Push(popupInfo.PopupList[_State]);
        }
    }
    public bool ActivePopupState(UI_TITLE_POPUP_LIST _State)
    {
        if (_State != UI_TITLE_POPUP_LIST.None) 
        {
            if (popupInfo.PopupList[_State].activeSelf) 
            {
                popupInfo.PopupList[_State].SetActive(false);
                return true;
            }
        }
        return false;
    }
    private void UpdatePopupActivation(bool _Active)
    {
        popupInfo.PopupCanvas.SetActive(_Active);
        popupInfo.PopupActivation = _Active ? UI_POPUP_STATE.Activate : UI_POPUP_STATE.Deactivate;
    }
    public void PopupDimActivation(bool _Active) => popupInfo.PopupDim.SetActive(_Active);

    internal async Task LodingText()
    {

        await UniTask .Yield();
    }
    private IEnumerator PopupActivation()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Escape)) ActivePopup(!popupInfo.PopupCanvas.activeInHierarchy, UI_TITLE_POPUP_LIST.None);
            yield return null;
        }
    }

    // Inventory
    //public void InitInventory()
    //{
    //    if (inventory == null)
    //    {
    //        GameObject prefab = Resources.Load<GameObject>("Prefab/UI/Inventory/Inventory");
    //        if (prefab == null)
    //        {
    //            Debug.LogError("Inventory prefab not found at Resources/Prefab/UI/Inventory/Inventory");
    //            return;
    //        }
    //        if (PopupCanvas == null)
    //        {
    //            Debug.Log("Popup null");
    //        }

    //        inventory = Instantiate(prefab, PopupCanvas.transform);
    //        inventory.SetActive(true);
    //    }
    //}

    //public void OpenInventory()
    //{
    //    if (inventory == null)
    //        InitInventory();

    //    isInvenOpen = !isInvenOpen;
    //    inventory.SetActive(isInvenOpen);

    //    GameShard.Instance.InputManager.isUIOpen = isInvenOpen;
    //}

    // force quit
    //public void CloseInventory()
    //{
    //    if (inventory != null && inventory.activeSelf)
    //        inventory.SetActive(false);
    //}

  

    //public void DeleteLoadingCanvas() => Destroy(GameObject.Find("Loading_Canvas").gameObject);
    //public void StartPopupActivation() => StartCoroutine(PopupActivation());
}
