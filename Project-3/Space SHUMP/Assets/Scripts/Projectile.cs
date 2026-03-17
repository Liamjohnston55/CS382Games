using UnityEngine;

public class Projectile : MonoBehaviour {
    [Header("Set Dynamically")]
    public Rigidbody rigid;
    public WeaponType type;              // The weapon type that fired this projectile

    private BoundsCheck bndCheck;
    private Renderer rend;

    private float spawnTime;
    private Quaternion spawnRotation;
    // Max rotation allowed
    private float maxRotationDeviation = 15f;
    // time before checking rotation
    private float rotationCheckDelay = 0.5f;

    void Awake() {
        rigid = GetComponent<Rigidbody>();
        bndCheck = GetComponent<BoundsCheck>();
        rend = GetComponent<Renderer>();
        spawnTime = Time.time;
        spawnRotation = transform.rotation;
    }

    void Start() {
        // Set the projectile's color based on its WeaponDefinition.
        WeaponDefinition def = GameManager.GetWeaponDefinition(type);
        if (def != null) {
            rend.material.color = def.projectileColor;
        }
    }

    void Update() {
        // Destroy the projectile if it goes off-screen.
        if (bndCheck != null && !bndCheck.isOnScreen) {
            Destroy(gameObject);
            return;
        }
        
        // check if the projectile has rotated too much.
        if (Time.time - spawnTime > rotationCheckDelay) {
            float angleDiff = Quaternion.Angle(spawnRotation, transform.rotation);
            if (angleDiff > maxRotationDeviation) {
                Destroy(gameObject);
                return;
            }
        }
    }
}
