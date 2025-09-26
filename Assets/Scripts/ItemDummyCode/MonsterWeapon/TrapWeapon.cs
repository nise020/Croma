using System;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class TrapWeapon : MonsterWeapon
{
    Transform activeTab { get; set; }
    public override void init(Transform _trs) => activeTab = _trs;

    public override MONSTER_WEAPON_TYPE WeaponType => MONSTER_WEAPON_TYPE.Trap;
    
    float range;
    public override void WeaponAttack(Vector3 _pos) 
    {
        gameObject.transform.SetParent(activeTab);

        gameObject.transform.position = _pos;
        base.WeaponAttack(gameObject.transform.position);

    }
    
    protected override void Awake()
    {
       base.Awake();
    }

    //public override void StateInit(Dictionary<CHARACTER_STATUS, int> _state)
    //{
    //    range = (float)_state[MONSTER_STATE.Attack_Range];
    //}

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
