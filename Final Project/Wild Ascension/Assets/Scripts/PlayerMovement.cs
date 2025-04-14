using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;      // How fast the character walks
    public float runSpeed = 10f;      // How fast the character runs
    public float rotationSmoothTime = 0.1f; // Lower = faster snap/turn

    [Header("Punch Settings")]
    public float punchDamage = 10f;   // Damage dealt by a punch
    public float punchRange = 2f;     // Players punch range 

    [Header("References")]
    public Transform cameraTransform;  // Main Camera transform
    public Animator animator;          // Character's Animator
    public Transform modelTransform;   // Visual model child

    private Rigidbody rb;
    private bool isRunning;
    private float speedPercent; // used for animation's

    void Start() {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; 
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        // fallback incase the correct monkey model is not assigned
        if (modelTransform == null) {
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

        // Check if the player is running or not
        isRunning = Input.GetKey(KeyCode.LeftShift);

        // trigger punch attack 
        if (Input.GetMouseButtonDown(0)){
            animator.SetTrigger("Punch");
            DoPunch();  
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

        // Determine speed for animations
        if (Mathf.Abs(h) < 0.1f && Mathf.Abs(v) < 0.1f) {
            speedPercent = 0f;
        }
        else {
            // Use 0.3 for walking backward, 0.5 for walking regularly, 1 for running
            if (v < 0f)
                speedPercent = 0.3f;
            else
                speedPercent = isRunning ? 1f : 0.5f;
        }
        animator.SetFloat("Speed", speedPercent, 0f, Time.deltaTime);

        // Snap the camera angle to nearest 45°
        float finalAngle = ComputeSnapAngle(h, v, cameraTransform.eulerAngles.y);

        // Rotate only the model if there's input
        if (speedPercent > 0f) {
            Quaternion targetRot = Quaternion.Euler(0f, finalAngle, 0f);
            modelTransform.rotation = Quaternion.Slerp(modelTransform.rotation, targetRot, Time.deltaTime * (1f / rotationSmoothTime));
        }

        MoveCharacter(h, v);
    }

    private void FixedUpdate(){
        // Movement is done in Update using rb.MovePosition
    }

    private void MoveCharacter(float h, float v){
        // If there's no input, do not move
        if (Mathf.Abs(h) < 0.1f && Mathf.Abs(v) < 0.1f) {
            return;
        }

        // Always use the same walking speed regardless of forward or backward
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

        // Mathf.Atan cacululates the angle in radians, and then multiply by Mathf.Rad2Deg to get it to degrees
        float rawAngle = Mathf.Atan2(h, v) * Mathf.Rad2Deg;
        if (rawAngle < 0f) {
            rawAngle += 360f; // if the resulting angle is negative, add 360 to keep it positive
        }

        // align player's movement direction to the direction the camera is facing
        float finalAngle = rawAngle + cameraY;
        if (finalAngle >= 360f) {
            finalAngle -= 360f;
        }

        // Snap to nearest 45°
        float snapped = Mathf.Round(finalAngle / 45f) * 45f;
        if (snapped < 0f) {
            snapped += 360f;
        }
        else if (snapped >= 360f) {
            snapped -= 360f;
        }
        return snapped;
    }

    // punch attack (add more later)
    private void DoPunch() {
        // Determine where to apply damage
        Vector3 punchOrigin = modelTransform.position + modelTransform.forward * (punchRange * 0.5f);
        Collider[] hitColliders = Physics.OverlapSphere(punchOrigin, punchRange);
        foreach (Collider hit in hitColliders) {
            // Try to get a Harvestable component from the hit object (later you may hit an enemy)
            Harvestable targetHarvestable = hit.GetComponent<Harvestable>();
            if (targetHarvestable != null) {
                targetHarvestable.Harvest();
            }
        }
    }

}
