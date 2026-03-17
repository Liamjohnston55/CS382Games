using UnityEngine;
using System;
using System.Collections;       

[RequireComponent(typeof(Inventory))]
public class PlayerStats : MonoBehaviour {
    public int level = 1;
    public int currentXP = 0;
    public int xpToNextLevel = 100;
    public int maxLevel = 100;

    [Header("Base Traits (at level 1)")]
    public float maxHealth = 100f;          // my HP when I spawn
    public float jumpForce = 5f;            // jump height
    public float punchDamage = 10f;         // default damage

    [Header("Runtime Stats")]
    public float currentHealth;             // tracks current health

    [Header("Growth per Level")]
    public float healthPerLevel = 20f;
    public float jumpPerLevel   = 0.5f;
    public float damagePerLevel = 2f;

    [Header("Respawn Settings")]    
    public Transform spawnPoint;            // assign your spawn/respawn Transform here
    public float deathAnimDuration = 4f;    // length of your death clip in seconds

    public event Action<int> OnLevelUp;
    public event Action<int,int> OnXPChanged;

    Animator animator; // death animation

    void Awake() {
        currentHealth = maxHealth;           // start with full health
        animator = GetComponent<Animator>(); // grab Animator
    }

    public void AddXP(int amount) {
        currentXP += amount;
        // tell UI about XP change
        OnXPChanged?.Invoke(currentXP, xpToNextLevel);
        // in case the player get enough XP to level up more than once
        while (currentXP >= xpToNextLevel && level < maxLevel) {
            currentXP -= xpToNextLevel;
            LevelUp();
            OnXPChanged?.Invoke(currentXP, xpToNextLevel);
        }
    }

    private void LevelUp() {
        level++;
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.2f);
        maxHealth += healthPerLevel;
        jumpForce += jumpPerLevel;
        punchDamage += damagePerLevel;
        currentHealth = maxHealth;
        OnLevelUp?.Invoke(level);
        // debugging
        // Debug.Log($"Leveled up! Now level {level}");
    }

    public void TakeDamage(float amount) {
        currentHealth -= amount;
        // debugging 
        // Debug.Log($"Took {amount} damage. Health now {currentHealth}/{maxHealth}");
        if (currentHealth <= 0f) {
            Die();    // call death handler
        }
    }

    void Die() {
        // Play death animation
        if (animator != null) {
            animator.SetTrigger("Die");  
        }
        //// Start coroutine to wait and respawn
        StartCoroutine(RespawnAfterDeath());  
    }

    IEnumerator RespawnAfterDeath() {
        // Wait for the death animation to finish
        yield return new WaitForSeconds(deathAnimDuration);
        
        // Teleport to spawn
        if (spawnPoint != null) {
            transform.position = spawnPoint.position;
            transform.rotation = spawnPoint.rotation; 
        } 
        else {
            // debugging
            // Debug.LogWarning("SpawnPoint not set on PlayerStats!");
        }

        // Reset health
        currentHealth = maxHealth;
        // debugging
        // Debug.Log("Player respawned.");
    }
}
