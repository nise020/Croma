using UnityEngine;

public class WindBladeSkill : SkillBase
{
    private float hitInterval = 2.0f;
    private int hitCount = 3;
    private float width = 2.0f;
    private float height = 1.0f;
    private float depth = 0.1f;
    private float damage = 15f;

    public WindBladeSkill()
        : base("windblade", "15", "Strikes forward with 3 wind blades", "45", "1.4", "Effects/WindBlade")
    {
    }

    public override void Execute(Player player)
    {
        Debug.Log("WindBladeSkill: Execute");

        float dir = Mathf.Sign(player.transform.localScale.x);
        Vector3 forward = player.transform.right * dir;

        for (int i = 0; i < hitCount; i++)
        {
            Vector3 center = player.transform.position + forward * (hitInterval * (i + 1));
            Vector3 halfExtents = new Vector3(width / 2f, height / 2f, depth / 2f);

            Collider[] hits = Physics.OverlapBox(center, halfExtents, Quaternion.identity, LayerMask.GetMask("Monster"));
            for (int j = 0; j < hits.Length; j++)
            {
                var target = hits[j].GetComponent<Character_Base>();
                if (target != null)
                {
                    //GameShard.Instance.BattleManager.DamageCheck(player, target);//Damage 수정 필요
                }
            }
        }
    }

    public override void Tick(Player player, float deltaTime) { }
}