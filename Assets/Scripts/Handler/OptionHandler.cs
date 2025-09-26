using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Enums;
using static LanguageTable;

public class OptionHandler : MonoBehaviour
{

    [SerializeField] private Toggle[] toggles;
    //float soundValue = 0.0f;
    //float effectValue = 0.0f;
    //float mouseValue = 0.0f;
    [SerializeField] GameObject optionObj;
    [SerializeField] Slider cameraRotSlider;
    [SerializeField] Slider BgmVolumeSlider;
    [SerializeField] Slider EffectVolumeSlider;

    [Header("Langauge")]
    [SerializeField] private Button leftBtn;
    [SerializeField] private Button rightBtn;
    [SerializeField] private TMP_Text langLabel;
    private int langIndex = 0;
    private LANGUEGE_TYPE[] langOptions;

    private void Start()
    {
        int value = PlayingDataLoad("Play");

        if (value != 0)
        {
            Transform option = gameObject.transform.Find("Option");
            optionObj = option.gameObject;


            float db1 = OptionDataLoad("MouseSpeed");
            ApplyMouseSpeed(db1).Forget();

            float speed = Mathf.Lerp(0.0f, 1.0f, db1);
            cameraRotSlider.value = speed;

            float db2 = OptionDataLoad("Bgm");
            BgmVolumeSlider.value = db2;
            Shared.Instance.SoundManager.audioMixer.SetFloat(SOUND_VOLUME.BgmVolume.ToString(), db2);

            float db3 = OptionDataLoad("Effect");
            EffectVolumeSlider.value = db3;
            Shared.Instance.SoundManager.audioMixer.SetFloat(SOUND_VOLUME.EffectVolume.ToString(), db3);

            Debug.Log($"MouseSpeed = {db1},Bgm = {db2}, Effect = {db3}");

            langOptions = (LANGUEGE_TYPE[])System.Enum.GetValues(typeof(LANGUEGE_TYPE));

            int saved = (int)LanguageDataDataLoad("Language");
            if (saved < 0 || saved > langOptions.Length) saved = 0;
            langIndex = saved;

            UpdateLanguageLabel();

            // 버튼 이벤트
            if (leftBtn) leftBtn.onClick.AddListener(() => ShiftLangauge(-1));
            if (rightBtn) rightBtn.onClick.AddListener(() => ShiftLangauge(+1));
            #region Memo
            //if (db1 != 0)
            //{
            //    cameraRotSlider.value = db1;
            //    Shared.Instance.SoundManager.audioMixer.SetFloat(SOUND_VOLUME.BgmVolume.ToString(), db1);
            //}
            //if (db2 != 0)
            //{
            //    BgmVolumeSlider.value = db2;
            //    Shared.Instance.SoundManager.audioMixer.SetFloat(SOUND_VOLUME.EffectVolume.ToString(), db2);
            //}
            //if (db3 != 0)
            //{
            //    EffectVolumeSlider.value = db3;
            //    ApplyMouseSpeed(db3).Forget();
            //}
            //if (lANGUEGE_TYPE != LANGUEGE_TYPE.None)
            //{
            //    Shared.Instance.LanguageManager.lANGUEGE_TYPE = lANGUEGE_TYPE;
            //}
            #endregion

        }
        else 
        {
            PlayingHistorygSave("Play",1).Forget();
        }
       
    }
    public enum SOUND 
    {
        Bgm,
        Effect,
    }

    public enum SOUND_VOLUME
    {
        BgmVolume,
        EffectVolume,
        MasterVolume,
    }
    public void OptionOff() 
    {
        optionObj.SetActive(false);
    }
    public void OnDropdownValueChanged(int index)
    {
        LanguageDataSave("Language", index).Forget();
        Shared.Instance.LanguageManager.lANGUEGE_TYPE = (LANGUEGE_TYPE)index;
        Shared.Instance.LanguageManager.LanguageChangeEvent?.Invoke();
        //manager
    }

    public void SetBgmVolume(float value)
    {
        float dB = value > 0 ? Mathf.Log10(value) * 20 : -80f;
        Shared.Instance.SoundManager.audioMixer.SetFloat(SOUND_VOLUME.BgmVolume.ToString(), dB);
        VolumDataSave("Bgm", value).Forget();
    }

    public void SetEffectVolume(float value)
    {
        float dB = value > 0 ? Mathf.Log10(value) * 20 : -80f;
        Shared.Instance.SoundManager.audioMixer.SetFloat(SOUND_VOLUME.EffectVolume.ToString(), dB);
        VolumDataSave("Effect", value).Forget();
    }

    public void MouseSpeed(float value)
    {
        float speed = Mathf.Lerp(1f, 5f, value);
        ApplyMouseSpeed(speed).Forget();
        VolumDataSave("MouseSpeed", value).Forget();
    }

    private async UniTask ApplyMouseSpeed(float speed)
    {
        float setSpeed = speed;
        await UniTask.WaitUntil(() => GameShard.Instance != null && GameShard.Instance.followCamera3D != null);

        GameShard.Instance.followCamera3D.rotSensitive = setSpeed;
    }
    public async UniTask PlayingHistorygSave(string _Key, int _Value) 
    {
        PlayerPrefs.SetInt(_Key, _Value);
        PlayerPrefs.Save();
        await UniTask.CompletedTask;
    }

    public async UniTask VolumDataSave(string _Key, float _Value)
    {
        PlayerPrefs.SetFloat(_Key, _Value);
        PlayerPrefs.Save();
        await UniTask.CompletedTask;
    }
  
    public async UniTask MouseSpeedDataSave(string _Key, float _Value)
    {
        PlayerPrefs.SetFloat(_Key, _Value);
        PlayerPrefs.Save();
        await UniTask.CompletedTask;
    }
    public async UniTask LanguageDataSave(string _Key, float _Value)
    {
        PlayerPrefs.SetFloat(_Key, _Value);
        PlayerPrefs.Save();
        await UniTask.CompletedTask;
    }

    public float LanguageDataDataLoad(string _Key)
    {
        return PlayerPrefs.GetFloat(_Key);
    }
    public float OptionDataLoad(string _Key)
    {
        return PlayerPrefs.GetFloat(_Key);
    }
    public int PlayingDataLoad(string _Key)
    {
        return PlayerPrefs.GetInt(_Key);
    }

    private void ShiftLangauge(int dir)
    {
        if (langOptions == null || langOptions.Length == 0) return;

        langIndex = (langIndex + dir + langOptions.Length) % langOptions.Length;
        ApplyLanguage(langOptions[langIndex]);
        UpdateLanguageLabel();
    }
    private void UpdateLanguageLabel()
    {
        if (!langLabel) return;
        langLabel.text = langOptions[langIndex].ToString();
    }

    private void ApplyLanguage(LANGUEGE_TYPE type)
    {
        // 저장
        LanguageDataSave("Language", (int)type).Forget();

        Shared.Instance.LanguageManager.lANGUEGE_TYPE = type;
        Shared.Instance.LanguageManager.LanguageChangeEvent?.Invoke();
    }
}
//    public OptionToggleGroup toggleGroup = null;

//    public static int currIdx;
//    public static Dictionary<int, int> selectedMap = new()
//    {
//        { 0, 0 },
//        { 1, 0 },
//        { 2, 0 },
//        { 3, 0 },
//        { 4, 0 },
//        { 5, 0 }
//    };

//    public static bool isContent = false;

//    public void ExitPopup() => Shared.Instance.UIManager.ActivePopup(false, UI_TITLE_POPUP_LIST.None);
//    public static void SetMap(int idx) => selectedMap[currIdx] = idx;

//    private void Update()
//    {
//        if (toggleGroup == null) return;

//        currIdx = toggleGroup.currIdx;
//        var uiManager = Shared.Instance.UIManager;

//        if (uiManager.popupInfo.PopupActivation != UI_POPUP_STATE.Activate)
//            return;

//        bool left = Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow);
//        bool right = Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow);
//        bool up = Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);
//        bool down = Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow);
//        bool enter = Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space);

//        if (left) isContent = false;
//        if (right) isContent = true;

//        int idx = toggleGroup.currIdx;
//        int selIdx = selectedMap[idx];

//        if (isContent)
//        {
//            var panel = toggleGroup.TogglePanels[idx];
//            var scrollRect = panel.Item1.GetComponent<ScrollRect>();
//            var obj = panel.Item2;
//            int count = obj.Count;

//            int prevSelIdx = selIdx;
//            if (up || down)
//            {
//                selIdx = up ? (selIdx == 0 ? count - 1 : selIdx - 1)
//                            : (selIdx == count - 1 ? 0 : selIdx + 1);

//                selectedMap[idx] = selIdx;

//                var prevToggle = obj.Contents[prevSelIdx].Item3;
//                var prevPointerEvent = prevToggle.GetComponent<OptionPointerEvent>();
//                if (prevPointerEvent != null)
//                    prevPointerEvent.OnPointerExit(null);

//                var selectedToggle = obj.Contents[selIdx].Item3;
//                var pointerEvent = selectedToggle.GetComponent<OptionPointerEvent>();
//                if (pointerEvent != null)
//                    pointerEvent.OnPointerEnter(null);

//                ScrollToSelectedSmooth(scrollRect, selectedToggle.GetComponent<RectTransform>());
//            }

//            if (enter)
//            {
//                var selectedToggle = obj.Contents[selIdx].Item3;
//                selectedToggle.isOn = true;
//            }
//        }
//        else
//        {
//            int toggleCount = toggleGroup.toggles.Count;

//            if (up || down)
//            {
//                int prevIdx = idx;
//                idx = up ? (idx == 0 ? toggleCount - 1 : idx - 1)
//                         : (idx == toggleCount - 1 ? 0 : idx + 1);

//                var prevToggle = toggleGroup.toggles[prevIdx];
//                var prevPointerEvent = prevToggle.GetComponent<OptionPointerEvent>();
//                if (prevPointerEvent != null)
//                    prevPointerEvent.OnPointerExit(null);

//                toggleGroup.currIdx = idx;

//                var toggle = toggleGroup.toggles[idx];
//                var pointerEvent = toggle.GetComponent<OptionPointerEvent>();
//                if (pointerEvent != null)
//                    pointerEvent.OnPointerEnter(null);
//            }

//            if (enter)
//            {
//                var toggle = toggleGroup.toggles[idx];
//                toggle.isOn = true;
//            }
//        }   
//    }
//    public void ScrollToSelectedSmooth(ScrollRect scrollRect, RectTransform target, float duration = 0.3f)
//    {
//        if (scrollRect == null || target == null || scrollRect.content == null)
//            return;

//        StartCoroutine(SmoothScrollTo(scrollRect, target, duration));
//    }

//    private IEnumerator SmoothScrollTo(ScrollRect scrollRect, RectTransform target, float duration)
//    {
//        Canvas.ForceUpdateCanvases();

//        RectTransform content = scrollRect.content;
//        RectTransform viewport = scrollRect.viewport;

//        Bounds contentBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(content, target);
//        Bounds viewportBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(content, viewport);

//        float contentHeight = content.rect.height;
//        float viewportHeight = viewport.rect.height;

//        float offset = contentBounds.center.y - viewportBounds.center.y;
//        float scrollOffset = offset / (contentHeight - viewportHeight);
//        float targetNormalizedPos = scrollRect.verticalNormalizedPosition - scrollOffset;
//        targetNormalizedPos = 1f - Mathf.Clamp01(targetNormalizedPos);

//        float startPos = scrollRect.verticalNormalizedPosition;
//        float elapsedTime = 0f;

//        while (elapsedTime < duration)
//        {
//            elapsedTime += Time.deltaTime;
//            float t = elapsedTime / duration;
//            t = Mathf.SmoothStep(0f, 1f, t); // 부드러운 곡선 적용
//            scrollRect.verticalNormalizedPosition = Mathf.Lerp(startPos, targetNormalizedPos, t);
//            yield return null;
//        }

//        scrollRect.verticalNormalizedPosition = targetNormalizedPos;
//    }

//}
