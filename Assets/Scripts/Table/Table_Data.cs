using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DataManager;

public class Table_Data : Table_Base
{
    [Serializable]
    public class CharacterInfo
    {
        public int Id;
        public byte Type;
        public int Skill;
        public int State;
        public string Prefabs;
        public string Img;
        public int Name;
        public int Dec;//Ό³Έν
    }

    public Dictionary<int, CharacterInfo> Dictionary = new Dictionary<int, CharacterInfo>();

    public CharacterInfo Get<t>(TableType _table, int _Id)
    {
        switch (_table) 
        {
            case TableType.Character:
                if (Dictionary.ContainsKey(_Id))
                    return Dictionary[_Id];
                break;
            default:
                return null;
        }
        return null;
    }
}
