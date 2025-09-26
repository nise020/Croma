using System.Collections.Generic;
using System;
using UnityEngine;

public class RewardTable : Table_Base
{
    [Serializable]
    public class Info
    {
        public int Id;
        public int ItemId;
        public int ItemCount;
        public int Exp;
        public int Score;
    }

    public Dictionary<int, Info> RewardDictionary = new Dictionary<int, Info>();

    public Info Get(int _Id)
    {
        if (RewardDictionary.ContainsKey(_Id))
            return RewardDictionary[_Id];
        return null;
    }
    public void Init_Binary(string _Name)
    {
        Load_Binary<Dictionary<int, Info>>(_Name, ref RewardDictionary);
    }

    public void Save_Binary(string _Name)
    {
        Save_Binary(_Name, RewardDictionary);
    }

    public void Init_Csv(string _Name, int StartRoe, int _StartCol)
    {
        CSVReader reader = GetCSVReader(_Name);
        if (reader == null) { return; }

        RewardDictionary.Clear();
        for (int row = StartRoe; row < reader.row; ++row)
        {
            Info info = new Info();

            if (Read(reader, info, row, _StartCol) == false)
                break;

            RewardDictionary.Add(info.Id, info);
        }
    }
    protected bool Read(CSVReader _Reader, Info _info, int _Row, int _Col)
    {
        if (_Reader.reset_row(_Row, _Col) == false) return false;
        _Reader.get(_Row, ref _info.Id);
        _Reader.get(_Row, ref _info.ItemId);
        _Reader.get(_Row, ref _info.ItemCount);
        _Reader.get(_Row, ref _info.Exp);
        _Reader.get(_Row, ref _info.Score);

        return true;
    }
}
