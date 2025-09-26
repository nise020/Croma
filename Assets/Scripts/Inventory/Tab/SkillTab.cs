using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Enums;
using static LanguageTable;
using static UnityEngine.Rendering.DebugUI;

public class SkillTab : MonoBehaviour
{
    [Header("Ref")]
    private Player player;
    [ReadOnly] List<SKILL_ID_TYPE> SkillType = new List<SKILL_ID_TYPE>() 
    {
         SKILL_ID_TYPE.Shot_Type_1,//0
         SKILL_ID_TYPE.Dash_Type_1,//1
         SKILL_ID_TYPE.Area_Type_1,//2
         SKILL_ID_TYPE.Auto_Type_1,//2
         SKILL_ID_TYPE.Burst_Type_1,//2
    };

    public List<SkillSlot> BeforSkillType = new List<SkillSlot>();

    class SkillSlotinfo 
    {
       public List<SkillSlot> skillSlots = new List<SkillSlot>();
    }

    private Dictionary <SkillSlot, SkillData> slotData = new Dictionary<SkillSlot, SkillData> ();
    private Dictionary <SkillKeyType, SkillSlotinfo> slotInfoData = new Dictionary<SkillKeyType, SkillSlotinfo> (); 


    //SkillSlot
    [SerializeField] private List<SkillSlot> skillSlots;
    [SerializeField] private List<GameObject> skills;
    [SerializeField] private TMP_Text PointText;

    int skillPointValue = 0;
    public bool isdescriping = false;

    [SerializeField] private Image skillImage;     // 아이콘
    [SerializeField] private TMP_Text skillNameTxt;  // 스킬 이름
    [SerializeField] private TMP_Text skillDecTxt;   // 설명
    [SerializeField] private TMP_Text salePointTxt;  // 필요 포인트

    [SerializeField] private SkillActionButton learnBtn;
    [SerializeField] private SkillActionButton settingBtn;
     
    [SerializeField] private RectTransform skillSelectObj;
    private SkillSlot selcetSlot;
    private SkillSlot buySlot;
    private SkillData nowselectskillData;
    private PlayerStateBar stateBar;

    private bool HasUnspent() => skillPointValue > 0;
    private ReminderTab R => GameShard.Instance?.GameUiManager?.ReminderTab;

    void Awake()
    {
        if (learnBtn) 
            learnBtn.Clicked += SkillUnlock;   // 그대로 사용

        if (settingBtn) 
            settingBtn.Clicked += SkillSetting;  // 그대로 사용

        Shared.Instance.LanguageManager.LanguageChangeEvent += TaxtChange;
    }

    public void ApplyPlayer(Player _player)
    {
        player = _player;
        player.SkillApply(SkillType);
    }

    public void Initialize(Player player)
    {
        ApplyPlayer(player);

        PointText.text = skillPointValue.ToString();

        if (R != null)
        {
            R.ShowSkill(skillPointValue, HasUnspent());
            R.UpdateSkill(HasUnspent(), skillPointValue);
        }

        stateBar = GameShard.Instance.GameUiManager.PlayerStateBar;
        if (stateBar != null)
        {
            for (int i = 0; i < SkillType.Count; i++)
            {
                var type = Skillkey(i);
                stateBar.SkillIconChange(SkillType[i], type);
            }
        }

        for (int i = 0; i < skills.Count; i++)
        {
            SkillSlot[] slots = skills[i].GetComponentsInChildren<SkillSlot>();

            skillSlots.AddRange(slots);
            SkillKeyType type = Skillkey(i);

            SkillSlotinfo slots2 = new(); 

            foreach (SkillSlot slot in slots)
            {
                SkillData skill = slot.Initialize(this, type, SkillType); 
                slotData.Add(slot, skill);
                slots2.skillSlots.Add(slot);
            }
            slotInfoData.Add(type, slots2);
        }

        settingBtn?.SetInteractable(false);
        learnBtn?.SetInteractable(false);
    }

    public void SkillPointAdd(int _value)
    {
        skillPointValue += _value;
        PointText.text = skillPointValue.ToString();

        if (R != null)
        {
            R.UpdateSkill(HasUnspent(), skillPointValue);
        }       
    }

    public SkillKeyType Skillkey(int _value) 
    {
        switch (_value)
        {
            case 0: return SkillKeyType.Click;
            case 1: return SkillKeyType.Space;
            case 2: return SkillKeyType.Q;
            case 3: return SkillKeyType.E;
            case 4: return SkillKeyType.R;
            default: return SkillKeyType.None;
        }
    }

    public void SkillSetting() //Button
    {
        if (selcetSlot == null || !selcetSlot.isLock) return; // 배운 칸만 허용
        selcetSlot.SkillSelect();

        selcetSlot = null;

        // skillSelectObj가 연결 안 돼 있어도 NRE 안 나게
        if (skillSelectObj != null) skillSelectObj.gameObject.SetActive(false);
    }

    public void ControllCancle() //Button
    {
        skillSelectObj.gameObject.SetActive(false);
    }

    public void SkillUnlock() //Button
    {
        if (buySlot == null)
        {
            return;
        }

        if (slotInfoData.TryGetValue(buySlot.key, out SkillSlotinfo info))
        {
            //버그 수정: 동일 라인의 이전 단계 확인은 info.skillSlots로 검사
            for (int i = 1; i < info.skillSlots.Count; i++)
            {
                if (info.skillSlots[i] == buySlot && !info.skillSlots[i - 1].isLock)
                {
                    Debug.LogWarning("이전 단계가 잠금 상태입니다.");
                    return;
                }
            }

            int prev = skillPointValue;
            skillPointValue = buySlot.SkillBuy(skillPointValue);
            bool purchased = (skillPointValue != prev);
          
            PointText.text = skillPointValue.ToString();
            if (R != null)
                R.UpdateSkill(HasUnspent(), skillPointValue);

            if (purchased)
            {
                settingBtn.SetInteractable(purchased);
                learnBtn.SetInteractable(!purchased);
            }

            buySlot = null;
        }       
    }

    //public void SkillSelectTabOn(PointerEventData eventData, SkillSlot _slot) //Butten
    //{
    //    selcetSlot = _slot;
    //    RectTransform skillTabRect = GetComponent<RectTransform>();
    //    Canvas canvas = GetComponentInParent<Canvas>();

    //    skillSelectObj.transform.SetAsLastSibling();

    //    RectTransformUtility.ScreenPointToLocalPointInRectangle(skillTabRect, eventData.position,
    //        canvas.worldCamera, out Vector2 localPoint
    //    );

    //    localPoint.x += 140;
    //    localPoint.y -= 90;

    //    skillSelectObj.anchoredPosition = localPoint;
    //    skillSelectObj.gameObject.SetActive(true);


    //}
    public void SkillChange(SKILL_ID_TYPE _skill, SkillKeyType _type,SkillSlot _slot) 
    {
        int number = GetSkillType(_type);

        SkillType[number] = _skill;
        player.SkillChange(number, _skill);
        stateBar.SkillIconChange(SkillType[number], _type);

        BeforSkillType[number].Skillclear();      
        BeforSkillType[number] = _slot;
    }

    private int GetSkillType(SkillKeyType type) 
    {
        switch (type) 
        {
            case SkillKeyType.Click: return 0;
            case SkillKeyType.Space: return 1;
            case SkillKeyType.Q: return 2;
            case SkillKeyType.E: return 3;
            case SkillKeyType.R: return 4;
            default:  return 10;
        }
    }

    private bool PreviousStepUnlocked(SkillSlot slot)
    {
        if (!slotInfoData.TryGetValue(slot.key, out var info)) return false;

        int idx = info.skillSlots.IndexOf(slot);
        if (idx <= 0)
            return true;

        return info.skillSlots[idx - 1].isLock;
    }

    private bool CanLearn(SkillSlot slot, SkillData data)
    {
        if (slot == null || data == null) return false;
        if (slot.isLock) return false;
        if (skillPointValue < data.salePoint) return false;
        return PreviousStepUnlocked(slot);
    }

    public void SkillDescriptionOn(SkillData data, SkillSlot slot, bool learned)
    {
        if (slot == null || data == null) return;

        buySlot = slot;
        selcetSlot = slot;

        skillImage.sprite = Shared.Instance.AtlasManager.Get(CONFIG_ATLAS_TYPE.Skill, data.iconPath); ;
        nowselectskillData = data;
        TaxtChange();

        salePointTxt.text = $"Need Point : {data.salePoint}";
        skillDecTxt.gameObject.SetActive(true);

        settingBtn?.SetInteractable(learned);
        learnBtn?.SetInteractable(CanLearn(slot, data));
    }

    public void TaxtChange()
    {
        var table1 = Shared.Instance.DataManager.Language_Table.Get(nowselectskillData.skillName);

        var table2 = Shared.Instance.DataManager.Language_Table.Get(nowselectskillData.desc);
        if (Shared.Instance.LanguageManager.lANGUEGE_TYPE == LANGUEGE_TYPE.Ko)
        {
            skillNameTxt.text = table1.Ko;
            skillDecTxt.text = table2.Ko;
        }
        else if (Shared.Instance.LanguageManager.lANGUEGE_TYPE == LANGUEGE_TYPE.En)
        {
            skillNameTxt.text = table1.En;
            skillDecTxt.text = table2.En;
        }
    }
    public void SkillDescriptionOff()
    {
        skillImage.sprite = null;
        skillDecTxt.text = "";
        salePointTxt.text = "Need Point : 0";
        settingBtn?.SetInteractable(false);
        learnBtn?.SetInteractable(false);
    }

    private void UpdateDetailUI(bool learned, SkillData data, Sprite icon)
    {
        if (icon != null)
            skillImage.sprite = icon;
    }
}
