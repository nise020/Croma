using UnityEngine;
using static Enums;

public class BuffData
{
    public int id;
    public BUFFTYPE type;
    public float value;
    public float time;
    public string effectPath;
    public string spriteName;
    public int soundID;


    public float elapsedTime = 0f;
    public bool IsDebuff;
    public bool IsExpired => elapsedTime >= time;

    public void DebuffCheck(int _id) 
    {
        if (_id <= 2000)
        {
            IsDebuff = false;
        }
        else 
        {
            IsDebuff = true;
        }
    }
}
