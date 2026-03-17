using UnityEngine;

public class Starfield : MonoBehaviour {
    public float scrollSpeed = 0.1f;
    private Renderer rend;

    void Awake() {
        rend = GetComponent<Renderer>();
    }

    void Update() {
        float offset = Time.time * scrollSpeed;
        // Use "_BaseMap" instead of "_MainTex"
        rend.material.SetTextureOffset("_BaseMap", new Vector2(0, offset));
    }
}
