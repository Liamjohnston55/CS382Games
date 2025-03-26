using UnityEngine;

public class Projectile : MonoBehaviour {
    [Header("Set Dynamically")]
    public Rigidbody rigid;
    public WeaponType type;              // The weapon type that fired this projectile
    private BoundsCheck bndCheck;
    private Renderer rend;

    void Awake() {
        rigid = GetComponent<Rigidbody>();
        bndCheck = GetComponent<BoundsCheck>();
        rend = GetComponent<Renderer>();
    }

    void Start() {
        // Set the projectile’s color based on its WeaponDefinition
        WeaponDefinition def = GameManager.GetWeaponDefinition(type);
        if (def != null) {
            rend.material.color = def.projectileColor;   // assign bullet color
        }
    }

    void Update() {
        // Destroy the projectile if it goes off-screen
        if (bndCheck != null && !bndCheck.isOnScreen) {
            Destroy(gameObject);
        }
    }
}
