using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;      
    public float runSpeed = 10f;      
    public float rotationSmoothTime = 0.1f; 

    [Header("Jump Settings")]
    public float jumpForce = 5f;      
    public float jumpCooldown = 0.5f; 
    public LayerMask groundLayer;     

    private bool readyToJump = true;  
    private bool grounded = false;    

    [Header("Punch Settings")]
    public float punchDamage = 10f;   
    public float punchRange = 2f;     

    [Header("References")]
    public Transform cameraTransform; 
    public Animator animator;         
    public Transform modelTransform;  

    private Rigidbody rb;
    private bool isRunning;
    private float speedPercent;

    private bool isPunching = false;      
    private Vector3 punchDirection = Vector3.forward; 
    private Quaternion punchRotation;      // ← store the rotation at punch

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        if (modelTransform == null)
        {
            Debug.LogWarning("No ModelTransform assigned, using parent transform instead.");
            modelTransform = transform;
        }
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (Mathf.Abs(h) < 0.1f) h = 0f;
        if (Mathf.Abs(v) < 0.1f) v = 0f;

        // cancel punch state if player moves
        if (isPunching && (h != 0f || v != 0f))
            isPunching = false;

        isRunning = Input.GetKey(KeyCode.LeftShift);

        // handle punch input
        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("Punch");
            DoPunch();
            isPunching = true;

            // capture rotation at punch start
            punchRotation = modelTransform.rotation;

            // capture direction at punch start (if you still need it)
            Vector3 camF = cameraTransform.forward; camF.y = 0f;
            Vector3 camR = cameraTransform.right;   camR.y = 0f;
            camF.Normalize(); camR.Normalize();
            punchDirection = (camF * v + camR * h).normalized;
        }

        // jump logic
        if (Input.GetButtonDown("Jump") && readyToJump && grounded)
        {
            readyToJump = false;
            animator.SetTrigger("JumpUp");
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // build camera-relative move vector
        Vector3 forward = cameraTransform.forward; forward.y = 0f;
        Vector3 right   = cameraTransform.right;   right.y   = 0f;
        forward.Normalize(); right.Normalize();

        Vector3 inputDirection = isPunching 
            ? punchDirection 
            : (forward * v + right * h).sqrMagnitude > 0.001f
                ? (forward * v + right * h).normalized
                : Vector3.zero;

        // update animator speed
        speedPercent = inputDirection.magnitude > 0f
            ? (isRunning ? 1f : 0.5f)
            : 0f;
        animator.SetFloat("Speed", speedPercent, 0f, Time.deltaTime);

        // ROTATION: lock at punchOrientation until movement resumes
        if (isPunching)
        {
            modelTransform.rotation = punchRotation;
        }
        else if (inputDirection.magnitude > 0f)
        {
            Quaternion targetRot = Quaternion.LookRotation(inputDirection);
            modelTransform.rotation = Quaternion.Slerp(
                modelTransform.rotation,
                targetRot,
                Time.deltaTime / rotationSmoothTime
            );
        }

        // move if not punching
        if (!isPunching)
            MoveCharacter(inputDirection);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private void MoveCharacter(Vector3 direction)
    {
        if (direction.magnitude < 0.01f) return;
        float speed = isRunning ? runSpeed : walkSpeed;
        Vector3 delta = direction * speed * Time.deltaTime;
        rb.MovePosition(rb.position + delta);
    }

    private void DoPunch()
    {
        Vector3 origin = modelTransform.position + modelTransform.forward * (punchRange * 0.5f);
        Collider[] hits = Physics.OverlapSphere(origin, punchRange);
        foreach (var c in hits)
        {
            var hlt = c.GetComponent<Harvestable>();
            if (hlt != null) hlt.Harvest();
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (((1 << col.gameObject.layer) & groundLayer) != 0)
            grounded = true;
    }

    void OnCollisionExit(Collision col)
    {
        if (((1 << col.gameObject.layer) & groundLayer) != 0)
            grounded = false;
    }
}
