// IBuffStrategy.cs
using UnityEngine;
using static Enums;

public abstract class IBuffStrategy
{
    protected BuffData buffData;
    protected Character_Base target;

    public BuffData BuffData => buffData;

    // ���� ����Ʈ(������������ ���)
    protected GameObject sharedEffectObj;
    protected ParticleSystem sharedEffect;
    protected Transform sharedOrigParent;

    // ���� ����Ʈ(���ݷ� ���������� ���)
    protected GameObject oneShotObj;
    protected ParticleSystem oneShotPs;
    protected float oneShotTimer;

    public virtual float GetElapsed() => 0f; // ���� Ŭ�������� override
    public virtual float GetDuration() => buffData != null ? buffData.time : 0f;

    public virtual void Init(Character_Base _user, BuffData data)
    {
        buffData = data;
        target = _user;
    }

    protected void PlayOneShotEffect(string path, Transform parent)
    {
        if (string.IsNullOrEmpty(path)) return;

        var prefab = Resources.Load<GameObject>(path);
        if (!prefab) return;

        oneShotObj = Object.Instantiate(prefab, parent, false);
        oneShotPs = oneShotObj.GetComponent<ParticleSystem>();
        if (oneShotObj) oneShotObj.SetActive(true);

        if (oneShotPs)
        {
            oneShotPs.Play();
            var main = oneShotPs.main;
            float startLifetime = main.startLifetime.mode == ParticleSystemCurveMode.TwoConstants
                                  ? main.startLifetime.constantMax
                                  : main.startLifetime.constant;
            oneShotTimer = main.duration + startLifetime; // ������ �ڵ� �ı� �ð�
        }

        else
        {
            oneShotTimer = 1.0f;
        }
    }

    protected void TickOneShot(float dt)
    {
        if (!oneShotObj) return;
        oneShotTimer -= dt;

        if (oneShotTimer <= 0f)
        {
            Object.Destroy(oneShotObj);
            oneShotObj = null;
            oneShotPs = null;
        }
    }

    protected void AcquireSharedEffect(string path, Transform parent)
    {
        if (string.IsNullOrEmpty(path)) return;

        sharedEffectObj = Shared.Instance.ResourcesManager.FindPrefab(CONFIG_OBJECT_TYPE.Buff, path, out sharedOrigParent);
        if (!sharedEffectObj)
        {
            AcquireOwnedEffect(path, parent); // ����
            return;
        }

        sharedEffectObj.transform.SetParent(parent, false);
        sharedEffectObj.transform.localPosition = new Vector3(0, 2.0f ,0);
        sharedEffectObj.SetActive(true);

        sharedEffect = sharedEffectObj.GetComponent<ParticleSystem>();

        if (sharedEffect)
        {
            sharedEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            sharedEffect.Play();
        }
    }

    protected void ReleaseSharedEffect()
    {
        if (!sharedEffectObj) return;

        if (sharedEffect)
        {
            sharedEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        sharedEffectObj.SetActive(false);

        if (sharedOrigParent)
        {
            sharedEffectObj.transform.SetParent(sharedOrigParent, false); // ���� ������ ����
        }
        sharedEffectObj = null;
        sharedEffect = null;
        sharedOrigParent = null;
    }

    public virtual void Tick(float dt)
    {
        TickOneShot(dt);
    }

    public virtual void RemoveBuff(Character_Base target)
    {
        ReleaseSharedEffect();
    }

    public abstract void ApplyBuff(Character_Base target);
    public abstract bool IsFinished();

    protected void AcquireOwnedEffect(string path, Transform parent)
    {
        if (string.IsNullOrEmpty(path)) return;
        var prefab = Resources.Load<GameObject>(path);
        if (!prefab) return;

        sharedEffectObj = Object.Instantiate(prefab, parent, false);
        sharedEffectObj.transform.localPosition = new Vector3(0, 2.0f, 0);
        sharedEffectObj.SetActive(true);

        sharedEffect = sharedEffectObj.GetComponent<ParticleSystem>();
        if (sharedEffect)
        {
            sharedEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            sharedEffect.Play();
        }
        sharedOrigParent = null;
    }
}