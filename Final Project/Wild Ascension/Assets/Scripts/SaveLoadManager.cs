using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;  // For AssetDatabase.Refresh() in the Editor
#endif

[Serializable]
public class GameData {
    //Player's stats
    public int level;
    public int currentXP;
    public int xpToNextLevel;
    public float maxHealth;
    public float jumpForce;
    public float punchDamage;

    // Inventory contents
    public List<ResourceRecord> resources = new List<ResourceRecord>();

    // Player transform
    public Vector3 playerPosition;
    public Vector3 playerRotation;

    // Hotbar state
    public List<string> hotbarItems = new List<string>(); 
    public int hotbarSelectedIndex = 0;  // Which slot was active
}

[Serializable]
public class ResourceRecord {
    public string resourceName;
    public int amount;
}

public static class SaveLoadManager {
    // where to save depending on if we're in the Editor or a build
    private static readonly string editorSaveFolder = Application.dataPath + "/Resources/SaveData";         // where the save data is being stored
    private static readonly string editorSaveFilePath  = editorSaveFolder + "/savegame.json";               // name of the saved data   
    private static readonly string runtimeSaveFilePath = Application.persistentDataPath + "/savegame.json"; // makes sure the game writes and reads its save file in builds

    // Choose correct path 
    private static string SaveFilePath {
        get {
            #if UNITY_EDITOR 
                return editorSaveFilePath;
            #else
                return runtimeSaveFilePath;
            #endif
        }
    }

    // Write out JSON of all game data
    public static void SaveGame() {
        #if UNITY_EDITOR
        // Ensure the folder exists in the Editor
        if (!Directory.Exists(editorSaveFolder)) {
            Directory.CreateDirectory(editorSaveFolder);
        }
        #endif

        // Grab PlayerStats & Inventory
        PlayerStats stats = GameObject.FindObjectOfType<PlayerStats>();
        if (stats == null) {
            // debugging
            // Debug.LogError("SaveGame: no PlayerStats found");
            return;
        }
        Inventory inventory = stats.GetComponent<Inventory>();
        if (inventory == null) {
            // debugging 
            // Debug.LogError("SaveGame: no Inventory on player");
            return;
        }

        // put all data into our serializable class
        GameData data = new GameData {
            level         = stats.level,
            currentXP     = stats.currentXP,
            xpToNextLevel = stats.xpToNextLevel,
            maxHealth     = stats.maxHealth,
            jumpForce     = stats.jumpForce,
            punchDamage   = stats.punchDamage,
            playerPosition= stats.transform.position,
            playerRotation= stats.transform.eulerAngles
        };

        // add resources to the list
        data.resources.Clear();
        foreach (var kvp in inventory.resources) {
            data.resources.Add(new ResourceRecord { resourceName = kvp.Key, amount = kvp.Value });
        }

        // save hotbar items
        var hotbarUI = HotbarUI.Instance;  
        if (hotbarUI != null) {
            data.hotbarItems.Clear();
            foreach (var name in hotbarUI.GetHotbarItemNames()) {
                data.hotbarItems.Add(name);
            }
            data.hotbarSelectedIndex = hotbarUI.GetSelectedIndex();
        }

        // Write JSON to disk (use try so if it doesn't exit, we will create it)
        try {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(SaveFilePath, json);
            // debugging
            // Debug.Log($"Saved game to {SaveFilePath}");

            #if UNITY_EDITOR
            AssetDatabase.Refresh(); // so the new file shows under Assets/Resources
            #endif
        } 
        catch (Exception ex) {
            // debugging
            // Debug.LogError("SaveGame error: " + ex.Message);
        }
    }

    // Read the save file and apply it to PlayerStats, Inventory, and hotbar
    public static bool LoadGame() {
        if (!File.Exists(SaveFilePath)) {
            //debugging
            // Debug.LogWarning($"LoadGame: no save file at {SaveFilePath}");
            return false;
        }

        try {
            string json = File.ReadAllText(SaveFilePath);
            GameData data = JsonUtility.FromJson<GameData>(json);
            if (data == null) {
                // debugging 
                // Debug.LogError("LoadGame: failed to parse JSON");
                return false;
            }

            // Find PlayerStats & Inventory
            PlayerStats stats = GameObject.FindObjectOfType<PlayerStats>();
            if (stats == null) {
                // debugging
                // Debug.LogError("LoadGame: no PlayerStats in scene");
                return false;
            }
            Inventory inventory = stats.GetComponent<Inventory>();
            if (inventory == null) {
                // debugging
                // Debug.LogError("LoadGame: no Inventory on player");
                return false;
            }

            // reload stats
            stats.level = data.level;
            stats.currentXP = data.currentXP;
            stats.xpToNextLevel = data.xpToNextLevel;
            stats.maxHealth = data.maxHealth;
            stats.jumpForce = data.jumpForce;
            stats.punchDamage = data.punchDamage;

            // reload inventory
            inventory.resources.Clear();
            foreach (var r in data.resources) {
                inventory.resources[r.resourceName] = r.amount;
            }

            // move the player to saved position
            stats.transform.position = data.playerPosition;
            stats.transform.eulerAngles = data.playerRotation;

            // reload hotbar
            var hotbarUI = HotbarUI.Instance;  
            if (hotbarUI != null && data.hotbarItems != null) {
                hotbarUI.SetHotbarFromNames(data.hotbarItems.ToArray());
                hotbarUI.SelectSlotIndex(data.hotbarSelectedIndex);
            }

            //debugging
            // Debug.Log("Loaded game from save file.");
            return true;
        } 
        catch (Exception ex) {
            // debugging
            // Debug.LogError("LoadGame error: " + ex.Message);
            return false;
        }
    }

    // Delete the save file, Editor or runtime
    public static void DeleteSave() {
        if (File.Exists(SaveFilePath)) {
            try {
                File.Delete(SaveFilePath);
                // debugging
                Debug.Log("Deleted save file at " + SaveFilePath);
                #if UNITY_EDITOR
                AssetDatabase.Refresh();
                #endif
            } 
            catch (Exception ex) {
                // debugging
                // Debug.LogError("DeleteSave error: " + ex.Message);
            }
        } else {
            // debugging
            // Debug.Log("DeleteSave: no file to delete");
        }
    }

    
}
