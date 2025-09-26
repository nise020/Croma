
//using static Delegate_Base;
//public class Info
//{
//    public UI_OPTION_TYPE MyType { get; set; }
//    public INFO_OPTION_DATA MyData { get; set; }
//    public Enum MyKey { get; set; }
//    public object MyValue { get; set; }
//    public List<GameObject> MyObj { get; set; }
//    public int Index { get; set; }
//}
//public class OptionContent_Base : MonoBehaviour
//{
//    public Info dataInfo = new();

//    protected void Init_Option()
//    {
//        dataInfo.MyData = StringToEnum<INFO_OPTION_DATA>(transform.parent.parent.parent.name);
//        //dataInfo.MyValue = Shared.Instance.SettingManager.OptionData[dataInfo.MyData][dataInfo.MyKey];
//    }
//    protected void Set_Key()
//    {
//        if (keyActionMap.TryGetValue(dataInfo.MyType, out var action)) action(this);
//    }
//    protected void Init_Value()
//    {
//        if (valueActionMap.TryGetValue(dataInfo.MyType, out var action)) action(this);
//    }

//    public void Type_Button_Set_Prev()
//    {
//        Debug.Log("Prev");
//    }
//    public void Type_Button_Set_Next()
//    {
//        Debug.Log("Next");
//    }

//    bool isWrite = false;
//    public void Type_Key_Set()
//    {
//        if (!isWrite) StartCoroutine(GetAnyKey());
//    }
//    IEnumerator GetAnyKey()
//    {
//        isWrite = true;
//        Shared.Instance.UIManager.PopupDimActivation(true);
//        while (true)
//        {
//            if (Input.anyKeyDown)
//            {
//                foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
//                {
//                    if (Input.GetKeyDown(kcode))
//                    {
//                        Shared.Instance.DataManager.Set<Option_Data>(Enums.EnumToCustomString(INFO_OPTION_DATA.KeyData), Enums.EnumToCustomString(dataInfo.MyKey), 1, kcode);
//                        dataInfo.MyObj[0].GetComponent<Image>().sprite = KeyIconFinder(kcode);
//                        isWrite = false;
//                        Shared.Instance.UIManager.PopupDimActivation(false);
//                        yield break;
//                    }
//                }
//            }
//            yield return null;
//        }
//    }
//    public void Type_Slider_Set()
//    {

//    }
//    public void Type_Dropdown_Set()
//    {

//    }

//    public void Type_Button_Init()
//    {
//        string text;
//        if (TryEnumOptionToString(dataInfo.MyValue, out string result)) text = result;
//        else text = dataInfo.MyValue.ToString();

//        var stringEvent = dataInfo.MyObj[0].transform.GetComponent<LocalizeStringEvent>();
//        stringEvent.StringReference.TableReference = "Language";
//        stringEvent.StringReference.TableEntryReference = text;
//        stringEvent.RefreshString();
//    }
//    public void Type_Key_Init() => dataInfo.MyObj[0].GetComponent<Image>().sprite = KeyIconFinder((KeyCode)dataInfo.MyValue);
//    public void Type_Slider_Init()
//    {
//        var slider = dataInfo.MyObj[0].GetComponent<Slider>();
//        var inputField = dataInfo.MyObj[1].GetComponent<TMP_InputField>();

//        Debug.Log(dataInfo.MyKey.ToString());
//        var value = int.Parse(Shared.Instance.DataManager.Get<Option_Data>
//            (Enums.EnumToCustomString(dataInfo.MyData),
//            dataInfo.MyKey.ToString(), 3).ToString());

//        if (value > 1) slider.wholeNumbers = true;
//        slider.maxValue = value;
//        slider.value = int.Parse(dataInfo.MyValue.ToString());
//        inputField.text = slider.value.ToString();

//        slider.onValueChanged.AddListener((val) => {
//            inputField.text = val.ToString(slider.wholeNumbers ? "F0" : "F2");
//        });

//        inputField.onEndEdit.AddListener((text) => {
//            if (float.TryParse(text, out float val))
//            {
//                if (val > slider.maxValue)
//                {
//                    inputField.text = slider.maxValue.ToString();
//                    slider.value = slider.maxValue;
//                }
//                else if (val < slider.minValue)
//                {
//                    inputField.text = slider.minValue.ToString();
//                    slider.value = slider.minValue;
//                }
//                else
//                {
//                    val = Mathf.Clamp(val, slider.minValue, slider.maxValue);
//                    slider.value = val;
//                }
//            }
//            else inputField.text = slider.value.ToString(slider.wholeNumbers ? "F0" : "F2");
//        });

//        inputField.text = slider.value.ToString(slider.wholeNumbers ? "F0" : "F2");

//    }

//    public void Type_Dropdown_Init()
//    {
//        var dropDown = dataInfo.MyObj[0].GetComponent<TMP_Dropdown>();
//        dropDown.ClearOptions();
//        var list = Shared.Instance.SettingManager.ResolutionList[(Enum)Shared.Instance.SettingManager.OptionData[INFO_OPTION_DATA.VideoData][OPTION_DATA_KEYS_VIDEO.Aspect_Ratio]];
//        dropDown.AddOptions(list);
//        dropDown.value = GetResolutionIdx();
//    }
//    private int GetResolutionIdx()
//    {
//        var list = Shared.Instance.SettingManager.ResolutionList[(Enum)Shared.Instance.SettingManager.OptionData[INFO_OPTION_DATA.VideoData][OPTION_DATA_KEYS_VIDEO.Aspect_Ratio]];
//        return list.FindIndex(s => 
//        s.Equals((string)Shared.Instance.SettingManager.OptionData[INFO_OPTION_DATA.VideoData][OPTION_DATA_KEYS_VIDEO.Resolution],
//        StringComparison.OrdinalIgnoreCase));
//    }
//    private Sprite KeyIconFinder(KeyCode _KeyCode)
//    {
//        string path = "", keyStr = _KeyCode.ToString();

//        if (!_KeyCode.ToString().Contains("Mouse")) path = "Key_" + keyStr;
//        else path = "Mouse_" + keyStr;
//        Sprite icon = Shared.Instance.AtlasManager.Get(CONFIG_ATLAS_TYPE.Key_Icon, path);
//        return icon;
//    }
//}
