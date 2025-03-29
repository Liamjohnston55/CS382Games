using UnityEngine;

public class DebugMovement : MonoBehaviour
{
    public Animator animator;

    void Update()
    {
        if (animator == null)
        {
            Debug.LogError("Animator is NULL! Did you assign it in the Inspector?");
            return;
        }

        // Force Speed to 0.5f every frame
        animator.SetFloat("Speed", 0.5f);
        Debug.Log("Setting Speed to 0.5f");
    }
}
