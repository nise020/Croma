using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static Enums;

public abstract class MonsterWeapon : Weapon
{
    public abstract MONSTER_WEAPON_TYPE WeaponType { get;}
    public override CONFIG_OBJECT_TYPE MasterType => CONFIG_OBJECT_TYPE.Monster;
    public bool isAttacking { get; set; } = false;
    //public bool isAttacking { get; set; } = false;
    protected GameObject weaponObj{ get; set; }
    protected ParticleSystem Effect { get; set; }
    protected GameObject EffectObj { get; set; }
    protected Character_Base Character { get; set; }
    protected Character_Base Target { get; set; }
    
    public virtual void init(Character_Base _user) => Character = _user;
 
    public Color CopyColor = Color.white;
    protected bool DamageCheck = false;
    protected CancellationTokenSource effectCTS;
    protected CancellationTokenSource ThrowCTS;
    protected virtual void Awake()
    {
        //Model
        MeshRenderer mesh = GetComponentInChildren<MeshRenderer>();
        weaponObj = mesh.gameObject;
        //Effect
        Effect = GetComponentInChildren<ParticleSystem>();
        EffectObj = Effect.gameObject;
    }
    protected async UniTaskVoid ResetDamageFlag()
    {
        await UniTask.Delay(500); // 0.5√  = 500ms
        DamageCheck = false;
    }
    public virtual void init(Transform _trs) { }
    public virtual void StateInit(Dictionary<CHARACTER_STATUS, int> _state)
    {
    }
    public virtual void WeaponAttack(Vector3 _targetPos) 
    {
        if (Effect != null)
        {
            Effect.gameObject.SetActive(true);
            Effect.Play();

            EffectDistanseCheck(effectCTS.Token).Forget();
        }
        else
        {
            Debug.LogError($"Effect = {Effect}");
        }

    }
    protected async UniTaskVoid EffectDistanseCheck(CancellationToken token)
    {
        float duration = Effect.main.duration;
        float elapsed = 0f;
        try
        {
            while (elapsed < duration)
            {
                if (!DamageCheck)
                {
                    float value = Vector3.Distance(Effect.transform.position, Target.transform.position);

                    if (value <= 0.3)//Before Effect
                    {
                        //GameShard.Instance.BattleManager.DamageCheck(Character, Target);
                        DamageCheck = false;
                        ResetDamageFlag().Forget();
                    }

                    await UniTask.Yield(token);
                    elapsed += Time.deltaTime;
                }

            }

            Effect.Stop();
            Effect.gameObject.SetActive(false);
            gameObject.transform.SetParent(Character.gameObject.transform,true);

            DamageCheck = false;
            isAttacking = false;

            gameObject.SetActive(false);
        }
        catch (OperationCanceledException)
        {

        }
    }

    public virtual float TargetDistanseCheck(Vector3 _pos)
    {
        Vector3 direction = _pos - transform.position;
        direction.y = 0f;
        direction.z = 0f;

        float distance = direction.magnitude;

        return distance;

    }
}
