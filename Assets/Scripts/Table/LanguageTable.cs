using System;
using System.Collections.Generic;
using UnityEngine;

public class LanguageTable : Table_Base
{
    [Serializable]
    public class Info
    {
        public int Id;
        public string Ko;
        public string En;
    }
    public enum LANGUEGE_TYPE 
    {
        Ko = 0,
        En = 1,
    }
    public Dictionary<int, Info> LanguageTableData = new Dictionary<int, Info>();

    public Info Get(int _Id)
    {
        if (LanguageTableData.ContainsKey(_Id))
            return LanguageTableData[_Id];
        return null;
    }
    //public string Get(int _Id, LANGUEGE_TYPE _type)
    //{
    //    if (LanguageTableData.TryGetValue(_Id,out Info inof)) 
    //    {
    //        if (_type == LANGUEGE_TYPE.Ko)
    //        {
    //            return inof.Ko;
    //        }
    //        else if (_type == LANGUEGE_TYPE.En)
    //        {
    //            return inof.En;
    //        }
    //        else 
    //        {
    //            return null;
    //        }
    //    }
    //    return null;
    //}
    public void Init_Binary(string _Name)
    {
        Load_Binary<Dictionary<int, Info>>(_Name, ref LanguageTableData);
    }

    public void Save_Binary(string _Name)
    {
        Save_Binary(_Name, LanguageTableData);
    }

    public void Init_Csv(string _Name, int StartRoe, int _StartCol)
    {
        CSVReader reader = GetCSVReader(_Name);
        if (reader == null) { return; }

        for (int row = StartRoe; row < reader.row; ++row)
        {
            Info info = new Info();

            //StartRoe = ex.id,type
            //_StartCol = ex.100001,3

            //var columnIndexDict = new Dictionary<CHARACTER_DATA, int>();

            //// Çì´õ ÆÄ½Ì
            //string[] headers = reader.GetRow(StartRoe);
            //for (int i = 0; i < headers.Length; i++)
            //{
            //    if (Enum.TryParse<CHARACTER_DATA>(headers[i], true, out var result))
            //        columnIndexDict[result] = i;
            //}



            if (Read(reader, info, row, _StartCol) == false)
                break;

            LanguageTableData.Add(info.Id, info);
        }
    }
    protected bool Read(CSVReader _Reader, Info _info, int _Row, int _Col)
    {
        if (_Reader.reset_row(_Row, _Col) == false) return false;
        _Reader.get(_Row, ref _info.Id);
        _Reader.get(_Row, ref _info.Ko);
        _Reader.get(_Row, ref _info.En);

        return true;
    }
}
