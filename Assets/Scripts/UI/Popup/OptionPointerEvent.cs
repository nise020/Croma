using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionPointerEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,IPointerDownHandler
{
    [SerializeField] private Vector2 targetSize, defaultSIze;
    [SerializeField] private float lerpTime = 0.05f;
    [SerializeField] private bool isToggle = false;
    private RectTransform Rect { get; set; }
    private Toggle Toggle { get; set; }
    void Awake()
    {
        Rect = transform.GetChild(0).GetComponent<RectTransform>();
        Toggle = GetComponent<Toggle>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!isToggle) StartCoroutine(LerpSize(targetSize));
        else if (!Toggle.isOn) StartCoroutine(LerpSize(targetSize));
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isToggle) StartCoroutine(LerpSize(defaultSIze));
        else if (!Toggle.isOn) StartCoroutine(LerpSize(defaultSIze));
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isToggle) StartCoroutine(LerpSize(defaultSIze));
        else if (!Toggle.isOn) StartCoroutine(LerpSize(defaultSIze));
    }
    private IEnumerator LerpSize(Vector2 _TargetSize)
    {
        float elapsedTime = 0f;
        Vector2 startSize = Rect.sizeDelta;

        while (elapsedTime < lerpTime)
        {
            elapsedTime += Time.deltaTime;
            Rect.sizeDelta = Vector2.Lerp(startSize, _TargetSize, elapsedTime / lerpTime);
            yield return null;
        }
        Rect.sizeDelta = _TargetSize;
    }
}
