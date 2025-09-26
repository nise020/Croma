using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;

public class UILayoutRefresher : MonoBehaviour
{
    private HorizontalLayoutGroup topLevelRect = null;

    void OnEnable()
    {
        LocalizationSettings.SelectedLocaleChanged += OnLanguageChanged;
        if (topLevelRect == null) topLevelRect = GetComponent<HorizontalLayoutGroup>();
    }
    void OnDisable() => LocalizationSettings.SelectedLocaleChanged -= OnLanguageChanged;
    private void OnLanguageChanged(Locale newLocale) => StartCoroutine(RebuildNow());

    public IEnumerator RebuildNow()
    {
        topLevelRect.childForceExpandWidth = false;
        yield return new WaitForSeconds(0.5f);
        topLevelRect.childForceExpandWidth = true;
    }
}
