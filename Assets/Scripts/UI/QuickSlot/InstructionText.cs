using TMPro;
using UnityEngine;

public class InstructionText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;  
    [SerializeField] private Animator animator;
    private string stateName = "ShowInst";
    private bool useUnscaledTime = true;


    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        gameObject.SetActive(false);
    }

    public void ShowInstruction()
    {
        gameObject.SetActive(true);
        if (!animator) return;

        animator.updateMode = useUnscaledTime ? AnimatorUpdateMode.UnscaledTime : AnimatorUpdateMode.Normal;
        animator.Play(stateName, 0, 0f);
    }
    public void HideInstruction()
    {
        gameObject.SetActive(false);
    }
}
