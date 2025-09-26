using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

using static Enums.SCENE_SCENES;

//This script will be used only once in the splash scene.
public class Initializer : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
        Initialize();
    }
    private async void Initialize()
    {
        if (Shared.Instance == null) Shared.Instance = GetComponent<Shared>();
        await Shared.Instance.LoadManagerScripts();

        await UniTask.WaitUntil(()=> Shared.Instance.isPlay == true);

        //var task = Shared.Instance.SceneShared.LoadSceneAndInitializeAsync(BamserMap_Test);

        //await UniTask.WaitUntil(() => GameShard.Instance?.StageManager != null);
        //GameShard.Instance.StageManager.EnterStage(STAGE.Stage1);
        //Debug.Log("Stage1 Entered");

        //var task = Shared.Instance.SceneManager.LoadSceneAndInitializeAsync(ProtoTest);
        //var task = Shared.Instance.SceneManager.LoadSceneAndInitializeAsync(Title);

        await Shared.Instance.SceneShared.LoadSceneAsync(Loading_1, LoadSceneMode.Additive);
        Shared.Instance.SceneShared.SetActiveScene(Loading_1);
        Shared.Instance.SceneShared.UpdateCurrentScene();

        var task = Shared.Instance.SceneShared.LoadSceneAndInitializeAsync(Title, LoadSceneMode.Additive);
        Shared.Instance.SceneShared.SetInitHandler(task);

        await SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName(Enums.EnumToCustomString(Splash)));

        //Shared.Instance.UIManager.DeleteLoadingCanvas();

        await Shared.Instance.SceneShared.WaitUntilSceneInitCompleteAsync();
        Debug.Log($"SplashScene initialized: {Shared.Instance.SceneShared.CurrentScene}");

        Shared.Instance.UIManager.InitPopup();

        //Shared.Instance.UIManager.StartPopupActivation();

        await SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName(Enums.EnumToCustomString(Loading_1)));
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(Enums.EnumToCustomString(Title)));
        //SceneManager.SetActiveScene(SceneManager.GetSceneByName(Enums.EnumToCustomString(BamserMap_Test)));

        Shared.Instance.SceneShared.UpdateCurrentScene();


        await Shared.Instance.SoundManager.BgmPlayerSetting((int)BGM_ID_TYPE.Title);

        Destroy(this);
    }
}
