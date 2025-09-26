using Cysharp.Threading.Tasks;
using NUnit.Framework;
using SimpleJSON;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public partial class Server : MonoBehaviour
{
    //string Http = "http://58.78.211.182:3000/";//서버 
    string NameCreatUrl = "process/namecreate";
    public List<RankEntry> serverData = new();
    public async UniTask RankData()
    {
        await LoadRankData(Http+NameCreatUrl);
    }
    public async UniTask LoadRankData(string url) 
    {
        UnityWebRequest www = UnityWebRequest.Get(url);

        await www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.Success)
        {
            JSONNode node = JSONNode.Parse(www.downloadHandler.text);
            for (int i = 0; i < node.Count; i++) 
            {
                string dbId = node[i]["id"];
                string dbPw = node[i]["name"];

                serverData[i].id = dbId;
                serverData[i].name = name;
            }
        }
        await UniTask.CompletedTask;
    }

    public async UniTask PostRankData(string url,string _id,string _name)
    {
        WWWForm form = new WWWForm();

        form.AddField("id", _id);
        form.AddField("name", _name);

        UnityWebRequest www = UnityWebRequest.Post(url,form);

        await www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("서버 응답: " + www.downloadHandler.text);
        }
        else
        {
            Debug.LogError("전송 실패: " + www.error);
        }
        await UniTask.CompletedTask;
    }
}
