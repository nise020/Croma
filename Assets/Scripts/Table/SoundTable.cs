using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundTable : Table_Base
{
    [Serializable]
    public class Info
    {
        public int Id;
        public int Type;
        public int Loop;
        public int Volume;
        public int Pitch;
        public int Priority;
        public int MinDistance;
        public int MaxDistance;
        public string Name;
        public string Path;
    }

    public Dictionary<int, Info> SoundTableData = new Dictionary<int, Info>();

    public Info Get(int id)
    {
        SoundTableData.TryGetValue(id, out var skill);
        return skill;
    }

    public void Init_Binary(string _Name)
    {
        Load_Binary<Dictionary<int, Info>>(_Name, ref SoundTableData);
    }

    public void Save_Binary(string _Name)
    {
        Save_Binary(_Name, SoundTableData);
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

            if (!Enum.IsDefined(typeof(SkILLTYPE), info.Type))
                continue;

            SoundTableData.Add(info.Id, info);
        }
    }
    protected bool Read(CSVReader _Reader, Info _info, int _Row, int _Col)
    {
        if (_Reader.reset_row(_Row, _Col) == false) return false;
        _Reader.get(_Row, ref _info.Id);
        _Reader.get(_Row, ref _info.Type);
        _Reader.get(_Row, ref _info.Loop);
        _Reader.get(_Row, ref _info.Volume);
        _Reader.get(_Row, ref _info.Pitch);
        _Reader.get(_Row, ref _info.Priority);
        _Reader.get(_Row, ref _info.MinDistance);
        _Reader.get(_Row, ref _info.MaxDistance);
        _Reader.get(_Row, ref _info.Name);
        _Reader.get(_Row, ref _info.Path);
        return true;
    }
}
