using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using static Enums;

public class BattleManager : MonoBehaviour
{
    [Header("Player")]
    //[SerializeField] Player PLAYER;
    //public GameObject playerObj;
    //PlayerStateBar StateBar;

    [Header("StartPoint")]
    [SerializeField] GameObject PlayerStartObject;

    [Header("HpBar")]
    [SerializeField] StateBar HPBAR;
    [SerializeField] GameObject hpBarObj;

    [Header("Creat")]
    [SerializeField] Transform Creatab;

    [Header("Ui")]
    public GameObject CanvasTab;


    [Header("Damage")]
    Shader damageShader;

    Material damageMaterial;

    [Header("Effect")]

    [SerializeField] List<GameObject> effectObj = new List<GameObject>();

    private readonly HashSet<int> _killReported = new HashSet<int>();

    public async UniTask InitAsync()
    {
        GameShard.Instance.BattleManager = this;
        damageShader = Shared.Instance.ResourcesManager.damageShader;
        damageMaterial = Shared.Instance.ResourcesManager.damageMaterial;
        await UniTask.Yield(); // 필요시 프레임 분산
    }


    public void DamageCheck(Character_Base _attacker, Character_Base _defender, SkillData _skill)
    {
        float defenserHp = _defender.StatusTypeLoad(CHARACTER_STATUS.Hp);

        if (defenserHp <= 0)
        {
            //Debug.LogError($"defenserHp = {defenserHp}");

            return;
        }

        float defenserDfs = _defender.StatusTypeLoad(CHARACTER_STATUS.Def) / 100;

        float attakerPower = _attacker.StatusTypeLoad(CHARACTER_STATUS.Atk);

        float skillPower = Random.Range(
            ((float)_skill.value / 100), 
            ((float)_skill.valueMax / 100));

        float damage = attakerPower * skillPower * (1f - defenserDfs);

        //_skill
        //defenserHp = defenserHp - (attakerPower - (attakerPower * defenserDfs));
        defenserHp -= damage;

        if (defenserHp > 0 && damage > 0)
        {
            _defender.KnockBackOn(damage);
        }

        _defender.HpUpdate(defenserHp);
        _defender.DamageImageOn(damage);

        if (_skill.buffId >= 2000)//Debuff 
        {
            _defender.BuffSystem.ApplyBuff((int)_skill.buffId);
            //if (_skill.id <= 2000)
            //{
            //    _defender.BuffSystem.ApplyBuff((int)_skill.buffId);
            //}
            //else
            //{
            //    _attacker.BuffSystem.ApplyBuff((int)_skill.buffId);
            //}
        }
        

        if (defenserHp <= 0 && _defender is Monster_Base)
        {
            Player player = _attacker as Player;
            Monster_Base monster = _defender as Monster_Base;

            int exp = _defender.ExpLoad();
            GameShard.Instance.GameUiManager.PlayerStateBar.ExpUpdateEvent?.Invoke(exp);
            GameShard.Instance.GameUiManager.PlayerStateBar.BurstExpUpdateEvent?.Invoke(1);//- = test

            int monsterId = (int)monster.IdType;
            //Quest_Type type = (monster is BossMonster) ? Quest_Type.Boss : Quest_Type.Kill;
            GameShard.Instance.QuestManager.OnObjective(Quest_Type.Kill, monsterId, 1);

            int score = Shared.Instance.DataManager.Character_Table.GetScore(monsterId);
            if (score > 0)
            {
                Debug.LogWarning($"[Score] {monsterId} Score : {score}");
                GameShard.Instance.GameManager.PlusGameScore(score);
            } 
        }


        if (defenserHp <= 0)
        {
            _attacker.AttackEvent?.Invoke(true);
        }
        else
        {
            _attacker.AttackEvent?.Invoke(false);
        }

        //sharder
        if (_defender.DamageEventCheck())
        {
            _defender.DamageEventUpdate(true);
            DamageColor(_defender);
        }
    }

    /// <summary>
    /// Hit Color Change
    /// </summary>
    /// <param name="_defender"></param>
    private void DamageColor(Character_Base _defender)
    {
        SkinnedMeshRenderer[] renderers = _defender.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
        //originalShaders.Clear();

        List<(SkinnedMeshRenderer renderer, Material[] originalMats)> backup = new();

        foreach (SkinnedMeshRenderer renderer in renderers)
        {
            Material[] originalMats = renderer.sharedMaterials;
            Material[] damageMet = new Material[originalMats.Length];

            for (int i = 0; i < originalMats.Length; i++)
            {
                Material damageInstance = new Material(damageMaterial);
                if (originalMats[i].HasProperty("_MainTex"))
                    damageInstance.SetTexture("_MainTex", originalMats[i].GetTexture("_MainTex"));

                damageMet[i] = damageInstance;

            }
            renderer.materials = damageMet;
            backup.Add((renderer, originalMats));

        }

        StartCoroutine(MaterialBackUp(_defender, backup, 0.1f));
    }
    IEnumerator MaterialBackUp(Character_Base _defender, List<(SkinnedMeshRenderer renderer,
        Material[] originalMats)> backup, float delay)
    {
        yield return new WaitForSeconds(delay);

        foreach (var (renderer, originalMats) in backup)
        {
            renderer.sharedMaterials = originalMats;
            _defender.DamageEventUpdate(false);
        }
    }

}
