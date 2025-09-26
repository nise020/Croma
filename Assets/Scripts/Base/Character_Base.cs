using System;
using System.Collections.Generic;
using UnityEngine;
using static Enums;
using static Enums.ANIMATION_PATAMETERS_TYPE;
using static Enums.CHARACTER_DATA;
using static Enums.CHARACTER_STATUS;
public abstract partial class Character_Base : MonoBehaviour
{
    public abstract CONFIG_OBJECT_TYPE ObjectType { get; }
    public CHARACTER_CONDITION_STATE ConditionState { get; set; } = CHARACTER_CONDITION_STATE.Default;
    //protected Dictionary<CHARACTER_STATUS_TYPE, object> Status { get; set; } = new();
    //protected StateBar StateBar { get; set; } = new();
    public virtual void HpBarInit(StateBar _hpBar) { }

    StateBar stateBar { get; set; }
    protected Animator CharacterAnimator { get; set; } = null;
    public Action<float, float> HpBarChanged { get; set; }

    public Action<bool> AttackEvent { get; set; }

    protected bool DamageColliderCheck = false;

    protected bool DamageMaterialCheck = false;

    protected Transform modelTrans;

    protected float RotateSpeed { get; set; } = 5.0f;

    protected BasicSkillSystem basicSkillSystem = new ();
    public void SkillInit(BasicSkillSystem basicSkill) => basicSkillSystem = basicSkill;
    public Dictionary<CHARACTER_DATA, string> PathData { get; protected set; } = new();
    public Dictionary<CHARACTER_STATUS, float> StatusData { get; protected set; } = new();
    public Dictionary<CHARACTER_DATA, float> InfoData { get; protected set; } = new();
    public Dictionary<int, ParticleSystem> SkillData { get; protected set; } = new();
    public Dictionary<string, bool> StateCheckData { get; protected set; } = new();

    //public int CharacterId;
    public CHARACTER_ID IdType;//Test

    
    protected Dictionary<int, SKILL_ID_TYPE> SkillKeyWardData = new Dictionary<int, SKILL_ID_TYPE>();

    [Header("Gravity")]
    [SerializeField,ReadOnly] protected bool IsGrounded = false;
    [SerializeField, ReadOnly] protected Vector3 Velocity = Vector3.zero;
    [SerializeField, ReadOnly] protected float Gravity = -9.81f;
    [SerializeField, ReadOnly] protected float JumpGravity = -9.81f;
    [SerializeField, ReadOnly] protected bool IsJumping = false;
    [SerializeField, ReadOnly] protected float GroundDistance = 0.5f;
    [SerializeField, ReadOnly] protected LayerMask GroundMask;

    [SerializeField,ReadOnly] protected bool IsDeathed = false;
    [SerializeField, ReadOnly] protected LayerMask DeathMask;

    public BuffSystem BuffSystem { get; protected set; } = new();
    protected Rigidbody rg { get; set; } = null;
    //public PLAYER_DEBUFF Debuff = PLAYER_DEBUFF.None;

    [Header("Controll")]
    [ReadOnly] public bool IsPaused = false;
    [ReadOnly] public bool IsSturn = false;
    [ReadOnly] public bool isJump = false;

    [SerializeField] protected GameObject WeaponObj;
    [SerializeField] protected GameObject FootObj;
    protected Dictionary<int, AudioClip> SoundDatas = new Dictionary<int, AudioClip>();
    protected AudioSource WeaponSoundPlayer;
    protected AudioSource CharacterSoundPlayer;
    [ReadOnly] public bool IsOnWalkable = false;
    [ReadOnly] public bool IsOnForest = false;

    protected virtual void Awake()
    {
        //BuffSystem.Init(this);
        rg = GetComponent<Rigidbody>();

    }
    protected virtual void Start()
    {
        CharacterAnimator = GetComponent<Animator>();

        SkinnedMeshRenderer render = GetComponentInChildren<SkinnedMeshRenderer>();
        modelTrans = render.gameObject.transform;

        Velocity = Vector3.one;
        GroundMask = LayerMask.GetMask(LAYER_TYPE.Walkable.ToString());
        DeathMask = LayerMask.GetMask(LAYER_TYPE.DeathZone.ToString());
        //GroundMask = LayerMask.GetMask(LAYER_TYPE.Forest.ToString());
        //GroundMask = LayerMask.GetMask(LAYER_TYPE.Walkable.ToString());

        //skillDataAdd();
    }
    protected virtual void OnDestroy()
    {
        InfoData = null;
        PathData = null;
        SkillKeyWardData = null;
        basicSkillSystem = null;
        BuffSystem = null;
    }
    protected virtual void AttackOn(ANIMATION_PATAMETERS_TYPE _type)
    {

    }
    protected virtual void OnAttack()
    {

    }
    public int ExpLoad() 
    {
        return (int)InfoData[CHARACTER_DATA.Exp];
    }
    public float StatusTypeLoad(CHARACTER_STATUS _type) 
    {
        return (float)StatusData[_type];
    }
    public virtual GameObject GetWeaponObj()
    {
        return gameObject;
    }
    public virtual GameObject GetHand()
    {
        return gameObject;
    }
    public virtual void HpUpdate(float _Hp)
    {
        StatusData[Hp] = (int)_Hp;
        
        if ((float)StatusData[Hp] <= 0) death();

        HpBarChanged?.Invoke((float)StatusData[MaxHp], (float)StatusData[Hp]);
    }
    public virtual void DamageImageOn(float _damage) 
    {

    }
    
    protected void death()
    {
        IsPaused = true;
        //AnimationParameterUpdate(ANIMATION_PATAMETERS_TYPE.Attack, false);
        AnimationParameterUpdate(ANIMATION_PATAMETERS_TYPE.Death, true);
    }
    public void AnimationParameterUpdate(ANIMATION_PATAMETERS_TYPE _type, bool _state)
    {
        if (_state)
        {
            CharacterAnimator.SetInteger(_type.ToString(), 1);
            return;
        }
        CharacterAnimator.SetInteger(_type.ToString(), 0);
        return;
    }
    public bool DamageEventCheck() 
    {
        return DamageMaterialCheck == false;
    }

    public void DamageEventUpdate(bool _check)
    {
        DamageMaterialCheck = _check;
    }

    //public bool HitStateCheck()
    //{
    //    if (StateCheckData[KnockBack.ToString()])
    //    {
    //        StateCheckData[KnockBack.ToString()] = false; 
    //        return true;
    //    }
    //    return false;
    //}
    public virtual void KnockBackOn(float _value)
    {
        //float nuckValue = 20 / defenserDfs;//Test

        //_defender.KnockBackOn(nuckValue);
        //float knockbackForce = Mathf.Clamp(_value * 0.05f, 1f, 4f);

    }
    public virtual void KnockBackEvent()//AnimationEvent
    {
        IsPaused = false;

        if (IsGrounded)
        {
            Vector3 v = rg.linearVelocity;
            rg.linearVelocity = new Vector3(0f, v.y, 0f);
        }
        AnimationParameterUpdate(KnockBack, false);
    }
    protected void GravityOperations()
    {
        ///GroundMask = LayerMask.GetMask(LAYER_TYPE.Walkable.ToString());
        //GroundMask = LayerMask.GetMask(LAYER_TYPE.Forest.ToString());

         Vector3 rayOrigin = transform.position + Vector3.up * 0.1f; 
         //Debug.DrawRay(rayOrigin, Vector3.down * GroundDistance, Color.red);
         IsGrounded = Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hitInfo1, GroundDistance, GroundMask);
         IsDeathed = Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hitInfo2, GroundDistance, DeathMask);

        if (IsDeathed) 
        {
            if (Velocity.y < 0f)
            {
                Velocity.y = -1f;
            }

            HpUpdate(0.0f);
            IsPaused = true;

        }
        else if (IsGrounded)
        {
            if (Velocity.y < 0f)
            {
                Velocity.y = -1f;
            }
        }
        else
        {
            //IsPaused = true;
            Velocity.y += Gravity * Time.deltaTime;
        }
        //rg.linearVelocity = Velocity;
    }

    public virtual void SkillEventOn(int value) //Event, 1 == skill1
    {

    }
    public virtual void SkillEventEnd(int _skillnumber) //Event <- Attack End Event
    {

    }

    public virtual void DeathEvent()//AnimationEvent
    {

    }

    public virtual float HealInstant(float percent)
    {
        if (percent <= 0f) return 0f;
        if (!StatusData.ContainsKey(CHARACTER_STATUS.Hp) ||
            !StatusData.ContainsKey(CHARACTER_STATUS.MaxHp)) return 0f;

        float max = StatusData[CHARACTER_STATUS.MaxHp];
        float amountAbs = percent * 0.01f * max;

        float healed = HealInstantAbs(amountAbs);

        if (healed > 0f)
            GameShard.Instance?.QuestManager?.NotifyPotionUsed();

        return healed;
        /*float amountAbs = Mathf.Max(0f, percent) * 0.01f * max;
        GameShard.Instance.QuestManager.NotifyPotionUsed();
        return HealInstantAbs(amountAbs);*/
    }


    public virtual float HealInstantAbs(float amountAbs)
    {
        if (amountAbs <= 0f) return 0f;
        if (!StatusData.ContainsKey(CHARACTER_STATUS.Hp) ||
            !StatusData.ContainsKey(CHARACTER_STATUS.MaxHp)) return 0f;

        float cur = StatusData[CHARACTER_STATUS.Hp];
        float max = StatusData[CHARACTER_STATUS.MaxHp];

        float next = Mathf.Clamp(cur + amountAbs, 0f, max);
        if (Mathf.Approximately(next, cur)) return 0f;

        StatusData[CHARACTER_STATUS.Hp] = next;

        HpBarChanged?.Invoke(max, next);
        return next - cur; 
    }
}
