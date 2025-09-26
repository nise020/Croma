using UnityEngine;
using Game.InGame;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

public class GameShard : MonoBehaviour
{
    public static GameShard Instance { get; set; }
    public BattleManager BattleManager { get; set; }
    public MonsterManager MonsterManager { get; set; }
    public  GameManager GameManager { get; set; }

    //Test
    public EquiptmentService EquiptmentService { get; set; }
    public StageManager StageManager { get; set; }
    public QuestManager QuestManager { get; set; }

    public InputManager InputManager { get; set; }
    public Game.InGame.UIManager GameUiManager { get; set; }

    public FollowCamera3D followCamera3D { get; set; }
    private void OnDestroy()
    {
        BattleManager = null;
        MonsterManager = null;
        GameManager = null;
        EquiptmentService = null;
        StageManager = null;
        InputManager = null;
        GameUiManager = null;
        Destroy(followCamera3D.gameObject);
    }

    private void Awake()
    {
        //if (Shared.Instance == null) 
        //{
        //    SceneManager.SetActiveScene(SceneManager.GetSceneByName(Enums.EnumToCustomString(SCENE_SCENES.Splash)));
        //}

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else 
        {
            Destroy(gameObject);
            return;
        }

        MonsterManager = gameObject.AddComponent<MonsterManager>();
        BattleManager = gameObject.AddComponent<BattleManager>();
        GameManager = gameObject.AddComponent<GameManager>();
        InputManager = gameObject.AddComponent<InputManager>();
        QuestManager = gameObject.AddComponent<QuestManager>();
        StageManager = gameObject.AddComponent<StageManager>();
        GameUiManager = gameObject.AddComponent<Game.InGame.UIManager>();
        EquiptmentService = new EquiptmentService();

        GameObject cam = Shared.Instance.ResourcesManager.FollowCamPrefab;
        cam = Instantiate(cam);
        followCamera3D = cam.gameObject.GetComponentInChildren<FollowCamera3D>();
        DontDestroyOnLoad (followCamera3D.gameObject);

    }

    //public IEnumerator GameStart() 
    //{
    //    yield return GameManager.Init();

    //    yield return GameUiManager.Init();

    //    //yield return GameUiManager.FadeEvent(true);

    //    yield return StageManager.Init();

    //    yield return MonsterManager.Init();

    //    yield return BattleManager.Init();

    //    yield return InputManager.Init();

    //    StageManager.EnterStage(STAGE.Stage1, () => 
    //    {
    //        MonsterManager.SpawnActive();
    //    });
    //    yield return new WaitForSeconds(2.0f) ;

    //}

    public async UniTask GameStart()
    {
        await UniTask.WhenAll(
            GameManager.InitAsync(),
            GameUiManager.InitAsync(),
            StageManager.InitAsync(),
            MonsterManager.InitAsync(),
            BattleManager.InitAsync(),
            InputManager.InitAsync()
        );

        StageManager.EnterStage(STAGE.Stage1);
    }


}
