using UnityEngine;
using static Enums;

public class PlayerStateController : Character_Base
{
    public override CONFIG_OBJECT_TYPE ObjectType => CONFIG_OBJECT_TYPE.Player;
    [Header("Movement")]
    public float WalkSpeed = 1.0f;
    public float DashSpeed = 1.0f;
    [SerializeField, ReadOnly] protected bool IsMoving = false;
    [SerializeField, ReadOnly] protected bool IsSprint = false;

    [Space(5)]
    [SerializeField, ReadOnly] protected Vector3 MovementDirection = Vector3.zero;
    [SerializeField, ReadOnly] protected float CurrentSpeed = 0.0f;

    [Space(5)]
    public float JumpHeight = 1.0f;
    [SerializeField, ReadOnly] protected bool IsJump = false;
    [SerializeField, ReadOnly] protected bool IsJumped = false;

    [Space(5)]
    public float DashForce = 1.0f;
    public float DashDuration = 1.0f;
    [SerializeField, ReadOnly] protected bool IsDash = false;
    [SerializeField, ReadOnly] protected bool IsDashed = false;

    [Space(5)]
    [SerializeField, ReadOnly] protected float DashTimer = 0f;
    [SerializeField, ReadOnly] protected Vector3 DashDirection = Vector3.zero;

    [Space(10)]
    [Header("Camera")]
    [SerializeField] protected CameraController CameraController = null;

    [Space(10)]
    [Header("Status")]
    [SerializeField] protected Animator Animator = null;
    [SerializeField] protected CharacterController Controller = null;

    [Space(5)]
    [ReadOnly] public PLAYER_MOVEMENT_STATUS MovementStatus = PLAYER_MOVEMENT_STATUS.Default;
    [SerializeField, ReadOnly] protected PLAYER_MOVEMENT_STATUS PrevMovementStatus = PLAYER_MOVEMENT_STATUS.Default;

    [Space(5)]
    [ReadOnly] public PLAYER_ACTION_STATUS ActionStatus = PLAYER_ACTION_STATUS.Default;
    [SerializeField, ReadOnly] protected PLAYER_ACTION_STATUS PrevActionStatus = PLAYER_ACTION_STATUS.Default;

    [Space(10)]
    [Header("Physics")]
    //[SerializeField] protected float Gravity = -9.81f;
    //[SerializeField] protected float JumpGravity = -9.81f;

    //[Space(5)]
    //[SerializeField] protected Transform GroundCheckTrans = null;
    //[SerializeField] protected float GroundDistance = 0.1f;
    //[SerializeField] protected LayerMask GroundMask = default;
    //[SerializeField, ReadOnly] protected bool IsGrounded = false;

    //[Space(5)]
    //[SerializeField, ReadOnly] protected Vector3 Velocity = Vector3.zero;

    [Space(5)]
    [SerializeField, ReadOnly] protected Vector3 HorizontalDirection = Vector3.zero;
    [SerializeField, ReadOnly] protected Vector3 VerticalDirection = Vector3.zero;

    [Space(10)]
    [Header("Color Systems")]
    public float ColorConsumptionValue = 1.0f;
    public float MaxColorGauge = 100.0f;
    public ColorSlotUI colorSlotUI = null;

    [Space(5)]
    [SerializeField] protected StealGaugeUI stealGaugeUI = null;

    protected ColorSystem colorSystem = new(COLOR_TYPE.None);
    protected ColorSteal colorSteal = new();
    protected ColorSlot colorSlot = new();

    [Space(10)]
    [Header("Stat")]

    public GaugeController HpGaugeController = null;
    public float MaxHp = 100.0f;
    [ReadOnly] public float CurrentHp = 100.0f;
    [SerializeField, ReadOnly] protected float targetHp = 100.0f;
    public float HpRecoveryAmount = 1.0f;

    [Space(5)]
    public GaugeController EpGaugeController = null;
    public float MaxEp = 100.0f;
    [ReadOnly] public float CurrentEp = 100.0f;
    [SerializeField, ReadOnly] protected float targetEp = 100.0f;
    public float EpRecoveryAmount = 1.0f;

    [Space(5)]
    public float AttackPower = 1.0f;
    public float DefensePower = 1.0f;

    [Space(5)]
    public float CriticalProbability = 1.0f;
    public float CriticalPower = 1.0f;

    [Space(10)]
    [Header("Buff")]
    public float Item_Drop_Probability_Multiplier = 1.0f;
    public float Item_Price_Reduction_Factor = 1.0f;
    public float Gold_Drop_Probability_Multiplier = 1.0f;
    public const float Color_Efficiency = 30.0f;
    public const float Buff_Color_Efficiency = 5.0f;

    [Space(10)]
    [Header("Other")]
    [SerializeField, ReadOnly] protected bool IsInventoryEnabled = false;
}
