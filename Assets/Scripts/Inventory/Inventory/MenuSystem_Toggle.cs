    using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class MenuSystem : MonoBehaviour
{
    [Header("Toggle UI")]
    [SerializeField] private GameObject[] panels;
    [SerializeField] private Toggle[] toggles;
    [SerializeField] public GameObject ToggleTab;

    private int toggleSoundID = 20011;

    private int lastTabIndex = 0;

    public void ToggelAdd()
    {
        if (panels == null || toggles == null || panels.Length != toggles.Length)
            Debug.LogWarning("[MenuSystem] panels/toggles 개수가 다릅니다.");

        for (int i = 0; i < toggles.Length; i++)
        {
            int index = i;
            toggles[index].onValueChanged.AddListener(isOn =>
            {
                if (!isOn) return;

                PlayUI(toggleSoundID);
                ShowOnlyPanel(index);
            });
        }

        ToggleTab.SetActive(false);
    }

    private void ShowOnlyPanel(int index)
    {
        for (int i = 0; i < panels.Length; i++)
        {
            bool active = (i == index);
            panels[i].SetActive(active);

            if (active)
            {
                currentSubUI = panels[i]; 
            }
        }

        lastTabIndex = index;

    }

}