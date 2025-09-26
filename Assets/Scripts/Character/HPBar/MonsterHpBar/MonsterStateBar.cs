using UnityEngine;

public partial class MonsterStateBar : StateBar
{
    private string bossName;
    //private void Awake()
    //{
    //    DamageSetting();
    //}
    protected override void Start()
    {
        base.Start();
        Initialize();
        //DamageSetting();
       // gameObject.SetActive(false);
    }

    public override void InitializeCharacter(Character_Base character)
    {
        base.InitializeCharacter(character);

        AttackDamageEvent += DamageImageActive;
        DamageSetting(DamageBarObj);
        Debug.Log("[HpBar] DamageImageActive 연결 완료");
        //AttackDamageEvent += DamageImageActive;
    }
    protected virtual void LateUpdate()
    {
        if (MainCamera != null)
        {
            transform.rotation = Quaternion.LookRotation
                (transform.position - MainCamera.transform.position, Vector3.up);

            DamageBarObj.transform.rotation = Quaternion.LookRotation
                (transform.position - MainCamera.transform.position, Vector3.up);
            //Debug.Log($"{transform.rotation}");
        }
        else 
        {
            Initialize();
        }
    }
}
