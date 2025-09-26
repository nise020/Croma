using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static Enums;
using static Enums.SCENE_SCENES;

public class GamePauseMenu : MonoBehaviour
{
    [SerializeField] private Button continueBtn;
    [SerializeField] private Button optionBtn;
    [SerializeField] private Button titleBtn;
    [SerializeField] private GameObject pausePanel;


    void Awake()
    {
        continueBtn.onClick.AddListener(ContinueGame);
        optionBtn.onClick.AddListener(OpenOption);
        titleBtn.onClick.AddListener(() => BacktoTitle().Forget());
    }


    private void ContinueGame()
    {

        Time.timeScale = 1f;

        if (pausePanel != null) pausePanel.SetActive(false);

        if (MenuSystem.Instance != null)
            MenuSystem.Instance.UiActive(false); 

    }

    private void OpenOption()
    {
        Shared.Instance.UIManager.ActivePopup(true, UI_TITLE_POPUP_LIST.Option);
    }

    async UniTaskVoid BacktoTitle()
    {
        Time.timeScale = 1f;

        UniTask task;
        if (GameShard.Instance?.GameManager != null)
        {
            task = GameShard.Instance.GameManager.FadeSceneChange(Title, async () =>
            {
                //if (GameShard.Instance != null)
                //    Destroy(GameShard.Instance.gameObject);
                await Shared.Instance.SoundManager.BgmPlayerSetting((int)BGM_ID_TYPE.Title);
                await UniTask.CompletedTask;
            });
        }
        else
        {
            task = Shared.Instance.SceneShared.SceneChangeAsync(
                Title,
                UnityEngine.SceneManagement.LoadSceneMode.Single,
                async (report) =>
                {
                    //if (GameShard.Instance != null)
                    //    Destroy(GameShard.Instance.gameObject);
                    await Shared.Instance.SoundManager.BgmPlayerSetting((int)BGM_ID_TYPE.Title);
                    await UniTask.CompletedTask;
                });
        }

        await task;
    }
}