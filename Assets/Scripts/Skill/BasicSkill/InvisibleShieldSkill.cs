using System.Collections;
using UnityEngine;
using static Enums;

public class InvisibleShieldSkill : IBasicSkill
{
    //public SkillData skillData { get; set; }
    //public override SKILL_ID_TYPE SkILL_Id => SKILL_ID_TYPE.InvisibleShield;

    private Player player;

    private bool isShieldActive = false;
    private bool isHitBlocked = false; // 무적 상태



    public override void Init(Character_Base character)
    {
        this.player = (Player)character;

        if (player != null && player.colorSteal != null)
        {
            player.colorSteal.OnStealEnd += TriggerOut;
        }
        base.Init(null);
    }

    public override void OnTrigger(Character_Base _defender)
    {
        if (isShieldActive)
            return;

        Debug.Log("Shield Ready");
        player.StartCoroutine(ShieldPhasedCoroutine());
    }

    private IEnumerator ShieldPhasedCoroutine()
    {
        // Ready (Effect and Animation)
        yield return new WaitForSeconds(0.2f);

        // Shield
        Debug.Log("InvisibleShield  Start");
        isShieldActive = true;
        isHitBlocked = false;

        player.SetShieldBlockFunc(ConsumeShield);
 
        yield return new WaitForSeconds(0.3f);

        // 보호 종료
        EndShield();
    }

    private bool ConsumeShield()
    {
        if (isShieldActive && !isHitBlocked)
        {
            Debug.Log("Defence Success");
            isHitBlocked = true;
            EndShield();
            return true;
        }
        return false;
    }


    private void EndShield()
    {
        Debug.Log("Shield End");
        isShieldActive = false;
        player.RemoveShieldBlockFunc();
    }
    public override void TriggerOut() { }
    public override void OnUpdate() { }

}
