using UnityEngine;
using static Enums;

public partial class NomalMonster : Monster_Base
{
    public override void HpBarInit(StateBar _hpBar)
    {
        StateBar = _hpBar as MonsterStateBar;
    }

    AI_Monster AI = new AI_Monster();

    //public bool isAttackDelayTimer = false;
    protected Renderer rend;
    protected BossMonster MasterMonster;

    public override void StateReset()
    {
        AI.MyAIState = AI_Monster.AI_STATE.Idle;
        StatusData[CHARACTER_STATUS.Hp] = StatusData[CHARACTER_STATUS.MaxHp];

        StateBar.InitializeImage();
    }

    protected override void Awake()
    {
        base.Awake();
        rend = GetComponentInChildren<Renderer>();
    }

    protected override void Start()
    {
        base.Start();

        AiInit();

        AttackEvent += AiTagetUpdate;
    }
    protected void FixedUpdate()
    {
        if (IsPaused) return;
        AI.State();
    }

    protected override void Update() 
    {
        base.Update();
        CameraRenderCheck();
    }

    protected void CameraRenderCheck() 
    {
        if (rend.isVisible)
        {
            // 카메라에 보이면 애니메이션 항상 돌림
            if (CharacterAnimator.cullingMode != AnimatorCullingMode.AlwaysAnimate)
                CharacterAnimator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        }
        else
        {
            // 안 보이면 완전히 멈춤
            if (CharacterAnimator.cullingMode != AnimatorCullingMode.CullCompletely)
                CharacterAnimator.cullingMode = AnimatorCullingMode.CullCompletely;
        }
    }
    private void AiInit() 
    {
        AI.Initialize(this);
        AI.PlayerDataInit(Player);

        AI.AI_Attack_Type(Attack_Type);
    }

    public override void HpUpdate(float _hp)
    {
        if (!StateBar.gameObject.activeSelf) 
        {
            StateBar.gameObject.SetActive(true);
        }

        StatusData[CHARACTER_STATUS.Hp] = (int)_hp;
        float maxHp = StatusData[CHARACTER_STATUS.MaxHp];

        if (_hp <= 0) death();

        HpBarChanged?.Invoke(maxHp, _hp);

        AI.HpUpdate(_hp, maxHp);
    }

    


    //public override GameObject GetWeaponObj()
    //{
    //    return WeaponObj;
    //}
    //public override GameObject GetHand()
    //{
    //    return GrabHand;
    //}

}
