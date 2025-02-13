using UnityEngine;

public class BasketMovement : MonoBehaviour
{
    public float speed = 1f; // Speed of the basket movement
    public float boundary = 29f; // Boundary for movement (adjusted for screen width)

    void Update()
    {
        // This will detect if the player hits the left or right keys
        // Left = -1, Right = 1, nothing = 0
        float horizontalInput = Input.GetAxis("Horizontal");

        // Basket movement
        // Move right or left multiplied by the speed
        float move = horizontalInput * speed;

        // Apply movement
        transform.Translate(move, 0, 0);

        // Mathf.Clamp will make sure the basket keep within the specified bounds
        // The other part makes sure that the basket stays in place
        float clampedX = Mathf.Clamp(transform.position.x, -boundary, boundary);
        transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
    }
}
