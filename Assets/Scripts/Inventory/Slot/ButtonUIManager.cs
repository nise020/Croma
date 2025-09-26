using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonUIManager : MonoBehaviour
{
    private float fadeDuration = 0.2f;

    [SerializeField] private List<ItemSlot> itemSlots;
    [SerializeField] private List<QuestSlot> questSlots;
    [SerializeField] private List<ItemMenuBtn> menuBtns;
    [SerializeField] private List<SkillActionButton> skillActionButtons;
    [SerializeField] private List<SkillSlot> skillSlots;
    [SerializeField] private List<OptionSlider> optionSliders;
    
    private SlotBase selectedSlot = null;


    private void Awake()
    {
        itemSlots.AddRange(GetComponentsInChildren<ItemSlot>(true));
        questSlots.AddRange(GetComponentsInChildren<QuestSlot>(true));
        menuBtns.AddRange(GetComponentsInChildren<ItemMenuBtn>(true));
        skillActionButtons.AddRange(GetComponentsInChildren<SkillActionButton>(true));
        skillSlots.AddRange(GetComponentsInChildren<SkillSlot>(true));
        optionSliders.AddRange(GetComponentsInChildren<OptionSlider>(true));
    }

    private void Start()
    {
        InitSlots(itemSlots);
        InitSlots(questSlots);
        InitSlots(menuBtns);
        InitSlots(skillActionButtons);
        InitSlots(skillSlots);
        InitSlots(optionSliders);
    }

    private void InitSlots<T>(List<T> slotList) where T : SlotBase
    {
        for (int i = 0; i < slotList.Count; i++)
        {
            var btn = slotList[i];
            btn.buttonManager = this;
            btn.isSelected = false;
            SetAlpha(btn, 0f); // Initial: hide highlight
        }
    }

    public void InitSlot(SlotBase slot)
    {
        if (slot == null)
        {
            Debug.Log("[ButtonUIManager] Save/Reset slot is null");
            return;
        }

        slot.buttonManager = this;
        slot.isSelected = false;
        SetAlpha(slot, 0f);
    }

    public void HandleSelection(SlotBase slot)
    {
        if (selectedSlot != null && selectedSlot != slot)
        {
            selectedSlot.isSelected = false;
            StartFade(selectedSlot, 1f, 0f); // Deselected ¡æ fade out
        }

        selectedSlot = slot;
        slot.isSelected = true;
        StartFade(slot, 0f, 1f); // Selected ¡æ fade in
    }

    public void StartFade(SlotBase slot, float from, float to)
    {
        if (slot.fadeCoroutine != null)
            StopCoroutine(slot.fadeCoroutine);
        slot.fadeCoroutine = StartCoroutine(FadeCoroutine(slot, from, to));
    }

    private IEnumerator FadeCoroutine(SlotBase slot, float start, float end)
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(start, end, elapsed / fadeDuration);
            SetAlpha(slot, alpha);
            yield return null;
        }
        SetAlpha(slot, end);
        slot.fadeCoroutine = null;
    }

    private void SetAlpha(SlotBase slot, float alpha)
    {
        if (!slot) return;
        for (int i = 0; i < slot.selectImages.Count; i++)
        {
            if (slot.selectImages[i] == null)
            {
                Debug.LogWarning($"[SetAlpha] Null image found in {slot.name} / index {i}");
                continue;
            }

            Color c = slot.selectImages[i].color;
            c.a = alpha;
            slot.selectImages[i].color = c;
        }
    }

    public void HideButton(SlotBase slot)
    {
        if (!slot) return;

        var imgs = slot.selectImages;
        for (int i = 0; i < imgs.Count; i++ )
        {
            if (!imgs[i]) continue;
            var c = imgs[i].color;

            c.a = 0f;
            imgs[i].color = c;
        }
        slot.isSelected = false;
    }
}