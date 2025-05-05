using UnityEngine;

public class EquipmentManager : MonoBehaviour {
    [Tooltip("Drag your character's hand bone (from the model) here")]
    public Transform handSocket; // this is where your tool will appear

    private GameObject currentWeaponGO; // this temporarly holds the model so it can be easily switch when swapping
    private WeaponSO equippedWeapon; // Scriptable Object data for whatever is equipped 

    public void EquipWeapon(WeaponSO weapon) {
        // Remove old weapon model
        if (currentWeaponGO != null)
            Destroy(currentWeaponGO);

        equippedWeapon = weapon;

        // Spawn the new model
        if (weapon != null && weapon.weaponPrefab != null && handSocket != null) {
            // Instantiate the prefab
            currentWeaponGO = Instantiate(weapon.weaponPrefab);

            // Parent under the socket (worldPositionStays = false)
            currentWeaponGO.transform.SetParent(handSocket, false);

            // Snap into the socket’s position & rotation
            currentWeaponGO.transform.localPosition = Vector3.zero;
            currentWeaponGO.transform.localRotation = Quaternion.identity;

            // Apply model rotations to get the perfect model placement
            currentWeaponGO.transform.localPosition += weapon.holdPositionOffset;
            currentWeaponGO.transform.localRotation *= Quaternion.Euler(weapon.holdRotationOffset);
        }
    }

    public WeaponSO GetEquipped() {
        return equippedWeapon;
    }
}
