using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

using static Enums.UI_BUTTON_STATE;

public class TitleHandler : MonoBehaviour, ISceneInitializable
{
    [SerializeField] private RectTransform selectedImg = null;
    private Color targetSelectedColor = new Color(150, 150, 150, 255) / 255;
    private Image SelectedImage { get; set; }

    private int _buttonIdx = 0;

    public TMP_FontAsset[] fontAssets = null;

    private readonly List<TitleButton> titleButtons = new();

    [ReadOnly] public KeyCode PressedKey = KeyCode.None;
    public int ButtonIdx
    {
        get => _buttonIdx;
        set
        {
            if (_buttonIdx != value)
            {
                _buttonIdx = value;
                SetButton();
            }
        }
    }
    private void Update()
    {
        HandleNavigationInput();
        HandleSelectionInput();
    }
    private void HandleNavigationInput()
    {
        if (titleButtons.Count == 0)
        {
            Init();
            return;
        }
        var up = Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W);
        var down = Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S);
        if (PressedKey == KeyCode.None && up) ButtonIdx = ButtonIdx == 0 ? titleButtons.Count - 1 : ButtonIdx - 1;
        if (PressedKey == KeyCode.None && down) ButtonIdx = ButtonIdx == titleButtons.Count - 1 ? 0 : ButtonIdx + 1;
    }
    private void HandleSelectionInput()
    {
        if (titleButtons.Count == 0)
        {
            Debug.LogError($"titleButtons.Count = {titleButtons.Count}");
        }
        else 
        {
            if (PressedKey == KeyCode.None && titleButtons[ButtonIdx].ButtonState == Selected)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    SetPressedKey(KeyCode.Return);
                    titleButtons[ButtonIdx].OnPointerDown(null);
                }
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    SetPressedKey(KeyCode.Space);
                    titleButtons[ButtonIdx].OnPointerDown(null);
                }
            }

            if (titleButtons[ButtonIdx].ButtonState == Pressed)
            {
                if (PressedKey != KeyCode.None && Input.GetKeyUp(PressedKey)) titleButtons[ButtonIdx].OnPointerUp(null);
            }
            else SetPressedKey(KeyCode.None);
        }
        
    }
    public void SetPressedKey(KeyCode _Key) => PressedKey = _Key;


    public UniTask Init()
    {
        Debug.Log($"{nameof(TitleHandler)} Init() called.");
        SelectedImage = selectedImg.GetComponent<Image>();
        var child = transform.GetChild(0).GetChild(0);
        for (int i = 0; i < child.childCount; i++)
        {
            var childObj = child.GetChild(i);
            if (childObj.CompareTag("TitleScene_Button"))
            {
                var obj = childObj.GetComponent<TitleButton>();
                titleButtons.Add(obj);
                obj.Index = i;
                obj.Init();
            }
        }
        SetButton();
        return UniTask.CompletedTask;
    }
    private Vector2 GetRect(RectTransform _Rect)
    {

        Canvas.ForceUpdateCanvases();
        float width = _Rect.rect.width / 2 + 50;
        return new Vector2(_Rect.localPosition.x + width, _Rect.localPosition.y);
    }
    public void SetSelectedColor(bool _IsPressed) => SelectedImage.color = _IsPressed ? targetSelectedColor : Color.white;
    [FuncTag("LanguageRefresh")]
    private void SetButton()
    {
        foreach (var obj in titleButtons)
        {
            if (ButtonIdx != obj.Index) obj.SetDefaultColor();
            else
            {
                obj.SetSelectedColor();
                selectedImg.localPosition = GetRect(obj.GetComponent<RectTransform>());
            }
        }
    }
}
