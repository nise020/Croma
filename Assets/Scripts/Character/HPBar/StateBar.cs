using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public partial class StateBar : MonoBehaviour
{
    protected Character_Base Character { get; set; } = null;
    [SerializeField] private Image hpImage = null;
    [SerializeField] private Image hpLateImage = null;
    
    protected float HpeffectTime = 2.0f;



    public Action<int> AttackDamageEvent { get; set; } = null;
    private Coroutine HPBarEvent { get; set; } = null;
    protected Camera MainCamera { get; set; } = null;
    protected virtual void Start()
    {
        InitializeImage();
    }
    protected virtual void Initialize()
    {
        MainCamera = Camera.main;
    }
    public virtual void InitializeCharacter(Character_Base character)
    {
        character.HpBarChanged += SetHP;
        Character = character;
        //Debug.Log($"[HpBar] Character 연결: {Character}");

        if (Character == null)
        {
            Debug.LogError("Character가 null입니다!");
            return;
        }

        Debug.Log("[HpBar] SetHP 이벤트 연결 완료");

    }
    public virtual void InitializeImage()
    {
        if (hpImage.fillAmount != 1) 
        {
            hpImage.fillAmount = 1.0f;
        }
        if( hpLateImage.fillAmount != 1) 
        {
            hpLateImage.fillAmount = 1.0f;
        }
    }

    public virtual void SetHP(float _MaxHP, float _CurHP)
    {
        Debug.Log("[HpBar] SetHP 호출 시작");

        if (!gameObject.activeSelf) return;
        hpImage.fillAmount = _CurHP / _MaxHP;

        if (_CurHP / _MaxHP > hpLateImage.fillAmount)
        {
            hpLateImage.fillAmount = hpImage.fillAmount; // 회복은 즉시 반영
        }
        else
        {
            if (HPBarEvent != null) StopCoroutine(HPBarEvent);
            HPBarEvent = StartCoroutine(setHpEvent());
        }

        HPBarEvent = StartCoroutine(setHpEvent());
    }

    private IEnumerator setHpEvent()
    {
        while (hpLateImage.fillAmount > hpImage.fillAmount)
        {
            hpLateImage.fillAmount -= Time.deltaTime / HpeffectTime;
            if (hpLateImage.fillAmount < hpImage.fillAmount) 
            {
                HPBarEvent = null;
                break;
            }
            yield return null;
        }
        //hpLateImage.fillAmount = hpImage.fillAmount;
        
    }


}