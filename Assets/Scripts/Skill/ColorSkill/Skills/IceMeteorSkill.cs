using UnityEngine;
using UnityEngine.ProBuilder;

public class IceMeteorSkill : SkillBase
{
    private float range = 5.0f;
    private float width = 3.0f;
    private float height = 6.0f;
    private float damage = 30f;
    private string projectilePath = "Prefabs/IceMeteor";

    public IceMeteorSkill()
        : base("icemeteor", "30", "Drops an ice meteor from above", "60", "1.5", "Effects/IceMeteor")
    {
    }

    public override void Execute(Player player)
    {
        Debug.Log("IceMeteorSkill: Execute");

        float dir = Mathf.Sign(player.transform.localScale.x);
        Vector3 forward = player.transform.right * dir;
        Vector3 center = player.transform.position + forward * (range / 2f);
        Vector3 dropPos = center + Vector3.up * height;

        GameObject prefab = Resources.Load<GameObject>(projectilePath);
        if (prefab == null) return;

    }

    public override void Tick(Player player, float deltaTime) { }
}