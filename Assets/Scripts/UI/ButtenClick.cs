using System.Collections;
using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class ButtenClick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public enum CLICK_TYPE 
    {
        None,
        Color,
        ButtenScale,
    }
    [SerializeField] CLICK_TYPE type;
    RectTransform rectTransform;
    Button Button;
    TMP_Text text;
    float scaleEventSpeed = 0.5f;
    Color textcolor;
    private Coroutine scaleCoroutine;
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        Button = GetComponent<Button>();
        text = GetComponentInChildren<TMP_Text>();
        textcolor = text.color;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        switch (type) 
        {
            case CLICK_TYPE.ButtenScale:
                if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);
                scaleCoroutine = StartCoroutine(ScaleEffect(true));
            break;
            case CLICK_TYPE.Color:
                TextColorEffect(true);
                break;
            default:
                return;
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        switch (type)
        {
            case CLICK_TYPE.ButtenScale:
                if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);
                scaleCoroutine = StartCoroutine(ScaleEffect(false));
                break;
            case CLICK_TYPE.Color:
                TextColorEffect(false);
                break;
            default:
                return;
        }
    }

    private void TextColorEffect(bool _check) 
    {
        if (_check) 
        {
            Color baseColor = text.color;
            Color darkerColor = baseColor * 0.7f;
            darkerColor.a = baseColor.a;
            text.color = darkerColor;
        }
        else 
        {
            text.color = textcolor;
        }
        
    }

    private IEnumerator ScaleEffect(bool _check) 
    {
        float minScale = 0.8f;
        float maxScale = 1.0f;
        if (_check)
        {
            while (rectTransform.localScale.x > minScale)
            {
                rectTransform.localScale -= Vector3.one * scaleEventSpeed * Time.deltaTime;
                yield return null;
            }
            rectTransform.localScale = Vector3.one * minScale;
        }
        else 
        {
            while (rectTransform.localScale.x <= maxScale)
            {
                rectTransform.localScale += Vector3.one * scaleEventSpeed * Time.deltaTime;
                yield return null;
            }

            rectTransform.localScale = Vector3.one * maxScale;
        }
    }

}
