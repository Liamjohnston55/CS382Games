using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class CraftingSlotUI : MonoBehaviour {
    public Image iconImage;     // picture of what you are crafting 
    public TMP_Text nameText;   // name of what you are crafting
    public Transform ingredientsParent;   // parent for small ingredient icons
    public GameObject ingredientPrefab;   // prefab of one ingridient slot
    public Button craftButton;            // button that you press to craft

    public void Setup(CraftingRecipe recipe, Action<CraftingRecipe> onCraftClicked) {
        nameText.text = recipe.recipeName;
        iconImage.sprite = recipe.icon;

        // clear old inventory
        foreach (Transform t in ingredientsParent) Destroy(t.gameObject);
        // create new inventory
        foreach (var ing in recipe.ingredients) {
            GameObject slot = Instantiate(ingredientPrefab, ingredientsParent);
            var ui = slot.GetComponent<IngredientSlotUI>();
            ui.Setup(ing.resourceName, ing.amount);
        }

        craftButton.onClick.RemoveAllListeners();
        craftButton.onClick.AddListener(() => onCraftClicked(recipe));
    }
}
