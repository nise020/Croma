using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum MenuType { Use, QuickSlot }

public class ItemMenuBtn : SlotBase
{
    [SerializeField] private MenuType type;
    [SerializeField] public Button btn;
    private Action onClick;

    protected override void Awake()
    {
        base.Awake();
    }
    

    public void Init(Action onUse, Action onQuick)
    {
        onClick = (type == MenuType.Use) ? onUse : onQuick;

        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => onClick?.Invoke());
        }
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (!isSelected)
            buttonManager?.StartFade(this, 0f, 1f);      
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (!isSelected)
            buttonManager?.StartFade(this, 1f, 0f);
    }

}
