using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public partial class Server : MonoBehaviour
{
    public static Server instanse;
    //string Http = "http://58.78.211.182:3000/";//서버
    string Http = "http://58.78.211.182:3000/";//서버
    //3000<--통로
    //12/22 한정으로 사용 가능
    //https<--보안됨
    //string ConnectUrl = "process/dbconnect";

    string LoginUrl = "process/login";
    string CountCreatUrl = "process/logincreate";
    string UserId;
    string Username;
    string Userpw;

    //string DisConnectUrl = "process/dbdisconnect";
    //string UserSelectUrl = "process/userselect";//유저 데이터
    public LoginTab loginTab { get; set; }

    private void Awake()
    {
        if (instanse == null)
        {
            instanse = this;
            DontDestroyOnLoad(this);
        }
        else 
        {
            Destroy(this);
        }

    }
    public void OnBtnConnect() 
    {
        //StartCoroutine(DBPost(Http+ LoginUrl, "dev"));
    }
    public void CountCreat(string id, string passward)
    {
        StartCoroutine(CraetDBPost(Http + CountCreatUrl, id, passward));
    }
    public void LoginDBPost(string id, string passward)
    {
        //SendFormData
        StartCoroutine(LoginDBPost(Http + LoginUrl, id, passward));
        //StartCoroutine(SendFormData(Http + CountCreatUrl, id, passward));
    }
    public void MessageOn(JSONNode _json) 
    {
        if (_json.HasKey("db"))
        {
            string resultMessage = _json["db"];
            switch (resultMessage)
            {
                case "failure id":
                    Debug.LogWarning("존재하지 않는 ID입니다.");
                    break;
                case "failure password":
                    Debug.LogWarning("비밀번호가 올바르지 않습니다.");
                    break;
                case "login success":
                    Debug.Log("로그인 성공!");
                    break;
            }
        }
        else if (_json.HasKey("err"))
        {
            string errorMessage = _json["err"];
            switch (errorMessage)
            {
                case "not id":
                    Debug.LogWarning("ID를 입력하지 않았습니다.");
                    break;
                case "not id or not pw":
                    Debug.LogWarning("ID 또는 비밀번호를 입력하지 않았습니다.");
                    break;
            }
        }
        else
        {
            Debug.LogWarning("알 수 없는 서버 응답입니다.");
        }

    }
    IEnumerator LoginDBPost(string Url, string _id, string _passward)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", _id);
        form.AddField("pw", _passward);

        UnityWebRequest www = UnityWebRequest.Post(Url,form);
        bool loginSuccess = false;

        yield return www.SendWebRequest();

        Debug.Log(www.downloadHandler.text);

        if (www.result == UnityWebRequest.Result.Success) 
        {
            JSONNode node = JSONNode.Parse(www.downloadHandler.text);

            string response = www.downloadHandler.text;
            Debug.Log("서버 응답: " + response);

            if (node.HasKey("db") || node.HasKey("err")) 
            {
                MessageOn(node);
                yield break;
            }
            
            for (int i = 0; i < node.Count; i++)
            {
                string dbId = node[i]["id"];
                string dbPw = node[i]["pw"];

                if (dbId == _id) 
                {
                    if (dbPw == _passward) 
                    {
                        UserId = _id;
                        Userpw = _passward;

                        loginSuccess = true;

                        Debug.Log("로그인 성공!");
                        break;
                    }
                }
            }
        }

        if (loginSuccess)
        {
            loginTab.GamePlay();
        }
    }
    IEnumerator CraetDBPost(string url, string id, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", id);
        form.AddField("pw", password);

        UnityWebRequest www = UnityWebRequest.Post(url, form);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("서버 응답: " + www.downloadHandler.text);
        }
        else
        {
            Debug.LogError("전송 실패: " + www.error);
        }
    }
    IEnumerator DBPost(string Url, string id)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", id);
       //form.AddField("pw", passward);

        UnityWebRequest www = UnityWebRequest.Post(Url, form);

        yield return www.SendWebRequest();//데이터를 받으면 아래 처리

        Debug.Log(www.downloadHandler.text);
        JSONNode node = JSONNode.Parse(www.downloadHandler.text);
        for (int i = 0; i < node.Count; i++)
        {
            node["id"] = i;
            node["pw"] = i;
        }

    }

}


