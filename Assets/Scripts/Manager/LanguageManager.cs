using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using static LanguageTable;

public class LanguageManager : MonoBehaviour
{
    public LANGUEGE_TYPE lANGUEGE_TYPE { get; set; } = LANGUEGE_TYPE.Ko;
    public Action LanguageChangeEvent { get; set; }

    public async UniTask Init()
    {

        await UniTask.Yield();
    }

    public void TaxtChange(LANGUEGE_TYPE _type)
    {
        //var table1 = Shared.Instance.DataManager.Language_Table.Get(Stageid);

        //if (_type == LANGUEGE_TYPE.Ko)
        //{
        //    //lodingTxt.text = table1.Ko;
        //}
        //else if (_type == LANGUEGE_TYPE.En)
        //{
        //    //lodingTxt.text = table1.En;
        //}
    }

}
