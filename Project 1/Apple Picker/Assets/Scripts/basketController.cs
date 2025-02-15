using UnityEngine;

public class BasketController : MonoBehaviour
{
    // Start is called before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialization code (if any) goes here
    }

    void OnCollisionEnter(Collision collisionInfo)
    {
        if (collisionInfo.gameObject.CompareTag("Apple"))
        {
            print("You got an apple!");
        }
        Destroy(collisionInfo.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        // Per-frame logic (if any) goes here
    }
}

