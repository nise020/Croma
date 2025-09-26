using UnityEngine;

public class JewelBladeSkill : SkillBase
{
    private float range = 2.5f;
    private float width = 2.0f;
    private float height = 1.0f;
    private float depth = 0.1f;
    private float damage = 30f;

    public JewelBladeSkill()
        : base("jewelblade", "30", "Slices enemies in front with jewel blade", "40", "1.2", "Effects/JewelBlade")
    {
    }

    public override void Execute(Player player)
    {
        Debug.Log("JewelBladeSkill: Execute");

        float dir = Mathf.Sign(player.transform.localScale.x);
        Vector3 forward = player.transform.right * dir;
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

    public override void Tick(Player player, float deltaTime) { }
}