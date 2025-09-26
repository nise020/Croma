using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static Enums;

public class BrokenLightKill : IBasicSkill
{
    //public override SKILL_ID_TYPE SkILL_Id => SKILL_ID_TYPE.BrokenLight;
    //public SkillData skillData { get; set; }
    //bool IsActive { get; set; } = false;
    private Player player;
    public override void Init(Character_Base character) 
    {
        this.player = (Player)character;
        base.Init(null);
    }

    public override void OnUpdate() 
    {
        
    }

    public override void TriggerOut() 
    {
        IsActive = true;
        //Timer(5);
        Debug.Log("BrokenLight");
    }


    public override void OnTrigger(Character_Base _defender)
    {
    }

}
