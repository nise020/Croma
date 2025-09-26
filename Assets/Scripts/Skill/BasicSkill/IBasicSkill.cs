using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using static Enums;
using static Enums.CONFIG_OBJECT_TYPE;

public abstract class IBasicSkill
{
    public enum HitPointDirection 
    {
        None,
        Character,
        GroundPoint,
        CharacterFoward,
        CharacterAround,
        Effect,
        EffectFoward,
        EffectAround,
        Weapon,
        WeaponFoward,
    }

    public HitPointDirection Hit_Point { get; set; }
    public SKILL_ID_TYPE SkILL_Id { get; set; }
    protected SkillData skillData { get; set; }
    protected GameObject EffectObj { get; set; }
    protected ParticleSystem effect{ get; set; }
    protected BUFFTYPE bUFFTYPE { get; set; }
    private CancellationTokenSource effectCTS { get; set; } = null;

    protected Character_Base CHARECTER;

    protected Transform SkillTab;
    protected bool IsActive = false;

    /*[Header("Buff")]
    private bool buffLoopRunning = false;
    private CancellationTokenSource buffLoopCTS = null;*/

    protected Rigidbody rd = null;
    protected GameObject HitObject = null;
    protected GameObject WeaponObj;
    protected AudioSource EffectSound { get; set; }
    protected AudioClip EffectClip { get; set; }
    CancellationTokenSource SkillCTS { get; set; } = null;
    int _effectRunId = 0;
    Transform oneHitTran;
    public virtual void Init(Character_Base _user)
    {
        CHARECTER = _user;
        rd = _user.GetComponent<Rigidbody>();
        skillData = Shared.Instance.DataManager.Skill_Table.Get((int)SkILL_Id);

        EffectAddData(_user.transform);
        //HitObject = GetHitPoint(Hit_Point);
        HitObject = GetHitPointType(skillData.type);

    }
    public GameObject GetHitPointType(SkILLTYPE _hit) 
    {
        switch (_hit) 
        {
            case SkILLTYPE.Short:
            case SkILLTYPE.ShortBuff:
               // return CHARECTER.GetWeaponObj();
                return CHARECTER.gameObject;

            case SkILLTYPE.Long:
            case SkILLTYPE.LongBuff:
                return CHARECTER.gameObject;

            case SkILLTYPE.Area:
            case SkILLTYPE.AreaBuff:
                return EffectObj;

            default: return CHARECTER.gameObject;
        }
    }


    protected void SkillOn(Character_Base _defender) 
    {
        oneHitTran = HitObject.transform;
        rd.MovePosition(rd.position + CHARECTER.transform.forward);

        SkillCTS?.Cancel();

        _effectRunId++;
        int myRunId = _effectRunId;

        SkillCTS = new CancellationTokenSource();

        if (EffectObj == null || effect == null) return;

        EffectObj.SetActive(true);

        if (EffectObj.transform.parent != CHARECTER)
        {
            EffectObj.transform.SetParent(CHARECTER.transform);
        }

        EffectObj.transform.localPosition = new Vector3(0, 2, 0);
        EffectObj.transform.rotation = Quaternion.LookRotation(CHARECTER.transform.forward);

        //effect.Play();
        effectCheckAsync(SkillCTS, _defender, myRunId).Forget();
        TimerOn();

    }

    protected async UniTaskVoid effectCheckAsync(CancellationTokenSource _skillToken, Character_Base _defender,int myRunId)
    {
        int repeatCount = skillData.skillCountMax;
        float duration = effect.main.duration;
        //EffectSound.Play();
        EffectSound.PlayOneShot(EffectClip);
        try
        {
            while (repeatCount > 0 && !_skillToken.IsCancellationRequested)
            {
                effect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                effect.Clear();
                effect.Play(true);

                await UniTask.Delay(TimeSpan.FromSeconds(0.1f));

                await DistanseCheckAsync(_skillToken, _defender); // 시작
                //await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken: _token.Token);

                await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: _skillToken.Token);//, cancellationToken: _skillToken.Token

                repeatCount--;
                //if (repeatCount > 0) 
                //{
                //    effect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                //    effect.Clear();
                //    effect.Play();
                //}

            }
           // _skillToken.Cancel();
        }
        catch 
        {

        }
        finally
        {
            if (CHARECTER != null) 
            {
                bool iAmCurrent = (myRunId == _effectRunId) && (_skillToken == SkillCTS);

                BuffCheck();

                _skillToken.Dispose();
                if (_skillToken == SkillCTS) // 혹시 다른 스킬 시작했을 수도 있으니 확인
                    SkillCTS = null;


                if (iAmCurrent)
                {
                    effect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                    EffectObj.SetActive(false);
                    EffectObj.transform.localPosition = Vector3.zero;
                }
            }
        }
    }
    protected bool RangeAttackDistanseCheck(Vector3 _weapon, Vector3 _target, float _dist)
    {
        //range 360
        float dist = Vector3.Distance(_weapon, _target);

        if (dist <= _dist)
        {
            return true;
        }
        else
        {
            //Debug.Log($"{dist}");
            return false;
        }
    }
    protected bool FowardAttackDistanseCheck(Vector3 _target, float _dist)
    {
        //short 60
        float dist = Vector3.Distance(oneHitTran.transform.position, _target);

        if (dist <= _dist)
        {
            Vector3 dirToTarget = (_target - oneHitTran.transform.position).normalized;

            Vector3 forward = oneHitTran.transform.forward;
            // 전방 방향과의 내적 확인
            float dot = Vector3.Dot(forward, dirToTarget);

            if (dot > 0f)
            {
                return true;

            }
            else
            {
                return false;
            }
        }
        else
        {
            //Debug.Log($"{dist}");
            return false;
        }
    }
    public async void TimerOn()
    {
        IsActive = true;
        await UniTask.Delay(TimeSpan.FromSeconds(skillData.time));
        IsActive = false;
        Debug.Log("UniTask Done");
    }
    public bool State()
    {
        return IsActive;
    }
    public void BuffCheck()
    {
        if (skillData.buffId <= 2000)
        {
            CHARECTER.BuffSystem.ApplyBuff(skillData.buffId);
        }
    }
    protected virtual async UniTask DistanseCheckAsync(CancellationTokenSource _token, Character_Base _defender)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(0.2f), cancellationToken: _token.Token);
    }
    public GameObject GetHitPoint(HitPointDirection _hit) 
    {
        switch (_hit) 
        {
            case HitPointDirection.Character: return CHARECTER.gameObject;
            case HitPointDirection.Weapon: return WeaponObj = CHARECTER.GetWeaponObj(); ;
            case HitPointDirection.EffectFoward: return EffectObj;
            default: return CHARECTER.gameObject;
        }
    }
    protected void EffectAddData(Transform _patrnt)
    {
        //skillData = Shared.Instance.DataManager.Skill_Table.Get((int)SkILL_Id);
        if (skillData == null)
        {
            Debug.LogError($"{this}skillData = {skillData}");
            return;
        }

        if (skillData.prefab != "" &&
            skillData.prefab != "0") 
        {
            EffectObj = Shared.Instance.ResourcesManager.CreatObject(Skill, skillData.prefab);
            EffectObj.transform.SetParent(_patrnt);

            effect = EffectObj.GetComponent<ParticleSystem>();

            EffectSound = EffectObj.AddComponent<AudioSource>();

            EffectSound = Shared.Instance.SoundManager.SoundSetting(skillData.SoundId, EffectSound);//Test
            //EffectClip = Shared.Instance.SoundManager.ClipGet(skillData.SoundId);
            
            EffectObj.SetActive(false);
            effect.Stop();

            //Debug.Log($"{this}skillData.PreFab = {skillData.prefabPath}");
        }
        else 
        {
            //Debug.LogError($"{this}skillData.PreFab = {skillData.prefabPath}"); 
        }
    }
    //public void EffectOn(Transform _patrnt, CancellationTokenSource _token) 
    //{
    //    if (EffectObj != null)
    //    {
    //        if (effectCTS != null)
    //        {
    //            effectCTS?.Cancel();
    //            effectCTS?.Dispose();
    //        }
    //        effectCTS = new CancellationTokenSource();

    //        EffectObj.SetActive(true);

    //        if (EffectObj.transform.parent != _patrnt)
    //        {
    //            EffectObj.transform.SetParent(_patrnt);
    //        }

    //        EffectObj.transform.localPosition = new Vector3(0, 2, 0);
    //        EffectObj.transform.rotation = Quaternion.LookRotation(_patrnt.forward);

    //        effect.Play();
    //        //effectCheckAsync(effectCTS, _token).Forget();
    //    }
    //}
    //public void EffectOn(Transform _patrnt) 
    //{
    //    if (EffectObj != null) 
    //    {
    //        if (effectCTS != null) 
    //        {
    //            effectCTS?.Cancel();
    //            effectCTS?.Dispose();
    //        }
    //        effectCTS = new CancellationTokenSource();

    //        EffectObj.SetActive(true);

    //        if (EffectObj.transform.parent != _patrnt) 
    //        {
    //            EffectObj.transform.SetParent(_patrnt);
    //        }

    //        EffectObj.transform.localPosition = new Vector3(0,2,0);
    //        //EffectObj.transform.rotation = Quaternion.LookRotation(_patrnt.forward);

    //        effect.Play();
    //        effectCheckAsync(effectCTS.Token).Forget();
    //    }
    //}
    protected async UniTaskVoid effectCheckAsync(CancellationToken _effecttoken) 
    {
        try
        {
            var main = effect.main;
            float duration = main.duration;

            await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: _effecttoken);

            if (!_effecttoken.IsCancellationRequested)
            {
                effect.Stop();
                effectCTS = null;
                //EffectObj.transform.SetParent(SkillTab);
                EffectObj.transform.localPosition = Vector3.zero;
                EffectObj.SetActive(false);
            }
        }
        catch (OperationCanceledException)
        {
            // ��� �� ���� ����
        }
    }



    public virtual void OnUpdate() { }
    public virtual void TriggerOut() { }
    public abstract void OnTrigger(Character_Base _defender);

    #region
    /*private void StartBuffTickLoop()
{
    buffLoopCTS?.Cancel();
    buffLoopCTS?.Dispose();

    buffLoopCTS = new CancellationTokenSource();
    buffLoopRunning = true;
    BuffTickLoopAsync(buffLoopCTS.Token).Forget();
}

private async UniTaskVoid BuffTickLoopAsync(CancellationToken token)
{
    try
    {
        while (!token.IsCancellationRequested)
        {
            if (CHARECTER == null || CHARECTER.ConditionState == Enums.CHARACTER_CONDITION_STATE.Death)
                break;

            CHARECTER.BuffSystem.Tick(Time.deltaTime);

            if (CHARECTER.BuffSystem.GetActiveBuffCount() == 0)
                break;

            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }
    }
    catch (OperationCanceledException)
    {
        // 취소 시 무시
    }
    finally
    {
        buffLoopRunning = false;
        buffLoopCTS?.Dispose();
        buffLoopCTS = null;
    }
}

public void OnOwnerDeath()
{
    effectCTS?.Cancel();
    effectCTS?.Dispose();
    effectCTS = null;

    buffLoopCTS?.Cancel();
    buffLoopCTS?.Dispose();
    buffLoopCTS = null;

    CHARECTER?.BuffSystem.ClearAllBuffs();

    buffLoopRunning = false;
}*/
    #endregion
}