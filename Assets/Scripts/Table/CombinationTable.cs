using System.Collections.Generic;
using System;
using UnityEngine;

public class CombinationTable : Table_Base
{
    [Serializable]
    public class Info
    {
        public int Id;
        public int SkillId;
        public int Color_1;
        public int Color_2;
        public int Color_3;
    }

    public Dictionary<int, Info> CombinationTableData = new Dictionary<int, Info>();

    public Info Get(int _Id)
    {
        if (CombinationTableData.ContainsKey(_Id))
            return CombinationTableData[_Id];
        return null;
    }
    public void Init_Binary(string _Name)
    {
        Load_Binary<Dictionary<int, Info>>(_Name, ref CombinationTableData);
    }

    public void Save_Binary(string _Name)
    {
        Save_Binary(_Name, CombinationTableData);
    }

    public void Init_Csv(string _Name, int StartRoe, int _StartCol)
    {
        CSVReader reader = GetCSVReader(_Name);
        if (reader == null) { return; }

        for (int row = StartRoe; row < reader.row; ++row)
        {
            Info info = new Info();

            if (Read(reader, info, row, _StartCol) == false)
                break;

            CombinationTableData.Add(info.Id, info);
        }
    }
    protected bool Read(CSVReader _Reader, Info _info, int _Row, int _Col)
    {
        if (_Reader.reset_row(_Row, _Col) == false) return false;
        _Reader.get(_Row, ref _info.Id);
        _Reader.get(_Row, ref _info.SkillId);
        _Reader.get(_Row, ref _info.Color_1);
        _Reader.get(_Row, ref _info.Color_2);
        _Reader.get(_Row, ref _info.Color_3);

        return true;
    }
}
