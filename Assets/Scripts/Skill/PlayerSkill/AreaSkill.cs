using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class AreaSkill : IBasicSkill
{
    //protected Character_Base CHARECTER;
    //CancellationTokenSource AttackCTS { get; set; } = null;
    List<Monster_Base> MonsterList = new List<Monster_Base>();
    public override void Init(Character_Base _user)
    {
        if (_user is Player)
        {
            CHARECTER = (Player)_user;
            //MonsterList = GameShard.Instance.MonsterManager.MonsterList;
        }
        else if (_user is Monster_Base)
        {
            CHARECTER = (Monster_Base)_user;
        }
        base.Init(_user);
        //EffectAddData(CHARECTER.transform);
    }

    public override void OnTrigger(Character_Base _defender)
    {
        SkillOn(_defender);


        //if (AttackCTS != null)
        //{
        //    AttackCTS.Cancel();
        //    AttackCTS.Dispose();
        //    AttackCTS = null;
        //}

        //AttackCTS = new CancellationTokenSource();
        //TimerOn();
        //if (_defender == null)
        //{

        //    DistanseCheckAsync(AttackCTS, null).Forget(); // 시작
        //}
        //else
        //{
        //    DistanseCheckAsync(AttackCTS, _defender).Forget(); // 시작
        //}
    }

    protected override async UniTask DistanseCheckAsync(CancellationTokenSource _token, Character_Base _defender)
    {
        MonsterList = GameShard.Instance.MonsterManager.NowStagMonterList;
        HashSet<Transform> damagedEnemies = new HashSet<Transform>();
        int count = skillData.hitCount;
        Transform checkPos = CHARECTER.transform;


        //EffectOn(CHARECTER.transform, _token);
        try 
        {
            if (_defender == null)
            {
                for (int iNum = 0; iNum < count; iNum++)
                {
                    for (int jNum = 0; jNum < MonsterList.Count; jNum++)
                    {
                        if (MonsterList[jNum] == null || damagedEnemies.Contains(MonsterList[jNum].transform)) continue;


                        if (RangeAttackDistanseCheck(HitObject.transform.position, MonsterList[jNum].transform.position, skillData.range))
                        {
                            Character_Base target = MonsterList[jNum];
                            GameShard.Instance.BattleManager.DamageCheck(CHARECTER, target, skillData);

                            damagedEnemies.Add(MonsterList[jNum].transform);
                            //Debug.LogError($"{MonsterList[jNum]},{iNum}");

                            //break; //break <- Only Boss
                        }
                        else
                        {
                           // Debug.LogError($"공격이 닺지 않는 거리 입니다");
                        }
                    }

                    await UniTask.Delay(TimeSpan.FromSeconds(0.1f), cancellationToken: _token.Token);
                    damagedEnemies.Clear();
                    await UniTask.Yield(PlayerLoopTiming.Update, _token.Token);
                    //await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: _token.Token);
                }
            }
            else
            {
                if (RangeAttackDistanseCheck(HitObject.transform.position, _defender.transform.position, skillData.range))
                {
                    GameShard.Instance.BattleManager.DamageCheck(CHARECTER, _defender, skillData);

                    //_token.Cancel();
                }
            }


            // 다음 프레임까지 대기
            //await UniTask.Yield(PlayerLoopTiming.Update, _token.Token);
        }
        catch 
        {
            BuffCheck();
            //_token.Dispose();
            //_token = null;
        }
       

        //BuffCheck();
        //AttackCTS.Cancel();
        //AttackCTS.Dispose();
        //AttackCTS = null;
    }

    public void AttackDistanseCheckOut()//Event
    {
        //AttackCTS.Cancel();
        //AttackCTS.Dispose();

        Debug.Log("DistanseCheck Off");
    }

    public override void TriggerOut()
    {
        //if (AttackCTS != null && !AttackCTS.IsCancellationRequested)
        //{
        //    AttackCTS.Cancel();
        //    AttackCTS.Dispose();
        //    AttackCTS = null;
        //}

    }

}
