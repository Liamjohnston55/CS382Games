using UnityEngine;

public class Slingshot : MonoBehaviour {
    [Header("Drag all objects here:")]
    public GameObject projectilePrefab;           // The ball prefab
    public Transform spawnPoint;                  // Where to spawn the ball prefab
    public GameObject slingshotCam;               // Camera that follows the ball

    [Header("Slingshot Settings")]
    public float maxPullDistance = 2f;            // Max distance the ball can be pulled

    [Header("Launch Settings")]
    public float launchForceMultiplier = 5f;      // Multiplier that scales how strong the launch force is

    public CameraManager cameraManager;
    private GameObject currentProjectile;
    private Rigidbody2D currentProjectileRb;
    private bool isDragging = false;

    public LineRenderer rubberBand;    // for the rubber band effect
    public AudioSource stretchSource; // Sound does not play properly without this 
    public AudioClip stretchClip;     // The sound that loops while dragging

    void Update() {
        // Check for mouse down
        if (Input.GetMouseButtonDown(0)) {
            Camera activeCam = cameraManager.GetActiveCamera();

            // debugging----------------------------------------------------------------------
            // Debug.Log("Active camera: " + activeCam);
            // if (activeCam == null) {
            //     Debug.LogWarning("No active camera found!");
            //     return;
            // }
            // debugging----------------------------------------------------------------------

            // Grab the mouse's coordinates and use raycast for the slingshot 
            // The raycast will detect if the slingshot is used
            Vector2 mouseWorldPos = activeCam.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

            // debugging----------------------------------------------------------------------
            // Debug.Log("Mouse clicked at: " + mouseWorldPos);
            // if (hit.collider != null) {
            //     Debug.Log("Hit collider: " + hit.collider.gameObject.name);
            // } else {
            //     Debug.Log("No collider hit!");
            // }
            // debugging----------------------------------------------------------------------

            // When the slingshot is selected, spawn a ball
            if (hit.collider != null && hit.collider.gameObject == this.gameObject) {
                // debugging
                // Debug.Log("Mouse button down detected!");
                // debugging
                SpawnProjectile();
                isDragging = true;

                // NEW: Start the stretch sound loop (if we have an AudioSource & clip)
                if (stretchSource != null && stretchClip != null) {
                    stretchSource.clip = stretchClip;
                    stretchSource.loop = true;
                    stretchSource.Play();
                }
            }
        }

        //  check to see if the ball is being launched
        if (isDragging && currentProjectile != null) {
            Camera activeCam = cameraManager.GetActiveCamera();
            
            // debugging----------------------------------------------------------------------
            // Debug.Log("Active camera: " + activeCam);
            // if (activeCam == null) {
            //     Debug.LogWarning("No active camera found!");
            //     return;
            // }
            // debugging----------------------------------------------------------------------

            Vector2 mousePos = activeCam.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = mousePos - (Vector2)spawnPoint.position;

            // This will make sure we cant pull the slingshot past the max pull distance
            if (direction.magnitude > maxPullDistance) {
                direction = direction.normalized * maxPullDistance;
            }

            // This will move the projectile to its new user-defined position when being held onto
            currentProjectile.transform.position = (Vector2)spawnPoint.position + direction;

            // turn on the rubber band effect
            if (rubberBand != null) {
                rubberBand.enabled = true; 
                rubberBand.SetPosition(0, spawnPoint.position);
                rubberBand.SetPosition(1, currentProjectile.transform.position);
            }
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

                cameraManager.SwitchToFollowCam();
                FollowCam.POI = currentProjectile;

                // Stop playing the stretch sound when we release
                if (stretchSource != null && stretchSource.isPlaying) {
                    stretchSource.Stop();
                }

                // now start the flying sound
                AudioSource ballAudio = currentProjectile.GetComponent<AudioSource>();
                if (ballAudio != null) {
                    ballAudio.loop = true;      
                    ballAudio.Play();           
                }

                // Stop the rubber band line as well
                if (rubberBand != null) {
                    rubberBand.enabled = false;
                }

            }

            FollowCam.POI = currentProjectile;

            // When we are not dragging a ball, do not show anything 
            currentProjectile = null;
            currentProjectileRb = null;
            isDragging = false;
        }
    }

    private void SpawnProjectile() {
        // If lives are less than 0, no more shots allowed.
        if (GameManager.lives == -1) {
            return;
        }
        
        // this is to allow the final shot to be completed before sent to the game over screen
        //if (GameManager.lives == 0) {
        //    GameManager.lives = -1;
        //}
        // normal shots
        //else {
        //     // if the player does not make it into the green zone, subtract a life
        //     // use GameManager.lives so the UI also updates
        //    GameManager.lives--;  
        //}

        GameManager.lives--;

        // Debug.Log("SpawnProjectile called!");
        // spawn the ball
        currentProjectile = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);

        // Get the Rigidbody2D for launching
        currentProjectileRb = currentProjectile.GetComponent<Rigidbody2D>();
        // Set it to Kinematic 
        currentProjectileRb.bodyType = RigidbodyType2D.Kinematic;

        // set up the camera 
        FollowCam.POI = currentProjectile;
    }
}
