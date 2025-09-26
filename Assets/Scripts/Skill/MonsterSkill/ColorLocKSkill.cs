using UnityEngine;
using static Enums;

public class ColorLocKSkill : IBasicSkill
{
    private Monster_Base monster;
    //public SkillData skillData { get; set; }
    //public override SKILL_ID_TYPE SkILL_Id => SKILL_ID_TYPE.ColorLock;
    //bool IsActive { get; set; } = false;
    public override void Init(Character_Base _user)
    {
        monster = (Monster_Base)_user;
    }
    public override void OnUpdate() { }
    public override void TriggerOut() { }
    public override void OnTrigger(Character_Base _defender) 
    {
        if (_defender is Player) 
        {
            float timer = 5f;
            Player player = (Player)_defender;
            player.colorSlotUI.ColorLock((int)timer);
        }
    }

    //public override bool State()
    //{
    //    return IsActive;
    //}
}
