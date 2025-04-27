using UnityEngine;

public class Health : MonoBehaviour {
    public float currentHealth = 100f;

    public void TakeDamage(float amount) {
        currentHealth -= amount;
        if (currentHealth <= 0) {
            Die();
        }
    }

    private void Die() {
        // Handle object destruction or harvesting logic here
        Destroy(gameObject);
    }
}
