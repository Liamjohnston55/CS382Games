using UnityEngine;

public class InventoryUI : MonoBehaviour {
    [Header("References")]
    public GameObject inventoryPanel;   // drag your InventoryPanel (GameObject) here 
    public KeyCode toggleKey = KeyCode.I; // Key to open/close the players inventory

    [Header("Slot Setup")]
    public GameObject slotPrefab;       // drag your InventorySlotUI prefab here
    public Transform  contentParent;    // the Content RectTransform under your ScrollView (so we can actually set up what is being displayed on it)

    void Start() {
        // Start with the players inventory closed
        inventoryPanel.SetActive(false); 
    }

    void Update() {
        if (Input.GetKeyDown(toggleKey)) {
            ToggleInventory();
        }
    }

    public void ToggleInventory() {
        if (inventoryPanel.activeSelf) {
            HideInventory();
        }
        else {
            ShowInventory();
        }
    }

    public void ShowInventory() {
        // unlock the mouse, so the player can select items to equip
        UnlockCursor();

        inventoryPanel.SetActive(true);
        PopulateSlots(); // rebuild every time we open it to get an accurate count of what is in the inventory                
    }

    public void HideInventory() {
        inventoryPanel.SetActive(false);

        // lock the cursor back so the camera/movement works
        LockCursor();
    }

    void PopulateSlots() {
        // wipe out the old ui
        for (int i = contentParent.childCount - 1; i >= 0; i--) {
            Destroy(contentParent.GetChild(i).gameObject);
        }
        
        // spawn one slot for each resource in the Inventory dictionary
        var inv = FindObjectOfType<Inventory>();
        foreach (var kvp in inv.resources) {
            var go = Instantiate(slotPrefab, contentParent);
            go.GetComponent<InventorySlotUI>().Setup(kvp.Key, null, kvp.Value);
        }
    }

    void UnlockCursor() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;
    }

    void LockCursor() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
    }
}
