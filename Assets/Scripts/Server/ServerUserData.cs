using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class ServerUserData //: MonoBehaviour
{
    int[] id;
    string[] account;
    string[] userName;
    public static void UserData(UnityWebRequest _www)
    {
        int count1 = _www.downloadHandler.text.Length;
        while (count1 > 0)
        {

        }
    }
    public int playerData(List<GameObject> _player) 
    {
        return _player.Count;
    }
}
