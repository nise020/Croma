using System.Collections.Generic;
using System;
using UnityEngine;
using static Enums;

public class QuestTable : Table_Base
{
    [Serializable]
    public class Info
    {
        public int Id;
        public STAGE Stage;
        public Quest_Type Type;
        public int TargetId;
        public int TargetCount;
        public int RewardId;
        public int Name;
        public int Dec;
    }

    public Dictionary<int, QuestData> QuestTableData = new Dictionary<int, QuestData>();

    public QuestData Get(int _Id)
    {
        if (QuestTableData.ContainsKey(_Id))
            return QuestTableData[_Id];
        return null;
    }

    public void Init_Binary(string _Name)
    {
        Load_Binary<Dictionary<int, QuestData>>(_Name, ref QuestTableData);
    }

    public void Save_Binary(string _Name)
    {
        Save_Binary(_Name, QuestTableData);
    }

    public void Init_Csv(string _Name, int StartRow, int _StartCol)
    {
        CSVReader reader = GetCSVReader(_Name);
        if (reader == null) return;

        for (int row = StartRow; row < reader.row; ++row)
        {
            Info info = new Info();
            if (!Read(reader, info, row, _StartCol))
                break;

            var questData = new QuestData
            {
                questId = info.Id,
                stage = (int)info.Stage,
                type = info.Type,
                targetId = info.TargetId,
                targetCount = info.TargetCount,
                rewardId = info.RewardId,
                questName = info.Name,
                description = info.Dec
            };

            QuestTableData.Add(info.Id, questData);
        }
    }

    protected bool Read(CSVReader _Reader, Info _info, int _Row, int _Col)
    {
        if (_Reader.reset_row(_Row, _Col) == false) return false;

        _Reader.get(_Row, ref _info.Id);


        string stageStr = string.Empty;
        _Reader.get(_Row, ref stageStr);
        if (!Enum.TryParse(stageStr, out _info.Stage))
        {
            Debug.LogError($"Invalid STAGE value: {stageStr}");
            return false;
        }

        string typeStr = string.Empty;
        _Reader.get(_Row, ref typeStr);
        if (!Enum.TryParse(typeStr, out _info.Type))
        {
            Debug.LogError($"Invalid Quest_Type value: {typeStr}");
            return false;
        }

        _Reader.get(_Row, ref _info.TargetId);
        _Reader.get(_Row, ref _info.TargetCount);
        _Reader.get(_Row, ref _info.RewardId);
        _Reader.get(_Row, ref _info.Name);
        _Reader.get(_Row, ref _info.Dec);

        return true;
    }
}
