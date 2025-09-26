using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using UnityEngine;
using static Enums;
using static Enums.ANIMATION_PATAMETERS_TYPE;
using System.Collections;

public partial class Monster_Base : Character_Base
{
    public void KnockBackEventOut() 
    {

    }
    public override void KnockBackOn(float _value)
    {
        float knockbackForce = Mathf.Clamp(Mathf.Log10(_value + 1) * 2f, 1f, 10f);

        Vector3 knockBackDir = -transform.forward;
        knockBackDir.y = 0f;
        knockBackDir.Normalize();
        if (StateCheckData[Attack.ToString()] == false && !IsPaused)
        {
            IsPaused = true;
            AnimationParameterUpdate(Walk, false);
            AnimationParameterUpdate(KnockBack, true);
        }
        rg.AddForce(knockBackDir * knockbackForce, ForceMode.Impulse);
    }
    //public override void KnockBackEvent()//AnimationEvent
    //{
    //    IsPaused = false;

    //    if (IsGrounded)
    //    {
    //        Vector3 v = rg.linearVelocity;
    //        rg.linearVelocity = new Vector3(0f, v.y, 0f);
    //    }
    //    AnimationParameterUpdate(KnockBack, false);
    //}
    public void AttackReady() //Event<- Idel Event
    {
        StateCheckData[Attack.ToString()] = false;
    }
    public override void SkillEventOn(int value) //Event, 1 == skill1
    {
        if (SkillKeyWardData.TryGetValue(value, out SKILL_ID_TYPE skill))
        {
            basicSkillSystem.SkillActive(skill, Player);
            AttackSoundPlay();
            TimerOn();
        }
        else
        {
            Debug.LogError($"{skill} Skill = null");
        }
    }
    protected void AttackSoundPlay() //Event
    {
        if (SoundDatas.TryGetValue((int)InfoData[CHARACTER_DATA.AttackSoundId], out AudioClip clip))
        {
            WeaponSoundPlayer.volume = 0.8f;
            WeaponSoundPlayer.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
            WeaponSoundPlayer.PlayOneShot(clip);
        }

    }
    protected ANIMATION_PATAMETERS_TYPE SkillAniamtiontype(int _skillnumber) 
    {
        switch (_skillnumber) 
        {
            case 0: return ANIMATION_PATAMETERS_TYPE.Skill_1;
            case 1: return ANIMATION_PATAMETERS_TYPE.Skill_2;
            case 2: return ANIMATION_PATAMETERS_TYPE.Skill_3;
            default: return ANIMATION_PATAMETERS_TYPE.None;
        }
    }
    public override void SkillEventEnd(int _skillnumber) //Event <- Attack End Event
    {
        ANIMATION_PATAMETERS_TYPE type = SkillAniamtiontype(_skillnumber);
        if (SkillKeyWardData.TryGetValue(_skillnumber, out SKILL_ID_TYPE skill))
        {
            AnimationParameterUpdate(type, false);
            //basicSkillSystem.SkillOff(skill);
        }
        else
        {
            Debug.LogError($"{skill} Skill = null");
        }

    }
    protected void EffectSoundOn() 
    {
        if (SoundDatas.TryGetValue((int)InfoData[CHARACTER_DATA.WalkSoundId], out AudioClip clip))
        {
            CharacterSoundPlayer.PlayOneShot(clip);
        }
    }
    protected IEnumerator DeathEffectOn(Monster_Base _monster,System.Action onComplete = null) 
    {
        if (!deathEffect.gameObject.activeSelf) 
        {
            deathEffect.gameObject.SetActive(true);
        }

        deathEffect.Play();
        EffectSoundOn();
        yield return new WaitForSeconds(deathEffect.main.duration);


        deathEffect.Stop();
        deathEffect.gameObject.SetActive(false);

        onComplete?.Invoke();

        GameShard.Instance.MonsterManager.Resurrection(_monster);
    }
    protected IEnumerator SpownEffectOn(Monster_Base _monster, System.Action onComplete = null)
    {
        if (!SpownEffect.gameObject.activeSelf)
        {
            SpownEffect.gameObject.SetActive(true);
        }

        SpownEffect.Play();
        EffectSoundOn();
        yield return new WaitForSeconds(SpownEffect.main.duration);


        SpownEffect.Stop();
        SpownEffect.gameObject.SetActive(false);

        onComplete?.Invoke();
    }
    //public async void AttackTimer(int value)//Event
    //{
    //    if (AttackStateCheck())
    //        return;

    //    AttackState = true;
    //    Debug.Log("Attack Start");

    //    await UniTask.Delay(TimeSpan.FromSeconds(value));

    //    Debug.Log("Attack End");
    //    AttackState = false;
    //}
    /// <summary>
    ///Not Trow Weapon, Not Trap Weapon
    /// </summary>
    //public void AttackDistanseCheckON()//Event
    //{
    //    AttackCTS = new CancellationTokenSource();
    //    DistanseCheckAsync(AttackCTS.Token).Forget(); // ����
    //    Debug.Log("DistanseCheck On");
    //}

    //public void AttackDistanseCheckOut()//Event
    //{
    //    //AttackState = false;
    //    //AnimationParameterCheck(MONSTER_AIMATION_PATAMETERS_TYPE.Skill_1, AttackState);

    //    if (AttackCTS != null)
    //    {
    //        AttackCTS.Cancel();
    //        AttackCTS.Dispose();
    //    }

    //    Debug.Log("DistanseCheck Off");
    //}
    public virtual PLAYER_DEBUFF GetDebuff(CHARACTER_ID _id) 
    {
        return PLAYER_DEBUFF.None;
    }
    protected virtual async UniTaskVoid DistanseCheckAsync(CancellationToken token)
    {
        while (true)
        {
            token.ThrowIfCancellationRequested(); // �ߴ� ��û ó��

            float dist = TargetDistanseCheck(PlayerTrans.position);

            //if (dist <= (float)State[MONSTER_STATE.Attack_Range])
            if (dist <= 0.2f)
            {
                Player character = PlayerTrans.gameObject.GetComponent<Player>();

                //GameShard.Instance.BattleManager.DamageCheck(this, character);

                
                break; 
            }
            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }
        
    }

    protected async void TimerOn()
    {
        IsPaused = true;
        await UniTask.Delay(TimeSpan.FromSeconds(3.0f));
        IsPaused = false;
        Debug.Log("UniTask Done");
    }



}
