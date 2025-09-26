using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;
using static Enums;

//public class OptionContentInitializer : MonoBehaviour, ISceneInitializable
//{
//    private readonly Dictionary<INFO_OPTION_DATA, Transform> contentDict = new();
//    private GameObject ContentBase { get; set; }
//    private List<GameObject> ContentType { get; set; }

//    public UniTask Init()
//    {
//        AsyncWrapper();
//        return UniTask.CompletedTask;
//    }
//    async void AsyncWrapper()
//    {
//        await GetDict();
//        await GetContent();
//        await CreateOptions();
//    }
//    UniTask GetDict()
//    {
//        foreach (Transform child in transform) contentDict.Add(StringToEnum<INFO_OPTION_DATA>(child.name), child.GetChild(0).GetChild(0));
//        return UniTask.CompletedTask;
//    }
//    UniTask GetContent()
//    {
//        ContentBase = Resources.Load<GameObject>("Prefab/UI/Option/Base");
//        if (ContentType == null) ContentType = new();
//        ContentType = Resources.LoadAll<GameObject>("Prefab/UI/Option/Type").OrderBy(obj => obj.name).ToList();
//        return UniTask.CompletedTask;
//    }

//    UniTask CreateOptions()
//    {
//        foreach (var option in contentDict)
//        {
//            foreach (var list in Shared.Instance.SettingManager.OptionData[option.Key])
//            {
//                var obj = Instantiate(ContentBase, option.Value);
//                obj.name = list.Key.ToString();

//                var data = obj.GetComponent<OptionContent>();
//                data.dataInfo.MyKey = list.Key;

//                var text = obj.transform.GetChild(1).GetComponent<LocalizeStringEvent>();
//                text.StringReference.TableReference = "Language";
//                text.StringReference.TableEntryReference = list.Key.ToString();
//                text.RefreshString();

//                GameObject type = null;
//                UI_OPTION_TYPE currType = UI_OPTION_TYPE.Button;
//                data.dataInfo.MyObj = new();

//                switch (list.Value)
//                {
//                    case int: case float: case byte: case ushort:
//                        currType = UI_OPTION_TYPE.Slider;
//                        type = Instantiate(ContentType[(int)currType], obj.transform);

//                        data.dataInfo.MyObj.Add(type.transform.GetChild(0).gameObject);
//                        data.dataInfo.MyObj.Add(type.transform.GetChild(1).gameObject);
//                        break;
//                    case bool:
//                    case OPTION_DATA_VALUES_SOUND_CHANNEL:
//                    case OPTION_DATA_VALUES_ANTI_ALIASING_MODE:
//                    case OPTION_DATA_VALUES_LANGUAGE:
//                    case OPTION_DATA_VALUES_ASPECTRATIO:
//                    case OPTION_DATA_VALUES_QUALITY:
//                    case OPTION_DATA_VALUES_DISPLAY_MODE:
//                        currType = UI_OPTION_TYPE.Button;
//                        type = Instantiate(ContentType[(int)currType], obj.transform);

//                        data.dataInfo.MyObj.Add(type.transform.GetChild(0).gameObject);
//                        var valueText = data.dataInfo.MyObj[0].GetComponent<LocalizeStringEvent>();
//                        data.dataInfo.MyObj[0].transform.GetChild(0).GetComponent<Button>().onClick.AddListener(data.Type_Button_Set_Prev);
//                        data.dataInfo.MyObj[0].transform.GetChild(1).GetComponent<Button>().onClick.AddListener(data.Type_Button_Set_Next);
//                        break;
//                    case KeyCode:
//                        currType = UI_OPTION_TYPE.Key;
//                        type = Instantiate(ContentType[(int)currType], obj.transform);

//                        data.dataInfo.MyObj.Add(type.transform.GetChild(0).gameObject);
//                        data.dataInfo.MyObj[0].GetComponent<Button>().onClick.AddListener(data.Active);
//                        break;
//                    case string:
//                        currType = UI_OPTION_TYPE.Dropdown;
//                        type = Instantiate(ContentType[(int)currType], obj.transform);

//                        data.dataInfo.MyObj.Add(type.transform.GetChild(0).gameObject);
//                        break;
//                    default: break;
//                }

//                obj.tag = Enums.EnumToCustomString(currType);
//                data.dataInfo.MyType = currType;
//                data.Init();
//            }
//            option.Value.GetComponent<OptionContentList>().Init();
//        }
//        return UniTask.CompletedTask;
//    }
//}
