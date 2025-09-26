using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using static Enums;

public class TrowSkill : IBasicSkill
{
    //public SkillData skillData { get; set; }
    //public override SKILL_ID_TYPE SkILL_Id => SKILL_ID_TYPE.Trow;
    private Monster_Base monster;
    //GameObject WeaponObj;
    TrowWeapon trowWeapon;
    bool isAttacking { get; set; } = false;
    public Transform activeTab { get; set; }
    protected CancellationTokenSource ThrowCTS;
    Character_Base target { get; set; }
    public override void Init(Character_Base _user)
    {
        monster = (Monster_Base)_user;
        //weaponObj = monster.GetWeaponObj();
        trowWeapon = WeaponObj.GetComponent<TrowWeapon>();
        if (trowWeapon != null) 
        {
            activeTab = trowWeapon.activeTab;
        }
        EffectAddData(monster.transform);
    }
    public override void OnTrigger(Character_Base _defender)
    {
        if (activeTab == null) 
        {
            Debug.LogError($"activeTab ={activeTab}");
            return;
        } 
        Vector3 pos = _defender.transform.position;
        //trowWeapon.WeaponAttack(pos);

        if (isAttacking) return;

        isAttacking = true;

        //Start Point test
        WeaponObj.transform.SetParent(activeTab);

        WeaponObj.transform.position = monster.transform.up;

        ThrowCTS?.Cancel();
        ThrowCTS = new CancellationTokenSource();
        TimerOn();
        ThrowAsync(pos, WeaponObj, ThrowCTS.Token).Forget();

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


        //Vector3 start = _obj.transform.position;
        Vector3 start = new Vector3(_obj.transform.position.x, 0, 0);
        float elapsed = 0f;

        Vector3 horizontal = new Vector3(_targetPos.x - start.x, 0, 0);
        float distance = horizontal.magnitude;
        Vector3 direction = horizontal.normalized;

        float Throuheight = 5.0f;
        float moveSpeed = trowWeapon.Speed;
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
    public void SkillEnd() 
    {
        ThrowCTS?.Cancel();

        if (WeaponObj.activeSelf)
        {
            WeaponObj.SetActive(false);

            float value = Vector3.Distance(WeaponObj.transform.position,
                                               target.transform.position);
            if (value <= 0.1)//Before Effect
            {
                GameShard.Instance.BattleManager.DamageCheck(monster, target, skillData);
            }
            else
            {
                return;
            }


        }
        BuffCheck();
    }

    public override void OnUpdate() { }
    public override void TriggerOut() { }
}
