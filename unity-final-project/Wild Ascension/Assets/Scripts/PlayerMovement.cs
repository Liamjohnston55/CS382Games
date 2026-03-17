using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour {
    [Header("Movement Settings")]
    public float walkSpeed = 5f;                            // Walking speed multiplier
    public float runSpeed = 10f;                            // Running speed multiplier
    public float rotationSmoothTime = 0.1f;                 // turning smoothness

    [Header("Jump Settings")]
    public float jumpCooldown = 0.5f;                       // seconds between jumps
    public LayerMask groundLayer;                           // what counts as “ground”

    [Header("Punch & Swing Settings")]
    [Tooltip("Range of your punch or weapon hit check")]
    public float punchRange = 2f;                           // Radius for overlap sphere
    [Tooltip("Default cooldown for unarmed punches")]
    public float defaultSwingCooldown = 0.5f;               // punch cool-down timer

    [Header("References")]
    public Transform cameraTransform;                       // so we know forward/backwards
    public Animator  animator;                              // Controls animation states
    public Transform modelTransform;                        // the root of our visible mode

    private Rigidbody rb;
    private PlayerStats stats;

    private bool isRunning;
    private bool isPunching;
    private bool readyToJump = true;
    private bool grounded    = false;

    private Vector3 punchDirection = Vector3.forward;  // Direction locked during swing
    private Quaternion punchRotation;                  // Rotation locked during swing

    void Start() {
        stats = FindObjectOfType<PlayerStats>();       // player stats for jumps/damage
        rb = GetComponent<Rigidbody>();                // Rigidbody for movement
        rb.freezeRotation   = true;                    // Disable physics-driven rotation
        rb.interpolation    = RigidbodyInterpolation.Interpolate; // Smooth physics

        if (modelTransform == null) {
            // debugging
            // Debug.LogWarning("No ModelTransform assigned, using this.transform instead.");
            modelTransform = transform;                // Fallback if not set in Inspector
        }
    }

    void Update() {
        // Get input
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        if (Mathf.Abs(h) < 0.1f) {
            h = 0f;
        }
        if (Mathf.Abs(v) < 0.1f) {
            v = 0f;
        }
        isRunning = Input.GetKey(KeyCode.LeftShift);

        // Handle attack (only if not already swinging)
        if (Input.GetMouseButtonDown(0) && !isPunching) {
            // check to see what you have in your hand
            var eq = FindObjectOfType<EquipmentManager>();
            WeaponSO ws = eq != null ? eq.GetEquipped() : null;

            // debugging
            // Debug.Log($"[Attack] equipped: {(ws != null ? ws.weaponName : "none")} ");

            // decide which trigger & cooldown based on WeaponType 
            string triggerName;
            float cooldown;

            if (ws != null) {
                // this switch stores all of the animation names and weapon cool-downs
                switch (ws.type) {
                    case WeaponType.Axe:
                        triggerName = "Swing_Axe";
                        cooldown    = ws.swingCooldown;
                        break;
                    case WeaponType.Pickaxe:
                        triggerName = "Swing_Pick";
                        cooldown    = ws.swingCooldown;
                        break;
                    default:
                        triggerName = "Punch";
                        cooldown    = defaultSwingCooldown;
                        break;
                }
            }
            // if no weapon is selected, do the default attack of punch
            else {
                triggerName = "Punch";
                cooldown = defaultSwingCooldown;
            }

            // fire animation trigger
            animator.SetTrigger(triggerName);

            // debugging
            // Debug.Log($"Branch: {triggerName}, cooldown={cooldown:0.00}s");

            // perform hit logic
            DoPunch();

            // block further swings for cooldown
            isPunching = true;
            StartCoroutine(SwingCooldown(cooldown));  // new comment: begin lock period

            // lock rotation for the duration of the swing
            punchRotation = modelTransform.rotation;
            Vector3 camF = cameraTransform.forward; camF.y = 0f;
            Vector3 camR = cameraTransform.right;   camR.y = 0f;
            camF.Normalize(); camR.Normalize();
            punchDirection = (camF * v + camR * h).normalized;
        }

        // jump
        if (Input.GetButtonDown("Jump") && readyToJump && grounded) {
            readyToJump = false;
            animator.SetTrigger("JumpUp");
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            float jf = stats != null ? stats.jumpForce : 5f;
            rb.AddForce(Vector3.up * jf, ForceMode.Impulse);
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // movement & speed blending
        Vector3 forward = cameraTransform.forward; forward.y = 0f;
        Vector3 right   = cameraTransform.right; right.y   = 0f;
        forward.Normalize(); right.Normalize();
        Vector3 inputDir = isPunching ? punchDirection : (forward * v + right * h).sqrMagnitude > 0.001f ? (forward * v + right * h).normalized : Vector3.zero;

        // Update animator Speed (zero while swinging)
        if (isPunching) {
            animator.SetFloat("Speed", 0f, 0f, Time.deltaTime);
        }
        else {
            float speedPercent = inputDir.magnitude > 0f ? (isRunning ? 1f : 0.5f) : 0f;
            animator.SetFloat("Speed", speedPercent, 0f, Time.deltaTime);
        }

        // Rotate model transform
        if (isPunching) {
            modelTransform.rotation = punchRotation;
        }
        else if (inputDir.magnitude > 0f) {
            Quaternion targetRot = Quaternion.LookRotation(inputDir);
            modelTransform.rotation = Quaternion.Slerp(modelTransform.rotation, targetRot, Time.deltaTime / rotationSmoothTime);
        }

        // Apply movement if not swinging
        if (!isPunching) {
            rb.MovePosition(rb.position + inputDir * (isRunning ? runSpeed : walkSpeed) * Time.deltaTime);
        }
    }

    private void ResetJump() => readyToJump = true;  // done jumping, reset flag

    private void DoPunch() {
        float dmg = stats != null ? stats.punchDamage : 10f;
        var eq2 = FindObjectOfType<EquipmentManager>();
        if (eq2 != null && eq2.GetEquipped() != null)
            dmg += eq2.GetEquipped().damageBonus;

        Vector3 origin = modelTransform.position +
                         modelTransform.forward * (punchRange * 0.5f);
        foreach (var c in Physics.OverlapSphere(origin, punchRange))
            c.GetComponent<Harvestable>()?.Harvest(dmg);
    }

    // This coroutine makes the full swing happen before the player can attack again
    private IEnumerator SwingCooldown(float duration) {
        yield return new WaitForSeconds(duration);  // wait weapon-specific cooldown
        isPunching = false;                         // re-enable attacks
        Debug.Log("Swing cooldown complete");
    }

    void OnCollisionEnter(Collision col) {
        if (((1 << col.gameObject.layer) & groundLayer) != 0) {
            grounded = true;   // landed on ground
        }
    }

    void OnCollisionExit(Collision col) {
        if (((1 << col.gameObject.layer) & groundLayer) != 0) {
            grounded = false;  // left ground
        }
    }
}
