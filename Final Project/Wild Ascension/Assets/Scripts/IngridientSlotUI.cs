using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IngredientSlotUI : MonoBehaviour {
    [Header("UI References")]
    public Image iconImage;        // Image of what resources you need to craft something
    public TMP_Text countText;     // how much of the resource you need to craft something 

    // Populates this ingredient‐slot with the given resource name and count
    public void Setup(string resourceName, int amount){
        countText.text = amount.ToString(); // to display 

        // grab the ingridients picture from /Assets/Resources/InventoryPictures
        Sprite sprite = Resources.Load<Sprite>("InventoryPictures/" + resourceName); 
        if (sprite == null) {
            string noSpace = resourceName.Replace(" ", "");
            sprite = Resources.Load<Sprite>("InventoryPictures/" + noSpace);
        }

        if (sprite != null) {
            iconImage.sprite = sprite;
        }
        else {
            iconImage.sprite = null;
            // debugging
            // Debug.LogWarning($"[IngredientSlotUI] No sprite for '{resourceName}'");
        }
    }
}
