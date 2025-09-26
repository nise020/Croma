using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using static Enums;

public class TentacleWeapon : MonsterWeapon
{
    Transform activeTab { get; set; }
    public override void init(Transform _trs) => activeTab = _trs;
    public override MONSTER_WEAPON_TYPE WeaponType => MONSTER_WEAPON_TYPE.Trap;
    Animator animator;
    float range;
    int coolTime;
    protected override void Awake() 
    {
        base.Awake();
        animator = GetComponent<Animator>();
    }

    public override void StateInit(Dictionary<CHARACTER_STATUS, int> _state)
    {
        //range = (float)_state[CHARACTER_STATUS.Attack_Range];
        //coolTime = (int)_state[MONSTER_STATE.Skill_Cool_1];
    }

    public override void WeaponAttack(Vector3 _pos) 
    {
        gameObject.transform.SetParent(activeTab);

        gameObject.transform.position = _pos;
        gameObject.SetActive(true);

        base.WeaponAttack(gameObject.transform.position);

        if (animator != null) 
        {
            animator.SetInteger(ANIMATION_PATAMETERS_TYPE.Skill_1.ToString(), 1);
            Timer(coolTime);
        }
    }
    public async void Timer(int _timer)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(_timer));
        animator.SetInteger(ANIMATION_PATAMETERS_TYPE.Skill_1.ToString(), 0);

        Debug.Log("UniTask Done");
    }

    private void OnCollisionEnter(Collision collision)
    {
        float value = TargetDistanseCheck(Target.transform.position);
        if (value <= range)
        {
            Debug.Log($"Player Hit");
            //GameShard.Instance.BattleManager.DamageCheck(Character, Target);
        }
    }

}
