using UnityEngine;

public class FollowBall : MonoBehaviour 
{
    [Tooltip("Script used for the camera to follow the ball")]
    public Transform target;  // balls prefab
    public Vector3 offset = new Vector3(0, 0, -10); // keeps the camera from being on-top of the ball

    public float smoothTime = 0.2f; // time in seconds it takes for the camera to catch up to the ball

    private Vector3 velocity = Vector3.zero;

    void LateUpdate() {
        if (target == null) return;
        
        Vector3 targetPosition = target.position + offset;

        // move the camera to the balls position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
