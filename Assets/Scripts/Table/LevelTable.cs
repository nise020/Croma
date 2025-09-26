using System.Collections.Generic;
using System;
using UnityEngine;

public class LevelTable : Table_Base
{
    [Serializable]
    public class Info
    {
        public int Id;
        public int Level;
        public int MaxHp;
        public int Atk;
        public int Def;
        public int Speed;
        public int TotalExp;
        public int SkillPoint;
        public int StatPoint;
    }

    public Dictionary<int, Info> LevelTableData = new Dictionary<int, Info>();

    public Info Get(int _Id)
    {
        if (LevelTableData.ContainsKey(_Id))
            return LevelTableData[_Id];
        return null;
    }

    public void Init_Binary(string _Name)
    {
        Load_Binary<Dictionary<int, Info>>(_Name, ref LevelTableData);
    }

    public void Save_Binary(string _Name)
    {
        Save_Binary(_Name, LevelTableData);
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

            LevelTableData.Add(info.Id, info);
        }
    }
    protected bool Read(CSVReader _Reader, Info _info, int _Row, int _Col)
    {
        if (_Reader.reset_row(_Row, _Col) == false) return false;
        _Reader.get(_Row, ref _info.Id);
        _Reader.get(_Row, ref _info.Level);
        _Reader.get(_Row, ref _info.MaxHp);
        _Reader.get(_Row, ref _info.Atk);
        _Reader.get(_Row, ref _info.Def);
        _Reader.get(_Row, ref _info.Speed);
        _Reader.get(_Row, ref _info.TotalExp);
        _Reader.get(_Row, ref _info.SkillPoint);
        _Reader.get(_Row, ref _info.StatPoint);



        return true;
    }
}
