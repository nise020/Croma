using TMPro;
using UnityEngine;

public class GameInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI bossTimerLabel;
    [SerializeField] private TextMeshProUGUI bossTimerText;
    [SerializeField] private TextMeshProUGUI scoreText;

    private int lastWhole = -1;


    private void Awake()
    {
        SetInfo();
    }

    private void OnDisable()
    {
        var sm = GameShard.Instance?.StageManager;
        if (sm != null)
        {
            sm.OnStageRemainTimeUpdate -= HandleTimer;
            sm.OnBossRemainTimeUpdate -= HandleBossTimer;
            sm.OnBossSpawned -= HandleBossSpawned;
        }

        var gm = GameShard.Instance?.GameManager;
        if (gm != null) gm.OnScoreChanged -= HandleScore;
    }

    private void HandleTimer(float sec)
    {
        if (!timerText) return;

        int s = Mathf.FloorToInt(sec);
        if (s == lastWhole)
            return;

        lastWhole = s;
        int m = s / 60;
        int r = s % 60;

        timerText.text = $"{m:00} : {r:00}";
    }

    private void HandleScore(int score)
    {
        if (scoreText) scoreText.text = $"Score : {score}";
    }

    private void SetInfo()
    {
        var sm = GameShard.Instance.StageManager;
        if (sm != null)
        {
            sm.OnStageRemainTimeUpdate += HandleTimer;
            sm.OnBossRemainTimeUpdate += HandleBossTimer;
            sm.OnBossSpawned += HandleBossSpawned;

            if (timerText) timerText.text = "00:00";
            if (bossTimerText) bossTimerText.text = "";
        }

        else
        {
            Debug.Log("[GameInfo] StageManager is null");
        }

        if (GameShard.Instance.GameManager != null)
        {
            GameShard.Instance.GameManager.OnScoreChanged += HandleScore;
            if (scoreText) scoreText.text = "Score : 0";
        }
    }

    private void HandleBossTimer(int sec)
    {
        if (!bossTimerText) return;

        bossTimerText.text = sec.ToString(); // 초 그대로

        // 연출(선택): 임박 시 색 강조
        if (sec <= 5)
        {
            bossTimerLabel.fontMaterial.SetColor(ShaderUtilities.ID_FaceColor, Color.red);
            bossTimerText.fontMaterial.SetColor(ShaderUtilities.ID_FaceColor, Color.red);
        }
        else
        {
            bossTimerLabel.fontMaterial.SetColor(ShaderUtilities.ID_FaceColor, Color.white);
            bossTimerText.fontMaterial.SetColor(ShaderUtilities.ID_FaceColor, Color.white);
        }
    }

    private void HandleBossSpawned()
    {
        if (bossTimerText) bossTimerText.text = "0";
    }
}
