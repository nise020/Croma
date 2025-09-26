using UnityEngine;
using static Enums;

public class StatBuffStrategy : IBuffStrategy
{
    private float elapsed;
    public float Elapsed => elapsed;
    public StatBuffStrategy(BuffData data) { this.buffData = data; }

    public override void Init(Character_Base _user, BuffData data)
    {
        base.Init(_user, data);
    }

    public override void ApplyBuff(Character_Base target)
    {
        switch (buffData.type)
        {
            case BUFFTYPE.AttackUp:
                if (target.StatusData.ContainsKey(CHARACTER_STATUS.Atk))
                    target.StatusData[CHARACTER_STATUS.Atk] += buffData.value;

                PlayOneShotEffect(buffData.effectPath, target.transform);
                break;

            case BUFFTYPE.SpeedUp:
                if (target.StatusData.ContainsKey(CHARACTER_STATUS.Speed))
                    target.StatusData[CHARACTER_STATUS.Speed] += buffData.value;

                AcquireSharedEffect(buffData.effectPath, target.transform);
                break;

            case BUFFTYPE.Slow:
                if (target.StatusData.ContainsKey(CHARACTER_STATUS.Speed))
                    target.StatusData[CHARACTER_STATUS.Speed] -= buffData.value;

                AcquireSharedEffect(buffData.effectPath, target.transform);
                break;

            case BUFFTYPE.Sturn:
                target.IsSturn = true;

                AcquireSharedEffect(buffData.effectPath, target.transform);

                break;
            case BUFFTYPE.PowerUp:
                if (target.StatusData.ContainsKey(CHARACTER_STATUS.Atk))
                {
                    target.StatusData[CHARACTER_STATUS.Speed] += buffData.value;
                    target.StatusData[CHARACTER_STATUS.Atk] += buffData.value;
                }

                AcquireSharedEffect(buffData.effectPath, target.transform);
                break;

            case BUFFTYPE.Berserker:
                if (target.StatusData.ContainsKey(CHARACTER_STATUS.Atk))
                    target.StatusData[CHARACTER_STATUS.Speed] += buffData.value;
                target.StatusData[CHARACTER_STATUS.Atk] += buffData.value;

                AcquireSharedEffect(buffData.effectPath, target.transform);
                break;
        }
    }

    public override void RemoveBuff(Character_Base target)
    {
        switch (buffData.type)
        {
            case BUFFTYPE.AttackUp:
                if (target.StatusData.ContainsKey(CHARACTER_STATUS.Atk))
                    target.StatusData[CHARACTER_STATUS.Atk] -= buffData.value;
                if (oneShotObj)
                {
                    Object.Destroy(oneShotObj);
                    oneShotObj = null;
                    oneShotPs = null;
                }
                break;

            case BUFFTYPE.SpeedUp:
                if (target.StatusData.ContainsKey(CHARACTER_STATUS.Speed))
                    target.StatusData[CHARACTER_STATUS.Speed] -= buffData.value;

                base.RemoveBuff(target);
                break;

            case BUFFTYPE.Slow:
                if (target.StatusData.ContainsKey(CHARACTER_STATUS.Speed))
                    target.StatusData[CHARACTER_STATUS.Speed] += buffData.value;

                base.RemoveBuff(target);
                break;

            case BUFFTYPE.Sturn:
                target.IsSturn = false;

                base.RemoveBuff(target);
                break;


            case BUFFTYPE.PowerUp:
                if (target.StatusData.ContainsKey(CHARACTER_STATUS.Atk))
                {
                    target.StatusData[CHARACTER_STATUS.Speed] -= buffData.value;
                    target.StatusData[CHARACTER_STATUS.Atk] -= buffData.value;
                }

                base.RemoveBuff(target);
                break;

            case BUFFTYPE.Berserker:
                if (target.StatusData.ContainsKey(CHARACTER_STATUS.Atk))
                    target.StatusData[CHARACTER_STATUS.Speed] -= buffData.value;
                target.StatusData[CHARACTER_STATUS.Atk] -= buffData.value;

                base.RemoveBuff(target);
                break;
        }
    }

    public override void Tick(float dt)
    {
        elapsed += dt;
        base.Tick(dt); // ¿ø¼¦ ÀÚµ¿ ÆÄ±« Å¸ÀÌ¸Ó
    }

    public override bool IsFinished() => elapsed >= buffData.time;
    public override float GetElapsed() => elapsed;
}