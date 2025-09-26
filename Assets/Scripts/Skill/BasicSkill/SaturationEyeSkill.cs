using UnityEngine;
using static Enums;

public class SaturationEyeSkill : IBasicSkill
{

    private Player Player;
    private bool isUsed = false;
    //public override SKILL_ID_TYPE SkILL_Id => SKILL_ID_TYPE.SaturationEye;

    public override void Init(Character_Base character)
    {
        this.Player = (Player)character;

        if (Player.colorSteal != null)
        {
            Player.colorSteal.OnStealStart += TriggerOut;
            Player.colorSteal.OnStealEnd += OnReset;
        }
        base.Init(null);
    }

    public override void TriggerOut()
    {
        if (isUsed)
            return;

        // Skill Logic

        Debug.Log("SaturationEye Skill Trigger!");
        isUsed = true;
    }

    public void OnReset()
    {
        Debug.Log("SaturationEye Skill End!");
        isUsed = false;

    }

    public override void OnUpdate() { }
    public override void OnTrigger(Character_Base _defender)
    {
    }
}
