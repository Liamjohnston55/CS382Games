using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalZone : MonoBehaviour {
    private void OnTriggerEnter2D(Collider2D other) {
        // only trigger if the ball has enter the zone
        if (other.CompareTag("Ball")) {
            Debug.Log("Ball entered the goal zone!");
            GameManager.Instance.CompleteLevel();
        }
    }
}

