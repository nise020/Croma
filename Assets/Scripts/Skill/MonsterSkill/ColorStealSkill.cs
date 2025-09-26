using UnityEngine;
using static Enums;

public class ColorStealSkill : IBasicSkill
{
    //public override SKILL_ID_TYPE SkILL_Id => SKILL_ID_TYPE.ColorSteal;
    //public SkillData skillData { get; set; }
    private Monster_Base monster;
   // bool IsActive { get; set; } = false;

    Color copyColor;
    //GameObject WeaponObj;
    MonsterWeapon weapon;
    public override void Init(Character_Base _user)
    {
        monster = (Monster_Base)_user;
        base.Init(null);
    }
    public override void OnUpdate() { }
    public override void TriggerOut() { }
    public override void OnTrigger(Character_Base _defender)
    {
       //WeaponObj = monster.GetWeaponObj();
        MonsterWeapon monsterWeapon = weapon.GetComponent<MonsterWeapon>();

        if (_defender is Player)
        {
            Player player = (Player)_defender;
            COLOR_TYPE colorType = player.StillColor(out float value);

            if (colorType == COLOR_TYPE.None)
            {
                Debug.LogError($"colorType = {colorType}");
                return;
            }

            Color colorValue = GetColor(colorType);

            copyColor = colorValue;

            if (monsterWeapon != null)
            {
                monsterWeapon.CopyColor = colorValue;
            }
            Debug.Log($"Color Copy = {copyColor}");
        }

    }

    public Color GetColor(COLOR_TYPE type)
    {
        return ColorPalette.GetColor(type);
    }
    //public override bool State()
    //{
    //    return IsActive;
    //}
}
