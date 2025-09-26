using UnityEngine;
using static Enums;

public class PurifyBladeSkill : IBasicSkill
{
    private Player player;
    //public SkillData skillData { get; set; }
    //public override SKILL_ID_TYPE SkILL_Id => SKILL_ID_TYPE.PurifyBlade;
   // bool IsActive { get; set; }

    private int comboCount = 0; // 연속 공격 횟수
    private float comboTimer = 0f; // 마지막 공격 후 경과시간
    private float inputDelay = 0.3f; // 공격 유효 시간
    private bool comboLocked = false; // 스킬 발동 후 공격 금지 상태
    private float comboLockTime = 0.5f; // 공격 금지 시간
    private float lockTimer = 0f; // 타이머

    public override void Init(Character_Base character)
    {
        player = (Player)character;
        base.Init(null);
    }

    public override void OnTrigger(Character_Base _defender) 
    {
        Debug.Log("PurifyBladeSkill Triggered");

        // 이곳에 이펙트, 타격, 애니메이션 호출 등 추가

        comboLocked = true;
        lockTimer = 0f;
        comboCount = 0;
        comboTimer = 0f;

        if (comboCount > 0 && !comboLocked)
        {
            comboTimer += Time.deltaTime;
            if (comboTimer > inputDelay)
            {
                ResetCombo();
            }
        }

        if (comboLocked)
        {
            lockTimer += Time.deltaTime;
            if (lockTimer >= comboLockTime)
            {
                comboLocked = false;
                lockTimer = 0f;
            }
        }

    }


    public override void TriggerOut() 
    {
        Debug.Log("PurifyBladeSkill Triggered");

        // 이곳에 이펙트, 타격, 애니메이션 호출 등 추가

        comboLocked = true;
        lockTimer = 0f;
        comboCount = 0;
        comboTimer = 0f;

        //Timer(1);
    }
    public override void OnUpdate()
    {
        if (comboCount > 0 && !comboLocked)
        {
            comboTimer += Time.deltaTime;
            if (comboTimer > inputDelay)
            {
                ResetCombo();
            }    
        }

        if (comboLocked)
        {
            lockTimer += Time.deltaTime;
            if (lockTimer >= comboLockTime)
            {
                comboLocked = false;
                lockTimer = 0f;
            }
        }

    }

    // 공격 시 카운트 계산 -> 
    public void RegisterAttack()
    {
        if (comboLocked)
        {
            Debug.Log("Combo Lock");
            return;
        }

        comboCount++;
        comboTimer = 0f;

        if (comboCount == 3)
        {
            TriggerOut(); // 3콤보 시 스킬 발동
        }
    }

    private void ResetCombo()
    {
        comboCount = 0;
        comboTimer = 0f;
    }

    //public override bool State()
    //{
    //    if (comboCount != 3) 
    //    {
    //        return true;
    //    }
    //    return comboLocked;
    //}
}
