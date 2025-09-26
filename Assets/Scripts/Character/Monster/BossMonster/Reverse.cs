using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class Reverse : BossMonster
{
    public void TentacleSummon() 
    {
        Debug.Log("ÃË¼ö ¼ÒÈ¯");
    }


    public void MnianList(List<GameObject> _List)
    {
        WeaponObjectList = _List;
    }

    public void SummonSkill()//AnimationEvent
    {
        basicSkillSystem.SkillActive(SKILL_ID_TYPE.MinianSummon, Player);
        //int falseCount = 0;
        //List<GameObject> minianObj = new List<GameObject>();

        //for (int i = 0; i < MinanObjectList.Count; i++)
        //{
        //    //GameObject obj = MinanObjectList[i];
        //    if (MinianData.ContainsKey(MinanObjectList[i]) &&
        //        MinianData[MinanObjectList[i]] == false)
        //    {
        //        minianObj.Add(MinanObjectList[i]);
        //        falseCount++;
        //    }
        //}

        //Debug.Log($"falseCount ={falseCount}");

        //if (falseCount >= 3)
        //{
        //    for (int i = 0; i < MinanObjectList.Count && i <= 3; i++)
        //    {
        //        if (minianObj.Count < i)
        //        {
        //            break;
        //        }
        //        else
        //        {
        //            minianObj[i].SetActive(true);

        //            Debug.Log($"{minianObj[i]}");

        //            MinianData[minianObj[i]] = true;
        //        }
        //    }
        //}
    }


}
