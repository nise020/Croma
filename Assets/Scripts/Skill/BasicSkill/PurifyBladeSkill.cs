using UnityEngine;
using static Enums;

public class PurifyBladeSkill : IBasicSkill
{
    private Player player;
    //public SkillData skillData { get; set; }
    //public override SKILL_ID_TYPE SkILL_Id => SKILL_ID_TYPE.PurifyBlade;
   // bool IsActive { get; set; }

    private int comboCount = 0; // ���� ���� Ƚ��
    private float comboTimer = 0f; // ������ ���� �� ����ð�
    private float inputDelay = 0.3f; // ���� ��ȿ �ð�
    private bool comboLocked = false; // ��ų �ߵ� �� ���� ���� ����
    private float comboLockTime = 0.5f; // ���� ���� �ð�
    private float lockTimer = 0f; // Ÿ�̸�

    public override void Init(Character_Base character)
    {
        player = (Player)character;
        base.Init(null);
    }

    public override void OnTrigger(Character_Base _defender) 
    {
        Debug.Log("PurifyBladeSkill Triggered");

        // �̰��� ����Ʈ, Ÿ��, �ִϸ��̼� ȣ�� �� �߰�

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

        // �̰��� ����Ʈ, Ÿ��, �ִϸ��̼� ȣ�� �� �߰�

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

    // ���� �� ī��Ʈ ��� -> 
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
            TriggerOut(); // 3�޺� �� ��ų �ߵ�
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
