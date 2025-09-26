using UnityEngine;
using static Enums;

public abstract partial class Item : MonoBehaviour
{
    public abstract CONFIG_OBJECT_TYPE ObjectType { get; }

    [SerializeField] protected int index = 0;
    [SerializeField] protected string itemImage = string.Empty;
    [SerializeField] protected int quantity = 0;
    //[SerializeField] protected ITEM_TYPE itemType = Default;
}