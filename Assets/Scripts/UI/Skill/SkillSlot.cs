using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;
using static Enums;

public enum SkillKeyType 
{
    None = 10,
    Click = 0,
    Space = 1,
    Q = 2,
    E = 3,
    R = 4,
}

public class SkillSlot : SlotBase
{
    [SerializeField] private SKILL_ID_TYPE skillId;   
    private SkillTab skilltab;
    private int sale = 0;
    private RectTransform checkObj;
    private SkillData skillData;

    public SkillKeyType key;
    public bool isLock = false;
    public Sprite Sprite;

    protected override void Awake()
    {
        base.Awake();
    }

    public SkillData Initialize(SkillTab _skilltab, SkillKeyType _key, List<SKILL_ID_TYPE>_list)
    {
        skilltab = _skilltab;
        key = _key;

        skillData = Shared.Instance.DataManager.Skill_Table.Get((int)skillId);

        Sprite = Shared.Instance.AtlasManager.Get(CONFIG_ATLAS_TYPE.Skill,skillData.iconPath);
        Image image = GetComponent<Image>();
        image.sprite = Sprite;
        /*for (int i = 0; i < selectImages.Count; i++)
        {
            selectImages[i].sprite = Sprite;
        }*/

        sale = skillData.salePoint;

        checkObj = (RectTransform)gameObject.transform.GetChild(0);

        for (int i = 0; i < _list.Count; i++) 
        {
            if (_list[i] == skillId) 
            {
                isLock = true;
                skilltab.BeforSkillType.Add(this);
                SkillSelect();
                if (skillId == SKILL_ID_TYPE.Shot_Type_1) 
                {
                    skilltab.SkillDescriptionOn(skillData, this, isLock);//skill view
                }
                break;
            }
        }

        return skillData;
    }

    public override void OnPointerClick(PointerEventData eventData) 
    {
        base.OnPointerClick(eventData);
        buttonManager.HandleSelection(this);

        if(skillId == SKILL_ID_TYPE.None) return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            skilltab.SkillDescriptionOn(skillData,this, isLock);//skill view
        }
    }

    public void SkillSelect() 
    {
        skilltab.SkillChange(skillId, key,this);
        checkObj.gameObject.SetActive(true);
    }

    public void Skillclear() 
    {
        checkObj.gameObject.SetActive(false);
    }

    public int SkillBuy(int value) 
    {
        if (isLock)
        {
            Debug.LogError($"[Skill Slot] Already learned {skillId}");
            return value;
        }

        int point = value - sale;
        if (point >= 0)
        {
            isLock = true;
            return value - sale;
        }

        else 
        {
            Debug.LogError($"{value}-{sale} =={value - sale}");
            return value;
        }
        
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (!isSelected)
            buttonManager?.StartFade(this, 0f, 1f);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (!isSelected)
            buttonManager?.StartFade(this, 1f, 0f);
    }

}
