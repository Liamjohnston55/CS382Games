using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
    public static GameManager S;  // Singleton for GameManager

    [Header("Set in Inspector")]
    public GameObject[] prefabEnemies; // e.g., Size=4 for [Base, Enemy_1, Enemy_2, Enemy_3]
    public float enemySpawnPerSecond = 0.5f;  // # enemies to spawn per second
    public float enemyDefaultPadding = 1.5f;  // Padding to keep them within screen horizontally

    public WeaponDefinition[] weaponDefinitions; // For weapon stats (blaster, spread, etc.)

    static private Dictionary<WeaponType, WeaponDefinition> weaponDict;
    private BoundsCheck bndCheck;

    void Awake() {
        // Set Singleton
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
    }

    void SpawnEnemy() {
        // 1) Pick a random enemy from the array
        int ndx = Random.Range(0, prefabEnemies.Length);
        GameObject go = Instantiate(prefabEnemies[ndx]) as GameObject;

        // 2) Position it above the screen at a random X position
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

        // 3) Schedule the next spawn
        Invoke("SpawnEnemy", 1f / enemySpawnPerSecond);
    }

    // Look up a WeaponDefinition by its WeaponType
    static public WeaponDefinition GetWeaponDefinition(WeaponType wt) {
        if (weaponDict.ContainsKey(wt)) {
            return weaponDict[wt];
        }
        // Return a default “empty” weapon definition if not found
        return new WeaponDefinition();
    }
}
