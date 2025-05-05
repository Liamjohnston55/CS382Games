using UnityEngine;
using System.Collections;

public class Harvestable : MonoBehaviour {
    public enum ResourceType { Stone, Wood }
    public ResourceType resourceType = ResourceType.Stone;  // Set what type of resource to harvest
    public int resourceYield = 5;                           // How much of the resource you get
    public int xpReward      = 10;                          // XP awarded on harvest
    public float health        = 50f;                       // Resources hit points 
    public float damagePerHit  = 10f;                       // no weapon equipped
    public GameObject harvestEffect;                        // Effect when broken (may use may not depending on needed resources)

    public float respawnTime = 300f;                        // time in seconds it takes to re-spawn the resource

    private float initialHealth;                            // initial health of the resource

    void Awake() {
        initialHealth = health; // set the object's health
    }

    public void Harvest(){
        Harvest(damagePerHit); // no-weapon harvest
    }

    public void Harvest(float damage) {
        health -= damage;         // apply bonus damage from weapon
        if (health <= 0f) {
            // give resources
            Inventory playerInventory = FindObjectOfType<Inventory>();
            if (playerInventory != null) {
                playerInventory.AddResource(resourceType.ToString(), resourceYield);
            }

            // give XP
            PlayerStats stats = FindObjectOfType<PlayerStats>();
            if (stats != null) {
                stats.AddXP(xpReward);
            }

            // harvest effect (may use may not depending on needed resource)
            if (harvestEffect != null) {
                Instantiate(harvestEffect, transform.position, Quaternion.identity);
            }

            // respawn logic (un-hide the object in respawnTime amount of timne)
            if (RespawnResources.Instance != null) {
                RespawnResources.Instance.StartRespawn(gameObject, respawnTime, initialHealth);
            }
            else {
                gameObject.SetActive(false);
            }
        }
    }
}
