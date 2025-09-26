
using TMPro;
using UnityEngine;

public class BossStateBar : MonsterStateBar
{
    [SerializeField] TMP_Text nameText;
    protected override void Start()
    {
        base.Start();
        gameObject.SetActive(true);
    }

    public override void InitializeCharacter(Character_Base character)
    {
        BossMonster boss = character as BossMonster;
        string text = boss.PathData[Enums.CHARACTER_DATA.Name];
        if (text != "" && text != "0") 
        {
            nameText.text = text;
        }
        Canvas canvas = boss.gameObject.GetComponentInChildren<Canvas>();
        DamageBarObj = canvas.transform.GetChild(0).gameObject;

        //DamageSetting(DamageBarObj);
        base.InitializeCharacter(boss);        
    }
    protected override void LateUpdate() 
    {
        if (MainCamera != null)
        {
            DamageBarObj.transform.rotation = Quaternion.LookRotation
                (transform.position - MainCamera.transform.position, Vector3.up);
        }
        else
        {
            Initialize();
        }
    }
}
