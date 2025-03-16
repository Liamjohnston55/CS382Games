using UnityEngine;

public class ExtraShotZone : MonoBehaviour
{
    [Header("Extra Shots Zone")]
    public int extraShots = 3; // How many extra balls to give
    bool destroyOnUse = true; 

    private void OnTriggerEnter2D(Collider2D other) {
        // Only grant extra shots if the ball enters
        if (other.CompareTag("Ball")) {
            Debug.Log("Ball hit the PowerUpZone! Gaining " + extraShots + " extra shots.");

            // Add extra lives to the player
            GameManager.lives += extraShots;

            // destroy on use 
            if (destroyOnUse) {
                Destroy(gameObject);
            }
        }
    }
}
