using UnityEngine;
using System.Collections;

public class Harvestable : MonoBehaviour
{
    public enum ResourceType { Stone, Wood }
    public ResourceType resourceType = ResourceType.Stone;  // Set what type of resource to harvest for the inventory
    public int resourceYield = 5;               // How much of the resource you get
    public float health = 50f;                  // Resources hitpoints 
    public float damagePerHit = 10f;            // Damage applied per punch (edit later for other weapons)
    public GameObject harvestEffect;            // add effect when broken later

    // set the amount of seconds you want before a resource responds
    public float respawnTime = 300f;  

    // Store the object's initial health for when it is respawned and needs that value again
    private float initialHealth;

    void Awake() {
        // Save the starting health for later reset
        initialHealth = health;
    }

    // Call this method when the object is harvested
    public void Harvest() {
        // change later
        health -= damagePerHit;
        if (health <= 0f)
        {
            // Add resource amount to the player's inventory 
            Inventory playerInventory = FindObjectOfType<Inventory>();
            if (playerInventory != null) {
                playerInventory.AddResource(resourceType.ToString(), resourceYield);
            }

            // Add harvest effect later
            if (harvestEffect != null) {
                Instantiate(harvestEffect, transform.position, Quaternion.identity);
            }
            
            // disable the object until the respawn time has passed 
            if (RespawnResources.Instance != null) {
                RespawnResources.Instance.StartRespawn(gameObject, respawnTime, initialHealth);
            }
            else {
                // failsafe incase anything breaks 
                gameObject.SetActive(false);
            }
        }
    }
}
