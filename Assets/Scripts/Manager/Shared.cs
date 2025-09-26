using Cysharp.Threading.Tasks;
using System.IO;
using UnityEditor.Overlays;
using UnityEngine;
using static Enums.SCENE_SCENES;
public interface ISceneInitializable
{
    UniTask Init();
}

public class Shared : MonoBehaviour
{
    public static Shared Instance { get; set; }
    public DataManager DataManager { get; private set; }
    public UiManager UIManager { get; private set; }
    public AtlasManager AtlasManager { get; private set; }
    //public Setting_Manager SettingManager { get; private set; }
    public SceneShared SceneShared { get; private set; }
    public SoundManager SoundManager { get; set; }

    public ResourcesManager ResourcesManager { get; set; }
    public LanguageManager LanguageManager { get; set; }

    public bool isReplay = false;
    public bool isPlay = false;
    private void Awake()
    {
        Instance = this;
        Debug.Log(this.gameObject);
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        //DontDestroyOnLoad(this);
    }

    public async UniTask LoadManagerScripts()
    {
        if (AtlasManager == null) AtlasManager = gameObject.AddComponent<AtlasManager>();
        await AtlasManager.Initialize(nameof(AtlasManager));

        if (DataManager == null) DataManager = gameObject.AddComponent<DataManager>();
        await DataManager.Initialize(nameof(DataManager));

        if(ResourcesManager == null) ResourcesManager = gameObject.AddComponent<ResourcesManager>();
        await ResourcesManager.Init();

        if (SceneShared == null) SceneShared = gameObject.AddComponent<SceneShared>();
        await SceneShared.Initialize(nameof(SceneShared));

        if (SoundManager == null) SoundManager = gameObject.AddComponent<SoundManager>();
        await SoundManager.Init();

        if (LanguageManager == null) LanguageManager = gameObject.AddComponent<LanguageManager>();
        await LanguageManager.Init();
       

        //if (SettingManager == null) SettingManager = gameObject.AddComponent<Setting_Manager>();
        //await SettingManager.Initialize(nameof(Setting_Manager));

        if (UIManager == null) UIManager = gameObject.AddComponent<UiManager>();
        await UIManager.Initialize(nameof(UiManager));

    }
    public async UniTask PlyerLoginDataSave(string _Key, string _Value)
    {
        PlayerPrefs.SetString(_Key, _Value);
        PlayerPrefs.Save();
        await UniTask.CompletedTask;
    }
    public string PlyerLoginDataLoad(string _Key)
    {
        return PlayerPrefs.GetString(_Key);
    }

    //public class PlayerData
    //{
    //    public string playerName;
    //    public int Score;
    //    public int KillCount;
    //}
    public string savePath { get; set; }
    public void PathsLaod()
    {
        savePath = Path.Combine(Application.persistentDataPath, "playerData.json");
    }
    public void Save(PlayerDataList dataList)
    {
        string json = JsonUtility.ToJson(dataList, true);
        File.WriteAllText(savePath, json);
        Debug.Log("저장 완료: " + savePath);
    }

    // 불러오기
    public PlayerDataList Load()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            PlayerDataList dataList = JsonUtility.FromJson<PlayerDataList>(json);
            Debug.Log("불러오기 완료");
            return dataList;
        }
        else
        {
            Debug.LogWarning("저장된 파일이 없음");
            return null;
        }
    }
}
