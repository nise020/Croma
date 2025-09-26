using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class AreaName : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private CanvasGroup group;
    [SerializeField] private float showTime = 2f;  // 화면에 머무는 시간
    [SerializeField] private float fade = 1f;  // 페이드 인/아웃 시간

    private void Awake()
    {
        if (!group) 
            group = GetComponent<CanvasGroup>();
        if (!label) 
            label = GetComponentInChildren<TextMeshProUGUI>(true);

        group.alpha = 0f;
        group.blocksRaycasts = false;

    }

    public async UniTask Play(string stageText, Action onDone)
    {
        label.text = stageText;
        gameObject.SetActive(true);
        await Fade(0f, 1f);                                     
        await UniTask.Delay(TimeSpan.FromSeconds(showTime));     
        await Fade(1f, 0f);                                   
        gameObject.SetActive(false);
        onDone?.Invoke();
    }

    private async UniTask Fade(float from, float to)
    {
        float t = 0f;
        float dur = Mathf.Max(0f, fade);
        var token = this.GetCancellationTokenOnDestroy();

        while (t < dur)
        {
            t += Time.deltaTime;

            if (group != null)
                group.alpha = Mathf.Lerp(from, to, t / dur);
            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }

        if (group != null)
            group.alpha = to; ;
    }
}