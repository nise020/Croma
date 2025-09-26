using UnityEngine;

public class Potal : MonoBehaviour
{
    [SerializeField]Room thisRoom;
    public Room connectedRoom;
    public Potal linkedPortal; // ¹Ý´ëÂÊ Æ÷Å»

    private bool triggered = false;
    Transform nextTrs;
    private void Start()
    {
        nextTrs = GetComponentInChildren<Transform>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        
        triggered = true;

        GameShard.Instance.GameManager.PotalEvent?.Invoke(thisRoom, connectedRoom);
        Player player = other.gameObject.GetComponent<Player>();
        player.RoomUpdate(connectedRoom);
        other.gameObject.transform.position += transform.forward;
    }

    private void OnTriggerExit(Collider other)
    {
        triggered = false;
    }
}
