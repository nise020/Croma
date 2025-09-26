using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Enums;
using static Enums.SCENE_SCENES;


public class SceneShared : Manager_Base
{
    public UniTask AsyncHandler { get; private set; }
    public SCENE_SCENES CurrentScene { get; private set; }
    public async override UniTask Initialize(string _str)
    {
        await base.Initialize(_str);
        UpdateCurrentScene();
        Initialized(_str);
    }
    public void UpdateCurrentScene() => CurrentScene = Enums.StringToEnum<SCENE_SCENES>(SceneManager.GetActiveScene().name);
    public void SetActiveScene(SCENE_SCENES _Scene) 
        => SceneManager.SetActiveScene(SceneManager.
            GetSceneByName(Enums.EnumToCustomString(_Scene)));
    public async UniTask LoadSceneAsync(SCENE_SCENES _Scene, LoadSceneMode _Mode)
    {
        if (CurrentScene == _Scene) return;
        await SceneManager.LoadSceneAsync(Enums.EnumToCustomString(_Scene), _Mode);
        UpdateCurrentScene();
        SetActiveScene(CurrentScene);
    }

    public async UniTask LoadSceneAndInitializeAsync(SCENE_SCENES _Scene, LoadSceneMode _mode)
    {
        if (CurrentScene == _Scene) return;

        var asyncScene = SceneManager.LoadSceneAsync(Enums.EnumToCustomString(_Scene), _mode);
        if (asyncScene == null) return;
        asyncScene.allowSceneActivation = false;

        while (asyncScene.progress < 0.9f) await UniTask.Yield();
        asyncScene.allowSceneActivation = true;

        Scene loaded = SceneManager.GetSceneByName(Enums.EnumToCustomString(_Scene));

        while (!loaded.isLoaded) await UniTask.Yield();

        GameObject[] rootObjects = loaded.GetRootGameObjects();
        List<UniTask> initTasks = new();
        foreach (GameObject obj in rootObjects)
        {
            ISceneInitializable[] initializables = obj.GetComponentsInChildren<ISceneInitializable>(true);
            foreach (var init in initializables) initTasks.Add(init.Init());
        }

        await UniTask.WhenAll(initTasks);
        return;
    }

    public async UniTask SceneChangeAsync(SCENE_SCENES _Scene, LoadSceneMode _mode, Func<Func<float, UniTask>, UniTask> onDone = null)
    {

        Shared.Instance.SoundManager.BgmPlaying(false);

        await Shared.Instance.UIManager.FadeEvent(true);

        #region Loding

        //await Shared.Instance.SceneShared.LoadSceneAsync(SCENE_SCENES.Loading_1, 
        //    LoadSceneMode.Additive);

        //Shared.Instance.SceneShared.SetActiveScene(SCENE_SCENES.Loading_1);
        //Shared.Instance.SceneShared.UpdateCurrentScene();

        // 2. 목표 씬 로드 (Single 모드)
        //var task = Shared.Instance.SceneShared.LoadNextSceneAndInitializeAsync(_Scene, _mode);

        //Shared.Instance.SceneShared.SetInitHandler(task);
        //await Shared.Instance.SceneShared.WaitUntilSceneInitCompleteAsync();


        // 3. 목표 씬 활성화
        //SceneManager.SetActiveScene(SceneManager.GetSceneByName(Enums.EnumToCustomString(_Scene)));

        #endregion
        await Shared.Instance.UIManager.LodingText();


        await Shared.Instance.SceneShared.LoadNextSceneAndInitializeAsync(_Scene, _mode, async (p) =>
        {
            await Shared.Instance.UIManager.UpdateLoadingBar(p * 0.5f, (int)_Scene);
            //SetInitHandler(task);
        });

        UpdateCurrentScene();


        //if (_mode == LoadSceneMode.Additive) 
        //{
        //    var titleScene = SceneManager.GetSceneByName(Enums.EnumToCustomString(SCENE_SCENES.Title));
        //    if (titleScene.IsValid() && titleScene.isLoaded)
        //        await SceneManager.UnloadSceneAsync(titleScene);
        //}

        //4.로딩씬 제거

        //if (onDone != null)  await onDone();


        if (onDone != null)
        {
            await onDone(async (p) =>
            {
                await Shared.Instance.UIManager.UpdateLoadingBar(0.5f + p * 0.5f, (int)_Scene);
                await UniTask.Yield();
            });
        }
        else
        {
            await Shared.Instance.UIManager.UpdateLoadingBar(1f, (int)_Scene);
        }
        //await UniTask.Delay(TimeSpan.FromSeconds(10.0f));

        #region Loding Destroy
        //var loadingScene = SceneManager.GetSceneByName(Enums.EnumToCustomString(SCENE_SCENES.Loading_1));
        //if (loadingScene.IsValid() && loadingScene.isLoaded)
        //    await SceneManager.UnloadSceneAsync(loadingScene);
        #endregion

       // await Shared.Instance.UIManager.FadeEvent(false);
        //Shared.Instance.SoundManager.BgmPlaying(true);
        //await Shared.Instance.UIManager.FadeEvent(false);


        //Shared.Instance.UIManager.MainCanvas.SetActive(false);
    }

    public void Play()
    {
        if (GameShard.Instance != null)
        {
            Destroy(GameShard.Instance.gameObject);
            GameShard.Instance = null;
        }

        SceneChangeAsync(BamserMap_Stage_1, LoadSceneMode.Single, async (report) =>
        {
            for(int i = 0; i < 5; i++)
            {

            await UniTask.Delay(300);
            await report(i / 5f); 

            }

            await WaitForGameStart();
            await report(1f);

        }).Forget();
        Debug.Log("Play");
    }
    public void RankingOn() 
    {
        SceneChangeAsync(RankingScene, LoadSceneMode.Single, async (report) =>
        {
            for (int i = 0; i < 5; i++)
            {

                await UniTask.Delay(300);
                await report(i / 5f);

            }

            await WaitLoadRankData();
            await report(1f);

        }).Forget();
        Debug.Log("Rank");
    }
    private async UniTask WaitLoadRankData()
    {
        await UniTask.WaitUntil(() => RankingTab.Instance != null);

        await RankingTab.Instance.RankSetting();

        var table = Shared.Instance.DataManager.Stage_Table.Get((int)SCENE_SCENES.RankingScene);
        await Shared.Instance.SoundManager.BgmPlayerSetting(table.SoundId);
    }
    private async UniTask WaitForGameStart()
    {
        await UniTask.WaitUntil(() => GameShard.Instance != null);

        await GameShard.Instance.GameStart();

        var table = Shared.Instance.DataManager.Stage_Table.Get((int)STAGE.Stage1);
        await Shared.Instance.SoundManager.BgmPlayerSetting(table.SoundId);
    }


    public void SetInitHandler(UniTask _handler) => AsyncHandler = _handler;
    public async UniTask LoadNextSceneAndInitializeAsync(SCENE_SCENES _Scene, LoadSceneMode _mode, Action<float> onProgress = null)
    {
        if (CurrentScene == _Scene) return;

        var asyncScene = SceneManager.LoadSceneAsync(Enums.EnumToCustomString(_Scene), _mode);
        if (asyncScene == null) return;
        asyncScene.allowSceneActivation = false;

        while (asyncScene.progress < 0.9f) 
        {
            onProgress?.Invoke(asyncScene.progress / 0.9f);
            await UniTask.Yield();
        }
        asyncScene.allowSceneActivation = true;

        Scene loaded = SceneManager.GetSceneByName(Enums.EnumToCustomString(_Scene));

        while (!loaded.isLoaded) await UniTask.Yield();

        //All Game Object Reset

        //GameObject[] rootObjects = loaded.GetRootGameObjects();
        //List<UniTask> initTasks = new();
        //foreach (GameObject obj in rootObjects)
        //{
        //    ISceneInitializable[] initializables = obj.GetComponentsInChildren<ISceneInitializable>(true);
        //    foreach (var init in initializables) initTasks.Add(init.Init());
        //}

        //await UniTask.WhenAll(initTasks);

        onProgress?.Invoke(1f);
        return;
    }
    public UniTask WaitUntilSceneInitCompleteAsync() => AsyncHandler.Status != UniTaskStatus.Pending ? UniTask.CompletedTask : AsyncHandler;
    //public UniTask WaitUntilSceneInitComplete();
}
