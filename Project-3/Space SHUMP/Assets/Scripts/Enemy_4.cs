// Enemy randomly finds a spot on the screen to shoot from then goes off screen
using UnityEngine;

public class Enemy_4 : Enemy {
    [Header("Set in Inspector: Enemy_4")]
    public float entranceDuration = 3f;   // Time to move from spawn position to the center
    public float pauseDuration = 2f;      // Time to pause in the center of the screen
    public float exitDuration = 3f;       // Time to move from the center to offscreen on the opposite side
    public float sideOffset = 5f;         // How far offscreen horizontally the enemy spawns

    // Internal timing and position variables
    private float birthTime;
    private Vector3 startPos;
    private Vector3 centerPos;
    private Vector3 exitPos;

    void Start() {
        birthTime = Time.time;
        
        // Get camera bounds from the attached BoundsCheck component
        BoundsCheck bc = GetComponent<BoundsCheck>();
        float camWidth = bc.camWidth;
        float camHeight = bc.camHeight;

        // Choose a random vertical position in the middle of the screen for the enemy
        float spawnY = Random.Range(-camHeight * 0.3f, camHeight * 0.3f);

        // Randomly decide whether to spawn from the left or right side.
        bool spawnFromLeft = Random.value < 0.5f;

        if (spawnFromLeft) {
            startPos = new Vector3(-camWidth - sideOffset, spawnY, 0);
            exitPos  = new Vector3(camWidth + sideOffset, spawnY, 0);
        } else {
            startPos = new Vector3(camWidth + sideOffset, spawnY, 0);
            exitPos  = new Vector3(-camWidth - sideOffset, spawnY, 0);
        }
        // The center position is the midpoint at the chosen y
        centerPos = new Vector3(0, spawnY, 0);

        // Set initial position to startPos
        pos = startPos;
    }

    public override void Move() {
        float elapsed = Time.time - birthTime;
        float totalDuration = entranceDuration + pauseDuration + exitDuration;
        
        // when done with path kill self
        if (elapsed > totalDuration) {
            Destroy(gameObject);
            return;
        }
        
        Vector3 newPos;
        if (elapsed < entranceDuration) {
            // move from startPos to centerPos over entranceDuration seconds
            float u = elapsed / entranceDuration;
            newPos = Vector3.Lerp(startPos, centerPos, u);
        }
        else if (elapsed < entranceDuration + pauseDuration) {
            // remain at centerPos during the pause
            newPos = centerPos;
        }
        else {
            // move from centerPos to exitPos over exitDuration seconds
            float u = (elapsed - entranceDuration - pauseDuration) / exitDuration;
            newPos = Vector3.Lerp(centerPos, exitPos, u);
        }
        pos = newPos;
    }
}
