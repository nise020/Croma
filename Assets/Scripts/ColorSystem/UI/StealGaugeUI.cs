using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StealGaugeUI : MonoBehaviour
{
    [SerializeField] private Image gaugeImage;

    private const float maxDuration = 5;

    private Coroutine stealRoutine;
    private Coroutine cooldownRoutine;



    public void StartSteal()
    {
        if (stealRoutine != null)
            StopCoroutine(stealRoutine);

        stealRoutine = StartCoroutine(StealRoutine());
    }

    public void EndSteal(float cooldown)
    {
        if (stealRoutine != null)
            StopCoroutine(stealRoutine);
        
        if (cooldownRoutine != null)
            StopCoroutine(cooldownRoutine);

        cooldownRoutine = StartCoroutine(CooldownRoutine(cooldown));
    }

    

    private IEnumerator StealRoutine()
    {
        float t = maxDuration;

        while (t > 0)
        {
            t -= Time.unscaledDeltaTime;
            gaugeImage.fillAmount = Mathf.Clamp01(t / maxDuration);
            yield return null;
        }
        gaugeImage.fillAmount = 0f;
    }

    private IEnumerator CooldownRoutine(float cooldown)
    {
        float startFill = gaugeImage.fillAmount;
        float t = 0f;

        while (t < cooldown)
        {
            t += Time.unscaledDeltaTime;
            float percent = t / cooldown;
            gaugeImage.fillAmount = Mathf.Clamp01(startFill + (1f - startFill) * percent);
            yield return null;
        }

        gaugeImage.fillAmount = 1f;
    }


}
