using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class BossHpBar : MonoBehaviour
{
    public Image bossHpImage;
    public Image bossHpLateImage;

    private float bossHpTarget;
    private float lerpSpeed = 1.5f;

    private Coroutine hpRoutine;





    public void SetHP(float currentHp, float maxHp)
    {
        bossHpTarget = currentHp / maxHp;

        if (hpRoutine != null )
            StopCoroutine(hpRoutine);

        hpRoutine = StartCoroutine(UpdateHpBar());

    }


    private IEnumerator UpdateHpBar()
    {
        // 1. YellowBar
        while (bossHpImage.fillAmount > bossHpTarget)
        {
            bossHpImage.fillAmount = Mathf.MoveTowards(bossHpImage.fillAmount, bossHpTarget, Time.deltaTime * lerpSpeed);
            yield return null;
        }

        // 2. WhiteBar
        while (bossHpLateImage.fillAmount > bossHpTarget)
        {
            bossHpLateImage.fillAmount = Mathf.MoveTowards(bossHpLateImage.fillAmount, bossHpTarget, Time.deltaTime * lerpSpeed);
            yield return null;
        }

        hpRoutine = null;
    }
}
