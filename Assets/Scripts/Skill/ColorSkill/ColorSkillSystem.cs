using System;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class ColorSkillSystem
{
    private Dictionary<COLOR_TYPE, SkillBase> skillDict;
    private SkillBase currentSkill;

    public ColorSkillSystem()
    {
        skillDict = new Dictionary<COLOR_TYPE, SkillBase>();

        // 컬러별 스킬명 연결 (기획서의 color_DB 기준)
        RegisterSkill(COLOR_TYPE.scred, "explosion");
        RegisterSkill(COLOR_TYPE.scgreen, "leafsniping");
        RegisterSkill(COLOR_TYPE.scblue, "waterball");
        RegisterSkill(COLOR_TYPE.sclightGreen, "windblade");
        RegisterSkill(COLOR_TYPE.scpurple, "jewelblade");
        RegisterSkill(COLOR_TYPE.scyellow, "sandstorm");
        RegisterSkill(COLOR_TYPE.scblack, "blackhole");
        RegisterSkill(COLOR_TYPE.scbrown, "treethorn");
        RegisterSkill(COLOR_TYPE.scskyBlue, "icemeteor");
        RegisterSkill(COLOR_TYPE.scwhite, "zero");
        RegisterSkill(COLOR_TYPE.scorange, "fireshot");
    }

    private void RegisterSkill(COLOR_TYPE color, string skillName)
    {
        var skill = SkillFactory.Create(skillName);
        if (skill != null)
        {
            skillDict[color] = skill;
            Debug.Log($"[SkillSystem] Registered skill '{skillName}' for color {color}");
        }
        else
        {
            Debug.LogWarning($"[SkillSystem] Failed to register skill '{skillName}' for color {color}");
        }
    }

    public void ChangeColor(COLOR_TYPE color)
    {
        Debug.Log($"[SkillSystem] ChangeColor to {color}");
        skillDict.TryGetValue(color, out currentSkill);
    }

    public void Execute(Player player)
    {
        if (currentSkill != null)
        {
            currentSkill.Execute(player);
        }
    }

    public void Tick(Player player, float deltaTime)
    {
        if (currentSkill != null)
        {
            currentSkill.Tick(player, deltaTime);
        }
    }
}