using System;
using System.Collections.Generic;
using UnityEngine;

public class DropTable : Table_Base
{
    [Serializable]
    public class Info
    {
        public int DropGroupID;   // 그룹 ID
        public int ItemID;        // 드랍 아이템 ID
        public int Weight;        // 가중치 (확률 대신 비율값)
        public int MinQty;        // 최소 수량
        public int MaxQty;        // 최대 수량
    }

    public class DropGroupCache
    {
        public int[] ItemIDs;
        public int[] CumulativeWeights;
        public int TotalWeight;
        public Info[] Infos; // 필요하면 원본 정보까지 보관
    }

    public Dictionary<int, Info> CharacterTableData = new Dictionary<int, Info>();
    public Dictionary<int, DropGroupCache> DropGroupCacheDict = new Dictionary<int, DropGroupCache>();

    public int GetDropItem_Weighted(int dropGroupId)
    {
        if (!DropGroupCacheDict.ContainsKey(dropGroupId))
            return -1;

        var group = DropGroupCacheDict[dropGroupId];
        int roll = UnityEngine.Random.Range(0, group.TotalWeight);

        int index = System.Array.BinarySearch(group.CumulativeWeights, roll);
        if (index < 0) index = ~index;

        return group.ItemIDs[index];
    }

    public Info Get(int _Id)
    {
        if (CharacterTableData.ContainsKey(_Id))
            return CharacterTableData[_Id];
        return null;
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

        Dictionary<int, List<Info>> tempGroups = new Dictionary<int, List<Info>>();

        for (int row = StartRoe; row < reader.row; ++row)
        {
            Info info = new Info();

            if (Read(reader, info, row, _StartCol) == false)
                break;

            if (!tempGroups.ContainsKey(info.DropGroupID))
                tempGroups[info.DropGroupID] = new List<Info>();

            tempGroups[info.DropGroupID].Add(info);
        }

        foreach (var kvp in tempGroups)
        {
            int groupId = kvp.Key;
            var infos = kvp.Value;

            int total = 0;
            int[] itemIds = new int[infos.Count];
            int[] cumulative = new int[infos.Count];

            for (int i = 0; i < infos.Count; i++)
            {
                total += infos[i].Weight;
                cumulative[i] = total;
                itemIds[i] = infos[i].ItemID;
            }

            DropGroupCacheDict[groupId] = new DropGroupCache
            {
                ItemIDs = itemIds,
                CumulativeWeights = cumulative,
                TotalWeight = total,
                Infos = infos.ToArray()
            };
        }


    }
    protected bool Read(CSVReader _Reader, Info _info, int _Row, int _Col)
    {
        if (_Reader.reset_row(_Row, _Col) == false) return false;
        _Reader.get(_Row, ref _info.DropGroupID);
        _Reader.get(_Row, ref _info.ItemID);
        _Reader.get(_Row, ref _info.Weight);
        _Reader.get(_Row, ref _info.MinQty);
        _Reader.get(_Row, ref _info.MaxQty);


        return true;
    }
}
