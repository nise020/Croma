using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Enums;
using static Enums.SCENE_SCENES;
public class TitleButtonEvent : MonoBehaviour
{
    public void Play()
    {
        Shared.Instance.SceneShared.Play();
    }
    public void Option()
    {
        Debug.Log("Option");
        Shared.Instance.UIManager.ActivePopup(true, UI_TITLE_POPUP_LIST.Option);
    }

    public void Rangking()
    {
        Shared.Instance.SceneShared.RankingOn();
    }

    public void Quit()
    {
        Debug.Log("Quit");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
Application.Quit();
#endif
        //Shared.Instance.UIManager.ActivePopup(true, UI_TITLE_POPUP_LIST.Quit);
    }

    //public void LoadSave()
    //{
    //    Debug.Log("LoadSave");
    //}

}
