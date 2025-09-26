using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AcquiredSlot : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI countText;

    public void SetSlot(Sprite sprite, int count)
    {
        if (sprite == null || count <= 0) return;

        itemIcon.sprite = sprite;
        countText.text = count.ToString();
    }

}
