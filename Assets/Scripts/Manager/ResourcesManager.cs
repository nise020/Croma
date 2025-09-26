using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using static Enums;

public class ResourcesManager : MonoBehaviour
{
    public List<GameObject> character_ResourseObj = new List<GameObject>();
    public GameObject player_ResourseObj;
    public List<GameObject> effectObj = new List<GameObject>();

    Dictionary<string,GameObject> effectPrefabData = new Dictionary<string,GameObject>();
    Dictionary<string,GameObject> MonsterPrefabData = new Dictionary<string,GameObject>();
    public Player PLAYERDATA;
    public Transform CreatTab;
    public Transform SkillTab;
    public Transform CharacterTab;

    [Header("UI Prefabs")]
    public GameObject MenuObj;
    public GameObject QuestHUD;
    public GameObject AreaName;
    public GameObject FadePrefab;
    public GameObject PlayerStatePrefab;
    public GameObject BossStatePrefab;
    public GameObject PointRemindPrefab;
    public GameObject ClearTabPrefab;
    public GameObject FailedTabPrefab;
    public GameObject ItemAcqstTabPrefab;
    public GameObject GameInfoPrefab;
    public GameObject QuickSlotPrefab;
    public GameObject DimmerImagePrefab;
    public GameObject FadeCanvasPrefab;

    [Header("UI Prefab Path")]
    private string areaPath = "Prefab/UI/AreaName/GamePlay_AreaName";
    private string hudPath = "Prefab/UI/Quest/QuestHUD";
    private string FadePath = "Prefab/UI/FadeImage";
    private string PlayerStateUiPath = "Prefab/UI/PlayerUI/PlayerState_Ui";
    private string BossStateUiPath = "Prefab/UI/BossUI/BossMonsterHpGauge";
    private string PointRemindPath = "Prefab/UI/Level/PointRemindTab";
    private string ClearTabPath = "Prefab/UI/GamePlay/ClearTab";
    private string FailedTabPath = "Prefab/UI/GamePlay/FailedTab";
    private string ItemAcqstPath = "Prefab/UI/Acqst/AcqstItemTab";
    private string QuickSlotPath = "Prefab/UI/QuickSlot/QuickSlotBar";
    private string DimmerImagePath = "Prefab/UI/QuickSlot/DimmerImage";
    private string GameInfoPath = "Prefab/UI/GamePlay/GameInfo";
    private string FadeCanvasPath = "Prefab/UI/Canvas/Fade_Canvas";


    [Header("Camera Anmation")]
    private string FollowCamPath = "Prefab/Camera/FollowCam";
    public GameObject FollowCamPrefab;

    [Header("Damage")]
    public Shader damageShader;
    public GameObject nomalDeathEffect;
    public GameObject bossDeathEffect;

    public GameObject SpownEffect;

    public Material damageMaterial;
    
    public async UniTask Init()
    {
        Shared.Instance.ResourcesManager = this;

        if (CreatTab == null)
        {
            GameObject go = new GameObject($"CreatTab");
            CreatTab = go.transform;
            DontDestroyOnLoad(CreatTab.gameObject);
        }

        if (SkillTab == null)
        {
            GameObject go = new GameObject($"SkillTab");
            SkillTab = go.transform;
            DontDestroyOnLoad(SkillTab.gameObject);
        }

        if (CharacterTab == null)
        {
            GameObject go = new GameObject($"CharacterTab");
            CharacterTab = go.transform;
            DontDestroyOnLoad(CharacterTab.gameObject);
        }
        
        ResoursLoadSkill();
        ResoursLoadCharacter();
        ResoursLoadPlyer();
        ResourcesLoadBuff();

        damageShader = Resources.Load<Shader>("Shader/WhiteShader");
        damageMaterial = Resources.Load<Material>("Material/Damage/WhiteMatarial");

        bossDeathEffect = Resources.Load<GameObject>("Prefab/Effect/Explosion/BigExplosion");
        nomalDeathEffect = Resources.Load<GameObject>("Prefab/Effect/Explosion/SmallExplosion");

        FollowCamPrefab = Resources.Load<GameObject>(FollowCamPath);


        MenuObj = Resources.Load<GameObject>("Prefab/UI/Menu/Menu_UI");
        QuestHUD = Resources.Load<GameObject>(hudPath);
        AreaName = Resources.Load<GameObject>(areaPath);
        PlayerStatePrefab = Resources.Load<GameObject>(PlayerStateUiPath);
        BossStatePrefab = Resources.Load<GameObject>(BossStateUiPath);
        FadePrefab = Resources.Load<GameObject>(FadePath);
        PointRemindPrefab = Resources.Load<GameObject>(PointRemindPath);
        ClearTabPrefab = Resources.Load<GameObject>(ClearTabPath);
        FailedTabPrefab = Resources.Load<GameObject>(FailedTabPath);
        ItemAcqstTabPrefab = Resources.Load<GameObject>(ItemAcqstPath);
        GameInfoPrefab = Resources.Load<GameObject>(GameInfoPath);
        QuickSlotPrefab = Resources.Load<GameObject>(QuickSlotPath);
        DimmerImagePrefab = Resources.Load<GameObject>(DimmerImagePath);
        FadeCanvasPrefab = Resources.Load<GameObject>(FadeCanvasPath);

        SpownEffect = Resources.Load<GameObject>("Prefab/Effect/Buff/Debuff_03");
        await UniTask.Yield(); 
    }

    public GameObject CreatObject(CONFIG_OBJECT_TYPE _type, string _path) 
    {
        GameObject go = null;
        switch (_type)
        {
            case CONFIG_OBJECT_TYPE.Player:
                go = PLAYERDATA.gameObject;
                break;

            case CONFIG_OBJECT_TYPE.Monster:
                go = MonsterPrefabData[_path];
                break;

            case CONFIG_OBJECT_TYPE.Weapon:
                ;
                break;

            case CONFIG_OBJECT_TYPE.Item:
                ;
                break;

            case CONFIG_OBJECT_TYPE.Skill:
                go = effectPrefabData[_path];
                break;

            case CONFIG_OBJECT_TYPE.Buff:
                go = effectPrefabData[_path];
                break;
        }
        GameObject obj = Instantiate(go,Vector3.zero,Quaternion.identity);
        return obj;
    }
    public GameObject FindPrefab(CONFIG_OBJECT_TYPE _type, string _path, out Transform _parent) 
    {
        GameObject go = null;
        _parent = null;

        switch (_type) 
        {
            case CONFIG_OBJECT_TYPE.Player:
                _parent = CharacterTab;
                return go = PLAYERDATA.gameObject;

            case CONFIG_OBJECT_TYPE.Monster:
                _parent = CharacterTab;
                return go = MonsterPrefabData[_path];

            case CONFIG_OBJECT_TYPE.Weapon:
                ;
                break;

            case CONFIG_OBJECT_TYPE.Item:
                ;
                break;

            case CONFIG_OBJECT_TYPE.Skill:
                _parent = SkillTab;
                return go = effectPrefabData[_path];

            case CONFIG_OBJECT_TYPE.Buff:
                _parent = SkillTab;
                return go = effectPrefabData[_path];
        }
        return go;
    }
    public void ResoursLoadSkill() 
    {
        var info = Shared.Instance.DataManager.Skill_Table.SkillTableData;

        foreach (var kvp in info)
        {
            int key = kvp.Key;
            string path = kvp.Value.prefab;

            if (string.IsNullOrWhiteSpace(path) || path == "0")
            {
                Debug.Log($"[SkillPrefab] Skip: invalid path (SkillId={key}, path='{path}')");
                continue;
            }

            if (effectPrefabData.ContainsKey(path))
            {
                continue;
            }

            GameObject prefab = Resources.Load<GameObject>(path);
            if (prefab == null)
            {
                Debug.LogError($"[SkillPrefab] NOT FOUND at Resources/{path} (SkillId={key})");
                continue;
            }

            //GameObject obj = Instantiate(prefab, Vector3.zero, Quaternion.identity, SkillTab);
            //obj.SetActive(false);
            effectPrefabData.Add(path, prefab);
        }
    }
    
        public void ResoursLoadPlyer()
        {
            GameObject go = Resources.Load<GameObject>("Prefab/Character/Player/Player");
            Character_Base character = go.GetComponent<Character_Base>();
            var table = Shared.Instance.DataManager.Character_Table.Get((int)character.IdType);

            if (table.Id == (int)CHARACTER_ID.Player)//Player
            {
                //go = Instantiate(go, Vector3.zero, Quaternion.identity, CreatTab);
                Player player = go.GetComponent<Player>();
                PLAYERDATA = player;
                //go.SetActive(false);
                //effectPrefabData.Add(path, obj);//?
            }
        }

    public void ResoursLoadCharacter()
    {
        //GameObject[] list = Resources.LoadAll<GameObject>("Prefab/Character/Monster");//Resources
        //character_ResourseObj = new List<GameObject>(list);

        var info = Shared.Instance.DataManager.Character_Table.CharacterTableData;
        foreach (var kvp in info) 
        {
            int key = kvp.Key;
            string path = kvp.Value.Prefab;

            GameObject go = Resources.Load<GameObject>(path);

            if (go == null)
            {
                Debug.LogError($"_path = {go}");
                continue;
            }

            Character_Base character = go.GetComponent<Character_Base>();

            var table = Shared.Instance.DataManager.Character_Table.Get((int)character.IdType);

            if (table.Id == (int)CHARACTER_ID.Player)//Player
            {
                //go = Instantiate(go, Vector3.zero, Quaternion.identity, CreatTab);
                Player player = go.GetComponent<Player>();
                PLAYERDATA = player;
                //go.SetActive(false);

                //character_ResourseObj.Remove(go);
                //effectPrefabData.Add(path, obj);//?
            }

            else //Monster
            {
                if (table.Id == (int)character.IdType)
                {
                    if (table.Prefab != "" && table.Prefab != "0") 
                    {
                        MonsterPrefabData.Add(table.Prefab, go);//?
                        character_ResourseObj.Add(go);
                    }
                }
                else 
                {
                    Debug.LogError($"{table.Id} != {character.IdType}\n{table.Id} != {(int)character.IdType}$");
                }
            }


        }
        //    for (int i = 0; i < count; i++)
        //{
            
        //    //effectObj.Add(obj);
        //    //effectPrefabData.Add(path, obj);//?
        //}
    }

    public void ResourcesLoadBuff()
    {
        var info = Shared.Instance.DataManager.Buff_Table.BuffTableData;
        int count = info.Count;

        foreach (var kvp in info)
        {
            var data = kvp.Value;
            string path = data.effectPath;

            if (string.IsNullOrWhiteSpace(path) || path == "0")
                continue;

            if (effectPrefabData.ContainsKey(path))
                continue;

            GameObject prefab = Resources.Load<GameObject>(path);
            if (prefab == null)
            {
                Debug.Log("BuffEffect Prefab is Null");
                continue;
            }

            GameObject obj = Instantiate(prefab, Vector3.zero, Quaternion.identity, SkillTab);
            obj.SetActive(false);

            effectPrefabData.Add(path, obj);
        }
        Debug.Log("All BuffEffectPrefab Load Success");
    }

    //public GameObject ResoursLoad(string _path) 
    //{
    //    GameObject obj = Resources.Load<GameObject>($"{_path}");

    //    if (obj == null) 
    //    {
    //        Debug.LogError($"_path = {obj}");
    //        return null;
    //    }

    //    effectPrefabData.Add(_path, obj);

    //    return obj;
    //}
    public void ResoursCreat(Transform _parent) 
    {
        CreatTab = _parent;

        //for (int i = 0; i < effectObj.Count; i++)
        //{
        //    GameObject go = Instantiate(effectObj[i], Vector3.zero, Quaternion.identity, CreatTab);
        //    go.SetActive(false);
        //    effectPrefabData.Add(go);
        //}
    }
    
}
