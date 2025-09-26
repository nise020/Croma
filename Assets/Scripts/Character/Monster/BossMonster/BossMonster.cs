
using UnityEngine;
using static Enums;

public partial class BossMonster : Monster_Base
{
    protected BoxCollider BossRoomColl;
    AI_Boss AI = new AI_Boss();
    private Color copyColor;
    bool IsBerserker = false;
    //List<GameObject> MonsterObj_Minan_List = new List<GameObject>();
    //Dictionary<GameObject, bool> Minian_Dict_Data = new Dictionary<GameObject, bool>();

    public override void StateReset()
    {
        AI.MyAIState = AI_Boss.AI_STATE.Idle;
    }

    protected override void Awake() 
    {
        base.Awake();
        GameObject go = GameObject.Find("BossRoom");
        if (go != null) 
        {
            BossRoomColl = go.GetComponent<BoxCollider>();
        }
    }

    protected override void Start()
    {
        //AI.Initialize(this);
        AttackEvent += AiTagetUpdate;
        //AI.InfoInit(monster_Info);<- type
        //AI.PlayerDataInit(Player);

        base.Start();

        AI.Initialize(this);
        AI.PlayerDataInit(Player);
        AI.BossTypeInit(IdType);
        AI.HpUpdate((float)StatusData[CHARACTER_STATUS.MaxHp], (float)StatusData[CHARACTER_STATUS.MaxHp]);
        //FindAnotherWeapon();

        //Hp = 100;
        //MaxHp = Hp;
        //Atk = 10;
    }
    private void FixedUpdate()
    {
        if (IsPaused) return;
        AI.State();
    }
    public override void HpBarInit(StateBar _hpBar)
    {
        StateBar = _hpBar as BossStateBar;
    }
    private void HPBarCheck()
    {
        if (StateBar.gameObject.activeSelf || StateBar == null) return;
        StateBar.gameObject.SetActive(true);
    }
    public void BurserKer() 
    {
        if(IsBerserker) return;
        IsBerserker = true;
        BuffSystem.ApplyBuff(2004);
    }
    public override void HpUpdate(float _hp)
    {
        HPBarCheck();
        base.HpUpdate(_hp);
        AI.HpUpdate(_hp, (float)StatusData[CHARACTER_STATUS.MaxHp]);
    }


    public override void AiTagetUpdate(bool _check)//Taget State update
    {
        AI.DefenderState(_check);
    }

   


}
