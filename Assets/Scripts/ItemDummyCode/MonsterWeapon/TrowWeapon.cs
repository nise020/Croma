using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static Enums;

public class TrowWeapon : MonsterWeapon
{
    public override MONSTER_WEAPON_TYPE WeaponType => MONSTER_WEAPON_TYPE.Trow;
    float Throuheight { get; } = 5.0f;
    private Vector3 startPos { get; set; }
    public Transform activeTab { get; set; }
    public override void init(Transform _trs) => activeTab = _trs;
    public float Speed;
    TrowSkill trowSkill { get; set; }
    public override void init(Character_Base _user) => Character = _user;
    //public override void StateInit(Dictionary<CHARACTER_STATUS, int> _state) 
    //{
    //    Speed = (float)_state[MONSTER_STATE.Projectile_Speed];
    //}
    protected override void Awake()
    {
        base.Awake();
        startPos = gameObject.transform.position;
    }
    public void SkillInit(IBasicSkill _skill) 
    {
        trowSkill = (TrowSkill)_skill;
    }
    public override void WeaponAttack(Vector3 _targetPos)
    {
        if (isAttacking) return;

        isAttacking = true;

        //Start Point test
        gameObject.transform.SetParent(activeTab);

        gameObject.transform.position = Character.gameObject.transform.up;

        ThrowCTS?.Cancel(); 
        ThrowCTS = new CancellationTokenSource();
        ThrowAsync(_targetPos, gameObject, ThrowCTS.Token).Forget();
    }
   
    protected async UniTaskVoid ThrowAsync(Vector3 _targetPos, GameObject _obj, CancellationToken token)
    {
        if (_obj == null)
        {
            isAttacking = false;
            return;
        }

        if (!_obj.activeSelf)
        {
            _obj.SetActive(true);
        }


        Vector3 start = _obj.transform.position;
        float elapsed = 0f;

        Vector3 horizontal = new Vector3(_targetPos.x - start.x, 0, _targetPos.z - start.z);
        float distance = horizontal.magnitude;
        Vector3 direction = horizontal.normalized;

        float moveSpeed = Speed;
        float moveTime = distance / moveSpeed;

        while (elapsed < moveTime)
        {
            elapsed += Time.deltaTime;
            float time = elapsed / moveTime;

            float parabola = 4 * Throuheight * time * (1 - time);
            Vector3 currentPos = start + direction * distance * time;
            currentPos.y = Mathf.Lerp(start.y, _targetPos.y, time) + parabola;

            _obj.transform.position = currentPos;

            await UniTask.Yield();
        }

        _obj.transform.position = _targetPos;
    }

    private void OnCollisionEnter(Collision collision)
    {
        trowSkill.SkillEnd();
        base.WeaponAttack(new Vector3());
    }
}
