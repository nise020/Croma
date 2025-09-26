using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ColorSlots : MonoBehaviour
{
    public int CurrentSlot = 0;
    public List<Transform> Slots = new();

    private Animator animator = null;

    public bool IsAniEnded = true;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        IsAniEnded = true;
    }
    public void ChangeSlot(InputAction.CallbackContext _ctx)
    {
        if (_ctx.performed && IsAniEnded)
        {
            IsAniEnded = false;
            if (CurrentSlot < 2) CurrentSlot++;
            else CurrentSlot = 0;

            if (CurrentSlot == 0) animator.SetTrigger("3To1");
            else if (CurrentSlot == 1) animator.SetTrigger("1To2");
            else animator.SetTrigger("2To3");
        }
    }
    public void SetSiblingIndex() => Slots[CurrentSlot].SetAsLastSibling();
    public void CheckAniEnd() => IsAniEnded = true; 
}
