using TMPro;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class RankEntry
{
    public int rank;
    public string id;
    public int score;
    public string name;
}


public class RankRowView : MonoBehaviour
{
    public TextMeshProUGUI rank_Text;
    public TextMeshProUGUI id_Text;
    public TextMeshProUGUI score_Text;

    public void Set(RankEntry entry)
    {
        if (rank_Text) rank_Text.text = entry.rank.ToString();
        if (id_Text) id_Text.text = entry.id;
        if (score_Text) score_Text.text = entry.score.ToString("N0"); // 20,000 Çü½Ä

    }

}
