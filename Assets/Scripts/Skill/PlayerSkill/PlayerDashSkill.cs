using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static Enums;

public class PlayerDashSkill : DashSkill
{
    List<Monster_Base> MonsterList = new List<Monster_Base>();
    //Room Room;
    public override void Init(Character_Base _user)
    {
        CHARECTER = _user as Player;
        base.Init(_user);
        //EffectAddData(CHARECTER.transform);
        //MonsterList = Player.MyRoom.inMonsterLists;
    }
    public override void OnTrigger(Character_Base _defender)
    {
        
        SkillOn(_defender);
        //if (DashCTS == null)
        //{
        //    speed = (float)CHARECTER.StatusData[CHARACTER_STATUS.Speed];
        //    DashCTS = new CancellationTokenSource();
        //    TimerOn();
        //    DashForward(_defender, speed, 0.3f, DashCTS.Token).Forget();
        //}

    }
    protected override async UniTask DistanseCheckAsync(CancellationTokenSource _token, Character_Base _defender)
    {
        MonsterList = GameShard.Instance.MonsterManager.NowStagMonterList;

        HashSet<Transform> damagedEnemies = new HashSet<Transform>();
        int count = skillData.hitCount;
        float elapsed = 0f;
        speed = (float)CHARECTER.StatusData[CHARACTER_STATUS.Speed];
        float dashDuration = 0.3f;
        try
        {
            while (elapsed < dashDuration)
            {
                elapsed += Time.deltaTime;

                float moveStep = speed * Time.deltaTime;

                Vector3 dashDirection = CHARECTER.transform.forward * 2;
                Vector3 move = dashDirection * moveStep;
                rd.MovePosition(rd.position + move);

                for (int iNum = 0; iNum < count; iNum++)
                {
                    for (int jNum = 0; jNum < MonsterList.Count; jNum++)
                    {
                        if (MonsterList[jNum] == null || damagedEnemies.Contains(MonsterList[jNum].transform)) continue;

                        if (FowardAttackDistanseCheck(MonsterList[jNum].transform.position, skillData.range))
                        {
                            GameShard.Instance.BattleManager.DamageCheck(CHARECTER, MonsterList[jNum], skillData);

                            damagedEnemies.Add(MonsterList[jNum].transform);
                        }
                    }
                }
                //await UniTask.Delay(TimeSpan.FromSeconds(0.1f), cancellationToken: _token.Token);
                //damagedEnemies.Clear();
                //Vector3 toTargetNow = _target.transform.position - Player.transform.position;
                //toTargetNow.y = 0; // 높이 무시

                //if (Vector3.Dot(dashDirection, toTargetNow.normalized) < 0f)
                //{
                //    break;
                //}

                await UniTask.Yield();
            }
        }

        catch 
        {
            BuffCheck();
        }

    }
}
