using UnityEngine;
using TMPro;  
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    public Inventory inventory;         // inventory script reference 
    public GameObject inventoryPanel;   // Inventory's pannel onscreen 
    public TMP_Text inventoryText;      

    // Key to access the players inventory
    public KeyCode inventoryKey = KeyCode.I;

    private bool isOpen = false;

    void Start() {
        // This makes sure the inventory is not visible when the game is first loaded
        if (inventoryPanel != null) {
            inventoryPanel.SetActive(false);
        }
    }

    void Update() {
        // Access inventory when the inventory key is pressed
        if (Input.GetKeyDown(inventoryKey)) {
            ToggleInventory();
        }
    }

    void ToggleInventory() {
        isOpen = !isOpen;
        inventoryPanel.SetActive(isOpen);
        if (isOpen) {
            UpdateInventoryDisplay();
        }
    }

    public void UpdateInventoryDisplay() {
        string display = "";
        foreach (KeyValuePair<string, int> kvp in inventory.resources)
        {
            display += kvp.Key + ": " + kvp.Value + "\n";
        }
        inventoryText.text = display;
    }
}
