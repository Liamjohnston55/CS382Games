using UnityEngine;
using System.Collections;

public class RespawnResources : MonoBehaviour {
    public static RespawnResources Instance { get; private set; }  // we use instance, we can call respawn without needing to locate the manager each time 

    void Awake() {
        if (Instance == null){
            Instance = this;
        }
        else {
            Destroy(gameObject); // ensures no duplicate resources 
        }
    }

    public void StartRespawn(GameObject obj, float respawnDelay, float resetHealth) {
        StartCoroutine(RespawnCoroutine(obj, respawnDelay, resetHealth));
    }

    // This function will unhide the resource once the time is up (respawns resources after x seconds)
    private IEnumerator RespawnCoroutine(GameObject obj, float delay, float resetHealth) {
        // Deactivate the object 
        obj.SetActive(false);
        yield return new WaitForSeconds(delay);
        // Reset the object's health 
        Harvestable h = obj.GetComponent<Harvestable>();
        if (h != null) {
            h.health = resetHealth;
        }
        // Reactivate the object 
        obj.SetActive(true);
    }
}
