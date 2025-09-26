using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Collections;
using UnityEngine;
using UnityEngine.AI;
using static Enums;
using static Enums.ANIMATION_PATAMETERS_TYPE;
using static Enums.CHARACTER_DATA;
using static Enums.CHARACTER_STATUS;
//using Unity.Android.Gradle.Manifest;

public partial class Monster_Base : Character_Base
{
    

    public override CONFIG_OBJECT_TYPE ObjectType => CONFIG_OBJECT_TYPE.Monster;
    //public Dictionary<CHARACTER_DATA, object> MonsterStatus { get; set; } = new();
    protected Player Player { get; set; } = null;

    protected Transform PlayerTrans { get; set; }

    protected IEnumerator AttackCorutine { get; set; } = null;

    


    //public void StateInit() => infoData = Infos[CHARACTER_DATA.State] as Dictionary<MONSTER_STATE, object>;
    protected bool AttackDelay { get; set; } = false;//DelayType Only

    protected bool ColorState { get; set; } = false;

    //public Dictionary<CHARACTER_DATA, object> Infos { get; set; } = new();
    protected Spawn Spawn { get; set; } = null;
    protected Vector3 MovePosition { get; set; } = Vector3.zero;
    protected Vector3 StartPoint { get; set; } = Vector3.zero;
    protected List<Vector3> MovePositionList { get; set; } = new();
    protected int SlotCount { get; set; } = 0;
    protected float StopDistance { get; set; } = 0.2f;

    protected float MoveDelrayTimer { get; set; } = 0.0f;
    protected float MoveDelrayTime { get; set; } = 2.0f;
    protected float StopDelrayTimer { get; set; } = 0.0f;
    protected float StopDelrayTime { get; set; } = 2.0f;
    protected float AttackDelayTimer { get; set; } = 0.0f;
    protected float AttackDelrayTime { get; set; } = 2.0f;
    protected int AttackCount { get; set; } = 0;
    protected int AttackMaxCount { get; set; } = 3;

    protected CancellationTokenSource AttackCTS { get; set; } = null;
    protected CancellationTokenSource MoveCTS { get; set; } = null;
    protected CancellationTokenSource DashCTS { get; set; } = null;
    protected CancellationTokenSource AniEventCTS { get; set; } = null;
    protected Dictionary<Item, GameObject> DropItemData { get; set; } = new();

    protected List<Item> ITEMLists { get; set; } = new();

    
    public MONSTER_ATTACK_TYPE Attack_Type = MONSTER_ATTACK_TYPE.None;

   
    protected MonsterWeapon MainWeapon { get; set; }
    protected List<GameObject> WeaponObjectList = new List<GameObject>();
    protected Dictionary<GameObject, bool> WeaponCheckDatas = new Dictionary<GameObject, bool>();
    protected Transform Creatab { get; set; }

    public ParticleSystem deathEffect { get; set; }
    public ParticleSystem SpownEffect { get; set; }

    protected StateBar StateBar { get; set; } = new();



    protected Vector3 WeightPos = new Vector3();

    [SerializeField] protected List<SKILL_ID_TYPE> SkillType;
    protected NavMeshAgent Agent { get; set; }
    protected override void Awake()
    {
        base.Awake();
        WeaponObj = GetWeaponObj();

        WeaponSoundPlayer = WeaponObj.AddComponent<AudioSource>();
        WeaponSoundPlayer.outputAudioMixerGroup = Shared.Instance.SoundManager.GetMixser(OptionHandler.SOUND.Effect);
        WeaponSoundPlayer.spatialBlend = 1.0f;

        CharacterSoundPlayer = gameObject.AddComponent<AudioSource>();
        CharacterSoundPlayer.outputAudioMixerGroup = Shared.Instance.SoundManager.GetMixser(OptionHandler.SOUND.Effect);
        CharacterSoundPlayer.spatialBlend = 1.0f;
    }

    public void GetData() 
    {
        var Infos = Shared.Instance.DataManager.Character_Table.Get((int)IdType);
        if (Infos != null)
        {
            InfoData.Add(Id, Infos.Id);
            InfoData.Add(CHARACTER_DATA.Type, Infos.Type);
            InfoData.Add(StateId, Infos.StateId);
            InfoData.Add(BookId, Infos.BookId);
            InfoData.Add(FOVLength, Infos.FOVLength);
            InfoData.Add(AttackLength, Infos.AttackLength);
            InfoData.Add(Exp, Infos.Exp);
            InfoData.Add(WalkSoundId, Infos.WalkSoundId);
            InfoData.Add(AttackSoundId, Infos.AttackSoundId);

            AudioClip audioClip1 = Shared.Instance.SoundManager.ClipGet((int)InfoData[WalkSoundId]);
            SoundDatas.Add((int)InfoData[WalkSoundId], audioClip1);

            AudioClip audioClip2 = Shared.Instance.SoundManager.ClipGet((int)InfoData[AttackSoundId]);
            SoundDatas.Add((int)InfoData[AttackSoundId], audioClip2);


            PathData.Add(Name, Infos.Name);
            PathData.Add(Dec, Infos.Dec);
            PathData.Add(Icon, Infos.Icon);
            PathData.Add(Prefab, Infos.Prefab);

            var state = Shared.Instance.DataManager.Stat_Table.Get(Infos.StateId);
            if (state != null)
            {
                StatusData.Add(Hp, state.Hp);
                StatusData.Add(MaxHp, state.Hp);
                StatusData.Add(Def, state.Def);
                StatusData.Add(Speed, state.Speed);
                StatusData.Add(Atk, state.Atk);
            }

        }
    }
    protected override void Start()
    {
        IsPaused = true;
        base.Start();

        //basicSkillSystem.Init(this);
        skillDataAdd();


        BuffSystem = new BuffSystem();
        BuffSystem.Init(this);

        StateCheckData.Add(ANIMATION_PATAMETERS_TYPE.Attack.ToString(), false);

        //StateCheckData.Add(ANIMATION_PATAMETERS_TYPE.Death.ToString(), false);
        //StateCheckData.Add(ANIMATION_PATAMETERS_TYPE.Walk.ToString(), false);
        //StateCheckData.Add(ANIMATION_PATAMETERS_TYPE.KnockBack.ToString(), false);

        FindWeapon();
        Agent = GetComponent<NavMeshAgent>();
        Agent.speed = (int)StatusData[Speed]/2;
        //Agent.stoppingDistance = InfoData[AttackLength];
        Agent.updateRotation = false;
        Agent.updatePosition = false;

        StartCoroutine(SpownEffectOn(this, () => 
        {
            IsPaused = false;
        }));
    }
    public override void DamageImageOn(float _damage)
    {
        StateBar.AttackDamageEvent?.Invoke((int)_damage);
    }
    public void SetAvoidanceDirection(Vector3 _pos) 
    {
        WeightPos = _pos;
    }
    class Skilllist
    {
        public Dictionary<int, string> skillTypeData = new Dictionary<int, string>();
    }

    protected virtual void Update()
    {
        if (isJump) return;
        GravityOperations();
        if (rg) 
            rg.linearVelocity = Velocity;
    }

    protected void skillDataAdd()
    {
        for (int i = 0; i < SkillType.Count; i++)
        {
            if (SkillType[i] != SKILL_ID_TYPE.None)
            {
                SkillKeyWardData.Add(i, SkillType[i]);
            }

        }
    }
    public override GameObject GetWeaponObj()
    {
        if (WeaponObj == null) 
        {
            return gameObject;
        }
        return WeaponObj;
    }
    protected void FindWeapon() 
    {
        MonsterWeapon[] Weapons = GetComponentsInChildren<MonsterWeapon>();
        if (Weapons.Length == 0) return;

        if (Weapons.Length == 1)
        {
            MainWeapon = Weapons[0];
        }

    }

    public void StateUpdate(bool paused) 
    {
        if (StatusData[CHARACTER_STATUS.Hp] <= 0) return;
        else 
        {
            IsPaused = paused;
        }

        var anim = CharacterAnimator;
        if (anim == null)
        {
            CharacterAnimator = anim = GetComponentInChildren<Animator>();
            if (anim == null) return;
        }

        anim.speed = paused ? 0f : 1f;

        if (CharacterSoundPlayer.isPlaying)
        {
            CharacterSoundPlayer.Stop();
        }
    }
    
   
    public List<GameObject> GetWeaponsKey() 
    {
        return WeaponObjectList;
    }
    public Dictionary<GameObject, bool> GetWeaponsData()
    {
        return WeaponCheckDatas;
    }
    public void WeaponsStateUpdate(GameObject monsterObj, bool isActive)
    {
        if (WeaponCheckDatas.ContainsKey(monsterObj))
        {
            WeaponCheckDatas[monsterObj] = isActive;
        }
    }
}
