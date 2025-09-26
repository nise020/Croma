using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static LanguageTable;

public class TextPachi : MonoBehaviour
{

    public enum TOOLTIP_TYPE : int
    {
        None,
        Move_Controll = 301,
        Dash_Controll = 302,
        Skill_Controll = 303,
        Burst_Controll = 304,
        Mouse_UnLock = 305,
        QuickSlot = 306,
        Menu_Controll = 307,

        Option_Camera_Rotation = 111,
        Option_Bgm_Sound = 112,
        Option_Effect_Sound = 113,
        Option_Language = 114,

        Title_NewGame = 151,
        Title_Rangking = 152,
        Title_Option = 153,
        Title_Quit = 154,



    }
    [SerializeField] TOOLTIP_TYPE type;
    TMP_Text toolTipTmp;

    Text tooltipText;

    void Start()
    {
        toolTipTmp = GetComponentInChildren<TMP_Text>();
        if (toolTipTmp == null) 
        {
            tooltipText = GetComponentInChildren<Text>();
        }

        LanguageLoad().Forget();
    }
    public async UniTask LanguageLoad() 
    {
        await UniTask.WaitUntil(() => Shared.Instance != null && Shared.Instance.DataManager != null);

        TaxtChange();

        Shared.Instance.LanguageManager.LanguageChangeEvent += TaxtChange;
    }
    public void TaxtChange()
    {
        var table1 = Shared.Instance.DataManager.Language_Table.Get((int)type);
        if (Shared.Instance.LanguageManager.lANGUEGE_TYPE == LANGUEGE_TYPE.Ko)
        {
            if (toolTipTmp != null) 
            {
                toolTipTmp.text = table1.Ko;
            }
            else if (tooltipText != null) 
            {
                tooltipText.text = table1.Ko;
            }

        }
        else if (Shared.Instance.LanguageManager.lANGUEGE_TYPE == LANGUEGE_TYPE.En)
        {
            if (toolTipTmp != null)
            {
                toolTipTmp.text = table1.En;
            }
            else if (tooltipText != null)
            {
                tooltipText.text = table1.En;
            }

        }
    }


}
