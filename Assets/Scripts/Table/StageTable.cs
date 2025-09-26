using System.Collections.Generic;
using System;
using UnityEngine;

public class StageTable : Table_Base
{
    [Serializable]
    public class Info
    {
        public int Id;
        public byte Type;
        public int Monster_1Id;
        public int Boss_1Id;
        public int[] QuestIds;
        public int RewardId;
        public int Sound;
        public string Icon;
        public string Prefab;
        public int Name;
        public int Dec;
        public int SpownTimer;
        public int LimitLevel;
        public int SoundId;

        #region 퀘스트ID반환
        public int[] GetValidQuestIds()
        {
            if (QuestIds == null) return new int[0];

            List<int> validIds = new List<int>();
            foreach (int id in QuestIds)
            {
                if (id > 0) // 0보다 큰 유효한 ID만
                    validIds.Add(id);
            }
            return validIds.ToArray();
        }
        public int GetQuestCount()
        {
            return GetValidQuestIds().Length;
        }
        #endregion
    }

    public Dictionary<int, Info> StageTableData = new Dictionary<int, Info>();

    public Info Get(int _Id)
    {
        if (StageTableData.ContainsKey(_Id))
            return StageTableData[_Id];
        return null;
    }

    public Dictionary<int, Info> GetAllStages()
    {
        return StageTableData;
    }

    public int[] GetStageQuestIds(int stageId)
    {
        var stageInfo = Get(stageId);
        return stageInfo?.GetValidQuestIds() ?? new int[0];
    }

    public void Init_Binary(string _Name)
    {
        Load_Binary<Dictionary<int, Info>>(_Name, ref StageTableData);
    }

    public void Save_Binary(string _Name)
    {
        Save_Binary(_Name, StageTableData);
    }

    public void Init_Csv(string _Name, int StartRoe, int _StartCol)
    {
        CSVReader reader = GetCSVReader(_Name);

        if (reader == null)
            return;

        for (int row = StartRoe; row < reader.row; ++row)
        {
            Info info = new Info();

            if (Read(reader, info, row, _StartCol) == false)
                break;

            if (StageTableData.ContainsKey(info.Id))
                Debug.LogWarning($"Duplicate Stage ID found: {info.Id}. Overwriting previous entry.");
        

        StageTableData.Add(info.Id, info);
        }
    }
    protected bool Read(CSVReader _Reader, Info _info, int _Row, int _Col)
    {
        if (_Reader.reset_row(_Row, _Col) == false)
        {
            Debug.LogWarning($"Failed to reset row {_Row} at column {_Col}");
            return false;
        }

        try
        {
            // 기본 정보 읽기
            _Reader.get(_Row, ref _info.Id);
            _Reader.get(_Row, ref _info.Type);
            _Reader.get(_Row, ref _info.Monster_1Id);
            _Reader.get(_Row, ref _info.Boss_1Id);

            // 퀘스트 ID 문자열 읽기 및 파싱
            string questIdsStr = string.Empty;
            _Reader.get(_Row, ref questIdsStr);
            _info.QuestIds = ParseQuestIds(questIdsStr);

            // 나머지 정보 읽기
            _Reader.get(_Row, ref _info.RewardId);
            _Reader.get(_Row, ref _info.Sound);
            _Reader.get(_Row, ref _info.Icon);
            _Reader.get(_Row, ref _info.Prefab);
            _Reader.get(_Row, ref _info.Name);
            _Reader.get(_Row, ref _info.Dec);
            _Reader.get(_Row, ref _info.SpownTimer);
            _Reader.get(_Row, ref _info.LimitLevel);
            _Reader.get(_Row, ref _info.SoundId);

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error reading stage data at row {_Row}: {e.Message}");
            return false;
        }
    }

    #region 파이프분리
    private int[] ParseQuestIds(string questIdsStr)
    {
        if (string.IsNullOrEmpty(questIdsStr) || string.IsNullOrWhiteSpace(questIdsStr))
        {
            Debug.LogWarning("Empty quest IDs string found");
            return new int[0];
        }

        try
        {
            string[] idStrings = questIdsStr.Split('|');
            List<int> questIds = new List<int>();

            foreach (string idStr in idStrings)
            {
                string trimmedId = idStr.Trim();

                if (string.IsNullOrEmpty(trimmedId))
                    continue;

                if (int.TryParse(trimmedId, out int questId))
                {
                    questIds.Add(questId);
                }
                else
                {
                    Debug.LogWarning($"Failed to parse quest ID: '{trimmedId}' from string: '{questIdsStr}'");
                }
            }

            return questIds.ToArray();
        }
        catch (Exception e)
        {
            Debug.LogError($"Error parsing quest IDs from string '{questIdsStr}': {e.Message}");
            return new int[0];
        }
    }
    #endregion


}
