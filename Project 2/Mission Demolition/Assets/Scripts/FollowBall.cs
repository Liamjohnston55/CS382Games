using UnityEngine;
using System.Collections;

public class FollowCam : MonoBehaviour {
    // point at which the camera follows
    public static GameObject POI; // this will be the launched ball that the follow cam focuses on

    [Header("Set Dynamically")]
    public float camZ; 

    void Awake() {

        // debugging----------------------------------------------------------------------
        // Debug.Log("FollowCam Awake called");
        // debugging----------------------------------------------------------------------


        // Record the initial z position of the camera
        // We need the z position to make sure we are not behind it and cant see the ball or map
        camZ = transform.position.z;
        FollowCam.POI = gameObject;  
    }

    void FixedUpdate() {
        if (POI == null) return;
        
        // debugging----------------------------------------------------------------------
        // Debug.Log("Following: " + POI.name);
        // debugging----------------------------------------------------------------------

        // Get the position of the ball and force the z value to be camZ so we can make sure to actually see it
        Vector3 destination = POI.transform.position;
        destination.z = camZ; 
        
        // Move the camera to the destination of the ball
        transform.position = destination;
    }
}