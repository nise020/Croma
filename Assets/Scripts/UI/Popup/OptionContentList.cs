using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using static Enums;

public class OptionContentList : MonoBehaviour
{
    //public List<Tuple<Transform, GameObject, Toggle>> Contents {  get; set; }
    //public int Count { get; private set; }

    //private readonly string[] targetSprite = new string[] { "Slot_Active", "Slot_DisActive" };

    //public void Init()
    //{
    //    Contents = new();
    //    for (int i = 0; i < transform.childCount; i++)
    //    {
    //        var child = transform.GetChild(i);
    //        child.GetComponent<OptionContent>().dataInfo.Index = i;

    //        var toggle = child.GetComponent<Toggle>();

    //        int idx = i;
    //        toggle.onValueChanged.AddListener((isOn) => Change(idx, isOn));
    //        toggle.group = GetComponent<ToggleGroup>();

    //        var dimArea = child.Find("DimArea").gameObject;
    //        dimArea.transform.SetAsLastSibling();

    //        Contents.Add(Tuple.Create(child, dimArea, toggle));
    //    }
    //    Count = Contents.Count;
    //}

    //void Change(int _Idx, bool _IsOn)
    //{
    //    if (!_IsOn) return;

    //    for (int i = 0; i < Count; i++)
    //    {
    //        var child = Contents[i].Item1.GetChild(0);
    //        if (i == _Idx)
    //        {
    //            OptionHandler.isContent = true;
    //            OptionHandler.SetMap(_Idx);
    //            Contents[i].Item2.SetActive(false);
    //            child.GetComponent<Image>().sprite = Shared.Instance.AtlasManager.Get(CONFIG_ATLAS_TYPE.Icon, targetSprite[0]);
    //        }
    //        else
    //        {
    //            Contents[i].Item2.SetActive(true);
    //            child.GetComponent<Image>().sprite = Shared.Instance.AtlasManager.Get(CONFIG_ATLAS_TYPE.Icon, targetSprite[1]);
    //            Contents[i].Item1.GetComponent<OptionPointerEvent>().OnPointerExit(null);
    //        }
    //    }
    //}
}
