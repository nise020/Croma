using UnityEngine;

public class BlackholeSkill : SkillBase
{
    private float range = 3.0f;
    private float radius = 2.0f;
    private float pullForce = 10.0f;
    private float damage = 30f;

    public BlackholeSkill()
        : base("blackhole", "30", "Creates a blackhole that pulls enemies", "50", "1.5", "Effects/Blackhole")
    {
    }

    public override void Execute(Player player)
    {
        Debug.Log("BlackholeSkill: Execute");

        float dir = Mathf.Sign(player.transform.localScale.x);
        Vector3 forward = player.transform.right * dir;
        Vector3 origin = player.transform.position + forward * range;

        Collider[] hits = Physics.OverlapSphere(origin, radius, LayerMask.GetMask("Monster"));
        for (int i = 0; i < hits.Length; i++)
        {
            var target = hits[i].GetComponent<Character_Base>();
            if (target != null)
            {
                Vector3 pullDir = (origin - target.transform.position).normalized;
                Rigidbody rb = target.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddForce(pullDir * pullForce, ForceMode.Impulse);
                }

                //GameShard.Instance.BattleManager.DamageCheck(player, target);//Damage 수정 필요
            }
        }

    }

    public override void Tick(Player player, float deltaTime) { }
}