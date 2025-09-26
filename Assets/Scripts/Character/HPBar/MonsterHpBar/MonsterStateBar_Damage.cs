using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Enums;

public partial class MonsterStateBar : StateBar
{
    [SerializeField] protected Transform Place_1 = null;
    [SerializeField] protected Transform Place_10 = null;
    [SerializeField] protected Transform Place_100 = null;
    [SerializeField] protected Transform Place_1000 = null;
    [SerializeField] protected Transform Place_10000 = null;

    [SerializeField] protected GameObject DamageBarObj;
    protected List<GameObject> numberImages_1 { get; set; } = new();
    protected List<GameObject> numberImages_10 { get; set; } = new();
    protected List<GameObject> numberImages_100 { get; set; } = new();
    protected List<GameObject> numberImages_1000 { get; set; } = new();
    protected List<GameObject> numberImages_10000 { get; set; } = new();
    protected void DamageSetting(GameObject _obj)
    {
        Place_1 = _obj.transform.Find("1");
        Place_10 = _obj.transform.Find("10");
        Place_100 = _obj.transform.Find("100");
        Place_1000 = _obj.transform.Find("1000");
        Place_10000 = _obj.transform.Find("10000");

        numberImages_1 = DamageTransformLoad(Place_1);
        numberImages_10 = DamageTransformLoad(Place_10);
        numberImages_100 = DamageTransformLoad(Place_100);
        numberImages_1000 = DamageTransformLoad(Place_1000);
        numberImages_10000 = DamageTransformLoad(Place_10000);

        numberImages_1 = DamageStting(numberImages_1);
        numberImages_10 = DamageStting(numberImages_10);
        numberImages_100 = DamageStting(numberImages_100);
        numberImages_1000 = DamageStting(numberImages_1000);
        numberImages_10000 = DamageStting(numberImages_10000);


        Place_1.gameObject.SetActive(false);
        Place_10.gameObject.SetActive(false);
        Place_100.gameObject.SetActive(false);
        Place_1000.gameObject.SetActive(false);
        Place_10000.gameObject.SetActive(false);
    }

    protected List<GameObject> DamageStting(List<GameObject> _list) 
    {
        for (int iNum = 0; iNum < _list.Count; iNum++) 
        {
            Image image = _list[iNum].GetComponent<Image>();

           Sprite sprite = Shared.Instance.AtlasManager.Get(CONFIG_ATLAS_TYPE.Damage_Icon
                ,$"Damage_{iNum}");

            image.sprite = sprite;
        }


         return _list;
    }

    protected List<GameObject> DamageTransformLoad(Transform _transform)
    {
        var children = _transform.GetComponentsInChildren<Transform>()
        .Where(child => child != _transform) 
        .OrderBy(child => int.Parse(child.name))
        .Select(child => child.gameObject)
        .ToList();

        return children;
    }
    protected void DamageImageActive(int _value)
    {
        Transform[] places = { Place_1, Place_10, Place_100, Place_1000, Place_10000 };
        List<GameObject>[] numberImages = { numberImages_1, numberImages_10, numberImages_100, numberImages_1000, numberImages_10000 };

        if (_value <= 0) 
        {
            _value = 0;
        }

        for (int i = 0; i < places.Length; i++)
        {
            places[i].gameObject.SetActive(false);

            for (int j = 0; j < numberImages[i].Count; j++)
                numberImages[i][j].SetActive(false);
        }

        List<GameObject> result = new();

        int value = _value;
        for (int i = 0; i < places.Length; i++)
        {
            int digit = value % 10;
            if (value > 0 || i == 0) 
            {
                places[i].gameObject.SetActive(true);
                numberImages[i][digit].SetActive(true);

                result.Add(numberImages[i][digit]);
                result.Add(places[i].gameObject);
            }
            value /= 10;
            if (value == 0) break; // 남은 값 없으면 중단
        }

        StartCoroutine(imageHide(result, 0.5f));
    }
    protected IEnumerator imageHide(List<GameObject> _lists, float _timer)
    {
        yield return new WaitForSeconds(_timer);
        foreach (GameObject _list in _lists) _list.SetActive(false);
    }
}
