using UnityEngine;
using static Enums;

public class PotionItem : ItemBase
{
    [Header("Potion Specific")]
    public SUBTYPE potionType;
    public int buffId;
    public float duration;

    public PotionItem()
    {
        type = ITEMTYPE.Potion;
        maxStackCount = 10; // 포션은 기본 10개까지 가능
        stackCount = 1;
    }

    public virtual void Use()
    {
        if (stackCount <= 0)
        {
            Debug.LogWarning("Not Potion.");
            return;
        }

        Debug.Log($"{itemName} Use.");

        Player player = GameShard.Instance.GameManager.Player;
        switch (potionType)
        {
            case SUBTYPE.Heal:
                {
                    float before =  player.StatusData[Enums.CHARACTER_STATUS.Hp];
                    player.HealInstant(amount);
                    float after = player.StatusData[Enums.CHARACTER_STATUS.Hp];
                    Debug.LogWarning($"[Potion-Heal] {itemName} +{amount} | HP {before}->{after}");
                    break;
                }
            case SUBTYPE.Buff:
                {
                    player.UseBuffPotion(buffId);
                    Debug.Log($"[Potion-Buff] {itemName} buffId={buffId}");
                    break;
                }
            default:
                Debug.LogWarning($"[Potion] Unknown subType={subType}");
                break;
        }

        stackCount--;

        if (stackCount <= 0)
        {
            Debug.Log($"{itemName}을(를) 모두 사용했습니다.");
            // 인벤토리에서 제거 로직
        }
    }
}
