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
        for (int i = 0; i < toggles.Length; i++)
        {
            int index = i;

            //toggles[index].onValueChanged.AddListener((isOn) =>
            toggles[index].onValueChanged.AddListener((isOn) =>
            {
                if (isOn)
                {
                    PlayUI(toggleSoundID);

                    if (statTab != null && currentSubUI == statTab.gameObject && panels[index] != statTab.gameObject)
                    {
                        bool canLeave = statTab.BeforeLeave(() =>
                        {
                            toggles[index].SetIsOnWithoutNotify(true);
                            toggles[lastTabIndex].SetIsOnWithoutNotify(false);
                            ShowOnlyPanel(index);
                        });

                        if (!canLeave)
                        {
                            toggles[lastTabIndex].SetIsOnWithoutNotify(true);
                            toggles[index].SetIsOnWithoutNotify(false);
                            return;
                        }
                    }

                    ShowOnlyPanel(index);
                }
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