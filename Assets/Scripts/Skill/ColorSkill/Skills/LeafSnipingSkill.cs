using UnityEngine;
using UnityEngine.ProBuilder;

public class LeafSnipingSkill : SkillBase
{
    private float speed = 18.0f;
    private float damage = 30f;
    private string projectilePath = "";

    public LeafSnipingSkill()
        : base("leafsniping", "30", "Shoots a sharp leaf forward", "40", "1.0", "Effects/Leaf")
    {
    }

    public override void Execute(Player player)
    {
        Debug.Log("LeafSnipingSkill: Execute");

        float dir = Mathf.Sign(player.transform.localScale.x);
        Vector3 forward = player.transform.right * dir;
        Vector3 spawnPos = player.transform.position + forward * 1.0f;

        GameObject prefab = Resources.Load<GameObject>(projectilePath);
        if (prefab == null) return;

        GameObject proj = GameObject.Instantiate(prefab, spawnPos, Quaternion.identity);
        
    }

    public override void Tick(Player player, float deltaTime) { }
}