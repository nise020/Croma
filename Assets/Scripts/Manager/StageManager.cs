using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using static Enums;
using Cysharp.Threading.Tasks;


public struct ItemGain
{
    public int itemId;
    public int count;
}

public class StageRecord
{
    public List<ItemGain> items = new List<ItemGain>();
    public int totalKills = 0;
}

public enum STAGE : int
{
    Title = 0,
    Stage1 = 1,
    Stage2 = 2,
    Stage3 = 3,
    Stage4 = 4
}

public enum EndReason { Clear, Failed }

public class StageManager : MonoBehaviour
{
    [Header("Stage Management")]
    private STAGE currentStage;
    private STAGE nextStage = STAGE.Stage1;
    private bool bossSpawned = false;
    private float bossSpawnRatio = 0.5f; // StagePlayTime / 2
    private Dictionary<STAGE, List<QuestData>> stageQuests;

    [Header("Stage Time")]
    private float stageStartTime;
    private float currentSurvivalTime;

    [Header("Stage UI")]
    private bool isPlayerAlive = true;
    private Coroutine stageTimerCo;
    private HUDQuestTab hud;

    [Header("StageClear/Failed")]
    private ClearTab clearTab;
    private FailedTab failedTab;
    private int beforeLevel;
    private int afterLevel;
    private int beforeScore;
    private int afterScore;
    private bool endWindowShow = false;
    
    public bool IsIntermission {  get; private set; }
    public event Action<bool> OnIntermissionChanged;

    public bool CanUseConsumables() => !IsIntermission && isPlayerAlive;

    [SerializeField, ReadOnly] private float stagePlayTime = 30f; // 스테이지당 시간
    private float remainStage;
    public Action<float> OnSurviveTimeUpdate;
    public Action<float> OnStageRemainTimeUpdate;

    private int bossSpawnTick;
    public Action<int> OnBossRemainTimeUpdate;
    public Action OnBossSpawned;

    // Stage ClearTab
    private readonly Dictionary<STAGE, StageRecord> stageLog = new();
    private bool itemCaptureSubscribed = false;
    private bool killCaptureSubscribed = false;

    public async UniTask InitAsync()
    {
        SetupStageQuest();
        clearTab    = GameShard.Instance.GameUiManager.ClearTab;
        failedTab   = GameShard.Instance.GameUiManager.FailedTab;

        StartNewRun();
        EnterStage(STAGE.Stage1);

        await UniTask.Yield();
    }

    private void StartNewRun()
    {
        isPlayerAlive = true;
        stageStartTime = 0;
        currentSurvivalTime = 0;
        endWindowShow = false;
        IsIntermission = false;

        stageLog.Clear();
    }

    private void FinishRun()
    {
        GoTitleClean();
    }

    public async UniTask StageSetting()
    {
        if (GameShard.Instance.GameUiManager.AreaName != null)
        {
            await GameShard.Instance.GameUiManager.AreaName.Play($"Stage {(int)currentStage}", () =>
            {
                StartStageTimer();
            });

            GameShard.Instance.MonsterManager.SpawnActive();
        }
        await UniTask.CompletedTask;
    }

    #region Stage Transition
    public void EnterStage(STAGE stage, Action onDone = null)
    {
        if (currentStage == stage && stageTimerCo != null)
        {
            onDone?.Invoke();
            return;
        }

        if (stageTimerCo != null)
        {
            StopCoroutine(stageTimerCo);
            stageTimerCo = null;
        }

        SetIntermission(true);

        remainStage = 0f;
        currentSurvivalTime = 0f;
        OnStageRemainTimeUpdate?.Invoke(0f);
        OnSurviveTimeUpdate?.Invoke(0f);
        OnBossRemainTimeUpdate?.Invoke(0);

        currentStage = stage;
        bossSpawned = false;
        endWindowShow = false;

        beforeScore = GameShard.Instance.GameManager.GetScore();
        GameShard.Instance.GameUiManager.PlayerStateBar.LimitLevelCheck((int)currentStage);
        beforeLevel = GameShard.Instance.GameUiManager.PlayerStateBar.GetLevel();

        if (!stageQuests.ContainsKey(stage))
        {
            Debug.Log("Not Stage KEY");
            return;
        }
        GameShard.Instance.MonsterManager.StageMonsterCreat(currentStage);

        GameShard.Instance.QuestManager.Clear();      
        hud = GameShard.Instance.GameUiManager.QuestHUD;
        if (hud)
        {
            hud.ClearItems(destroy: false);
            hud.gameObject.SetActive(true);
        }

        List<QuestData> quests = stageQuests[stage];
        for (int i = 0; i < quests.Count; i++)
        {
            GameShard.Instance.QuestManager.AcceptQuest(quests[i]);
        }

        GameShard.Instance.QuestManager.OnStageStarted();
        BeginClearTabCapture();

        MenuSystem.Instance.questTab.RefreshQuest();
        if (hud) hud.RebuildAll();
        Debug.Log($"{(int)stage} Start");
    }

    private void OnStageClear()
    {
        GameShard.Instance?.QuestManager?.OnStageCleared();
        currentSurvivalTime = Mathf.Max(currentSurvivalTime, stagePlayTime);
        TriggerEnd(EndReason.Clear);
    }

    public void SkipStage() 
    {
        OnStageClear();
    }

    public void BossTimerSkip()
    {
        bossSpawned = true;
        OnBossSpawned?.Invoke();
        _ = GameShard.Instance?.MonsterManager.BossSpawn();
    }

    public void GoNextStage()
    {
        if (!Enum.IsDefined(typeof(STAGE), nextStage))
        {
            Debug.LogWarning("[StageManager] 마지막 스테이지이거나 잘못된 nextStage 입니다.");
            return;
        }

        var table = Shared.Instance.DataManager.Stage_Table.Get((int)nextStage);

        var targetScene = GetSceneByStage(nextStage);
        Debug.Log($"[StageManager] FadeEvent start -> {targetScene}");

        GameShard.Instance.GameManager.FadeEvent(targetScene, async () =>
        {
            await UniTask.WhenAll(NewStageSetting());
            await Shared.Instance.SoundManager.BgmPlayerSetting(table.SoundId);
        }).Forget();
    }
    #endregion

    #region Timer
    private void StartStageTimer()
    {
        remainStage = stagePlayTime;
        stageStartTime = Time.unscaledTime;
        currentSurvivalTime = 0f;
        isPlayerAlive = true;

        SetIntermission(false);

        bossSpawnTick = Mathf.FloorToInt(stagePlayTime * bossSpawnRatio);
        OnBossRemainTimeUpdate?.Invoke(bossSpawnTick);

        OnStageRemainTimeUpdate?.Invoke(stagePlayTime);   
        OnSurviveTimeUpdate?.Invoke(currentSurvivalTime); 

        if (stageTimerCo != null) StopCoroutine(stageTimerCo);
        stageTimerCo = StartCoroutine(StageCountDown());
    }

    private IEnumerator StageCountDown()
    {
        int elapsed = 0;

        while (isPlayerAlive && remainStage > 0 )
        {
            if (GameShard.Instance?.InputManager?.isUIOpen == true || IsIntermission)
            {
                yield return new WaitForSecondsRealtime(0.1f);
                continue;
            }

            currentSurvivalTime = elapsed;
            OnSurviveTimeUpdate?.Invoke(currentSurvivalTime);

            int bossRemain = Mathf.Max(0, bossSpawnTick - elapsed);
            OnBossRemainTimeUpdate?.Invoke(bossRemain);

            if (!bossSpawned && elapsed >= bossSpawnTick)
            {
                bossSpawned = true;
                OnBossSpawned?.Invoke();
                _ = GameShard.Instance?.MonsterManager.BossSpawn();
            }

            OnStageRemainTimeUpdate?.Invoke(remainStage);

            yield return new WaitForSecondsRealtime(1f);

            elapsed++;
            remainStage = Mathf.Max(0f, remainStage - 1f);
        }

        if (remainStage <= 0f)
        {
            remainStage = 0f;
            OnStageRemainTimeUpdate?.Invoke(0f);
        }

        if (isPlayerAlive)
            OnStageClear();
    }
    #endregion

    #region Setup & Misc
    private void SetupStageQuest()
    {
        stageQuests = new Dictionary<STAGE, List<QuestData>>();
        Array stageValues = Enum.GetValues(typeof(STAGE));
        for (int i = 0; i < stageValues.Length; i++)
        {
            STAGE stage = (STAGE)stageValues.GetValue(i);
            int stageId = (int)stage;
            int[] questIds = Shared.Instance.DataManager.Stage_Table.GetStageQuestIds(stageId);

            List<QuestData> quests = new List<QuestData>();
            for (int j = 0; j < questIds.Length; j++)
            {
                int id = questIds[j];
                var quest = Shared.Instance.DataManager.Quest_Table.Get(id);
                if (quest != null) quests.Add(quest);
            }
            stageQuests[stage] = quests;
        }
    }

    private async UniTask NewStageSetting()
    {
        var player = GameShard.Instance.GameManager.Player;
        if (player == null)
            player = GameShard.Instance.GameManager.PlayerCreat();

        GameShard.Instance.GameUiManager.MenuSystem.statTab.ApplyPlayer(player);
        GameShard.Instance.GameUiManager.MenuSystem.skillTab.ApplyPlayer(player);
        GameShard.Instance.MonsterManager.PlayerInit(player);
        GameShard.Instance.GameUiManager.PlayerInit(player);
        MenuSystem.Instance.ApplyPlayer(player);
        EnterStage(nextStage);

        await UniTask.CompletedTask;
    }

    private SCENE_SCENES GetSceneByStage(STAGE stage)
    {
        switch (stage)
        {
            case STAGE.Title: return SCENE_SCENES.Title;
            case STAGE.Stage1: return SCENE_SCENES.BamserMap_Stage_1;
            case STAGE.Stage2: return SCENE_SCENES.BamserMap_Stage_2;
            case STAGE.Stage3: return SCENE_SCENES.BamserMap_Stage_3;
            case STAGE.Stage4: return SCENE_SCENES.BamserMap_Stage_4;
            default: return SCENE_SCENES.BamserMap_Stage_1;
        }
    }
    void OnDestroy()
    {
        if (stageTimerCo != null)
        {
            StopCoroutine(stageTimerCo);
            stageTimerCo = null;
        }
    }
    #endregion

    #region EndWindow(Clear/Failed)
    private void TriggerEnd(EndReason reason) // TriggerEnd로 실행 -> EndWindow (3s Wait) -> ShowEndTab (UI 활성화)
    {
        if (endWindowShow) return;
        endWindowShow = true;

        SetIntermission(true);
        isPlayerAlive = false;
        if (stageTimerCo != null) { StopCoroutine(stageTimerCo); stageTimerCo = null; }

        PreEndCleanUp();
        StartCoroutine(EndWindow(reason));
    }

    private void PreEndCleanUp()
    {
        EndClearTabCapture();

        GameShard.Instance?.MonsterManager.CharacterActive(true);
        GameShard.Instance?.MonsterManager.DespawnAllEnemies(includeBoss: true);

        EnsureBuffCleared();
    }

    private void EnsureBuffCleared()
    {
        var player = GameShard.Instance?.GameManager?.Player;
        var hud = GameShard.Instance?.GameUiManager?.PlayerStateBar?.buffHUD;
        if (hud != null) hud.ClearAllHud();
        if (player != null) player.BuffSystem.ClearAllBuffs();
    }

    private IEnumerator EndWindow(EndReason reason)
    {
        float t = 0f;
        float wait = 3f;

        while (t < wait)
        {
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        ShowEndTab(reason);
    }

    private void ShowEndTab(EndReason reason)
    {
        EnsureBuffCleared();

        afterLevel = GameShard.Instance.GameUiManager.PlayerStateBar.GetLevel();
        afterScore = GameShard.Instance.GameManager.GetScore();

        if (reason == EndReason.Clear)
        {
            nextStage = (STAGE)((int)currentStage + 1);
            bool isLastStage = !Enum.IsDefined(typeof(STAGE), nextStage);

            var rec = stageLog.TryGetValue(currentStage, out var v) ? v : new StageRecord();
            var acquired = new List<ClearTab.ItemEntry>();
            foreach (var g in rec.items)
            {
                var proto = Shared.Instance.DataManager.Item_Table.Get(g.itemId);
                acquired.Add(new ClearTab.ItemEntry
                {
                    itemId = g.itemId,
                    count = g.count,
                    sprite = proto != null ? Shared.Instance.AtlasManager.Get(CONFIG_ATLAS_TYPE.Item, proto.spriteName): null
                });
            }

            if (clearTab != null)
            {
                clearTab.ShowClearTab(
                    currentStage, 
                    beforeLevel, 
                    afterLevel,
                    beforeScore, 
                    afterScore,

                    onNextPressed: () =>
                    {
                        GoNextStage();
                    },
                    onGoTitle: () =>
                    {
                        FinishRun();
                    },
                    acquired: acquired,
                    totalKills: rec.totalKills,
                    lastStage: isLastStage
                );
            }
            else 
            { 
                if (isLastStage) FinishRun(); 
                else GoNextStage(); 
            }
        }
        else // Failed
        {
            if (failedTab != null)
            {
                failedTab.ShowFailedTab(
                    currentStage, beforeLevel, afterLevel, afterScore,
                    () => { FinishRun(); }
                );
            }
            else { FinishRun(); }
        }
    }
    #endregion

    #region ClearTab Info

    private void BeginKillCapture()
    {
        var mm = GameShard.Instance?.MonsterManager;

        if (mm != null && !killCaptureSubscribed)
        {
            mm.OnMonsterDied += MonsterDied;
            killCaptureSubscribed = true;
        }

        EnsureStageRecord(currentStage);
    }

    private void EndKillCapture()
    {
        var mm = GameShard.Instance?.MonsterManager;

        if (mm != null && killCaptureSubscribed)
        {
            mm.OnMonsterDied -= MonsterDied;
            killCaptureSubscribed = false;
        }
    }

    private void MonsterDied(Monster_Base monster)
    {
        EnsureStageRecord(currentStage);
        stageLog[currentStage].totalKills++;
    }

    public void StageMonsterCount(int value)
    {
        EnsureStageRecord(currentStage);
        stageLog[currentStage].totalKills += value;
    }

    private void BeginItemCapture()
    {
        EnsureStageRecord(currentStage);

        var tab = MenuSystem.Instance?.itemTab;
        if (tab != null && !itemCaptureSubscribed)
        {
            tab.OnItemAcqst += ItemAcqst;
            itemCaptureSubscribed = true;
        }
    }

    private void EndItemCapture()
    {
        var tab = MenuSystem.Instance?.itemTab;
        if (tab != null && itemCaptureSubscribed)
        {
            tab.OnItemAcqst -= ItemAcqst;
            itemCaptureSubscribed = false;
        }
    }

    private void ItemAcqst(ItemBase item, int count, AcsqtType type) => OnItemAcquiredThisStage(item, count, type);

    private void OnItemAcquiredThisStage(ItemBase item, int count, AcsqtType type)
    {
        if (item == null || count <= 0) return;

        EnsureStageRecord(currentStage);
        var rec = stageLog[currentStage];

        int id = item.itemId;
        int idx = rec.items.FindIndex(g => g.itemId == id);

        if (idx < 0)
            rec.items.Add(new ItemGain { itemId = id, count = count });
        else
            rec.items[idx] = new ItemGain { itemId = id, count = rec.items[idx].count + count };
    }

    private void EnsureStageRecord(STAGE stage)
    {
        if (!stageLog.ContainsKey(stage))
            stageLog[stage] = new StageRecord();
        if (stageLog[stage].items == null)
            stageLog[stage].items = new List<ItemGain>();
    }
    
    private void BeginClearTabCapture()
    {
        EnsureStageRecord(currentStage);
        BeginItemCapture();
        BeginKillCapture();    
    }

    private void EndClearTabCapture()
    {
        EndItemCapture();
        EndKillCapture(); 
    }

    #endregion
    public void PlayerDied()
    {
        TriggerEnd(EndReason.Failed);
    }

    private void GoTitleClean()
    {
        GameShard.Instance.GameManager.FadeEvent(SCENE_SCENES.Title, async () =>
        {
            GameShard.Instance.GameUiManager.CanvasDelet();
            await UniTask.CompletedTask;
        }).Forget();
    }

    private void SetIntermission(bool on)
    {
        IsIntermission = on;
        OnIntermissionChanged?.Invoke(on);
    }
}
