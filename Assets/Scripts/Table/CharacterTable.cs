using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class CharacterTable : Table_Base
{
    [Serializable]
    public class Info
    {
        public int Id;     
        public byte Type;   
        public int StateId;    
        public int BookId;    
        public int FOVLength;   
        public int AttackLength;  
        public string Icon;  //Image
        public string Prefab;//Path
        public string Name;     
        public string Dec;
        public int Exp;
        public int Score;
        public int WalkSoundId;
        public int AttackSoundId;
    }

    public Dictionary<int, Info> CharacterTableData = new Dictionary<int, Info>();

    public Info Get(int _Id)
    {
        if (CharacterTableData.ContainsKey(_Id))
            return CharacterTableData[_Id];
        return null;
    }

    public int GetScore(int _Id)
    {
        if (CharacterTableData.ContainsKey(_Id))
            return CharacterTableData[_Id].Score;

        return 0;
    }

    public void Init_Binary(string _Name)
    {
        Load_Binary<Dictionary<int, Info>>(_Name, ref CharacterTableData);
    }

    public void Save_Binary(string _Name)
    {
        Save_Binary(_Name, CharacterTableData);
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

            CharacterTableData.Add(info.Id, info);
        }
    }
    protected bool Read(CSVReader _Reader, Info _info, int _Row, int _Col)
    {
        if (_Reader.reset_row(_Row, _Col) == false) return false;
        _Reader.get(_Row, ref _info.Id);
        _Reader.get(_Row, ref _info.Type);
        _Reader.get(_Row, ref _info.StateId);
        _Reader.get(_Row, ref _info.BookId);
        _Reader.get(_Row, ref _info.FOVLength);
        _Reader.get(_Row, ref _info.AttackLength);
        _Reader.get(_Row, ref _info.Icon);
        _Reader.get(_Row, ref _info.Prefab);
        _Reader.get(_Row, ref _info.Name);
        _Reader.get(_Row, ref _info.Dec);
        _Reader.get(_Row, ref _info.Exp);
        _Reader.get(_Row, ref _info.Score);
        _Reader.get(_Row, ref _info.WalkSoundId);
        _Reader.get(_Row, ref _info.AttackSoundId);

        return true;
    }
}
