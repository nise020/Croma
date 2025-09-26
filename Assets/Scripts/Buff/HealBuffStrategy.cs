using UnityEngine;
using static Enums;

public class HealBuffStrategy : IBuffStrategy
{
    private float elapsed;
    private float tickTimer;
    public float Elapsed => elapsed;
    public HealBuffStrategy(BuffData buffData)
    {
        this.buffData = buffData;
        this.elapsed = 0f;
        this.tickTimer = 0f;
    }

    public override void ApplyBuff(Character_Base target) { }
    public override void RemoveBuff(Character_Base target) { }

    public override void Tick(float deltaTime)
    {
        elapsed += deltaTime;
        tickTimer += deltaTime;

        if (tickTimer >= 1f)
        {
            tickTimer = 0;

            if (target.StatusData.TryGetValue(CHARACTER_STATUS.Hp, out float hp) && 
                target.StatusData.TryGetValue(CHARACTER_STATUS.MaxHp, out float maxHp))
            {
                float newHp = Mathf.Min(hp + buffData.value, maxHp);
                target.StatusData[CHARACTER_STATUS.Hp] = newHp;
                target.HpBarChanged?.Invoke(maxHp, newHp); 
            }
        }
    }

    public override bool IsFinished() => elapsed >= buffData.time;
    public override float GetElapsed() => elapsed;
}
