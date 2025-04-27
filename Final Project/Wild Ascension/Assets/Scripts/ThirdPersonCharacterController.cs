using UnityEngine;

// Require a Rigidbody component on the same GameObject for physics movement
[RequireComponent(typeof(Rigidbody))]
public class ThirdPersonCharacterController : MonoBehaviour
{
    [Header("References")]
    public Transform model;    // The child object containing the character model (visuals)
    public Animator animator;  // Animator for the character model (should have a "Speed" parameter)

    [Header("Movement Settings")]
    public float walkSpeed = 2.0f;
    public float runSpeed = 5.0f;
    public float backwardSpeed = 1.5f;
    public float rotationSpeed = 10.0f;  // How fast the character model rotates to face movement direction

    private Rigidbody rb;
    private Transform cam;  // Reference to the main camera transform

    void Start()
    {
        // Get the Rigidbody component and camera transform
        rb = GetComponent<Rigidbody>();
        if (Camera.main != null)
            cam = Camera.main.transform;
        else
            cam = null;

        // Configure Rigidbody constraints for stable movement:
        // Freeze rotations so physics won't tilt or turn the character’s root.
        rb.freezeRotation = true;  // Equivalent to freezing X, Y, Z rotation constraints&#8203;:contentReference[oaicite:5]{index=5}

        // Enable interpolation for smooth movement between physics updates&#8203;:contentReference[oaicite:6]{index=6}
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        
        // (Optional) Ensure gravity is enabled on the Rigidbody for natural falling/jumping
        rb.useGravity = true;
    }

    void Update()
    {
        // 1. Read input for movement
        float horizontal = Input.GetAxis("Horizontal"); // A/D or Left/Right arrows
        float vertical   = Input.GetAxis("Vertical");   // W/S or Up/Down arrows

        bool runPressed = Input.GetKey(KeyCode.LeftShift);

        // 2. Determine movement direction relative to camera orientation
        Vector3 camForward = Vector3.forward;
        Vector3 camRight   = Vector3.right;
        if (cam != null)
        {
            // Use camera's forward and right vectors, projected onto the horizontal plane (y=0)
            camForward = Vector3.Scale(cam.forward, new Vector3(1, 0, 1)).normalized;
            camRight   = Vector3.Scale(cam.right,   new Vector3(1, 0, 1)).normalized;
        }
        // Calculate direction to move based on input and camera
        Vector3 moveDirection = (camForward * vertical) + (camRight * horizontal);
        if (moveDirection.sqrMagnitude > 1f)
            moveDirection.Normalize();  // Normalize to prevent faster diagonal movement

        // 3. Choose movement speed based on run/walk/backward
        float moveSpeed = 0f;
        if (vertical < 0f)
        {
            // Moving backward (pressing S): use slower backward speed
            moveSpeed = backwardSpeed;
        }
        else if (runPressed && moveDirection.sqrMagnitude > 0.001f)
        {
            // Running (Shift held and moving): use run speed
            moveSpeed = runSpeed;
        }
        else
        {
            // Walking (no run or just horizontal movement)
            moveSpeed = walkSpeed;
        }

        // 4. Apply movement via Rigidbody
        // Compute the desired velocity for this frame (horizontal plane only)
        Vector3 velocity = moveDirection * moveSpeed;
        // Preserve existing vertical velocity (for gravity)
        velocity.y = rb.velocity.y;
        // Assign the velocity to the Rigidbody to move the character
        rb.velocity = velocity;

        // 5. Smoothly rotate the character's model to face the movement direction
        if (moveDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation;
            if (vertical < 0f)
            {
                // If moving backward: face opposite to movement direction (so the model looks forward while moving backward)
                targetRotation = Quaternion.LookRotation(-moveDirection);
            }
            else
            {
                // Face the movement direction normally (forward or strafing directions)
                targetRotation = Quaternion.LookRotation(moveDirection);
            }
            // Smoothly interpolate the model's rotation towards the target rotation
            model.rotation = Quaternion.Slerp(model.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        // (If no movement input, we keep the current rotation of the model)

        // 6. Update Animator parameters for movement (if an animator is assigned)
        if (animator != null)
        {
            float speedParam;
            if (moveDirection.sqrMagnitude < 0.1f)
            {
                // No significant movement input: idle
                speedParam = 0f;
            }
            else if (vertical < 0f)
            {
                // Backward movement
                speedParam = 0.3f;
            }
            else if (runPressed)
            {
                // Running forward/side
                speedParam = 1.0f;
            }
            else
            {
                // Walking (forward/sideways)
                speedParam = 0.5f;
            }
            // Set the "Speed" parameter to control blend tree or animation state
            animator.SetFloat("Speed", speedParam);
        }
    }
}
