using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject apple;

    // Start is called before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating(nameof(AppleFall), 1f, 1f);
    }

    void AppleFall()
    {
        Instantiate(apple, new Vector3(Random.Range(-8f, 8f), 5f, 0f), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        // Per-frame logic (if any) goes here
    }
}
