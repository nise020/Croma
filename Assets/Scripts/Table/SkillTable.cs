using System.Collections.Generic;
using System;
using UnityEngine;

public class SkillTable : Table_Base
{
    [Serializable]
    public class Info
    {
        public int Id;
        public int Type;
        public int Value;
        public int ValueMax;
        public int Time;
        public int BuffId;
        public int Range;
        public int HitCount;
        public int SkillCountMax;
        public string IconPath;
        public string PrefabPath;
        public int Name;
        public int Dec;
        public int SalePoint;
        public int SoundId;
    }

    public Dictionary<int, SkillData> SkillTableData = new Dictionary<int, SkillData>();

    public SkillData Get(int id)
    {
        SkillTableData.TryGetValue(id, out var skill);
        return skill;
    }

    public void Init_Binary(string _Name)
    {
        Load_Binary<Dictionary<int, SkillData>>(_Name, ref SkillTableData);
    }

    public void Save_Binary(string _Name)
    {
        Save_Binary(_Name, SkillTableData);
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


            SkillData skill = new SkillData
            {
                id = info.Id,
                type = (SkILLTYPE)info.Type, // ← enum으로 변환
                value = info.Value,
                valueMax = info.ValueMax,
                time = info.Time,
                buffId = info.BuffId,
                range = info.Range,
                hitCount = info.HitCount,
                skillCountMax = info.SkillCountMax,
                iconPath = info.IconPath,
                prefab = info.PrefabPath,
                skillName = info.Name,
                desc = info.Dec,
                salePoint = info.SalePoint,
                SoundId = info.SoundId,
            };

            skill.LoadResource();
            SkillTableData.Add(skill.id, skill);
        }
    }
    protected bool Read(CSVReader _Reader, Info _info, int _Row, int _Col)
    {
        if (_Reader.reset_row(_Row, _Col) == false) return false;
        _Reader.get(_Row, ref _info.Id);
        _Reader.get(_Row, ref _info.Type);
        _Reader.get(_Row, ref _info.Value);
        _Reader.get(_Row, ref _info.ValueMax);
        _Reader.get(_Row, ref _info.Time);
        _Reader.get(_Row, ref _info.BuffId);
        _Reader.get(_Row, ref _info.Range);
        _Reader.get(_Row, ref _info.HitCount);
        _Reader.get(_Row, ref _info.SkillCountMax);
        _Reader.get(_Row, ref _info.IconPath);
        _Reader.get(_Row, ref _info.PrefabPath);
        _Reader.get(_Row, ref _info.Name);
        _Reader.get(_Row, ref _info.Dec);
        _Reader.get(_Row, ref _info.SalePoint);
        _Reader.get(_Row, ref _info.SoundId);

        return true;
    }
}
