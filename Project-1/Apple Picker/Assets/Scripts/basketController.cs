using UnityEngine;
using ApplePicker; 

public class BasketController : MonoBehaviour
{
    void Start() {
       
    }

    void Update() { 
        // Mouse Based movement script

        // get the current position of the mouse 
        Vector3 mousePos2D = Input.mousePosition;

        // link the z-axis with the camera (basket does not appear w/o this do not remove)
        mousePos2D.z = -Camera.main.transform.position.z;

        // convert the x-space to 2D to only move from left to right
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);
        Vector3 pos = transform.position;

        // replace basket's x-value with the mouse's
        pos.x = mousePos3D.x;
        // applies mouse position to the basket
        transform.position = pos;
    }

    void OnCollisionEnter(Collision collisionInfo) {
        if (collisionInfo.gameObject.CompareTag("Apple")) {
            print("You got an apple!");
        
            // This will let the gameManager file know that an apple has collided with the basket
            GameManager gm = FindObjectOfType<ApplePicker.GameManager>();
            if (gm != null)
                gm.AppleCaught();
            Destroy(collisionInfo.gameObject);
        }
        else if (collisionInfo.gameObject.CompareTag("Stick")) {
            print("You got a stick!");

            // This will let the gameManager file know that a stick has collided with the basket
            GameManager gm = FindObjectOfType<ApplePicker.GameManager>();
            if (gm != null)
                gm.HealthDecrease();
            Destroy(collisionInfo.gameObject);
        }
    }
}

