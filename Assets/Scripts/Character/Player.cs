using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static Enums;
using static Enums.ANIMATION_PATAMETERS_TYPE;
using static Enums.CHARACTER_DATA;
using static Enums.CHARACTER_STATUS;

public partial class Player : Character_Base
{
    public Room MyRoom;
    public override CONFIG_OBJECT_TYPE ObjectType => CONFIG_OBJECT_TYPE.Player;
    protected PlayerStateBar StateBar { get; set; } = new();
    //Test
    public BuffHUD buffHUD;
    public override void HpBarInit(StateBar _hpBar) => StateBar = _hpBar as PlayerStateBar;
    public Camera mainCamera;
    public Image SkillImg;
    //private Rigidbody rb { get; set; } = null;
    //[SerializeField] private float moveSpeed = 5f;
    //[SerializeField] private float jumpForce = 7f;
    //[SerializeField] private float dashForce = 30f;
    //[SerializeField] private float dashDuration = 0.15f;
    //[SerializeField] private float gravityMultiplier = 3f;
    //private Vector2 moveInput;
    //private bool isDashing = false;
    //private float dashTimer = 0f;
    //private Vector3 dashDirection;

    [SerializeField] public GameObject inventory;
    [SerializeField] public GameObject popupCanvas;
    private GameObject inventoryObj;
    public MenuSystem menuSystem;
    //Status
    Dictionary<PLAYER_DEBUFF, CancellationTokenSource> debuffTimers = new();

    // Color Test
    [Header("Color Systems")]
    public ColorSystem colorSystem;
    public ColorSlot colorSlot;
    public bool isInvenOpen = false;

    public ColorSteal colorSteal;

    [SerializeField] public ColorSlotUI colorSlotUI;

    // Basic Skill
    private Func<bool> shieldBlockFunc;

    //public PlayerController playerController => GetComponent<PlayerController>();
    public PlayerInput playerInPut => GetComponent<PlayerInput>();

    [Header("Weapon")]
    

    [Header("Input Controll")]
    private Vector3 moveDir;
    private KeyCode inputKey;
    private MOUSE_INPUT_TYPE mouseType;

    int LevelId = 1;

  
    public CancellationTokenSource moveCTS;

    FollowCamera3D viewcam;
    private Queue<(int value, SKILL_ID_TYPE type)> reservedSkills
        = new Queue<(int, SKILL_ID_TYPE)>();
    protected override void Awake()
    {
        base.Awake();
        //viewcam = Camera.main;
        StateBar = GetComponentInChildren<PlayerStateBar>();

        WeaponObj = GetWeaponObj();

        WeaponSoundPlayer = WeaponObj.AddComponent<AudioSource>();
        WeaponSoundPlayer.outputAudioMixerGroup = Shared.Instance.SoundManager.GetMixser(OptionHandler.SOUND.Effect);
        WeaponSoundPlayer.spatialBlend = 0.0f;

        CharacterSoundPlayer = gameObject.AddComponent<AudioSource>();
        WeaponSoundPlayer.outputAudioMixerGroup = Shared.Instance.SoundManager.GetMixser(OptionHandler.SOUND.Effect);
        CharacterSoundPlayer.spatialBlend = 0.0f;
    }

    public void StatDataUpdate(LevelTable.Info info)
    {
        // 키 보장
        if (!StatusData.ContainsKey(CHARACTER_STATUS.MaxHp)) StatusData[CHARACTER_STATUS.MaxHp] = 0f;
        if (!StatusData.ContainsKey(CHARACTER_STATUS.Hp)) StatusData[CHARACTER_STATUS.Hp] = 0f;
        if (!StatusData.ContainsKey(CHARACTER_STATUS.Atk)) StatusData[CHARACTER_STATUS.Atk] = 0f;
        if (!StatusData.ContainsKey(CHARACTER_STATUS.Def)) StatusData[CHARACTER_STATUS.Def] = 0f;
        if (!StatusData.ContainsKey(CHARACTER_STATUS.Speed)) StatusData[CHARACTER_STATUS.Speed] = 0f;

        float oldMax = StatusData[CHARACTER_STATUS.MaxHp];
        float oldCur = StatusData[CHARACTER_STATUS.Hp];

        StatusData[CHARACTER_STATUS.Speed] = info.Speed;
        StatusData[CHARACTER_STATUS.Atk] = info.Atk;
        StatusData[CHARACTER_STATUS.Def] = info.Def;
        StatusData[CHARACTER_STATUS.MaxHp] = info.MaxHp;

        // 현재 HP 비율 유지
        if (oldMax > 0f)
        {
            float ratio = Mathf.Clamp01(oldCur / oldMax);
            StatusData[CHARACTER_STATUS.Hp] = ratio * StatusData[CHARACTER_STATUS.MaxHp];
        }
        else
        {
            StatusData[CHARACTER_STATUS.Hp] = StatusData[CHARACTER_STATUS.MaxHp];
        }

        HpBarChanged?.Invoke(StatusData[CHARACTER_STATUS.MaxHp], StatusData[CHARACTER_STATUS.Hp]);
    }

    public void StatValueUpdate(Dictionary<CHARACTER_STATUS, int> PlusStatData) 
    {
        //StatusData[_type] += _value;
    }

    protected override void Start()
    {
        CapsuleCollider capsuleCollider = GetComponentInChildren<CapsuleCollider>();
        Debug.Log($"capsuleCollider = {capsuleCollider}");
        base.Start();

        //StateCheckData.Add(ANIMATION_PATAMETERS_TYPE.Walk.ToString(), false);
        //StateCheckData.Add(ANIMATION_PATAMETERS_TYPE.KnockBack.ToString(), false);;
    }

    protected override void OnDestroy()
    {
        if (GameShard.Instance.InputManager != null) 
        {
            GameShard.Instance.InputManager.KeyinPutEventData -= inPutKeyEvent;
            GameShard.Instance.InputManager.MouseInputEventData -= inPutMouseEvent;
            GameShard.Instance.InputManager.PlayerMoveEventData -= inPutMoveEvent;
        }
        //GameShard.Instance.InputManager.KeyinPutUiEventData -= inPutUiEvent;

        if (BuffSystem != null)
        {
            BuffSystem.OnBuffStarted -= OnBuffStartedHandler;
            BuffSystem.OnBuffProgress -= OnBuffProgressHandler;
            BuffSystem.OnBuffEnded -= OnBuffEndedHandler;
        }
    }

    #region Sagmin Code
    //public void OnMovement(InputAction.CallbackContext ctx)
    //{
    //    if (ConditionState == CHARACTER_CONDITION_STATE.Death ||
    //        Debuff == PLAYER_DEBUFF.Bind) return;

    //    moveInput = ctx.ReadValue<Vector2>();
    //    // Player movedirection Update
    //}

    //public void OnJump(InputAction.CallbackContext ctx)
    //{
    //    if (ConditionState == CHARACTER_CONDITION_STATE.Death ||
    //        Debuff == PLAYER_DEBUFF.Bind) return;

    //    //if (ctx.performed && IsGrounded())
    //    //{
    //    //    rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
    //    //    rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    //    //}
    //}

    //public void OnDash(InputAction.CallbackContext ctx)
    //{
    //    if (ConditionState == CHARACTER_CONDITION_STATE.Death ||
    //        Debuff == PLAYER_DEBUFF.Bind) return;

    //    if (ctx.performed && !isDashing && moveInput.sqrMagnitude > 0.1f)
    //    {
    //        isDashing = true;
    //        dashTimer = dashDuration;
    //        dashDirection = new Vector3(moveInput.x, 0, 0).normalized;
    //        rb.linearVelocity = dashDirection * dashForce;
    //    }
    //}

    //public void OnSteal(InputAction.CallbackContext ctx)
    //{
    //    if (ctx.started && colorSteal.CanStartSteal(colorSystem.GetGauge(colorSystem.GetCurrentColor())))
    //    {
    //        colorSteal.StartSteal();
    //        Debug.Log("Steal Mode Start");
    //    }
    //}


    //public void OnSkill(InputAction.CallbackContext ctx)
    //{
    //    if (ctx.performed)
    //    {
    //        // Skill Logic 
    //        colorSystem.ExecuteActiveSkill(this);
    //    }
    //    if (ConditionState == CHARACTER_CONDITION_STATE.Sick ||
    //        ConditionState == CHARACTER_CONDITION_STATE.Death) return;
    //    //Sick == Debuff(Test)

    //}
    //public void OnInteract(InputAction.CallbackContext ctx)
    //{
    //    if (ConditionState == CHARACTER_CONDITION_STATE.Death ||
    //        Debuff == PLAYER_DEBUFF.Bind) return;

    //    if (ctx.performed)
    //    {
    //        if (interactNpc == null)
    //            return;

    //        interactNpc.Interaction();
    //    }
    //}

    //public void OnAttack(InputAction.CallbackContext ctx)
    //{
    //    if ( ConditionState == CHARACTER_CONDITION_STATE.Death|| 
    //        Debuff == PLAYER_DEBUFF.Bind) return;

    //    if (colorSteal.IsStealing)
    //        return;


    //    if (ctx.performed)
    //    {
    //        Debug.Log("Attack");
    //        if (basicSkillSystem.SkillActive(PLAYER_SKILL.PurifyBlade))
    //        {
    //            Debug.Log($"PurifyBlade Animation start");
    //            //AnimationParameterUpdate(PLAYER_ANIMATION_PATAMETERS_TYPE.PurifyBlade, true);
    //        }
    //    }
    //}

    //public void OnSlot(InputAction.CallbackContext ctx)
    //{
    //    if (ctx.performed)
    //    {
    //        colorSystem.colorSlot.ColorRotate((color) => colorSystem.HasGauge(color));
    //    }
    //}
    //public void OnInven(InputAction.CallbackContext ctx)
    //{
    //    if (ctx.performed)
    //    {
    //        Shared.Instance.UIManager.OpenInventory();
    //        inventory.SetActive(gameObject.activeSelf);       
    //    }
    //}

    //private bool IsGrounded()
    //{
    //    return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    //}
    #endregion
    private void Update()
    {     
        if (IsPaused || IsSturn) return;

        if (moveDir != Vector3.zero)
        {
            if (!StateCheckData[Attack.ToString()])//Attack = false
            {
                AnimationParameterUpdate(ANIMATION_PATAMETERS_TYPE.Walk, true);
            }            
        }
        else 
        {
            AnimationParameterUpdate(ANIMATION_PATAMETERS_TYPE.Walk, false); 
        }

        move(moveDir);

        if (rg != null) 
        {
            GravityOperations();

            //Vector3 horizontalVelocity = moveDir * StatusData[CHARACTER_STATUS.Speed];

            //Velocity.x = horizontalVelocity.x;
            //Velocity.z = horizontalVelocity.z;

            rg.linearVelocity = Velocity;
        }
        moveDir = Vector3.zero;
    }

    public void SkillChange(int _value, SKILL_ID_TYPE _type) 
    {
        if (StateCheckData[Attack.ToString()]) 
        {
            reservedSkills.Enqueue((_value, _type));
            return;
        }
        
        if (SkillKeyWardData.ContainsKey(_value))
        {
            SkillKeyWardData[_value] = _type;
        }
        
    }
    public void SkillApply(List<SKILL_ID_TYPE> _lists) 
    {
        for (int i = 0; i < _lists.Count; i++)
        {
            if (_lists[i] != SKILL_ID_TYPE.None)
            {
                SkillKeyWardData.Add(i, _lists[i]);
            }
        }
    }
    public void ApplyData(Player _player, FollowCamera3D _followCamera)
    {
        rg = GetComponent<Rigidbody>();
        viewcam = _followCamera;
        CharacterAnimator = GetComponent<Animator>();
        basicSkillSystem.Init(this);
        BuffSystem = new BuffSystem();
        BuffSystem.Init(this);
        //skillDataAdd();

        StateCheckData.Add(ANIMATION_PATAMETERS_TYPE.Attack.ToString(), false);
        StateCheckData.Add(ANIMATION_PATAMETERS_TYPE.Death.ToString(), false);

        var Infos = Shared.Instance.DataManager.Character_Table.Get((int)IdType);
        if (Infos != null)
        {
            InfoData.Add(Id, Infos.Id);
            InfoData.Add(CHARACTER_DATA.Type, Infos.Type);
            InfoData.Add(StateId, Infos.StateId);
            InfoData.Add(BookId, Infos.BookId);
            InfoData.Add(FOVLength, Infos.FOVLength);
            InfoData.Add(AttackLength, Infos.AttackLength);
            InfoData.Add(WalkSoundId, Infos.WalkSoundId);
            InfoData.Add(AttackSoundId, Infos.AttackSoundId);

            PathData.Add(Name, Infos.Name);
            PathData.Add(Dec, Infos.Dec);
            PathData.Add(Icon, Infos.Icon);
            PathData.Add(Prefab, Infos.Prefab);
        }

        GameShard.Instance.InputManager.KeyinPutEventData += inPutKeyEvent;
        GameShard.Instance.InputManager.MouseInputEventData += inPutMouseEvent;
        GameShard.Instance.InputManager.PlayerMoveEventData += inPutMoveEvent;

        AudioClip audioClip1 = Shared.Instance.SoundManager.ClipGet((int)InfoData[WalkSoundId]);
        SoundDatas.Add((int)InfoData[WalkSoundId], audioClip1);

        AudioClip audioClip2 = Shared.Instance.SoundManager.ClipGet((int)InfoData[AttackSoundId]);
        SoundDatas.Add((int)InfoData[AttackSoundId], audioClip2);

        //EffectClip = Shared.Instance.SoundManager.ClipGet(skillData.SoundId);
        //GameShard.Instance.InputManager.KeyinPutUiEventData += inPutUiEvent;
    }

    public void UseBuffPotion(int buffId)
    {
        BuffData data = Shared.Instance.DataManager.Buff_Table.Get(buffId);

        if (data == null)
        {
            Debug.Log("Data is Null");
            return;
        }

        if (data.type == BUFFTYPE.AttackUp)
        {
            if (StatusData.TryGetValue(CHARACTER_STATUS.Atk, out float atk))
            {
                Debug.Log($"[Attack Power: {atk}] → [{atk + data.value}]");
            }
            else
            {
                Debug.LogWarning("StatusData에 Atk 항목이 없습니다.");
            }
        }
        else if (data.type == BUFFTYPE.SpeedUp)
        {
            if (StatusData.TryGetValue(CHARACTER_STATUS.Speed, out float speed))
            {
                Debug.Log($"[Speed Power: {speed}] → [{speed + data.value}]");
            }
            else
            {
                Debug.LogWarning("StatusData에 Speed 항목이 없습니다.");
            }
        }

        BuffSystem.ApplyBuff(buffId);

        GameShard.Instance.QuestManager.NotifyPotionUsed();
    }
    
    public override void HpUpdate(float _hp)
    {
        if (TryShieldBlock())
        {
            return;
        }

        float prevHp = 0f;
        if (StatusData.TryGetValue(CHARACTER_STATUS.Hp, out float cur)) 
            prevHp = cur;

        base.HpUpdate(_hp);

        if (prevHp > _hp)
        {
            GameShard.Instance?.QuestManager?.NotifyPlayerDamaged();
        }

        StateCheckData[KnockBack.ToString()] = true;
    }

    public void RoomUpdate(Room _room) 
    {
        MyRoom = _room;
        transform.SetParent(_room.transform);

    }

    public COLOR_TYPE StillColor(out float imageFill)
    {
        float vlaue  = 0;

        COLOR_TYPE type = colorSlotUI.LoadSlotImage(out vlaue);

        if (vlaue == 1.0f && basicSkillSystem.SkillActive(SKILL_ID_TYPE.BrokenLight)) 
        {
            Debug.Log($"BrokenLight Animation start");
            //AnimationParameterUpdate(PLAYER_ANIMATION_PATAMETERS_TYPE.BrokenLight, true);
        }
        imageFill = vlaue;

        return type;       
    }
    
    // InvisibleShield
    public void SetShieldBlockFunc(Func<bool> func)
    {
        shieldBlockFunc = func;
    }

    public void RemoveShieldBlockFunc()
    {
        shieldBlockFunc = null;
    }

    public bool TryShieldBlock()
    {
        return shieldBlockFunc != null && shieldBlockFunc.Invoke();
    }


    #region BUff
    public void BuffHUDInit(BuffHUD hud)
    {
        if (hud == null) 
            return;
        this.buffHUD = hud;
        WireBuffHud();
    }

    private void WireBuffHud()
    {
        // 중복 방지
        BuffSystem.OnBuffStarted -= OnBuffStartedHandler;
        BuffSystem.OnBuffProgress -= OnBuffProgressHandler;
        BuffSystem.OnBuffEnded -= OnBuffEndedHandler;
        
        BuffSystem.OnBuffStarted += OnBuffStartedHandler;
        BuffSystem.OnBuffProgress += OnBuffProgressHandler;
        BuffSystem.OnBuffEnded += OnBuffEndedHandler;
    }

    private void OnBuffStartedHandler(BUFFTYPE type, BuffData data)
    => buffHUD?.ShowBuff(data);

    private void OnBuffProgressHandler(BUFFTYPE type, float elapsed, float duration)
        => buffHUD?.UpdateBuff(type, elapsed, duration);

    private void OnBuffEndedHandler(BUFFTYPE type)
        => buffHUD?.EndBuff(type);
    #endregion
}

