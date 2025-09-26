using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class MinianSummonSkill : IBasicSkill
{
    private Monster_Base monster;
    //public override SKILL_ID_TYPE SkILL_Id => SKILL_ID_TYPE.MinianSummon;
    //public SkillData skillData { get; set; }

    List<GameObject> MinanObjectList = new List<GameObject>();
    Dictionary<GameObject, bool> MinianData = new Dictionary<GameObject, bool>();
    public override void Init(Character_Base _user) 
    {
        monster = (Monster_Base)_user;
        List<GameObject> objLists = monster.GetWeaponsKey();

        if (objLists.Count != 0)
        {
            for (int i = 0; i < objLists.Count; i++)
            {
                MinianData.Add(objLists[i], false);
            }
        }
        base.Init(null);
    }
    public override void OnUpdate() { }
    public override void TriggerOut() { }
    public override void OnTrigger(Character_Base _defender) 
    {
        int falseCount = 0;
        List<GameObject> minianObj = new List<GameObject>();

        for (int i = 0; i < MinanObjectList.Count; i++)
        {
            //GameObject obj = MinanObjectList[i];
            if (MinianData.ContainsKey(MinanObjectList[i]) &&
                MinianData[MinanObjectList[i]] == false)
            {
                minianObj.Add(MinanObjectList[i]);
                falseCount++;
            }
        }

        Debug.Log($"falseCount ={falseCount}");

        if (falseCount >= 3)
        {
            for (int i = 0; i < MinanObjectList.Count && i <= 3; i++)
            {
                if (minianObj.Count < i)
                {
                    break;
                }
                else
                {
                    minianObj[i].SetActive(true);

                    Debug.Log($"{minianObj[i]}");

                    MinianData[minianObj[i]] = true;
                }
            }
        }
    }


}
