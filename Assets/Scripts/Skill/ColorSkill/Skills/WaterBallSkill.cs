using UnityEngine;
using UnityEngine.ProBuilder;

public class WaterBallSkill : SkillBase
{
    private float range = 5.0f;
    private float height = 4.0f;
    private float damage = 30f;
    private string projectilePath = "Prefabs/Waterball";

    public WaterBallSkill()
        : base("waterball", "30", "Drops waterballs on enemies", "50", "1.2", "Effects/Waterball")
    {
    }

    public override void Execute(Player player)
    {
        Debug.Log("WaterBallSkill: Execute");

        float dir = Mathf.Sign(player.transform.localScale.x);
        Vector3 forward = player.transform.right * dir;
        Vector3 center = player.transform.position + forward * (range / 2f);
        Vector3 halfExtents = new Vector3(range / 2f, 1f, 0.05f);

        Collider[] hits = Physics.OverlapBox(center, halfExtents, Quaternion.identity, LayerMask.GetMask("Monster"));
        for (int i = 0; i < hits.Length; i++)
        {
            var target = hits[i].GetComponent<Character_Base>();
            if (target != null)
            {
                Vector3 dropPos = target.transform.position + Vector3.up * height;

                GameObject prefab = Resources.Load<GameObject>(projectilePath);
                if (prefab == null) continue;

                GameObject waterball = GameObject.Instantiate(prefab, dropPos, Quaternion.identity);

            }
        }
    }

    public override void Tick(Player player, float deltaTime) { }
}