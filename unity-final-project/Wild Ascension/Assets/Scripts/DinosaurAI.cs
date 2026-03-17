using UnityEngine;
using UnityEngine.AI;

public class DinosaurAI : MonoBehaviour {
    [Header("References")]
    public Transform playerTarget;              // Drag your moving player “Idle” child Transform here

    [Header("Detection & Attack Settings")]
    public float detectionRadius = 30f;         // how far until the dino detects you
    public float attackRange = 2f;              // How close to the player before swinging
    public float attackCooldown = 2f;           // Seconds between attacks
    public float chaseSpeed = 3f;               // Run speed
    public float damage = 20f;                  // Damage per hit

    private NavMeshAgent agent;
    private Animator animator;
    private float nextAttackTime;
    private enum State { Idle, Chase, Attack }
    private State state = State.Idle;

    void Start() {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;           // rotating manually below
        agent.updatePosition  = true;
        agent.stoppingDistance = attackRange;   // stop right at swing distance
        agent.speed = chaseSpeed;
        agent.autoTraverseOffMeshLink = true;   

        animator = GetComponent<Animator>();
        animator.SetFloat("Speed", 0f);         // start idle
    }

    void Update() {
        if (playerTarget == null) {
            // If player is not in radius do nothing
            return;  
        }

        float dist = Vector3.Distance(transform.position, playerTarget.position);

        switch (state) {
            case State.Idle:
                // look around for player
                if (dist <= detectionRadius) {
                    state = State.Chase;
                    // debugging
                    // Debug.Log("[DinoAI] Player detected – starting chase");
                }
                animator.SetFloat("Speed", 0f);
                break;

            case State.Chase:
                if (dist <= attackRange) {
                    state = State.Attack;
                }
                else {
                    // chase
                    if (agent.isStopped) {
                        agent.isStopped = false;
                    }
                    agent.SetDestination(playerTarget.position);
                    FaceTarget();
                    animator.SetFloat("Speed", 1f);

                    // fallback if stuck
                    if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && agent.velocity.sqrMagnitude < 0.1f){
                        Vector3 dir = (playerTarget.position - transform.position).normalized;
                        transform.position += dir * chaseSpeed * Time.deltaTime;
                    }
                }
                break;

            case State.Attack:
                if (dist > attackRange) {
                    // player is not in attack range
                    agent.isStopped = false;
                    state = State.Chase;
                }
                else {
                    // in attack range
                    if (!agent.isStopped) agent.isStopped = true;
                    FaceTarget();
                    animator.SetFloat("Speed", 0f);

                    // randomly pick one of two swing animations
                    if (Time.time >= nextAttackTime) {
                        nextAttackTime = Time.time + attackCooldown;
                        if (Random.value > 0.5f) {
                            animator.SetTrigger("Attack1");
                        }
                        else {
                            animator.SetTrigger("Attack2");
                        }

                        var ps = playerTarget.GetComponent<PlayerStats>();
                        if (ps != null) ps.TakeDamage(damage);

                        // Debugging
                        // Debug.Log("[DinoAI] ATTACK");
                    }
                }
                break;
        }
    }

    void FaceTarget() {
        Vector3 dir = playerTarget.position - transform.position;
        dir.y = 0f; // don't tilt up/down
        if (dir.sqrMagnitude > 0.01f) {
            Quaternion look = Quaternion.LookRotation(dir.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, Time.deltaTime * 5f);
        }
    }

    #if UNITY_EDITOR
        void OnDrawGizmosSelected() {
            // draw detection and attack spheres for debugging
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    #endif
}
