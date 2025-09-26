using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static Enums;

public class ColorSlotUI : MonoBehaviour
{
    [SerializeField] private Image[] slotImages;
    [SerializeField] private Image stealGaugeImage;

    Dictionary<Image, COLOR_TYPE> slotUiData = new Dictionary<Image, COLOR_TYPE>();
    private Color emptyColor => ColorPalette.emptyColor;
    
    private ColorSystem colorSystem;

    public void Init(ColorSystem system)
    {
        colorSystem = system;
    }
    public void ColorDataSave(List<COLOR_TYPE> slots) 
    {
        for (int i = 0; i < slotImages.Length; i++) 
        {
            slotUiData.Add(slotImages[i],slots[i]) ;
        }
    }
    public void UpdateSlot(List<COLOR_TYPE> slots)
    {
        for (int i = 0; i < slotImages.Length; i++)
        {
            if (i < slots.Count)
            {
                COLOR_TYPE color = slots[i];
                slotImages[i].color = SetSlotColor(color);

                if (colorSystem != null)
                {
                    int gauge = colorSystem.GetGauge(color);
                    slotImages[i].fillAmount = Mathf.Clamp01(gauge / 100f);
                }
            }
            else
            {
                slotImages[i].color = emptyColor;
                slotImages[i].fillAmount = 0f;
            }
        }
    }
    public COLOR_TYPE LoadSlotImage(out float _value) 
    {
        COLOR_TYPE type = slotUiData.GetValueOrDefault(slotImages[0]);
        _value = slotImages[0].fillAmount;

        return type;
    }
    public void UpdateGauge()
    {
      
    }

    public Color SetSlotColor(COLOR_TYPE type)
    {
        return ColorPalette.GetColor(type);
    }

    public bool ColorSlotState() 
    {
        for (int i = 0; i < slotImages.Length; i++)
        {
            if (slotImages[i].color == emptyColor)
            {
                return true;
            }
        }
        return false;
    }


    public void ColorLock(int timer) 
    {
        for(int i = 0;i < slotImages.Length;i++) 
        {
            Color color = slotImages[i].color;

            if (color != emptyColor) 
            {
                LockTimer(i, timer).Forget();
                break;
            }
        }
    }


    public async UniTask LockTimer(int value, int timer)//Test
    {
        Color color = slotImages[value].color;
        slotImages[0].color = emptyColor;

        await UniTask.Delay(TimeSpan.FromSeconds(timer));

        slotImages[0].color = color;
    }

}
