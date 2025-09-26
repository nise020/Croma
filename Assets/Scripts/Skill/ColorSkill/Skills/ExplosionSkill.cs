using UnityEngine;
using System.Collections.Generic;
using System.Threading;

public class ExplosionSkill : SkillBase
{
    private float range = 3.0f;
    private float width = 2.0f;
    private float height = 1.0f;
    private float depth = 0.1f;
    private float damage = 30f;

    public ExplosionSkill()
        : base("explosion", "30", "Explodes ground in front of the player", "50", "1.5", "Effects/Explosion")
    {
    }

    public override void Execute(Player player)
    {
        Debug.Log("ExplosionSkill: Execute");

        // Direction based on facing (2.5D logic)
        float dir = Mathf.Sign(player.transform.localScale.x);
        Vector3 forward = player.transform.right * dir;

        // OverlapBox center in front
        Vector3 center = player.transform.position + forward * (range / 2f);
        Vector3 halfExtents = new Vector3(width / 2f, height / 2f, depth / 2f);

        Collider[] hits = Physics.OverlapBox(center, halfExtents, Quaternion.identity, LayerMask.GetMask("Monster"));
        for (int i = 0; i < hits.Length; i++)
        {
            var target = hits[i].GetComponent<Character_Base>();
            if (target != null)
            {
                //GameShard.Instance.BattleManager.DamageCheck(player, target);//Damage 수정 필요
            }
        }

    }

    public override void Tick(Player player, float deltaTime)
    {
        // No passive effect
    }
}