using UnityEngine;

// Defines the different weapon categories
public enum WeaponType {
    Unarmed,
    Axe,
    Pickaxe,
    // add more types as we add more weapons
}

[CreateAssetMenu(menuName = "Weapons/WeaponSO")]
public class WeaponSO : ScriptableObject {
    [Header("Basic Info")]
    public string    weaponName;      // name shown in UI & logs
    public Sprite    icon;            // hotbar / inventory icon
    public float     damageBonus;     // Extra damage when equipped
    public GameObject weaponPrefab;   // The 3D model spawned under hand

    [Header("Weapon Behavior")]
    [Tooltip("What kind of weapon this is")]
    public WeaponType type = WeaponType.Unarmed;  

    [Tooltip("How long the swing animation takes (seconds)")]
    public float swingCooldown = 0.75f; 

    [Header("Hold Offsets")]
    [Tooltip("Local offset (in hand-space) to position the grip correctly")]
    public Vector3 holdPositionOffset = Vector3.zero; 

    [Tooltip("Local Euler rotation (in hand-space) to align the grip correctly")]
    public Vector3 holdRotationOffset = Vector3.zero; 
}
