using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum LevelNoticeType { Skill, Stat }

public class PointReminder : MonoBehaviour
{
    [SerializeField] private LevelNoticeType type;

    [Header("Refs")]
    [SerializeField] private Animator animator;
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text valueText;
    [SerializeField] private CanvasGroup cg;

    [Header("Type Sprite")]
    [SerializeField] private Sprite skillIcon;
    [SerializeField] private Sprite statIcon;

    [Header("Reminder")]
    [SerializeField] private bool reminderEnabled = true;
    [SerializeField] private float reminderInterval = 5f;
    [SerializeField] private bool useUnscaledTime = true;

    private Coroutine remindCo;
    private bool hasUnspent;
    private int lastAmount;


    void Awake()
    {
        if (!cg && !TryGetComponent(out cg)) 
            cg = gameObject.AddComponent<CanvasGroup>();

        if (animator == null)
            animator = GetComponent<Animator>();

        if (cg)
        {
            cg.interactable = false;
            cg.blocksRaycasts = false;
            cg.alpha = 0f;
        }

        if (useUnscaledTime && animator) 
            animator.updateMode = AnimatorUpdateMode.UnscaledTime;
    }

    private void OnDisable()
    {
        if (remindCo != null) { StopCoroutine(remindCo); remindCo = null; }
    }

    public void SetType(LevelNoticeType type) => this.type = type;
    public void SetReminderEnabled(bool enabled)
    {
        reminderEnabled = enabled;
        ArmReminderIfNeeded();
    }

    public void SetReminderInterval(float seconds)
    {
        reminderInterval = Mathf.Max(0.1f, seconds);
        ArmReminderIfNeeded();
    }

    public void ForceOff()
    {
        hasUnspent = false;
        if (remindCo != null)
        { 
            StopCoroutine(remindCo); 
            remindCo = null; 
        }

        cg.alpha = 0f;
        cg.blocksRaycasts = false;
    }

    public void PlayOnce(int amount, bool hasUnspentNow)
    {
        if(animator) animator.enabled = true;
        lastAmount = amount;
        hasUnspent = hasUnspentNow;

        if (icon)
            icon.sprite = (type == LevelNoticeType.Stat) ? statIcon : skillIcon;

        if (valueText)
            valueText.text = (amount >= 0) ? $"+{amount}" : amount.ToString();

        cg.alpha = 1f;
        if (animator) animator.Play("Show", 0, 0f);

        ArmReminderIfNeeded();
    }

    public void SetHasUnspent(bool value, int currentPoint = 0)     
    {
        hasUnspent = value;
        if (value) lastAmount = currentPoint;
        ArmReminderIfNeeded();
    }

    public void OnShowEnd()
    {
        cg.alpha = 0f;
        cg.blocksRaycasts = false;
    }

    private void ArmReminderIfNeeded()
    {
        if (!reminderEnabled || !hasUnspent)
        {
            if (remindCo != null) { StopCoroutine(remindCo); remindCo = null; }
            return;
        }
        if (remindCo == null) remindCo = StartCoroutine(RemindLoop());
    }

    private IEnumerator RemindLoop()
    {
        while (hasUnspent && reminderEnabled)
        {
            if (useUnscaledTime) yield return new WaitForSecondsRealtime(reminderInterval);
            else yield return new WaitForSeconds(reminderInterval);

            if (!hasUnspent || !reminderEnabled) break;

            valueText.text = (lastAmount >= 0) ? $"+{lastAmount}" : lastAmount.ToString();

            cg.alpha = 1.0f;
            if (animator)
            {
                //cg.alpha = 1.0f;
                animator.Play("Show", 0, 0f);
            }
        }
        remindCo = null;
    }
}
