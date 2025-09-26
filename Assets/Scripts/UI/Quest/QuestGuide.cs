/*using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestGuide : MonoBehaviour
{
    [SerializeField] Image backgroundImage;
    [SerializeField] Image markImage;
    [SerializeField] TextMeshProUGUI questTitle;

    [SerializeField] private List<Sprite> backgroundImages; // [0] : Main, [1] : Sub
    [SerializeField] private List<Sprite> markImages; // [0] : Main, [1] : Sub

    private QuestData quest;

    
    private void Init(QuestData quest)
    {
        if (quest.questType == eQUESTTYPE.MAINQUEST)
        {
            backgroundImage.sprite = backgroundImages[0];
            markImage.sprite = markImages[0];
        }
        else
        {
            backgroundImage.sprite = backgroundImages[1];
            markImage.sprite = markImages[1];
        }

        questTitle.text = quest.questName;
    }

}*/
