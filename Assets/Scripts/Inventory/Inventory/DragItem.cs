using UnityEngine;
using UnityEngine.UI;

public class DragItem : MonoBehaviour
{
    [SerializeField] private Image iconImage;

    private RectTransform rectTransform;
    private Canvas canvas;

    private void Awake()
    {
        iconImage = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        Hide(); 
    }

    // Show Icon
    public void Show(Sprite icon)
    {
        iconImage.sprite = icon;
        iconImage.enabled = true;
        gameObject.SetActive(true);
    }

    // Hide Icon
    public void Hide()
    {
        iconImage.enabled = false;
        gameObject.SetActive(false);
    }

    // Dragging position update
    public void Follow(Vector2 screenPosition)
    {
        if (!gameObject.activeSelf) return;
        rectTransform.position = screenPosition;
    }
}
