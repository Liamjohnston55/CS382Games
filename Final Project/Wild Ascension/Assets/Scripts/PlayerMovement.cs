using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;      // How fast the character walks
    public float runSpeed = 10f;      // How fast the character runs
    public float rotationSmoothTime = 0.1f; // Lower = faster snap/turn
    public float movementSmoothTime = 0.05f; // (Currently unused; kept for consistency)

    [Header("References")]
    public Transform cameraTransform;  // Main Camera transform
    public Animator animator;          // Character's Animator
    public Transform modelTransform;   // Visual model child

    private Rigidbody rb;
    private bool isRunning;
    private float speedPercent; // 0 = idle, 0.5 = walk, 1.0 = run

    void Start() {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; 
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        if (modelTransform == null) {
            // Debugging -------------------------------------------------------------------------------------
            Debug.LogWarning("No ModelTransform assigned, using parent transform instead.");
            modelTransform = transform;
        }
    }

    void Update() {
        // Get raw input
        float h = Input.GetAxisRaw("Horizontal"); 
        float v = Input.GetAxisRaw("Vertical");   

        // If input is tiny, zero it out
        if (Mathf.Abs(h) < 0.1f && Mathf.Abs(v) < 0.1f) {
            h = 0f;
            v = 0f;
        }

        // Check if running
        isRunning = Input.GetKey(KeyCode.LeftShift);

        // Punch (left mouse)
        // add this later
        if (Input.GetMouseButtonDown(0)){
            animator.SetTrigger("Punch");
        }

        // Convert input to camera-based direction
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 inputDirection = (camForward * v + camRight * h);
        if (inputDirection.sqrMagnitude > 0.001f) {
            inputDirection = inputDirection.normalized;
        }
        else {
            inputDirection = Vector3.zero;
        }

        // Determine speed percentage for animations
        // Regardless of whether vertical input is forward or backward, use the same values.
        if (inputDirection.sqrMagnitude > 0.001f) {
            speedPercent = isRunning ? 1f : 0.5f;
        }
        else {
            speedPercent = 0f;
        }

        animator.SetFloat("Speed", speedPercent, 0f, Time.deltaTime);

        // Snap the angle to nearest 45°, offset by camera
        float finalAngle = ComputeSnapAngle(h, v, cameraTransform.eulerAngles.y);

        // Rotate only the model if there's input
        if (speedPercent > 0f) {
            Quaternion targetRot = Quaternion.Euler(0f, finalAngle, 0f);
            modelTransform.rotation = Quaternion.Slerp(modelTransform.rotation, targetRot, Time.deltaTime * (1f / rotationSmoothTime)
            );
        }

        // Move character
        MoveCharacter(h, v);
    }

    private void FixedUpdate(){
        // Movement is done in Update using rb.MovePosition
    }

    private void MoveCharacter(float h, float v){
        // If there's no input, do not move
        // this is to prevent the character from always walking foward 
        if (Mathf.Abs(h) < 0.1f && Mathf.Abs(v) < 0.1f) {
            return;
        }

        // Always use the same target speed regardless of forward or backward
        float targetSpeed = isRunning ? runSpeed : walkSpeed;

        // Compute movement direction using snapped angle
        float angle = ComputeSnapAngle(h, v, cameraTransform.eulerAngles.y);
        Vector3 direction = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;

        Vector3 moveDelta = direction * targetSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + moveDelta);
    }

    // Snap angle to nearest 45°, offset by camera
    private float ComputeSnapAngle(float h, float v, float cameraY) {
        // If no input, return current rotation
        if (Mathf.Abs(h) < 0.1f && Mathf.Abs(v) < 0.1f)
            return modelTransform.eulerAngles.y;

        // 1) Base angle from input using Atan2 (swapping h and v so that W = 0°)
        float rawAngle = Mathf.Atan2(h, v) * Mathf.Rad2Deg;
        if (rawAngle < 0f) {
            rawAngle += 360f;
        }

        // 2) Offset by camera's Y rotation
        float finalAngle = rawAngle + cameraY;
        if (finalAngle >= 360f) {
            finalAngle -= 360f;
        }

        // 3) Snap to nearest 45°
        float snapped = Mathf.Round(finalAngle / 45f) * 45f;
        if (snapped < 0f) {
            snapped += 360f;
        }
        else if (snapped >= 360f) {
            snapped -= 360f;
        }
        return snapped;
    }
}
