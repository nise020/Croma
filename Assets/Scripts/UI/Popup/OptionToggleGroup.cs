using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using TMPro.Examples;
using System;
using static Enums;

//public class OptionToggleGroup : MonoBehaviour, ISceneInitializable
//{
//    [SerializeField] private TMP_FontAsset[] fontAssets = null;
//    [ReadOnly] public List<Toggle> toggles = new();
//    public Transform panelGroup = null;
//    public List<Tuple<GameObject,OptionContentList>> TogglePanels { get; set; }

//    private Vector2 targetSize = new(410, 140), defaultSize = new(370, 120);
//    private readonly string[] targetSprite = new string[] { "MenuSlot_Active", "MenuSlot_DisActive" };

//    [ReadOnly] public int currIdx = 0;
//    public UniTask Init()
//    {
//        TogglePanels = new();
//        foreach (Transform child in transform)
//        {
//            if (child.TryGetComponent(out Toggle toggle))
//            {
//                toggles.Add(toggle);
//                toggle.onValueChanged.AddListener((isOn) => OnToggleChanged(toggles.IndexOf(toggle), isOn));
//            }
//        }
//        foreach (Transform child in panelGroup)
//        {
//            TogglePanels.Add(Tuple.Create(child.gameObject, child.GetChild(0).GetChild(0).GetComponent<OptionContentList>()));
//            child.gameObject.SetActive(false);
//        }
//        toggles[0].isOn = true;
//        return UniTask.CompletedTask;
//    }
//    private void OnToggleChanged(int _Index, bool _IsOn)
//    {
//        if(!_IsOn) return;
//        foreach (var toggle in toggles)
//        {
//            OptionHandler.isContent = false;
//            var idx = toggles.IndexOf(toggle);
//            if (idx == _Index) currIdx = _Index;
//            TogglePanels[idx].Item1.SetActive(idx == _Index);
//            var child = toggle.transform.GetChild(0);

//            var atlas = Shared.Instance.AtlasManager;
//            child.GetComponent<Image>().sprite =
//                idx == _Index ? atlas.Get(CONFIG_ATLAS_TYPE.Icon, targetSprite[0]) :
//                atlas.Get(CONFIG_ATLAS_TYPE.Icon, targetSprite[1]);

//            child.GetComponent<RectTransform>().sizeDelta =
//                child.GetChild(0).GetComponent<RectTransform>().sizeDelta =
//                idx == _Index ? targetSize : defaultSize;

//            toggle.GetComponentInChildren<TMP_Text>().font = idx == _Index ? fontAssets[1] : fontAssets[0];
//        }
//    }
//}
