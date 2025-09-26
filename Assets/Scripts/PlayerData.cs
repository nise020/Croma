using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int playerId;
    public int Score;
    public int KillCount;
}
[System.Serializable]
public class PlayerDataList
{
    public List<PlayerData> players = new List<PlayerData>();
}