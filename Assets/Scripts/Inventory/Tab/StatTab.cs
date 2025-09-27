using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Enums;

public class StatTab : MonoBehaviour
{
    [Header("Ref")]
    private Player PLAYER;
    private PlayerStateBar stateBar;

    [Header("Skill")]
    private SkillTab skillTab;

    [Header("Level")]
    [SerializeField] private int levelId = 1;
    private Dictionary<int, LevelTable.Info> levelTable = new Dictionary<int, LevelTable.Info>();

    // 저장(커밋)된 누적 포인트
    private readonly Dictionary<CHARACTER_STATUS, int> committedPoints = new()
    {
        { CHARACTER_STATUS.MaxHp, 0 },
        { CHARACTER_STATUS.Atk,   0 },
        { CHARACTER_STATUS.Def,   0 },
        { CHARACTER_STATUS.Speed, 0 },
    };

    // 가용(남은) 포인트
    private int statPoint = 0;

    // 가중치(HP는 포인트당 10)
    private const int MAXHP_POINT_WEIGHT = 10;

    [Header("Point Tab")]
    [SerializeField] private TMP_Text PointTxt;
    [SerializeField] private TMP_Text MaxHp_Point;
    [SerializeField] private TMP_Text Atk_Point;
    [SerializeField] private TMP_Text Def_Point;
    [SerializeField] private TMP_Text Spd_Point;

    [Header("Player Stat")]
    [SerializeField] private TMP_Text PlayerLevel;
    [SerializeField] private TMP_Text PlayerHP;
    [SerializeField] private TMP_Text PlayerAtk;
    [SerializeField] private TMP_Text PlayerDef;
    [SerializeField] private TMP_Text PlayerSpd;

    [Header("Control")]
    private readonly List<StatControlButton> slots = new();
    private bool HasUnspent() => statPoint > 0;
    private ReminderTab R => GameShard.Instance.GameUiManager?.ReminderTab;

    private void Awake()
    {
        slots.AddRange(GetComponentsInChildren<StatControlButton>());
        for (int i = 0; i < slots.Count; i++)
            slots[i].init(this);  
    }

    private void OnDestroy()
    {
        if (stateBar != null)
            stateBar.LevelUpEvent -= OnLevelUp;
    }

    public void ApplyPlayer(Player _player)
    {
        PLAYER = _player;
        PLAYER.StatDataUpdate(BuildFinalStatsFromCommitted());
    }

    public void Initialize(Player player, SkillTab skilltab)
    {
        skillTab = skilltab;
        PLAYER = player;

        levelTable = Shared.Instance.DataManager.Level_Table.LevelTableData;

        stateBar = GameShard.Instance.GameUiManager.PlayerStateBar;
        int curLevel = (stateBar != null) ? Mathf.Max(1, stateBar.GetLevel()) : 1;

        var row = GetRowByIdOrLevel(curLevel);

        if (row == null) row = GetRowByIdOrLevel(levelId);
        if (row == null)
        {
            Debug.LogWarning("[StatTab] Init row not found.");
            return;
        }

        levelId = row.Id;
        statPoint = row.StatPoint;

        if (skillTab != null) skillTab.SkillPointAdd(row.SkillPoint);

        if (PlayerLevel != null) PlayerLevel.text = row.Level.ToString();

        if (stateBar != null)
        {
            stateBar.LevelUpEvent += OnLevelUp;
            stateBar.TotalExpUpdate(row.TotalExp);
        }


        PLAYER.StatDataUpdate(BuildFinalStatsFromCommitted());
        RefreshUI();

        R?.ShowStat(statPoint, HasUnspent());
    }

    public void OnLevelUp(int _level)
    {
        var row = GetRowByIdOrLevel(_level);
        if (row == null)
        {
            Debug.Log("[StatTab] Level is Max or row missing");
            return;
        }

        levelId = row.Id;                                // ★ 내부는 언제나 Id 유지
        if (stateBar != null) stateBar.TotalExpUpdate(row.TotalExp);

        PlayerLevel.text = row.Level.ToString();

        statPoint += row.StatPoint;
        skillTab.SkillPointAdd(row.SkillPoint);

        PLAYER.StatDataUpdate(BuildFinalStatsFromCommitted());
        RefreshUI();
        R?.UpdateStat(HasUnspent(), statPoint);
    }

    private void RefreshUI()
    {
        if (MaxHp_Point) MaxHp_Point.text   = committedPoints[CHARACTER_STATUS.MaxHp].ToString();
        if (Atk_Point) Atk_Point.text       = committedPoints[CHARACTER_STATUS.Atk].ToString();
        if (Def_Point) Def_Point.text       = committedPoints[CHARACTER_STATUS.Def].ToString();
        if (Spd_Point) Spd_Point.text       = committedPoints[CHARACTER_STATUS.Speed].ToString();

        if (PointTxt) PointTxt.text = statPoint.ToString();

        if (levelTable != null && levelTable.TryGetValue(levelId, out var baseInfo))
        {
            if (PlayerHP) PlayerHP.text     = FormatWithCommitted(baseInfo.MaxHp, GetCommitted(CHARACTER_STATUS.MaxHp));
            if (PlayerAtk) PlayerAtk.text   = FormatWithCommitted(baseInfo.Atk, GetCommitted(CHARACTER_STATUS.Atk));
            if (PlayerDef) PlayerDef.text   = FormatWithCommitted(baseInfo.Def, GetCommitted(CHARACTER_STATUS.Def));
            if (PlayerSpd) PlayerSpd.text   = FormatWithCommitted(baseInfo.Speed, GetCommitted(CHARACTER_STATUS.Speed));
        }
    }

    public void StatPlus(CHARACTER_STATUS type, int value)
    {
        if (value <= 0) return;     
        if (statPoint <= 0) return;  

        committedPoints[type] += value;  
        statPoint -= value;

        PLAYER.StatDataUpdate(BuildFinalStatsFromCommitted());
        RefreshUI();

        R?.UpdateStat(HasUnspent(), statPoint);
    }


    private LevelTable.Info BuildFinalStatsFromCommitted()
    {
        var info = levelTable[levelId];
        return new LevelTable.Info
        {
            Id = info.Id,
            Level = info.Level,
            MaxHp = info.MaxHp + committedPoints[CHARACTER_STATUS.MaxHp] * MAXHP_POINT_WEIGHT,
            Atk = info.Atk + committedPoints[CHARACTER_STATUS.Atk],
            Def = info.Def + committedPoints[CHARACTER_STATUS.Def],
            Speed = info.Speed + committedPoints[CHARACTER_STATUS.Speed],
            TotalExp = info.TotalExp,
            SkillPoint = info.SkillPoint,
            StatPoint = info.StatPoint,
        };
    }


    private LevelTable.Info GetRowByIdOrLevel(int idOrLevel)
    {
        if (levelTable.TryGetValue(idOrLevel, out var byId))
            return byId;

        foreach (var kv in levelTable)
            if (kv.Value.Level == idOrLevel)
                return kv.Value;

        Debug.LogWarning($"[StatTab] No row for Id/Level = {idOrLevel}");
        return null;
    }
    
    private int GetCommitted(CHARACTER_STATUS type)
    {
        int raw = committedPoints.TryGetValue(type, out var v) ? v : 0;
        return (type == CHARACTER_STATUS.MaxHp) ? raw * MAXHP_POINT_WEIGHT : raw;
    }

    private string FormatWithCommitted(int baseValue, int committedPoint)
    {
        int total = baseValue + committedPoint;

        if (committedPoint <= 0)
            return total.ToString();

        return $"{total} <size=70%><color=#FF0000>( + {committedPoint})</color></size>";       
    }

}
