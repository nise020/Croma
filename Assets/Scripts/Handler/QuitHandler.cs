using UnityEngine;
using static Enums;
public class QuitHandler : MonoBehaviour
{
    public void Quit()
    {
        Application.Quit();
    }
    public void Return() => Shared.Instance.UIManager.ActivePopup(false, UI_TITLE_POPUP_LIST.None);
}
