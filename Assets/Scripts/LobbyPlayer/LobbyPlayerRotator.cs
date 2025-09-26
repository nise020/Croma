using UnityEngine;
using UnityEngine.EventSystems;

public class LobbyPlayerRotator : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private LobbyPlayerContext context;
    [SerializeField] private float sensitivity = 0.5f;

    private bool invert = false;
    private bool dragging;
    private float lastX;

    private void OnEnable()
    {
        if (context == null)
        {
            var provider = FindFirstObjectByType<LobbyPlayerProvider>(FindObjectsInactive.Include);

            if (provider != null)
                context = provider.Context;
        }

        if (context != null)
        {
            context.OnReady += HandleReady;
            HandleReady();
        }
    }

    private void OnDisable()
    {
        if (context != null) context.OnReady -= HandleReady;
        dragging = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!dragging || context == null || !context.IsReady) return;

        Debug.Log("Dragging");
        float dx = eventData.position.x - lastX;
        lastX = eventData.position.x;

        float sign = invert ? 1 : -1;
        float deltaYaw = dx * sensitivity * sign;

        var t = context.lobbyPlayer.transform;
        var e = t.eulerAngles;
        e.y += deltaYaw;
        t.eulerAngles = e;

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (context == null || !context.IsReady) return;

        Debug.Log("Down!");
        dragging = true;
        lastX = eventData.position.x;   
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        dragging = false;
    }

    private void HandleReady()
    {

    }
}
