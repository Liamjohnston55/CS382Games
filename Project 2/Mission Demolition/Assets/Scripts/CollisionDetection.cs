using UnityEngine;
using System.Collections;

// This script is used to tell the follow camera when to transfer back to the orginial camera
public class CollisionDetections : MonoBehaviour {
    private bool hasCollided = false;

    void OnCollisionEnter2D(Collision2D collision) {
        if (!hasCollided) {
            hasCollided = true;
            //Debug.Log("ball hit: " + collision.gameObject.name);

            // start the timer then switch back
            StartCoroutine(SwitchBackAndDestroyBall(3f));
        }
    }

    // IEnumerator allows us to tell unity this method is a coroutine
    // Since it is a coroutine, we can delay the function calls using seconds 
    private IEnumerator SwitchBackAndDestroyBall(float delay)
    {
        // yield allows us to wait a specified amount of time before calling the next function
        yield return new WaitForSeconds(delay);

        // timer is up, so switch back to the main camera and destroy the ball
        CameraManager camMan = FindObjectOfType<CameraManager>();
        if (camMan != null) {
            camMan.SwitchBackFromFollowCam();
        }

        // if the player has no more lives call the ending scene 
        if (GameManager.lives < 0) {
            GameManager.Instance.GameOver();
        }

        //Debug.Log("Destroying projectile after collision delay");
        Destroy(gameObject);
    }
}
