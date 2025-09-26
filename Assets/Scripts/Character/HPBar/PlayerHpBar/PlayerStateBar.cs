using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Enums;

public partial class PlayerStateBar : StateBar
{
    [Header("ExpImg")]
    [SerializeField] private Image expImage = null;

    [Header("Skill")]
    [SerializeField] private Image Skill_1Image = null;
    [SerializeField] private TMP_Text Skill_1Text = null;

    [SerializeField] private Image Skill_2Image = null;
    [SerializeField] private TMP_Text Skill_2Text = null;

    [SerializeField] private Image Skill_3Image = null;
    [SerializeField] private TMP_Text Skill_3Text = null;

    [SerializeField] private Image Skill_4Image = null;
    [SerializeField] private TMP_Text BurstText = null;

    [Header("HpText")]
    [SerializeField] private Text maxHpText = null;
    [SerializeField] private Text hpText = null;

    [SerializeField] private Text maxExpText = null;
    [SerializeField] private Text expText = null;

    //[SerializeField] public BuffHUD buffHud;
    private const float maxDuration = 2;
    public Action<float> ExpUpdateEvent { get; set; } = null;
    public Action<float> BurstExpUpdateEvent { get; set; } = null;
    Coroutine burstCharging = null;
    public event Action<int> LevelUpEvent;
    StatTab stateControllUi;

    [Header("ToolTip")]
    [SerializeField] private GameObject GuidTab = null;
    [SerializeField] List<GameObject> ToolTipPanel;

    [Header("Skip")]
    [SerializeField] Button NextStage;
    [SerializeField] Button bOSSoN;

    [Header("StatValue")]
    [ReadOnly] public int currentLevel = 1;
    [ReadOnly] public float currentExp = 0;
    [ReadOnly] public float expToLevelUp = 0;
    [ReadOnly] public float expIncreaseRate = 1.5f;
    [ReadOnly] public float currentBurst = 0;
    protected float ExpeffectTime = 1.0f;

    private IEnumerator ExpBarEvent { get; set; } = null;
    private float targetFill;

    public BuffHUD buffHUD;
    private void Awake()
    {
        var table = Shared.Instance.DataManager.Skill_Table.SkillTableData;

        foreach (var value in table)
        {
            SKILL_ID_TYPE key = (SKILL_ID_TYPE)value.Key;
            SkillData data = value.Value;
            SkillDatas.Add(key, data);
        }

        buffHUD = GetComponentInChildren<BuffHUD>();
    }

    protected override void Start()
    {
        base.Start();
        ExpUpdateEvent += SetExp;
        BurstExpUpdateEvent += SetBurst;
        if (Skill_4Image.fillAmount != 0) 
        {
            Skill_4Image.fillAmount = 0.0f;
        }

        if (Shared.Instance.isReplay == false) 
        {
            Shared.Instance.isReplay = true;
            GuidTabOn();
        }

        NextStage.onClick.AddListener(GameShard.Instance.StageManager.SkipStage);
        bOSSoN.onClick.AddListener(GameShard.Instance.StageManager.BossTimerSkip);
    }


    public override void InitializeCharacter(Character_Base character)
    {
        base.InitializeCharacter(character);
    }
    public override void InitializeImage()
    {
        base .InitializeImage();
        if (expImage.fillAmount != 0)
        {
            expImage.fillAmount = 0.0f;
        }
        
    }
    public override void SetHP(float _MaxHP, float _CurHP)
    {
        maxHpText.text = _MaxHP.ToString();
        hpText.text = _CurHP.ToString();

         base.SetHP(_MaxHP, _CurHP);
    }
    public void GuidTabOn() 
    {
        GuidTab.SetActive(true);
        GameShard.Instance.GameUiManager.UiActiveSatckData.Push(GuidTab);
        GameShard.Instance.InputManager.isUIOpen = true;
        GameShard.Instance.MonsterManager.UiStateUpdate(true);
    }

    public void GuidTabOFF()
    {
        GuidTab.SetActive(false);
        GameShard.Instance.GameUiManager.UiActiveSatckData.Pop();
        GameShard.Instance.InputManager.isUIOpen = false;
        GameShard.Instance.InputManager.isMouse = false;
        GameShard.Instance.MonsterManager.UiStateUpdate(false);
    }

}
