using UnityEngine;
using System.Collections;

public class CollisionDetections : MonoBehaviour
{
    private bool hasCollided = false;

    void OnCollisionEnter2D(Collision2D collision) {
        if (!hasCollided)
        {
            hasCollided = true;
            //Debug.Log("Projectile collided with: " + collision.gameObject.name);

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

        //Debug.Log("Destroying projectile after collision delay");
        Destroy(gameObject);
    }
}
