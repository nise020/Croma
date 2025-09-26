using UnityEngine;
using UnityEngine.ProBuilder;


// projectile XXXXXXXXXXXXXX
public class FireShotSkill : SkillBase
{
    private float speed = 12.0f;
    private float damage = 30f;
    private string projectilePath = "Prefabs/Fireball";

    public FireShotSkill()
        : base("fireshot", "30", "Shoots a fireball forward", "40", "1.2", "Effects/Fireball")
    {
    }

    public override void Execute(Player player)
    {
        Debug.Log("FireShotSkill: Execute");

        float dir = Mathf.Sign(player.transform.localScale.x);
        Vector3 forward = player.transform.right * dir;
        Vector3 spawnPos = player.transform.position + forward * 1.0f;

/*        GameObject prefab = Resources.Load<GameObject>(projectilePath);
        if (prefab == null) return;

        GameObject proj = GameObject.Instantiate(prefab, spawnPos, Quaternion.identity);
        Rigidbody rb = proj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = forward * speed;
        }

        Projectile projectile = proj.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.Initialize(damage);
        }*/
    }

    public override void Tick(Player player, float deltaTime) { }
}