using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FailedTab : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI lastStageText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Button titleBtn;
    [SerializeField] private Animator animator;

    private Action onNext;
    private bool clicked;
    private bool isPaused;

    private const int failedSoundID = 20018;

    private void Awake()
    {
        if (animator == null) { animator = GetComponent<Animator>(); }

        gameObject.SetActive(false);
    }

    public void ShowFailedTab(STAGE stage, int startLevel, int endLevel, int clearScore, Action onNextPressed)
    {
        if (Shared.Instance.SoundManager)
            Shared.Instance.SoundManager.PlaySFXOneShot(failedSoundID);

        clicked = false;

        gameObject.SetActive(true);
        levelText.text = $"Level : Lv. {startLevel}  ¡æ  Lv. {endLevel}";
        lastStageText.text = $"Last Stage  : {stage.ToString()}";
        scoreText.text = $"Score : {clearScore}";

        titleBtn.interactable = true;
        onNext = onNextPressed;

        if (titleBtn != null)
        {
            titleBtn.onClick.RemoveAllListeners();
            titleBtn.onClick.AddListener(() =>
            {
                if (clicked) return;

                clicked = true;
                Hide();
                onNext?.Invoke();
            });

            if (EventSystem.current) EventSystem.current.SetSelectedGameObject(null);
        }

        SetPaused(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        SetPaused(false);
    }

    private void SetPaused(bool paused)
    {
        if (isPaused == paused) return;

        var im = GameShard.Instance?.InputManager;
        var mm = GameShard.Instance?.MonsterManager;
        if (im == null || mm == null)
        {
            Debug.Log("[FailedTab] InputManager or MonsterManager is Null");
            return;
        }
        im.isUIOpen = paused;
        mm.UiStateUpdate(paused);
        isPaused = paused;
    }

    private void EnableTitleButton()
    {
        if (!titleBtn) return;
        titleBtn.interactable = true;

        if (EventSystem.current)
            EventSystem.current.SetSelectedGameObject(titleBtn.gameObject);
    }
}
