using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using static Enums;
using static Enums.ANIMATION_PATAMETERS_TYPE;

public partial class NomalMonster : Monster_Base
{
    protected override async UniTaskVoid DistanseCheckAsync(CancellationToken token)
    {
        while (true)
        {
            token.ThrowIfCancellationRequested(); // �ߴ� ��û ó��

            // �Ÿ� ���
            //float dist = Vector3.Distance(WeaponObject.transform.position,PlayerTrans.position);

            float dist = TargetDistanseCheck(PlayerTrans.position);

            //if (dist <= (float)State[MONSTER_STATE.Attack_Range])
            if (dist <= 3.0f)
            {
                Character_Base character = PlayerTrans.gameObject.GetComponent<Character_Base>();
                //GameShard.Instance.BattleManager.DamageCheck(this, character);
                break; // �ѹ��� �����ϰ� ���� (��� �����ϰ� ������ break ����)
            }

            // ���� �����ӱ��� ���
            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }
    }

    public override void DeathEvent()//AnimationEvent
    {
        //ITEMLists.Count
        int key = UnityEngine.Random.Range(0, DropItemData.Count);
        GameShard.Instance.MonsterManager.MonsterAcquired(this);

        if (DropItemData.Count != 0) 
        {
            if(DropItemData.TryGetValue(ITEMLists[key], out GameObject itemObj) &&
            IdType != CHARACTER_ID.Default)
            {
                itemObj.transform.position = transform.position;
                itemObj.SetActive(true);
            }
        }
        StateCheckData[KnockBack.ToString()] = false;
        StartCoroutine(DeathEffectOn(this,() =>
        {
            //Resurrection
            AnimationParameterUpdate(Death, false);
           // GameShard.Instance.MonsterManager.Resurrection(this);
        }));
    
    }
    //public void JumpOn()//Event
    //{
    //    MoveParentToTargetAsync().Forget();
    //}

    //private async UniTaskVoid MoveParentToTargetAsync()
    //{
    //    Vector3 start = gameObject.transform.position;
    //    Vector3 end = Player.gameObject.transform.position;
    //    float jumpDuration = 0.3f;
    //    float elapsed = 0f;
    //    while (elapsed < jumpDuration)
    //    {
    //        elapsed += Time.deltaTime;
    //        float t = Mathf.Clamp01(elapsed / jumpDuration);

    //        // ���� �̵���
    //        transform.position = Vector3.Lerp(start, end, t);

    //        await UniTask.Yield();
    //    }

    //    transform.position = end; // ��Ȯ�� ����
    //}
}
