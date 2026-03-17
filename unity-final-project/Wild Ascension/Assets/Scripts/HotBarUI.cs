using UnityEngine;
using UnityEngine.UI;

public class HotbarUI : MonoBehaviour {
    public static HotbarUI Instance;   // singleton for so other functions can easily use it, and we don't have multiple hot bars

    [Header("Slots (1-6): you can add more later if needed or wanted")]
    public Button[] slotButtons;       // Hotbar prefab's go here
    public Image[]  slotIcons;         // Icon Child of each slot to set it if needed
    public Image[]  highlights;        // Highlight's of each image to let the player know what he is holding

    private WeaponSO[] hotbar = new WeaponSO[6]; // what is currently assigned in each slot
    private int selectedIndex = 0; // which of the hot bar slots are active

    void Awake() {
        Instance = this; // set the singleton (only 1 hot bar allowed)
    }

    void Start() {
        // Set each button’s "onClick" to call "SelectSlot" with its own index
        for (int i = 0; i < slotButtons.Length; i++) {
            int idx = i;
            slotButtons[i].onClick.AddListener(() => SelectSlot(idx));
        }
        UpdateHighlight(); // initial highlight
    }

    void Update() {
        // if the player selects hotbar 1-6 it will go to that hotbar
        for (int i = 0; i < 6; i++) {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i)) {
                SelectSlot(i);
            }
        }

        // The player can also use the scroll wheel to cycle between items (minecraft ease of access)
        float scroll = Input.mouseScrollDelta.y;
        if (scroll > 0f) {
            SelectSlot((selectedIndex + 1) % 6);
        }
        else if (scroll < 0f) {
            SelectSlot((selectedIndex + 5) % 6);
        }
    }

    // assign the selected weapon to the currently selected hotbar slot
    public void AssignWeaponToSelectedSlot(WeaponSO weapon) {
        // debugging 
        // Debug.Log($"[Hotbar] Assign slot {selectedIndex} → {(weapon!=null?weapon.weaponName:"null")}");

        hotbar[selectedIndex] = weapon;
        slotIcons[selectedIndex].sprite  = weapon != null ? weapon.icon : null;
        slotIcons[selectedIndex].enabled = (weapon != null);

        // auto-equip immediately
        var eq = FindObjectOfType<EquipmentManager>();
        if (eq != null) {
            // debugging
            // Debug.Log($"[Hotbar] Calling EquipmentManager.EquipWeapon({weapon?.weaponName})");
            eq.EquipWeapon(weapon);
        }
        else {
            // debugging (ebug so it shows up yellow for keeping track)
            // ebug.LogWarning("[Hotbar]  No EquipmentManager found in scene");
        }
    }

    // change selected slot index & equip
    void SelectSlot(int index) {
        selectedIndex = index;
        UpdateHighlight();

        var eq = FindObjectOfType<EquipmentManager>();
        if (eq != null) {
            eq.EquipWeapon(hotbar[index]);
        }
    }

    // update only the highlight overlays
    void UpdateHighlight() {
        for (int i = 0; i < highlights.Length; i++) {
            highlights[i].enabled = (i == selectedIndex);
        }
    }

    // Return the names of the WeaponSO in each hotbar slot ("" if empty)
    public string[] GetHotbarItemNames() {
        string[] names = new string[hotbar.Length];
        for (int i = 0; i < hotbar.Length; i++) {
            names[i] = hotbar[i] != null ? hotbar[i].weaponName : "";
        }
        return names;
    }

    // Return which slot index is currently selected
    public int GetSelectedIndex() {
        return selectedIndex;
    }

    // select a slot by index (used when loading)
    public void SelectSlotIndex(int idx) {
        SelectSlot(idx);
    }

    // Set up all slots from saved names and re-equip the selected one
    public void SetHotbarFromNames(string[] names) {
        for (int i = 0; i < hotbar.Length && i < names.Length; i++) {
            string weaponName = names[i];
            if (string.IsNullOrEmpty(weaponName)) {
                hotbar[i] = null;
                slotIcons[i].enabled = false;
            } else {
                WeaponSO ws = Resources.Load<WeaponSO>("Weapons/" + weaponName);
                hotbar[i] = ws;
                slotIcons[i].sprite  = ws != null ? ws.icon : null;
                slotIcons[i].enabled = (ws != null);
            }
        }
        // Re-apply highlight & equip
        SelectSlot(selectedIndex);
    }
}
