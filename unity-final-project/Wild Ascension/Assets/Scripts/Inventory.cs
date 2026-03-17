using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {
    // Use dictionary instead of array so we can store the resource and the amount gained as a pair
    public Dictionary<string, int> resources = new Dictionary<string, int>();

    // Adds the resource to the players inventory 
    public void AddResource(string resourceName, int amount) {
        // already have the resource, so add the amount to total
        if (resources.ContainsKey(resourceName)) {
            resources[resourceName] += amount;
        }
        // do not have resource, so add it to the inventory and popululate the slot and amount
        else {
            resources.Add(resourceName, amount);
        }
        // Debug.Log("Added " + amount + " " + resourceName + ". Total: " + resources[resourceName]);
    }

    // check to see if player has at least the amount of the given resource 
    public bool HasResource(string resourceName, int amount) {
        return resources.ContainsKey(resourceName) && resources[resourceName] >= amount;
    }
    
}
