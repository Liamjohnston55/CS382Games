using UnityEngine;

public class Slingshot : MonoBehaviour
{
    [Header("Drag all objects here:")]
    public GameObject projectilePrefab;           // The ball prefab
    public Transform spawnPoint;                  // Where to spawn the ball prefab

    [Header("Slingshot Settings")]
    public float maxPullDistance = 2f;           // Max distance the ball can be pulled

    [Header("Launch Settings")]
    public float launchForceMultiplier = 5f;     // Multiplier that scales how strong the launch force is

    private GameObject currentProjectile;
    private Rigidbody2D currentProjectileRb;
    private bool isDragging = false;

    void Update() {
        // Check for mouse down
        if (Input.GetMouseButtonDown(0)) {
            // Grab the mouses cordinates and use raycast for the slingshot 
            // The raycast will detect if the slingshot is used
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

            // When the slingshot is selected, spawn a ball
            if (hit.collider != null && hit.collider.gameObject == this.gameObject) {
                SpawnProjectile();
                isDragging = true;
            }
        }

        //  check to see if the ball is being launched
        if (isDragging && currentProjectile != null) {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = mousePos - (Vector2)spawnPoint.position;

            // This will make sure we cant pull the slingshot past the max pull distance
            if (direction.magnitude > maxPullDistance) {
                direction = direction.normalized * maxPullDistance;
            }

            // This will move the projectile to its new user-defined position when being held onto
            currentProjectile.transform.position = (Vector2)spawnPoint.position + direction;
        }

        // When the mouse button is released, launch the ball
        if (Input.GetMouseButtonUp(0) && isDragging) {
            if (currentProjectileRb != null) {
                // LaunchDir will store the the angle at which the ball is launched
                // Distance will be storing how powerful our shot is
                Vector2 launchDir = (Vector2)spawnPoint.position - (Vector2)currentProjectile.transform.position;
                float distance = launchDir.magnitude;

                // Change the ball to dynamic so it can affect the blocks from blocking its path
                currentProjectileRb.bodyType = RigidbodyType2D.Dynamic;

                // Launch the ball using the direction, distance, and power, the forceMode makes sure that the ball is dynamic and can affect the world around it 
                currentProjectileRb.AddForce(launchDir.normalized * distance * launchForceMultiplier, ForceMode2D.Impulse);
            }

            // When we are not dragging a ball, do not show anything 
            currentProjectile = null;
            currentProjectileRb = null;
            isDragging = false;
        }
    }

    private void SpawnProjectile() {
        // Create the projectile at the spawn point
        currentProjectile = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);
        currentProjectileRb = currentProjectile.GetComponent<Rigidbody2D>();

        // make it kinematic so it does not fall when being dragged
        currentProjectileRb.bodyType = RigidbodyType2D.Kinematic;
    }
}
