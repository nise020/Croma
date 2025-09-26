using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatTable : Table_Base
{
    [Serializable]
    public class Info
    {
        public int Id;
        public byte Type;
        public int Hp;   
        public int Atk;  
        public int Def;  
        public int Speed;
        //public int Exp;

    }
 
    public Dictionary<int, Info> StateTableData = new Dictionary<int, Info>();

    public Info Get(int _Id)
    {
        if (StateTableData.ContainsKey(_Id))
            return StateTableData[_Id];
        Debug.LogError($"Id ={_Id}의 해당하는 State의 값이 없습니다");
        return null;
    }
    public void Init_Binary(string _Name)
    {
        Load_Binary<Dictionary<int, Info>>(_Name, ref StateTableData);
    }

    public void Save_Binary(string _Name)
    {
        Save_Binary(_Name, StateTableData);
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
            StateTableData.Add(info.Id, info);
        }
    }
    protected bool Read(CSVReader _Reader, Info _info, int _Row, int _Col)
    {
        if (_Reader.reset_row(_Row, _Col) == false) return false;
        _Reader.get(_Row, ref _info.Id);
        _Reader.get(_Row, ref _info.Type);
        _Reader.get(_Row, ref _info.Hp);
        _Reader.get(_Row, ref _info.Atk);
        _Reader.get(_Row, ref _info.Def);
        _Reader.get(_Row, ref _info.Speed);
        //_Reader.get(_Row, ref _info.Exp);
        //_Reader.get(_Row, ref _info.CritRate);
        //_Reader.get(_Row, ref _info.CritDamage);


        return true;
    }
}
