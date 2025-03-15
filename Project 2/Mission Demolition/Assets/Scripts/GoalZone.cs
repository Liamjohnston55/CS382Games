using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalZone : MonoBehaviour {
    private void OnTriggerEnter2D(Collider2D other) {
        // only trigger if the ball has entered the zone
        if (other.CompareTag("Ball")) {
            Debug.Log("Ball entered the goal zone!");
            GameManager.levelCompleted = true;
            GameManager.Instance.CompleteLevel();
            // Destroy the ball so its collision script does not later call GameOver at level three
            Destroy(other.gameObject);
        }
    }
}
