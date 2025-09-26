using UnityEngine;

public class Cloud : MonoBehaviour
{
    Renderer rend;
    [SerializeField] float scrollSpeed = 0.05f;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        float offset = Time.time * scrollSpeed;
        rend.material.mainTextureOffset = new Vector2(0, offset); // Y축 방향으로 흐르도록
    }
}
