using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ClearTab : MonoBehaviour
{
    public struct ItemEntry
    {
        public int itemId;
        public int count;
        public Sprite sprite;
    }

    [SerializeField] private TextMeshProUGUI stageText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI killText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Button nextStageBtn;
    [SerializeField] private Animator animator;

    [Header("Aquired Item / Kill UI")]
    [SerializeField] private Transform itemRoot;
    [SerializeField] private GameObject slot;


    private Action onNext;
    private bool clicked;
    private bool isPaused;

    private const int clearSoundID = 20017;


    private void Awake()
    {
        if (animator == null) { animator = GetComponent<Animator>(); }

        gameObject.SetActive(false);
    }

    public void ShowClearTab(STAGE stage, int startLevel, int endLevel, int beforeScore, int clearScore, Action onNextPressed, List<ItemEntry> acquired = null,int totalKills = 0, bool toTitle = false)
    {
        if (Shared.Instance.SoundManager)
            Shared.Instance.SoundManager.PlaySFXOneShot(clearSoundID);

        clicked = false;

        gameObject.SetActive(true);
        nextStageBtn.interactable = false;


        stageText.text = toTitle ? "All Clear!" : $"{stage} Clear!"; ;
        levelText.text = $"Level : Lv. {startLevel}  ¡æ  Lv. {endLevel}";
        killText.text = $"Kills : {totalKills}";
        scoreText.text = $"Score : {beforeScore}  ->  {clearScore}";

        onNext = onNextPressed;

        var btnLavel = nextStageBtn.GetComponentInChildren<TextMeshProUGUI>(true);
        if (btnLavel)
            btnLavel.text = toTitle ? "Title" : "Next Stage";

        if (nextStageBtn != null)
        {
            nextStageBtn.onClick.RemoveAllListeners();
            nextStageBtn.onClick.AddListener(() =>
            {
                if (clicked) return;

                clicked = true;
                Hide();
                onNext?.Invoke();
            });

            if (EventSystem.current) EventSystem.current.SetSelectedGameObject(null);
        }

        BuildAcquired(acquired);    
        SetPasued(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        SetPasued(false);

    }

    private string FormatMMSS(float seconds)
    {
        int total = Mathf.FloorToInt(seconds);
        int m = total / 60;
        int s = total % 60;
        return $"{m:00}:{s:00}";
    }

    private void SetPasued(bool paused)
    {
        if (isPaused == paused) return;

        var im = GameShard.Instance?.InputManager;
        var mm = GameShard.Instance?.MonsterManager;

        if (im == null || mm == null)
        {
            Debug.Log("[ClearTab] InputManager or MonsterManager is Null");
            return;
        }

        im.isUIOpen = paused;
        mm.UiStateUpdate(paused);
        isPaused = paused;
    }

    private void EnableNextButton()
    {
        if (!nextStageBtn) 
            return;
        nextStageBtn.interactable = true;

        if (EventSystem.current) 
            EventSystem.current.SetSelectedGameObject(nextStageBtn.gameObject);
    }

    private void BuildAcquired(List<ItemEntry> acquired)
    {
        if (!itemRoot) return;

        for (int i = itemRoot.childCount - 1; i >= 0; i--)
            Destroy(itemRoot.GetChild(i).gameObject);

        if (acquired == null || acquired.Count == 0 || !slot)
            return;

        for (int i = 0; i < acquired.Count; i++)
        {
            var e = acquired[i];
            var slotGo = GameObject.Instantiate(slot, itemRoot);
            if (slotGo != null)
            {
                var acquiredSlot = slotGo.GetComponent<AcquiredSlot>();
                acquiredSlot.SetSlot(e.sprite, e.count);
            }
        }
    }

}
