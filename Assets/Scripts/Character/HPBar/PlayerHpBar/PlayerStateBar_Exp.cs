using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public partial class PlayerStateBar : StateBar
{
    int Level = 1;
    int LimitLevel = 0;

    [SerializeField] TextMeshProUGUI level_Text;
    public void LimitLevelCheck(int _id) 
    {
        var data = Shared.Instance.DataManager.Stage_Table.Get(_id);
        LimitLevel = data.LimitLevel;
    }

    public void TotalExpUpdate(int _maxValue) 
    {
        expToLevelUp =_maxValue;
        maxExpText.text = expToLevelUp.ToString();
        expText.text = currentExp.ToString();
    }

    private void LevelUpdate()
    {
        level_Text.text = Level.ToString();
    }

    public void SetExp(float _curExp)
    {
        if (LimitLevel <= Level) return;

        float startExp = currentExp;
        currentExp += _curExp;

        bool leveledUp = false;

        while (currentExp >= expToLevelUp)
        {
            if (LimitLevel <= Level) break;

            currentExp -= expToLevelUp;
            Level += 1;
            LevelUpEvent?.Invoke(Level);
            leveledUp = true;
            LevelUpdate();

            expToLevelUp *= expIncreaseRate;

            if (currentExp < expToLevelUp)
                break;
        }

        float targetFill = currentExp / expToLevelUp;

        expText.text = currentExp.ToString();

        if (leveledUp)
            StartCoroutine(SetExpEvent(true, targetFill)); 
        else
            StartCoroutine(SetExpEvent(false, targetFill)); 
    }

    private IEnumerator SetExpEvent(bool leveledUp, float targetFill)
    {
        if (leveledUp)
        {

            float elapsed = 0f;
            float start = expImage.fillAmount;

            while (elapsed < ExpeffectTime)
            {
                elapsed += Time.deltaTime;
                expImage.fillAmount = Mathf.Lerp(start, 1f, elapsed / ExpeffectTime);
                yield return null;
            }

            elapsed = 0f;
            while (elapsed < ExpeffectTime)
            {
                elapsed += Time.deltaTime;
                expImage.fillAmount = Mathf.Lerp(1f, 0f, elapsed / ExpeffectTime);
                yield return null;
            }

            elapsed = 0f;
            while (elapsed < ExpeffectTime)
            {
                elapsed += Time.deltaTime;
                expImage.fillAmount = Mathf.Lerp(0f, targetFill, elapsed / ExpeffectTime);
                yield return null;
            }

            expImage.fillAmount = targetFill;
        }
        else
        {
            float elapsed = 0f;
            float start = expImage.fillAmount;

            while (elapsed < ExpeffectTime)
            {
                elapsed += Time.deltaTime;
                expImage.fillAmount = Mathf.Lerp(start, targetFill, elapsed / ExpeffectTime);
                yield return null;
            }

            expImage.fillAmount = targetFill;
        }
    }

    public int GetLevel()
    {
        return Level;
    }

}
