using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeEffect : MonoBehaviour
{
    public Image FadeImg;
    public Image LoadImg;
    //bool fade = false;
    [SerializeField] float fadeTime = 1.0f;
    Canvas Canvas;
    private void Start()
    {
        Canvas = GetComponentInParent<Canvas>();
        FadeImg = GetComponent<Image>();

        //LoadImg = Canvas.transform.GetChild(1).GetComponent<Image>();
        //LoadImg.fillAmount = 0;
        gameObject.SetActive(false);
    }
    public IEnumerator Loading(int _value) 
    {
        while(LoadImg.fillAmount == 100/_value) 
        {
            LoadImg.fillAmount += Time.deltaTime / _value;
        }
        yield return null;
    }
    public IEnumerator Fade(bool isFadeOut)
    {
        if (!gameObject.activeSelf) 
        {
            gameObject.SetActive(true);
        }
        float timer = 0f;
        Color color = FadeImg.color;
        float startAlpha = color.a;
        float endAlpha = isFadeOut ? 1f : 0f;

        FadeImg.raycastTarget = true;

        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            float t = timer / fadeTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, t);
            FadeImg.color = color;
            yield return null;
        }

        color.a = endAlpha;
        FadeImg.color = color;

        FadeImg.raycastTarget = (color.a != 0f);
        //gameObject.SetActive(false);
        if (isFadeOut == false)
        {
            gameObject.SetActive(false);
        }
    }
}
