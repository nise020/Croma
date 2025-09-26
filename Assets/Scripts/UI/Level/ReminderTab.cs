using UnityEngine;

public class ReminderTab : MonoBehaviour
{
    [SerializeField] public PointReminder statReminder;
    [SerializeField] public PointReminder skillReminder;

    private void Awake()
    { 
        if (statReminder)
        { 
            statReminder.SetType(LevelNoticeType.Stat);
            statReminder.ForceOff(); 
        }
        if (skillReminder) 
        { 
            skillReminder.SetType(LevelNoticeType.Skill);
            skillReminder.ForceOff(); 
        }
    }

    public void Show(LevelNoticeType type, int amount, bool hasUnspent)
        => Get(type).PlayOnce(amount, hasUnspent);

    public void SetHasUnspent(LevelNoticeType type, bool hasUnspent, int currentAmount = 0)
        => Get(type).SetHasUnspent(hasUnspent, currentAmount);

    public void ShowStat(int amount, bool hasUnspent) => statReminder.PlayOnce(amount, hasUnspent);
    public void ShowSkill(int amount, bool hasUnspent) => skillReminder.PlayOnce(amount, hasUnspent);
    public void UpdateStat(bool hasUnspent, int cur = 0) => statReminder.SetHasUnspent(hasUnspent, cur);
    public void UpdateSkill(bool hasUnspent, int cur = 0) => skillReminder.SetHasUnspent(hasUnspent, cur);

    public void ForceOffAll()
    {
        if (statReminder) statReminder.ForceOff();
        if (skillReminder) skillReminder.ForceOff();
    }

    public void SetReminderEnabled(bool enabled)
    {
        if (statReminder) statReminder.SetReminderEnabled(enabled);
        if (skillReminder) skillReminder.SetReminderEnabled(enabled);
    }

    public void SetReminderInterval(float seconds)
    {
        if (statReminder) statReminder.SetReminderInterval(seconds);
        if (skillReminder) skillReminder.SetReminderInterval(seconds);
    }

    private PointReminder Get(LevelNoticeType type)
        => (type == LevelNoticeType.Stat) ? statReminder : skillReminder;
}
