using UnityEngine;
using static Enums;

public abstract class Weapon : Item
{
    public abstract CONFIG_OBJECT_TYPE MasterType { get; }
    public override CONFIG_OBJECT_TYPE ObjectType => CONFIG_OBJECT_TYPE.Weapon;
}
