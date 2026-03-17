using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName="Crafting/Recipe")]
public class CraftingRecipe : ScriptableObject { // scriptable so we can make it a prefab item
    public string recipeName;           // Displayed in the crafting ui pannel
    public Sprite icon;                 // Picture of what you crafted

    public List<ResourceAmount> ingredients; // recources needed to craft

    public string resultResourceName;   // name of what you crafted
    public int resultAmount = 1;
    public int xpReward = 0;            // Amount of xp to award when crafted

    public WeaponSO weaponData;         // links the data needed if its a weapon
}

[System.Serializable]
public class ResourceAmount {
    public string resourceName;
    public int amount;
}
