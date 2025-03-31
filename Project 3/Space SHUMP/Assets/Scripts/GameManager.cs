using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager S;  // Singleton for GameManager

    [Header("Set in Inspector")]
    public GameObject[] prefabEnemies;          // the array of enemy's 
    public float enemySpawnPerSecond = 0.5f;    // number of enemies to spawn per second
    public float enemyDefaultPadding = 1.5f;    // offset to keep them on screen 

    public WeaponDefinition[] weaponDefinitions; // For weapon stats 

    // NEW: For power-up spawning
    public GameObject[] prefabPowerUps;
    public float powerUpSpawnInterval = 15f;

    static private Dictionary<WeaponType, WeaponDefinition> weaponDict;
    private BoundsCheck bndCheck;

    void Awake() {
        S = this;

        // Grab BoundsCheck if it exists on the same GameObject
        bndCheck = GetComponent<BoundsCheck>();

        // Build a dictionary from weaponDefinitions, so we can look up stats by WeaponType
        weaponDict = new Dictionary<WeaponType, WeaponDefinition>();
        foreach (WeaponDefinition def in weaponDefinitions) {
            weaponDict[def.type] = def;
        }

        // Start spawning enemies
        Invoke("SpawnEnemy", 1f / enemySpawnPerSecond);

        // NEW: Start spawning power-ups
        InvokeRepeating("SpawnPowerUp", 5f, powerUpSpawnInterval);
    }

    void SpawnEnemy() {
        // Pick a random enemy from the array
        int ndx = Random.Range(0, prefabEnemies.Length);
        GameObject go = Instantiate(prefabEnemies[ndx]) as GameObject;

        // Position it above the screen at a random X position
        float enemyPadding = enemyDefaultPadding;
        BoundsCheck enemyBC = go.GetComponent<BoundsCheck>();
        if (enemyBC != null) {
            enemyPadding = Mathf.Abs(enemyBC.radius);
        }

        Vector3 pos = Vector3.zero;
        float xMin = -bndCheck.camWidth + enemyPadding;
        float xMax =  bndCheck.camWidth - enemyPadding;
        pos.x = Random.Range(xMin, xMax);
        pos.y = bndCheck.camHeight + enemyPadding;
        go.transform.position = pos;

        // Schedule the next spawn
        Invoke("SpawnEnemy", 1f / enemySpawnPerSecond);
    }

    // NEW: Spawn a power-up from the prefabPowerUps array
    void SpawnPowerUp() {
        if (prefabPowerUps.Length == 0) return;
        int ndx = Random.Range(0, prefabPowerUps.Length);
        GameObject pu = Instantiate(prefabPowerUps[ndx]) as GameObject;
        // Position it above the screen at a random X position
        float padding = enemyDefaultPadding;
        Vector3 pos = Vector3.zero;
        float xMin = -bndCheck.camWidth + padding;
        float xMax = bndCheck.camWidth - padding;
        pos.x = Random.Range(xMin, xMax);
        pos.y = bndCheck.camHeight + padding;
        pu.transform.position = pos;
    }

    // Look up a WeaponDefinition by its WeaponType
    public static WeaponDefinition GetWeaponDefinition(WeaponType wt) {
        if (!weaponDict.ContainsKey(wt)) {
            return new WeaponDefinition(); 
        }
        WeaponDefinition foundDef = weaponDict[wt];
        return foundDef;
    }

    public static void ReloadSceneAfterDelay(float delay) {
        // We call a static coroutine on S
        // this is so the game will restart after 5 seconds
        S.StartCoroutine(ReloadSceneCoroutine(delay));
    }

    private static IEnumerator ReloadSceneCoroutine(float delay) {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
