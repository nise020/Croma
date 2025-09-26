using System;
using System.Collections.Generic;

public static class SkillFactory
{
    private static readonly Dictionary<string, Func<SkillBase>> skillMap = new()
    {
        { "explosion", () => new ExplosionSkill() },
        { "fireshot", () => new FireShotSkill() },
        { "leafsniping", () => new LeafSnipingSkill() },
        { "sandstorm", () => new SandstormSkill() },
        { "waterball", () => new WaterBallSkill() },
        { "treethorn", () => new TreeThornSkill() },
        { "blackhole", () => new BlackholeSkill() },
        { "windblade", () => new WindBladeSkill() },
        { "jewelblade", () => new JewelBladeSkill() },
        { "icemeteor", () => new IceMeteorSkill() },
        { "zero", () => new ZeroSkill() },
    };

    public static SkillBase Create(string skillName)
    {
        if (skillMap.TryGetValue(skillName.ToLowerInvariant(), out var constructor))
        {
            return constructor.Invoke();
        }

        
        return null;
    }
}