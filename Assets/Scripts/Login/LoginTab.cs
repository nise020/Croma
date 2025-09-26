using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;

public class LoginTab : MonoBehaviour
{
    [Header("DefoltView")]
    [SerializeField] GameObject DefoltObj;
    [SerializeField] Button CreatBut;
    [SerializeField] Button LoginViewBut;
    [SerializeField] GameObject ErrorUi;
    Coroutine errorEvent;

    [Header("LoginUi")]
    [SerializeField] GameObject LoginObj;
    [SerializeField] Text IdText;
    [SerializeField] Text PasswordText;

    [SerializeField] Button LoginBut;
    [SerializeField] GameObject LoginButObj;

    [SerializeField] Button creatBut;
    [SerializeField] GameObject creatButObj;
    //[SerializeField] Toggle AutoLogin;
    //IEnumerator Laoding;
    string IDKey;
    string UserPassKey;
    bool isLogin = false;
    private void Awake()
    {
        creatBut.onClick.AddListener(CreatPrecces);
        LoginBut.onClick.AddListener(LoginPrecces);

        LoginViewBut.onClick.AddListener(() => 
        {
            DefoltObj.SetActive(false);
            LoginObj.SetActive(true);
            LoginButObj.SetActive(true);
            creatButObj.SetActive(false);

        });

        CreatBut.onClick.AddListener(() => 
        {
            DefoltObj.SetActive(false);
            LoginObj.SetActive(true);
            creatButObj.SetActive(true);
            LoginButObj.SetActive(false);
            
        });

        ErrorUi.SetActive(false);
    }
    private void Start()
    {
        Server.instanse.loginTab = this;
    }
    public void AutoLoginCheck() 
    {
        //int value = PlayingDataLoad("AutoLogin");

        //if (value == 1)
        //{
        //    string id = Shared.Instance.PlyerLoginDataLoad("UserID");
        //    string pass = Shared.Instance.PlyerLoginDataLoad("UserPassward");

        //    if ((id != "" && id.Length > 0) &&
        //       (pass != "" && pass.Length > 0))
        //    {
        //        Shared.Instance.isPlay = true;
        //    }
        //    else 
        //    {
        //        if (errorEvent != null) StopCoroutine(errorEvent);
        //        errorEvent = StartCoroutine(TextEvent(ErrorUi));
        //        ErrorUi.SetActive(true);
        //        return;
        //    }
        //}
        //else
        //{
        //    return;
        //}
    }
    public IEnumerator TextEvent(GameObject _ui)
    {
        _ui.SetActive(true);
        TMP_Text text = _ui.GetComponent<TMP_Text>();
        Color color = text.color;
        color.a = 255;

        while (color.a <= 0)
        {
            color.a -= 0.1f * Time.deltaTime;
            yield return null;
        }

        _ui.SetActive(false);
    }

    public void ToggleEvent() 
    {
        isLogin = !isLogin;
        if (isLogin)
        {
            PlayingHistorygSave("AutoLogin",1).Forget();
        }
        else 
        {
            PlayingHistorygSave("AutoLogin",0).Forget();
        }
    }
    public int PlayingDataLoad(string _Key)
    {
        return PlayerPrefs.GetInt(_Key);
    }
    public async UniTask PlayingHistorygSave(string _Key, int _Value)
    {
        PlayerPrefs.SetInt(_Key, _Value);
        PlayerPrefs.Save();
        await UniTask.CompletedTask;
    }
    public void CreatPrecces()
    {
        if ((IdText.text == "" || IdText.text.Length <= 0) &&
            (PasswordText.text == "" || PasswordText.text.Length <= 0)) return;

        CreatPassword().Forget();
    }
    public void LoginPrecces()
    {
        //if ((IdText.text == "" || IdText.text.Length <= 0) &&
        //    (PasswordText.text == "" || PasswordText.text.Length <= 0)) return;

        LoginOn().Forget();
    }
    //public async UniTask CreatPassword()//계정 생성
    //{
    //    LoginBut.interactable = false;
    //    Debug.Log("1초 후 로비 화면으로 이동합니다");

    //    IDKey = IdText.text;
    //    UserPassKey = PasswordText.text;

    //    await Shared.Instance.PlyerLoginDataSave("UserID", IDKey);
    //    Debug.Log("저장 직후 = " + PlayerPrefs.GetString("UserID"));

    //    await Shared.Instance.PlyerLoginDataSave("UserPassward", UserPassKey);
    //    Debug.Log("저장 직후 = " + PlayerPrefs.GetString("UserPassward"));

    //    //Shared.SceneMgr.SaveFile(UserPassKey);
    //    //ActKey = UserPassKey;

    //    //yield return new WaitForSeconds(5);
    //    await UniTask.Delay(TimeSpan.FromSeconds(1.0f));
    //    //IDSave(IDKey, UserPassKey);
    //    Debug.Log($"UserPassKey={UserPassKey},IDKey={IDKey}");

    //    LoginBut.interactable = true;

    //    Shared.Instance.isPlay = true;

    //    await UniTask.CompletedTask;
    //}
    public async UniTask LoginOn() 
    {
        IDKey = IdText.text;
        UserPassKey = PasswordText.text;

        Server.instanse.LoginDBPost(IDKey, UserPassKey);

        await UniTask.Delay(TimeSpan.FromSeconds(1.0f));

        //Shared.Instance.isPlay = true;

        await UniTask.CompletedTask;
    }
    public async UniTask CreatPassword()//계정 생성
    {
        LoginBut.interactable = false;
        Debug.Log("1초 후 로비 화면으로 이동합니다");

        IDKey = IdText.text;
        UserPassKey = PasswordText.text;

        Server.instanse.CountCreat(IDKey,UserPassKey);

        
        await UniTask.Delay(TimeSpan.FromSeconds(1.0f));
        

        LoginBut.interactable = true;

        GamePlay();

        await UniTask.CompletedTask;
    }
    public void GamePlay() 
    {

        Shared.Instance.isPlay = true;
    }
}
