using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Room : MonoBehaviour
{
    public List<Monster_Base> inMonsterLists = new List<Monster_Base>();
    
    Transform[] roomTrans;
    public bool IsActive { get; private set; }

    [SerializeField] private bool isStartRoom = false;

    [SerializeField] List<Potal> potal; //Off
    [SerializeField] List<Potal> Onpotal; //ON

    public void init() 
    {
        roomTrans = GetComponentsInChildren<Transform>().Where(t => t != transform).ToArray();

        if (isStartRoom)
        {
            RoomEventcheck(true);
            GameShard.Instance.GameManager.CurrentRoom = this;

        }
        else
        {
            RoomEventcheck(false);
        }
    }

    public void RoomEventcheck(bool _state) 
    {
        IsActive = _state;
        if (inMonsterLists.Count != 0) 
        {
            for (int i = 0; i < inMonsterLists.Count; i++)
            {
                inMonsterLists[i].gameObject.SetActive(_state);
            }
        }
        

        for (int i = 0; i < roomTrans.Length; i++)
        {
            roomTrans[i].gameObject.SetActive(_state);    
        }


        //for (int i = 0; i < potal.Count; i++)
        //{
        //    potal[i].gameObject.SetActive(false);
        //}
        //for (int i = 0; i < Onpotal.Count; i++)
        //{
        //    Onpotal[i].gameObject.SetActive(true);
        //}


        //if (_state == true)
        //{
        //    for (int i = 0; i < potal.Count; i++)
        //    {
        //        potal[i].gameObject.SetActive(_state);
        //    }
        //    for (int i = 0; i < Onpotal.Count; i++)
        //    {
        //        Onpotal[i].gameObject.SetActive(_state);
        //    }
        //}
        //else 
        //{
        //    for (int i = 0; i < potal.Count; i++)
        //    {
        //        potal[i].gameObject.SetActive(_state);
        //    }
        //    for (int i = 0; i < Onpotal.Count; i++)
        //    {
        //        Onpotal[i].gameObject.SetActive(_state);
        //    }
        //}
        
    }
    
}
