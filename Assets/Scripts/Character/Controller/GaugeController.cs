using UnityEngine;
using UnityEngine.UI;

public class GaugeController : MonoBehaviour
{
    [SerializeField] private Image FillAmount = null;
    [SerializeField] private Image DelayedAmount = null;

    public float MaxValue = 0.0f;
    public float CurrentValue = 0.0f;
    public float TargetValue = 0.0f;
    private void LateUpdate()
    {
        if (Mathf.Abs(CurrentValue - TargetValue) > 0.01f)
        {
            var lerpValue = Mathf.Clamp(Mathf.Lerp(CurrentValue, TargetValue, Time.deltaTime * 10.0f), 0, MaxValue);
            CurrentValue = lerpValue;
            float percent = lerpValue / MaxValue;
            FillAmount.fillAmount = percent;
        }

        if (DelayedAmount != null)
        {
            float currentFill = DelayedAmount.fillAmount;
            float targetFill = MaxValue > 0 ? CurrentValue / MaxValue : 0f;
            float delayedLerp = Mathf.Lerp(currentFill, targetFill, Time.deltaTime * 4.5f);
            DelayedAmount.fillAmount = delayedLerp;
        }
    }
}
