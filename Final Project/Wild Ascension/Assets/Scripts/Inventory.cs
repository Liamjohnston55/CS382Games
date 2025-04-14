using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {
    // Use dictionary instead of array so we can store the resource and the amount gained as a pair
    public Dictionary<string, int> resources = new Dictionary<string, int>();

    // Adds the resource to the players inventory 
    public void AddResource(string resourceName, int amount) {
        if (resources.ContainsKey(resourceName)) {
            resources[resourceName] += amount;
        }
        else {
            resources.Add(resourceName, amount);
        }
        // Debug.Log("Added " + amount + " " + resourceName + ". Total: " + resources[resourceName]);
    }
}
