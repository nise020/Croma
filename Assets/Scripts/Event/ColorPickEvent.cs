using UnityEngine;

public class ColorPickEvent : MonoBehaviour
{
    public Camera mainCamera;
    public Color debugColor;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PickColorAtClick();
        }
    }

    void PickColorAtClick()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Renderer renderer = hit.collider.GetComponent<Renderer>();
            if (renderer == null || renderer.material == null) return;

            Vector2 uv = hit.textureCoord;

            Texture2D texture = renderer.material.GetTexture("_MainTex") as Texture2D;

            if (texture == null)
            {
                Debug.LogWarning("Material에 _MainTex가 없거나 Texture2D가 아님");
                return;
            }

            if (!texture.isReadable)
            {
                Debug.LogWarning("Texture가 Read/Write Enabled 되어 있지 않음");
                return;
            }

            int x = Mathf.FloorToInt(uv.x * texture.width);
            int y = Mathf.FloorToInt(uv.y * texture.height);

            x = Mathf.Clamp(x, 0, texture.width - 1);
            y = Mathf.Clamp(y, 0, texture.height - 1);

            debugColor = texture.GetPixel(x, y);
            Debug.Log($"Picked Color: {debugColor}");
        }
    }

}
