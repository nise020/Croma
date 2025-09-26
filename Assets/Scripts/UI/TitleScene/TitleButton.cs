using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using static Enums;

public class TitleButton : MonoBehaviour, IPointerEnterHandler, IPointerUpHandler, IPointerDownHandler
{
    public UI_BUTTON_STATE ButtonState {  get; set; }

    private TMP_Text Text { get; set; }

    readonly public Color defaultColor = Color.white;
    readonly public Color selectedColor = new Color(76, 214, 254, 255) / 255;
    readonly public Color pressedColor = new Color(60, 144, 169, 255) / 255;

    public TitleHandler TitleHandler { get; set; }
    public int Index { get; set; }
    [SerializeField] private UnityEvent onClick;
    public void Init()
    {
        Text = transform.GetChild(0).GetComponent<TMP_Text>();
        TitleHandler = transform.parent.parent.parent.GetComponent<TitleHandler>();
        if (TitleHandler == null) Debug.LogError($"{(nameof(TitleHandler))} is Null");
    }
    public void OnPointerEnter(PointerEventData _EventData) => TitleHandler.ButtonIdx = Index;
    public void SetDefaultColor()
    {
        if (Text.font != TitleHandler.fontAssets[0]) Text.font = TitleHandler.fontAssets[0];
        Text.color = defaultColor;
        ButtonState = UI_BUTTON_STATE.Default;
    }
    public void SetSelectedColor()
    {
        if (Text.font != TitleHandler.fontAssets[0]) Text.font = TitleHandler.fontAssets[0];
        Text.color = selectedColor;
        ButtonState = UI_BUTTON_STATE.Selected;
    }
    public void SetPressedColor()
    {
        if (Text.font != TitleHandler.fontAssets[1]) Text.font = TitleHandler.fontAssets[1];
        Text.color = pressedColor;
        ButtonState = UI_BUTTON_STATE.Pressed;
    }
    public void OnPointerDown(PointerEventData _EventData)
    {
        if (TitleHandler.PressedKey == KeyCode.None) TitleHandler.SetPressedKey(KeyCode.Mouse0);
        SetPressedColor();
        TitleHandler.SetSelectedColor(true);
    }
    public void OnPointerUp(PointerEventData _EventData)
    {
        TitleHandler.SetPressedKey(KeyCode.None);
        if (TitleHandler.ButtonIdx == Index) SetSelectedColor();
        else SetDefaultColor();
        TitleHandler.SetSelectedColor(false);
        onClick.Invoke();
    }
}
