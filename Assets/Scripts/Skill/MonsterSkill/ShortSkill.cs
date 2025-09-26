using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static Enums;

public class ShortSkill : IBasicSkill
{
    //public SkillData skillData { get; set; }
    //public override SKILL_ID_TYPE SkILL_Id => SKILL_ID_TYPE.Shot;
    //protected Character_Base CHARECTER;
    List<Monster_Base> MonsterList = new List<Monster_Base>();
    //bool IsActive { get; set; } = false;
    //CancellationTokenSource AttackCTS { get; set; } = null;

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
            
        //    DistanseCheckAsync(AttackCTS, null).Forget(); // ����
        //}
        //else 
        //{
        //    DistanseCheckAsync(AttackCTS, _defender).Forget(); // ����
        //}
    }

    protected override async UniTask DistanseCheckAsync(CancellationTokenSource _token, Character_Base _defender)
    {
        MonsterList = GameShard.Instance.MonsterManager.NowStagMonterList;
        HashSet<Transform> damagedEnemies = new HashSet<Transform>();
        int count = skillData.hitCount;

        try 
        {
            if (_defender == null)
            {
                for (int iNum = 0; iNum < count; iNum++)
                {
                    //_token.Token.ThrowIfCancellationRequested(); // 중단 요청 처리

                    for (int jNum = 0; jNum < MonsterList.Count; jNum++)
                    {
                        if (MonsterList[jNum] == null || damagedEnemies.Contains(MonsterList[jNum].transform)) continue;

                        if (FowardAttackDistanseCheck(MonsterList[jNum].transform.position, skillData.range))
                        {
                            Character_Base target = MonsterList[jNum];
                            GameShard.Instance.BattleManager.DamageCheck(CHARECTER, target, skillData);

                            damagedEnemies.Add(MonsterList[jNum].transform);
                            //Debug.LogError($"{MonsterList[jNum]},{iNum}");
                        }
                    }

                    await UniTask.Delay(TimeSpan.FromSeconds(0.1f), cancellationToken: _token.Token);//, cancellationToken: _token.Token
                    damagedEnemies.Clear();
                }
                //await UniTask.Yield(PlayerLoopTiming.Update, _token.Token);//, cancellationToken: _token.Token
            }
            else
            {
                if (FowardAttackDistanseCheck(_defender.transform.position, skillData.range))
                {
                    GameShard.Instance.BattleManager.DamageCheck(CHARECTER, _defender, skillData);

                    //AttackCTS.Cancel();
                }
                //await UniTask.Yield(PlayerLoopTiming.Update, _token.Token);
            }
        }
        catch (OperationCanceledException)
        {
            BuffCheck();
            //_token.Dispose();
            //_token = null;
        }


    }

    //protected bool AttackDistanseCheck(Vector3 _weapon, Vector3 target, float _dist)
    //{
    //    //_weapon.y = 0;
    //    //target.y = 0;

    //    float dist = Vector3.Distance(_weapon, target);

    //    //if (dist <= (float)State[MONSTER_STATE.Attack_Range])
    //    if (dist <= _dist)
    //    {
    //        return true;
    //    }
    //    else
    //    {
    //        Debug.Log($"{dist}");
    //        return false;
    //    }
    //}

    //public void AttackDistanseCheckOut()//Event
    //{
    //    AttackCTS.Cancel();
    //    AttackCTS.Dispose();

    //    Debug.Log("DistanseCheck Off");
    //}



    //public override void OnUpdate() { }

    //public override void TriggerOut() 
    //{
    //    if (AttackCTS != null && !AttackCTS.IsCancellationRequested) 
    //    {
    //        AttackCTS.Cancel();
    //        AttackCTS.Dispose();
    //        AttackCTS = null;
    //    }
        
    //}
}
