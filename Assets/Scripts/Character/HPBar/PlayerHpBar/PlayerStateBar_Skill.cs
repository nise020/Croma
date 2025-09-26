using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Enums;

public partial class PlayerStateBar : StateBar
{
    Dictionary<SKILL_ID_TYPE, SkillData> SkillDatas = new Dictionary<SKILL_ID_TYPE, SkillData>();
    Dictionary<SkillKeyType, SkillData> SkillKeyDatas = new Dictionary<SkillKeyType, SkillData>();
    bool isBurst = false;
    public bool BurstSkillStat() 
    {
        if (Skill_4Image.fillAmount == 1)
        {
            return true;
        }
        else
        {
            return false;

        }
    }
    public void SkillIconChange(SKILL_ID_TYPE _id, SkillKeyType _key)
    {
       int keyValue = skillkey(_key);
        switch (keyValue) 
        {
            case 1: SkillSprite(Skill_1Image, _id); break; // space
            case 2: SkillSprite(Skill_2Image, _id); break; // Q
            case 3: SkillSprite(Skill_3Image, _id); break; // E
            case 4: SkillSprite(Skill_4Image, _id); break; // R
            default:  break;
        }
    }
    public void SkillSprite(Image _image, SKILL_ID_TYPE _id) 
    {
        if (SkillDatas.TryGetValue(_id, out SkillData skill)) 
        {
            var Sprite = Shared.Instance.AtlasManager.Get(CONFIG_ATLAS_TYPE.Skill, skill.iconPath);
            _image.sprite = Sprite;

            /*Transform trans = _image.transform.GetChild(0);
            Image child = trans.GetComponentInChildren<Image>();
            child.sprite = Sprite;*/
        }
    }
    public int skillkey(SkillKeyType _key)
    {
        switch (_key)
        {
            case SkillKeyType.Click : return 0;
            case SkillKeyType.Space : return 1;
            case SkillKeyType.Q : return 2;
            case SkillKeyType.E : return 3;
            case SkillKeyType.R : return 4;
            default: return 0;
        }
    }
    public void SkillActive(int _value, SKILL_ID_TYPE _type)
    {
        if (SkillDatas.TryGetValue(_type, out SkillData data)) 
        {
            switch (_value)
            {
                case 1://skill2
                    if (Skill_1Image.fillAmount != 1) return; ;
                    StartCoroutine(Skill_1CoolEvent(data.time));
                    break;
                case 2://skill3
                    if (Skill_2Image.fillAmount != 1) return; ;
                    StartCoroutine(Skill_2CoolEvent(data.time));
                    break;
                case 3://skill3
                    if (Skill_3Image.fillAmount != 1) return; ;
                    StartCoroutine(Skill_3CoolEvent(data.time));
                    break;
                case 4://burst
                    if (Skill_4Image.fillAmount != 1) return;
                    StartCoroutine(BurstEvent(data.time)); ;
                    break;
                default://0 = no cool
                    Debug.Log($"_skill = null");
                    break;
            }
        }
    }
    private IEnumerator Skill_1CoolEvent(int _timer)
    {
        Skill_1Image.fillAmount = 0;
        float viewTimer = _timer;
        while (Skill_1Image.fillAmount < 1)
        {
            Skill_1Image.fillAmount += Time.deltaTime / _timer;
            viewTimer -= 1 * Time.deltaTime;

            Skill_1Text.text = Mathf.CeilToInt(viewTimer).ToString();
            yield return null;
        }
        Skill_1Image.fillAmount = 1;
        Skill_1Text.text = "";
    }
    private IEnumerator Skill_2CoolEvent(int _timer)
    {
        Skill_2Image.fillAmount = 0;
        float viewTimer = _timer;
        while (Skill_2Image.fillAmount < 1)
        {
            Skill_2Image.fillAmount += Time.deltaTime / _timer;
            viewTimer -= 1 * Time.deltaTime;

            Skill_2Text.text = Mathf.CeilToInt(viewTimer).ToString();
            yield return null;
        }
        Skill_2Image.fillAmount = 1;
        Skill_2Text.text = "";
    }
    private IEnumerator Skill_3CoolEvent(int _timer)
    {
        Skill_3Image.fillAmount = 0;
        float viewTimer = _timer;
        while (Skill_3Image.fillAmount < 1)
        {
            Skill_3Image.fillAmount += Time.deltaTime / _timer;
            viewTimer -= 1 * Time.deltaTime;

            Skill_3Text.text = Mathf.CeilToInt(viewTimer).ToString();
            yield return null;
        }
        Skill_3Image.fillAmount = 1;
        Skill_3Text.text = "";
    }
    private IEnumerator BurstEvent(int _timer)
    {
        float elapsed = 0;
        float viewTimer = _timer;
        isBurst = true;
        while (elapsed < 1.0f)
        {
            elapsed += Time.deltaTime;
            Skill_4Image.fillAmount = Mathf.Lerp(1f, 0f, elapsed / 1.0f);

            yield return null;
        }

        Skill_4Image.fillAmount = 0f; // 마지막 보정
        isBurst = false;

        while (viewTimer > 0f)
        {
            viewTimer -= 1 * Time.deltaTime;

            BurstText.text = Mathf.CeilToInt(viewTimer).ToString();
            yield return null;
        }
        BurstText.text = "";

        // burstCharging = null;
    }
    private async void SetBurst(float _curBurst)
    {
        if (Skill_4Image.fillAmount == 1) return;

        currentBurst = Mathf.Clamp(currentBurst + _curBurst, 0, 10);

        await UniTask.WaitUntil(() => !isBurst);

        if (burstCharging == null)
        {
            burstCharging = StartCoroutine(
                SetChargingEvent(currentBurst / 10f));
        }
        else
        {
            // 코루틴이 돌고 있으면 목표치만 갱신
            targetFill = currentBurst / 10f;
        }
    }

    private IEnumerator SetChargingEvent(float target)
    {
        targetFill = target;
        while (Skill_4Image.fillAmount < targetFill)
        {
            if (Skill_4Image.fillAmount >= 1f)
                break;

            Skill_4Image.fillAmount = Mathf.MoveTowards(
                Skill_4Image.fillAmount,
                targetFill,
                Time.deltaTime / HpeffectTime
            );
            yield return null;
        }

        burstCharging = null;
    }

}
