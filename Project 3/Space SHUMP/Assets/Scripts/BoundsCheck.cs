using UnityEngine;

public class BoundsCheck : MonoBehaviour {
    [Header("Set in Inspector")]
    public float radius = 1f;
    public bool keepOnScreen = true;

    [Header("Set Dynamically")]
    public bool isOnScreen = true;
    public float camWidth;
    public float camHeight;

    // These can be public if you’d like to check them in the Inspector
    [HideInInspector] public bool offRight, offLeft, offUp, offDown;

    void Awake() {
        // Get camera bounds based on main camera’s orthographic size and aspect ratio
        camHeight = Camera.main.orthographicSize;
        camWidth = camHeight * Camera.main.aspect;
    }

    void LateUpdate() {
        Vector3 pos = transform.position;
        isOnScreen = true;

        // Reset these each frame
        offRight = offLeft = offUp = offDown = false;

        // Check boundaries (left, right, top, bottom)
        if (pos.x > camWidth - radius) {
            pos.x = camWidth - radius;
            offRight = true;
        }
        if (pos.x < -camWidth + radius) {
            pos.x = -camWidth + radius;
            offLeft = true;
        }
        if (pos.y > camHeight - radius) {
            pos.y = camHeight - radius;
            offUp = true;
        }
        if (pos.y < -camHeight + radius) {
            pos.y = -camHeight + radius;
            offDown = true;
        }

        // If any of these flags is true, the object is off screen
        isOnScreen = !(offRight || offLeft || offUp || offDown);

        if (keepOnScreen && !isOnScreen) {
            // If we’re supposed to keep the object on screen, reposition
            transform.position = pos;
            isOnScreen = true;
            offRight = offLeft = offUp = offDown = false;
        }
    }
}
