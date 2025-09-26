using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillActionButton : SlotBase
{
    [SerializeField] private Button btn;

    public event Action Clicked;

    protected override void Awake()
    {
        base.Awake();

        if (btn == null)
            btn = GetComponent<Button>();

        if (btn)
        {
            btn.onClick.AddListener(() =>
            {
                if (btn.interactable) Clicked?.Invoke();
            });
        }
    }

    protected new void OnDisable()
    {
        base.OnDisable();
    }

    public void SetInteractable(bool value)
    {
        if (btn) 
            btn.interactable = value;
        
        if (!value)
        {
            isSelected = false;
            buttonManager?.HideButton(this);
        }
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (!isSelected && btn.interactable)
            buttonManager?.StartFade(this, 0f, 1f);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (!isSelected && btn.interactable)
            buttonManager?.StartFade(this, 1f, 0f);
    }
}
