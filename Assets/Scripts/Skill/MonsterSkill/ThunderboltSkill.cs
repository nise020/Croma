using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class ThunderboltSkill : IBasicSkill
{
    //public SkillData skillData { get; set; }
    private Monster_Base monster;
    //public override SKILL_ID_TYPE SkILL_Id => SKILL_ID_TYPE.Thunderbolt;

    Color copyColor;
    GameObject WeaponObj;
    MonsterWeapon weapon;
    BoxCollider BossRoomColl;


    List<GameObject> trapObjList = new List<GameObject>();
    Dictionary<GameObject, TrapWeapon> trapData = new Dictionary<GameObject, TrapWeapon>();

    public override void Init(Character_Base _user) 
    {
        monster = (Monster_Base)_user;
        List<GameObject> objLists = monster.GetWeaponsKey();
       // Dictionary<GameObject, bool> objData = monster.GetWeaponsData();

        if (objLists.Count != 0)
        {
            for (int i = 0; i < objLists.Count; i++)
            {
                TrapWeapon weapon = objLists[i].GetComponent<TrapWeapon>();
                if (weapon != null)
                {
                    trapData.Add(objLists[i], weapon);
                    trapObjList.Add(objLists[i]);
                }
            }
        }
        base.Init(null);
    }
    public override void OnUpdate() { }
    public override void TriggerOut() { }
    public override void OnTrigger(Character_Base _defender) 
    {
        int falseCount = 0;
        List<GameObject> objList = new List<GameObject>();
        Dictionary<GameObject, bool> datas = monster.GetWeaponsData();


        if (trapObjList.Count == 0) return;

        for (int i = 0; i < objList.Count; i++)
        {
            if (datas.ContainsKey(trapObjList[i]) &&
                datas[trapObjList[i]] == false)
            {
                objList.Add(trapObjList[i]);
                falseCount++;
            }
        }

        Debug.Log($"falseCount = {falseCount}");

        if (falseCount >= 3)
        {
            for (int i = 0; i < objList.Count && i <= 3; i++)
            {
                if (objList.Count < i)
                {
                    break;
                }
                else
                {
                    TrapWeapon trapWeapon = trapData[objList[i]];

                    Vector3 position = GetPoisotion(BossRoomColl);
                    trapWeapon.WeaponAttack(position);

                    monster.WeaponsStateUpdate(objList[i],true);
                }
            }
        }


    }
    protected Vector3 GetPoisotion(BoxCollider _coll)
    {
        Bounds bounds = _coll.bounds;

        float RanbomX = UnityEngine.Random.Range(bounds.min.x, bounds.max.x);

        return new Vector3(RanbomX, 0, 0);
    }
    public Color GetColor(COLOR_TYPE type)
    {
        return ColorPalette.GetColor(type);
    }

}
