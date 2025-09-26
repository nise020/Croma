using UnityEngine;

public enum SkILLTYPE
{
    Short = 1,
    Long = 2,
    Area = 3,
    Buff = 4,
    ShortBuff = 5,
    LongBuff = 6,
    AreaBuff = 7, 
}

public class SkillData
{
    public int id;
    public SkILLTYPE type;
    public int value;
    public int valueMax;
    public int time;
    public int buffId;
    public int range;
    public int hitCount;
    public int skillCountMax;
    public string iconPath;
    public string prefab; // Effect?
    public int skillName;
    public int desc;
    public int salePoint;
    public int SoundId;

    public GameObject effectPrefab;
    public Sprite icon;

    public void LoadResource()
    {

        //if (!string.IsNullOrEmpty(prefabPath))
            //effectPrefab = Shared.Instance.AtlasManager.Get();

        //if (!string.IsNullOrEmpty (iconPath))
        //    icon = Shared.Instance.AtlasManager.Get(Enums.CONFIG_ATLAS_TYPE.Skill,iconPath);
    }

    public bool IsBuff => type == SkILLTYPE.Buff || type == SkILLTYPE.ShortBuff || type == SkILLTYPE.AreaBuff;
    public bool IsArea => type == SkILLTYPE.Area || type == SkILLTYPE.AreaBuff || type == SkILLTYPE.AreaBuff;
    public bool IsNormal => type == SkILLTYPE.Short || type == SkILLTYPE.ShortBuff || type == SkILLTYPE.ShortBuff;
}
