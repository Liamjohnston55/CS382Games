using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

// This is the script that will make sure that the player is moving in third person

public class PlayerMovement : MonoBehaviour {
    [Header("Movement")]
    public float MovementSpeed;         // How fast the character will run

    public float Drag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Keybinds")]
    public KeyCode jumpkey = KeyCode.Space;

    [Header("Ground Check")]
    public float characterHeight;
    public LayerMask whatIsGround;
    bool grounded;

    float HorizontalInput;              // Horizontal keyboard input
    float VerticalInput;                // Vertical keyboard input
    public Transform Orientation;       // Where the character is facing 
    Vector3 moveDirection;              // What direction the character is moving in
    Rigidbody RB;                       // Character's gravity 

    private void Start() {
        // This makes sure the character model has the proper physics
        RB = GetComponent<Rigidbody>();
        RB.freezeRotation = true;

        // Getting the jump key ready
        readyToJump = true;
    }

    private void Update() {
        // Check to see if character is on the ground
        grounded = Physics.Raycast(transform.position, Vector3.down, characterHeight * 0.5f + 0.3f, whatIsGround);

        // we need to call character input so the game can receive the data 
        characterInput();
        // we also need to make sure the character's speed does not go out of control
        SpeedControl();

        // Applying the drag to the character
        if (grounded) {
            RB.drag = Drag;
        }
        else {
            RB.drag = 0;
        }
    }

    private void FixedUpdate() {
        characterMovement();
    }

    private void characterInput() {
        // get the x and y data from the player's input
        HorizontalInput = Input.GetAxisRaw("Horizontal");
        VerticalInput = Input.GetAxisRaw("Vertical");

        // We will also check to see when the player hits the jump key
        // this makes sure the player is on the ground, and ready to jump
        if(Input.GetKey(jumpkey) && readyToJump && grounded) {
            readyToJump = false;
            Jump(); 

            // This allows the character to jump continuously 
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void characterMovement() {
        // Find the character's movement direction
        moveDirection = Orientation.forward * VerticalInput + Orientation.right * HorizontalInput;

        // character is on the ground
        if (grounded) {
            RB.AddForce(moveDirection * MovementSpeed * 10f, ForceMode.Force);
        }
        // character is in the air 
        else if (!grounded) {
            RB.AddForce(moveDirection * MovementSpeed * 10f * airMultiplier, ForceMode.Force);            
        }
    }

    private void SpeedControl() {
        // This will us to find the characters speed 
        // flatVel will store the x and z axis's
        Vector3 flatVel = new Vector3(RB.velocity.x, 0f, RB.velocity.z);

        // if the velocity is greater than our movement speed
        // we will manually change the max velocity to ensure it stays under the max 
        if (flatVel.magnitude > MovementSpeed) {
            Vector3 limitedVel = flatVel.normalized * MovementSpeed;
            RB.velocity = new Vector3(limitedVel.x, RB.velocity.y, limitedVel.z);
        }
    }

    private void Jump() {
        // Before we apply any forces, we need to set y velocity to zero
        // This the character will always jump at the exact same height 
        RB.velocity = new Vector3(RB.velocity.x, 0f, RB.velocity.z);

        // Now we can apply the jump's force 
        RB.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump() {
        readyToJump = true;
    }
}