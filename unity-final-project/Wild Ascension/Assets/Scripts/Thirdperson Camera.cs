using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// This is the script that will make sure the camera is following the character in third person
// we want to rotate the orientation of the player in sync with the camera
// this is how we will store the forward(w) value of the player
// We also want to rotate the player in the direction of the camera
// the camera and player need to move in sync with the input


public class ThirdpersonCamera : MonoBehaviour {
    [Header("ThirdPerson Camera")]
    public Transform orientation;   // Where the character is facing 
    public Transform player;        // This is the player container itself
    public Transform playerObj;     // This is the players 3D model 
    public Rigidbody rb;            // Character's's gravity 
    public float RotationSpeed;     // how fast the camera will move 

    // This is to make the cursor invisible 
    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update() {
        // We want to calcululate the direction from the camera to the player 
        // This is used to find out the direction the player should face 
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized; 

        // Now we need to use the input the player gives to control the character
        float HorizontalInput = Input.GetAxis("Horizontal");
        float VerticalInput = Input.GetAxis("Vertical");
        Vector3 inputDir = orientation.forward * VerticalInput + orientation.right * HorizontalInput;

        // This will make sure that the camera movement is fluid and doesn't jerk around 
        if (inputDir != Vector3.zero) {
            playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * RotationSpeed);
        }
        
    }

}
