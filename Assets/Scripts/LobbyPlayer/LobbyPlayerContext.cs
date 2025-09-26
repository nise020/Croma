using UnityEngine;
using System;

[CreateAssetMenu(menuName = "UI/LobbyPlayerContext")]
public class LobbyPlayerContext : ScriptableObject
{
    [NonSerialized] public Camera previewCam;
    [NonSerialized] public LobbyPlayer lobbyPlayer;
    [NonSerialized] public RenderTexture previewRT;

    public bool IsReady => previewCam && lobbyPlayer && previewRT;

    public event Action OnReady;

    public void Register(Camera cam, LobbyPlayer player, RenderTexture rt)
    {
        previewCam = cam;
        lobbyPlayer = player;
        previewRT = rt;
        OnReady?.Invoke();

        Debug.Log("Context Create Success!");
    }
}
