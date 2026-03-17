using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {
    [Header("Camera Angles")]
    public GameObject slingshotCam;  // camera that is over the slingshot
    public GameObject castleCam;     // camera that is over the castle 
    public GameObject worldCam;      // camera that is over the entire game
    public GameObject followCam;     // camera that follows the ball

    private int currentIndex = 0;   // currently active camera 
    private int previousIndex = 0;  // Store which camera was active before switching to followCam

    private void Start(){
        // first cam is the slingshot cam
        currentIndex = 0; 
        SwitchToSlingshotCam();
    }

    public void SwitchToSlingshotCam() {
        slingshotCam.SetActive(true);
        castleCam.SetActive(false);
        worldCam.SetActive(false);
        followCam.SetActive(false);
        currentIndex = 0;
    }

    public void SwitchToCastleCam() {
        slingshotCam.SetActive(false);
        castleCam.SetActive(true);
        worldCam.SetActive(false);
        followCam.SetActive(false);
        currentIndex = 1;
    }

    public void SwitchToWorldCam() {
        slingshotCam.SetActive(false);
        castleCam.SetActive(false);
        worldCam.SetActive(true);
        followCam.SetActive(false);
        currentIndex = 2;
    }

    public void CycleCamera() {
        // Disable the currently active camera
        if (currentIndex == 0) {
            slingshotCam.SetActive(false);
        }
        else if (currentIndex == 1) {
            castleCam.SetActive(false);
        }
        else if (currentIndex == 2) {
            worldCam.SetActive(false);
        }

        currentIndex++;
        if (currentIndex > 2) {
            currentIndex = 0;
        }

        // Enable the new camera
        if (currentIndex == 0) {
            slingshotCam.SetActive(true);
        }
        else if (currentIndex == 1) {
            castleCam.SetActive(true);
        }
        else if (currentIndex == 2) {
            worldCam.SetActive(true);
        }
    }
    
    // How to tell which camera is active
    public Camera GetActiveCamera() {
        if (slingshotCam.activeSelf) {
            // Debug.Log("Returning SlingshotCam camera");
            return slingshotCam.GetComponent<Camera>();
        }
        else if (castleCam.activeSelf) {
            // Debug.Log("Returning CastleCam camera");
            return castleCam.GetComponent<Camera>();
        }
        else if (worldCam.activeSelf) {
            // Debug.Log("Returning WorldCam camera");
            return worldCam.GetComponent<Camera>();
        }
        else if (followCam.activeSelf) {
            // Debug.Log("Returning FollowCam camera");
            return followCam.GetComponent<Camera>();
        }
        else {
            Debug.LogWarning("No camera is currently active!");
            return null;
        }
    }

    // Switch to the follow camera
    public void SwitchToFollowCam() {
        previousIndex = currentIndex;  // remember which camera you were at so you can be returned to it once the ball de-spawns 
        slingshotCam.SetActive(false);
        castleCam.SetActive(false);
        worldCam.SetActive(false);
        followCam.SetActive(true);
        //Debug.Log("Switched to FollowCam");
    }

    // switch back to the orginal camera
    public void SwitchBackFromFollowCam() {
        followCam.SetActive(false);
        if (previousIndex == 0)
            SwitchToSlingshotCam();
        else if (previousIndex == 1)
            SwitchToCastleCam();
        else if (previousIndex == 2)
            SwitchToWorldCam();
        // Debug.Log("Switched back from FollowCam");
    }
}
