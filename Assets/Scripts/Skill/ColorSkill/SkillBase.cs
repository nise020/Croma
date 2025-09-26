public abstract class SkillBase
{
    
    public string skillName { get; protected set; }
    public string skillDamage { get; protected set; }
    public string skillDescription { get; protected set; }
    public string consumptionValue { get; protected set; }
    public string delay { get; protected set; }
    public string skillDataPath { get; protected set; }

    //public string skillType;

    public SkillBase(string name, string damage, string desc, string cost, string delay, string path)
    {
        this.skillName = name;
        this.skillDamage = damage;
        this.skillDescription = desc;
        this.consumptionValue = cost;
        this.delay = delay;
        this.skillDataPath = path;
    }

    public abstract void Execute(Player player);
    public abstract void Tick(Player player, float deltaTime);
}
