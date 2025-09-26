using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static Enums;
using static Enums.CHARACTER_STATUS;

public class BuffSystem
{
    private Character_Base target;
    private Dictionary<BUFFTYPE, IBuffStrategy> activeBuffs = new();

    private IBuffStrategy strategy;
    public event Action<BUFFTYPE, BuffData> OnBuffStarted;
    public event Action<BUFFTYPE, float, float> OnBuffProgress; // elapsed, duration
    public event Action<BUFFTYPE> OnBuffEnded;
    public event Action OnBuffClear;

    private CancellationTokenSource buffLoopCTS = null;
    private bool buffLoopRunning = false;
    public void Init(Character_Base target)
    {
        this.target = target;       
    }

    public void ApplyBuff(int buffId)
    {
        if (GameShard.Instance.StageManager?.IsIntermission == true) return;
        if (target == null) return;
        

        var data = Shared.Instance.DataManager.Buff_Table.Get(buffId);
        if (data == null) return;

        if (activeBuffs.TryGetValue(data.type, out var existing))
        {
            existing.RemoveBuff(target);
            activeBuffs.Remove(data.type);
        }

        var strategy = CreateStrategy(data);
        if (strategy == null) return;

        strategy.ApplyBuff(target);
        activeBuffs[data.type] = strategy;

        OnBuffStarted?.Invoke(data.type, data);
        Debug.Log($"[Buff] Try apply: id={buffId}, soundID={data.soundID}");
        if (data.soundID != 0)
        {
            Debug.Log($"[Buff] Play SFX: soundID={data.soundID}");
            Shared.Instance.SoundManager.PlaySFXOneShot(data.soundID);
        }
        else
        {
            Debug.LogWarning("[Buff] soundID is 0 → no SFX");
        }

        StartBuffTickLoop();
    }

    private void StartBuffTickLoop()
    {
        if (buffLoopRunning) return;
        buffLoopCTS?.Cancel();
        buffLoopCTS?.Dispose();

        buffLoopCTS = new CancellationTokenSource();
        buffLoopRunning = true;
        BuffTickLoopAsync(buffLoopCTS.Token).Forget();
    }

    private async UniTaskVoid BuffTickLoopAsync(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                if (target == null || target.ConditionState == Enums.CHARACTER_CONDITION_STATE.Death)
                    break;

                Tick(Time.unscaledDeltaTime);

                if (GetActiveBuffCount() == 0)
                    break;

                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
        }
        catch (OperationCanceledException)
        {
            // 취소 시 무시
        }
        finally
        {
            buffLoopRunning = false;
            buffLoopCTS?.Dispose();
            buffLoopCTS = null;
        }
    }
    public void Tick(float deltaTime)
    {
        var keys = new List<BUFFTYPE>(activeBuffs.Keys);
        List<BUFFTYPE> expired = new();

        for (int i = 0; i < keys.Count; i++)
        {
            var key = keys[i];
            var strategy = activeBuffs[key];

            strategy.Tick(deltaTime);

            OnBuffProgress?.Invoke(key, strategy.GetElapsed(), strategy.GetDuration());

            if (strategy.IsFinished())
            {
                OnBuffEnded?.Invoke(key);
                strategy.RemoveBuff(target);
                expired.Add(key);
            }
        }

        for (int i = 0; i < expired.Count; i++)
        {
            activeBuffs.Remove(expired[i]);
        }
    }

    private IBuffStrategy CreateStrategy(BuffData data)
    {
        switch (data.type)
        {
            case BUFFTYPE.AttackUp:
            case BUFFTYPE.SpeedUp:
                strategy = new StatBuffStrategy(data);
                strategy.Init(target, data);
                break;

            case BUFFTYPE.Heal:
                strategy = new HealBuffStrategy(data);
                strategy.Init(target, data);
                break;
            case BUFFTYPE.PowerUp:
                strategy = new StatBuffStrategy(data);
                strategy.Init(target, data);
                break;
            case BUFFTYPE.Slow:
                strategy = new StatBuffStrategy(data);
                strategy.Init(target, data);
                break;
            case BUFFTYPE.Sturn:
                strategy = new StatBuffStrategy(data);
                strategy.Init(target, data);
                break;
            case BUFFTYPE.Berserker:
                strategy = new StatBuffStrategy(data);
                strategy.Init(target, data);
                break;
        }
        return strategy;
    }

    public void ClearAllBuffs()
    {
        var keys = new List<BUFFTYPE>(activeBuffs.Keys);
        for (int i = 0; i < keys.Count; i++)
        {
            BUFFTYPE key = keys[i];
            OnBuffEnded?.Invoke(key);
            activeBuffs[key].RemoveBuff(target);
        }
        activeBuffs.Clear();
    }

    public int GetActiveBuffCount() => activeBuffs.Count;

    public void Dispose()
    {
        buffLoopCTS?.Cancel();
        buffLoopCTS?.Dispose();
        buffLoopCTS = null;
        ClearAllBuffs();
    }
}