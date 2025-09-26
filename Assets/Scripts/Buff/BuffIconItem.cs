using UnityEngine;
using UnityEngine.UI;


public class BuffIcon : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Image fill;

    public void SetSprite(Sprite s)
    {
        if (icon) 
            icon.sprite = s;       
    }

    public void SetRemain(float remain)
    {
        if (!fill) return;

        if (fill.type != Image.Type.Filled) 
            fill.type = Image.Type.Filled;

        fill.fillMethod = Image.FillMethod.Horizontal;
        fill.fillOrigin = (int)Image.OriginHorizontal.Left;
        fill.fillAmount = Mathf.Clamp01(remain);
    }
}
