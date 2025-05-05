using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class CraftingUI : MonoBehaviour
{
    [Header("References")]
    public Inventory inventory;        // inventory component refrence
    public GameObject craftingPanel;   // Drag your Crafting-Panel here
    public KeyCode toggleKey = KeyCode.C; // Key to open/close crafting

    [Header("Recipe Setup")]
    public GameObject recipeSlotPrefab; // Prefab for each recipe row
    public Transform contentParent;    // where to spawn the rows of crafting materials 
    public List<CraftingRecipe> recipes; // List of all recipes to show

    void Start() {
        craftingPanel.SetActive(false); // This makes sure the crafting pannel is off by default
    }

    void Update() {
        if (Input.GetKeyDown(toggleKey)) {
            bool nowOpen = !craftingPanel.activeSelf;
            craftingPanel.SetActive(nowOpen);

            if (nowOpen){
                Populate();         // Show all recipes
                UnlockCursor();     // let players use the mouse while the crafting-menu
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
    }

    void Populate() {
        // Clear old recipe slots
        foreach (Transform t in contentParent) {
            Destroy(t.gameObject);
        }

        // Spawn one slot per recipe
        foreach (var r in recipes) {
            var go = Instantiate(recipeSlotPrefab, contentParent);
            go.GetComponent<CraftingSlotUI>().Setup(r, TryCraft);
        }
    }

    void TryCraft(CraftingRecipe recipe) {
        // ensure you have all needed materials
        foreach (var ing in recipe.ingredients)
            if (!inventory.HasResource(ing.resourceName, ing.amount)) {
                return;
            }

        // subtract materials from inventory
        foreach (var ing in recipe.ingredients) {
            inventory.AddResource(ing.resourceName, -ing.amount);
        }

        // add result item to inventory
        inventory.AddResource(recipe.resultResourceName, recipe.resultAmount);

        // award XP
        PlayerStats ps = FindObjectOfType<PlayerStats>();
        if (ps != null && recipe.xpReward > 0) {
            ps.AddXP(recipe.xpReward);
        }

        // Refreshes inventory and crafting UI to get accurate results
        Populate();

        // Close crafting UI on completed craft
        craftingPanel.SetActive(false);
        //LockCursor();

        // When player craft's something, re-open the inventory to get accurate results of what you have currently in your inventory
        var invUI = FindObjectOfType<InventoryUI>();
        if (invUI != null) {
            invUI.HideInventory();
            invUI.ShowInventory();
        }
    }

    // unlock the cursor so players can use the crafting ui
    // it should be unlocked from the inventory, but just to be safe
    void UnlockCursor() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;
    }

    // no longer needed for current build
    //void LockCursor(){
    //    Cursor.lockState = CursorLockMode.Locked;
    //    Cursor.visible   = false;
    //}
}
