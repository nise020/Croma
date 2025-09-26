using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Enums;
using static Enums.SCENE_SCENES;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private ColorSlotUI colorSlotUI;
    private ColorSystem colorSystem;
    private ColorSlot colorSlot;

    public Dictionary<Room, GameObject> roomData = new Dictionary<Room, GameObject>();
    public List<Room> rooms = new List<Room>();
    public Action<Room,Room> PotalEvent;
    public Room CurrentRoom;
    public Player Player;
    public GameObject PlayerObj;
    Transform CreatTab;

    private GameObject FadeObj;
    private Camera playerVeiwCam;
    private FollowCamera3D followCam;

    public int GameScore = 0;
    public Action<int> OnScoreChanged;

    public async UniTask InitAsync()
    {
        GameShard.Instance.GameManager = this;

        //playerVeiwCam = Camera.main;
        //DontDestroyOnLoad(playerVeiwCam);

        Player = PlayerCreat();
        //GameShard.Instance.BattleManager.PlayerStart(Player);
        GameShard.Instance.GameUiManager.PlayerInit(Player);
        GameShard.Instance.MonsterManager.PlayerInit(Player);
        await UniTask.Yield(); // 필요시 프레임 분산
    }
    public Player PlayerCreat() 
    {
        Player player = null;
        if (CreatTab == null)
        {
            GameObject go = new GameObject($"PlayerTab");
            CreatTab = go.transform;
        }

        player = Shared.Instance.ResourcesManager.PLAYERDATA;

        Vector3 spawnXZ = new Vector3(0, 200f, 0); // 임시로 높이 위쪽
        Ray ray = new Ray(spawnXZ, Vector3.down);

        LayerMask ground = LayerMask.GetMask(LAYER_TYPE.Walkable.ToString());
        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, ground)) 
        {
            float groundY = hitInfo.point.y;
            PlayerObj = Instantiate(player.gameObject, new Vector3(0, groundY, 0),
                                Quaternion.identity, CreatTab);
            PlayerObj.SetActive(true);
        }

        player = PlayerObj.GetComponent<Player>();

        //Camera camObj = Camera.main;
        //GameObject camObj = new GameObject("PlayerVeiwCam");
        //camObj.AddComponent<Camera>();
        //followCam = camObj.gameObject.AddComponent<FollowCamera3D>();
        //followCam = camObj.gameObject.AddComponent<FollowCamera3D>();

        //GameObject cam = Shared.Instance.ResourcesManager.FollowCamPrefab;
        //cam = Instantiate(cam);
        followCam = GameShard.Instance.followCamera3D;
        followCam.init(player);

        player.ApplyData(player, followCam);
        Player = player;

        //playerVeiwCam = Camera.main;

        return Player;
    }
    //private IEnumerator FadeSceneTitle(SCENE_SCENES _scene, Action onDone)
    //{
    //    GameShard.Instance.InputManager.isFade = true;
    //    GameShard.Instance.MonsterManager.isFade = true;

    //    GameShard.Instance.MonsterManager.CharacterActive(false);

    //    yield return GameShard.Instance.GameUiManager.FadeEvent(true);

    //    var currentScene = SceneManager.GetActiveScene().name;

    //    // 씬 로드 (비동기라면 Async로 변경)
    //    yield return Shared.Instance.SceneShared.SceneChangeAsync(_scene, LoadSceneMode.Single);

    //    // 이전 씬 언로드
    //    yield return SceneManager.UnloadSceneAsync(currentScene);

    //    yield return null;

    //    onDone?.Invoke();

    //    yield return GameShard.Instance.GameUiManager.FadeEvent(false);

    //    GameShard.Instance.InputManager.isFade = false;
    //    GameShard.Instance.MonsterManager.isFade = false;
    //}
    public async UniTask FadeEvent(SCENE_SCENES _scene, Func<UniTask> onDone = null)
    {
        await FadeSceneChange(_scene, onDone);
    }

    public async UniTask FadeSceneChange(SCENE_SCENES _scene, Func<UniTask> onDone = null) 
    {
        GameShard.Instance.InputManager.isFade = true;
        GameShard.Instance.MonsterManager.isFade = true;

        GameShard.Instance.MonsterManager.CharacterActive(false);

        //yield return GameShard.Instance.GameUiManager.FadeEvent(true);

        //var currentScene = SceneManager.GetActiveScene().name;

        // 씬 로드 + fade
        await Shared.Instance.SceneShared.SceneChangeAsync(_scene, LoadSceneMode.Single, async (report) => 
        {
            if (onDone != null)
                await onDone();
            await report(1f);
            //Shared.Instance.SoundManager.BgmPlayer.Stop();
        });

        //yield return null;

        //yield return GameShard.Instance.GameUiManager.FadeEvent(false);

        GameShard.Instance.InputManager.isFade = false;
        GameShard.Instance.MonsterManager.isFade = false;

        //await Shared.Instance.SoundManager.BgmPlayerSetting(BGM_ID_TYPE.Stage1);

        //StageManager.EnterStage(nextStage, () =>
        //{
        //    GameShard.Instance.MonsterManager.SpawnActive();
        //});


        // 이전 씬 언로드
        //yield return SceneManager.UnloadSceneAsync(currentScene);
    }

    public void PlusGameScore(int score)
    {
        if (score < 0) return;

        GameScore += score;
        OnScoreChanged?.Invoke(GameScore);
    }
    
    public int GetScore()
    {
        return GameScore;
    }



    //private void Start()
    //{

    //}
    //public void PotalEventCheck(Room _fromRoom, Room _toRoom)
    //{
    //    if (_fromRoom == _toRoom) return;

    //    if (roomData.TryGetValue(_fromRoom, out GameObject fromObj))
    //    {
    //        _fromRoom.RoomEventcheck(false); // ���� �� ��Ȱ��ȭ
    //    }

    //    if (roomData.TryGetValue(_toRoom, out GameObject toObj))
    //    {
    //        _toRoom.RoomEventcheck(true); // ���� �� Ȱ��ȭ
    //        CurrentRoom = _toRoom;
    //    }
    //}

    //public void RoomTransition(Room fromRoom, Room toRoom)
    //{
    //    if (!FadeObj.activeSelf)
    //    {
    //        FadeObj.SetActive(true);
    //    }

    //    StartCoroutine(FadeAndChangeRoom(fromRoom, toRoom));
    //}

    //private IEnumerator FadeAndChangeRoom(Room from, Room to)
    //{
        
    //    Player.playerInPut.enabled = false;
    //    yield return StartCoroutine(FadeEffect.Fade(true));

    //    if (from != to)
    //    {
    //        if (roomData.TryGetValue(from, out GameObject fromObj))
    //        {
    //            from.RoomEventcheck(false); // ���� �� ��Ȱ��ȭ
    //        }

    //        if (roomData.TryGetValue(to, out GameObject toObj))
    //        {
    //            to.RoomEventcheck(true); // ���� �� Ȱ��ȭ
    //            CurrentRoom = to;
    //        }
    //    }

    //    yield return StartCoroutine(FadeEffect.Fade(false));
    //    FadeEffect.gameObject.SetActive(false);
    //    Player.playerInPut.enabled = true;
    //}

   
    //private IEnumerator UpdateGaugeRoutine()
    //{
    //    WaitForSeconds wait = new WaitForSeconds(0.1f); 
    //    while (true)
    //    {
    //        colorSlotUI.UpdateSlot(colorSystem.colorSlot.GetColors());
    //        colorSlotUI.UpdateGauge();
    //        yield return wait;
    //    }
    //}

   

}