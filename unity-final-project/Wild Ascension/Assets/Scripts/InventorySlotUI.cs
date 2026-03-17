using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour {
    [Header("UI References")]
    public Image    iconImage;     // the Image component for the icon
    public TMP_Text countText;     // the text for the count label
    private string resourceName;   // holds the name so OnClick knows which resource we clicked

    public void Setup(string resourceName, Sprite iconSprite, int amount){
        // debugging
        // Debug.Log($"[InvSlot] Setup called for '{resourceName}', count = {amount}");

        // keep track of which resource this is
        this.resourceName = resourceName;

        // show how many of them we have
        countText.text = amount.ToString();

        // set up the icon
        if (iconSprite != null) {
            iconImage.sprite  = iconSprite;
            iconImage.enabled = true;
        }
        // if we cant find the passed in image, check Resources/InventoryPictures
        else {
            Sprite s = Resources.Load<Sprite>("InventoryPictures/" + resourceName);
            if (s == null) {
                s = Resources.Load<Sprite>("InventoryPictures/" + resourceName.Replace(" ", ""));
            }
            iconImage.sprite  = s;
            iconImage.enabled = (s != null);
        }

        // ensure there's a Button component to click
        var btn = GetComponent<Button>();
        if (btn == null) btn = gameObject.AddComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(OnClick);
    }

    // when you  click the shot, load a matching WeaponSO and assign it to a slot in the hot bar 
    private void OnClick() {
        // debugging
        // Debug.Log($"[InvSlot] OnClick → resourceName = '{resourceName}'");               
        WeaponSO ws = Resources.Load<WeaponSO>("Weapons/" + resourceName);

        // debugging
        // Debug.Log(ws != null ? $"[InvSlot] Loaded WeaponSO: {ws.weaponName}" : $"[InvSlot]  No WeaponSO found at 'Resources/Weapons/{resourceName}.asset'");  

        if (ws != null) {
            // debugging
            // Debug.Log($"[InvSlot] Calling HotbarUI.AssignWeaponToSelectedSlot({ws.weaponName})");
            HotbarUI.Instance?.AssignWeaponToSelectedSlot(ws);                           
        }
    }

}
