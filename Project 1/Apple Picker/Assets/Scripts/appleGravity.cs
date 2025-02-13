using UnityEngine;

public class AppleMechs : MonoBehaviour 
{
    // speed at which the apples will fall
    // this will need to be increased if we want harder levels
    public float fallSpeed = 5f; 

    // This will be the point at which the apple is destroyed
    public float destroyHeight = -5f; 

    void Update() {
        // Apply gravity to the apple
        transform.Translate(Vector3.down * fallSpeed);

        // Constantly checking if the apple has reached the destroy point
        if (transform.position.y < destroyHeight)
        {
            Destroy(gameObject);
        }
    }
}
