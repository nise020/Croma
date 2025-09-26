using UnityEngine;


[DefaultExecutionOrder(-5000)]
public class LobbyPlayerProvider : MonoBehaviour
{
    [SerializeField] private LobbyPlayerContext context; 
    [SerializeField] private Camera previewCam;          
    [SerializeField] private LobbyPlayer lobbyPlayer;      
    [SerializeField] private RenderTexture targetRT;

    public LobbyPlayerContext Context => context;
    private void Awake()
    {
        if (context != null && context.IsReady)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        if (targetRT == null)
        {
            targetRT = new RenderTexture(512, 512, 16, RenderTextureFormat.ARGB32);
            targetRT.Create();
        }

        if (previewCam)
        {
            previewCam.enabled = false;
            previewCam.targetTexture = targetRT;
        }

        int layer = LayerMask.NameToLayer("Preview3D");
        if (layer >= 0)
        {
            if (previewCam) previewCam.cullingMask = 1 << layer;
            SetLayerRecursively(lobbyPlayer?.gameObject, layer);
        }

        context?.Register(previewCam, lobbyPlayer, targetRT);
    }

    private void SetLayerRecursively(GameObject go, int layer)
    {
        if (!go) return;
        go.layer = layer;
        foreach (Transform t in go.transform) SetLayerRecursively(t.gameObject, layer);
    }
}
