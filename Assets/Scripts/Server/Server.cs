using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public partial class Server : MonoBehaviour
{
    public static Server instanse;
    //string Http = "http://58.78.211.182:3000/";//����
    string Http = "http://58.78.211.182:3000/";//����
    //3000<--���
    //12/22 �������� ��� ����
    //https<--���ȵ�
    //string ConnectUrl = "process/dbconnect";

    string LoginUrl = "process/login";
    string CountCreatUrl = "process/logincreate";
    string UserId;
    string Username;
    string Userpw;

    //string DisConnectUrl = "process/dbdisconnect";
    //string UserSelectUrl = "process/userselect";//���� ������
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
                    Debug.LogWarning("�������� �ʴ� ID�Դϴ�.");
                    break;
                case "failure password":
                    Debug.LogWarning("��й�ȣ�� �ùٸ��� �ʽ��ϴ�.");
                    break;
                case "login success":
                    Debug.Log("�α��� ����!");
                    break;
            }
        }
        else if (_json.HasKey("err"))
        {
            string errorMessage = _json["err"];
            switch (errorMessage)
            {
                case "not id":
                    Debug.LogWarning("ID�� �Է����� �ʾҽ��ϴ�.");
                    break;
                case "not id or not pw":
                    Debug.LogWarning("ID �Ǵ� ��й�ȣ�� �Է����� �ʾҽ��ϴ�.");
                    break;
            }
        }
        else
        {
            Debug.LogWarning("�� �� ���� ���� �����Դϴ�.");
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
            Debug.Log("���� ����: " + response);

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

                        Debug.Log("�α��� ����!");
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
            Debug.Log("���� ����: " + www.downloadHandler.text);
        }
        else
        {
            Debug.LogError("���� ����: " + www.error);
        }
    }
    IEnumerator DBPost(string Url, string id)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", id);
       //form.AddField("pw", passward);

        UnityWebRequest www = UnityWebRequest.Post(Url, form);

        yield return www.SendWebRequest();//�����͸� ������ �Ʒ� ó��

        Debug.Log(www.downloadHandler.text);
        JSONNode node = JSONNode.Parse(www.downloadHandler.text);
        for (int i = 0; i < node.Count; i++)
        {
            node["id"] = i;
            node["pw"] = i;
        }

    }

}


