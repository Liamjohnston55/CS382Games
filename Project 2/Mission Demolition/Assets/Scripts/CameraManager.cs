using UnityEngine;

public class CameraManager : MonoBehaviour 
{
    [Header("Camera Angles")]
    public GameObject slingshotCam;  
    public GameObject castleCam;     
    public GameObject worldCam;      

    private int currentIndex = 0;

    private void Start()
    {
        // start off at the slingshot
        currentIndex = 0; // Slingshot index
        SwitchToSlingshotCam();
    }

    public void SwitchToSlingshotCam() {
        slingshotCam.SetActive(true);
        castleCam.SetActive(false);
        worldCam.SetActive(false);
    }

    public void SwitchToCastleCam() {
        slingshotCam.SetActive(false);
        castleCam.SetActive(true);
        worldCam.SetActive(false);
    }

    public void SwitchToWorldCam() {
        slingshotCam.SetActive(false);
        castleCam.SetActive(false);
        worldCam.SetActive(true);
    }

    public void CycleCamera() {
        // Disable the camera not in use
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

        // Enable the camera that we are currently on
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
}