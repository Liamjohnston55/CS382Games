using UnityEngine;
using System.Collections.Generic;
public class Hero : MonoBehaviour {
    public static Hero S;                        // Singleton instance&#8203;:contentReference[oaicite:32]{index=32}
    
    [Header("Set in Inspector")]
    public float speed = 30;                     // Movement speed (units per second)
    public float rollMult = 45;                  // Rotation multiplier for banking 
    public float pitchMult = 30;                 // Rotation multiplier for pitching
    public float projectileSpeed = 40;           // Speed for hero projectiles
    // (Other weapon settings come from WeaponDefinitions; see Weapon.cs)

    [Header("Set Dynamically")]
    [SerializeField] private float _shieldLevel = 1;  // Hero shield strength (health)
    // The Hero ships starts with shieldLevel 1 (one hit before destruction)
    
    public Weapon[] weapons;                     // Array of Weapon objects (children)

    void Awake() {
        if (S == null) {
            S = this;   // Set the singleton instance&#8203;:contentReference[oaicite:33]{index=33}
        } else {
            Debug.LogError("Hero.Awake() - Attempted to assign a second Hero.S!");
        }
    }

    void Start() {
        // Initialize weapons array (we expect at least one Weapon child)
        weapons = GetComponentsInChildren<Weapon>();
        // Set the first weapon to blaster by default, others to inactive
        weapons[0].SetType(WeaponType.blaster);
        for (int i = 1; i < weapons.Length; i++) {
            weapons[i].SetType(WeaponType.none);
        }
    }

    void Update() {
        // MOVEMENT
        float xAxis = Input.GetAxis("Horizontal"); // -1 to 1 from keyboard A/D or arrow keys&#8203;:contentReference[oaicite:34]{index=34}
        float yAxis = Input.GetAxis("Vertical");   // -1 to 1 from W/S or arrow keys&#8203;:contentReference[oaicite:35]{index=35}
        // Change position based on input and speed:
        Vector3 pos = transform.position;
        pos.x += xAxis * speed * Time.deltaTime;
        pos.y += yAxis * speed * Time.deltaTime;
        transform.position = pos;
        // Tilt the ship model for a bit of dynamic feel (bank on horizontal, pitch on vertical)&#8203;:contentReference[oaicite:36]{index=36}
        transform.rotation = Quaternion.Euler(yAxis * pitchMult, xAxis * -rollMult, 0);
        
        // SHOOTING
        if (Input.GetAxis("Jump") == 1 && fireDelegate != null) {  
            fireDelegate();   // Call all Weapon.Fire() methods via delegate&#8203;:contentReference[oaicite:37]{index=37}&#8203;:contentReference[oaicite:38]{index=38}
        }
    }

    // Delegate field for weapons to fire 
    public delegate void WeaponFireDelegate();
    public WeaponFireDelegate fireDelegate;

    void OnTriggerEnter(Collider other) {
        // Triggered when another collider enters this Hero’s trigger (e.g., enemy or power-up)
        Transform rootT = other.gameObject.transform.root;
        GameObject go = rootT.gameObject;
        // Check what hit the hero
        if (go.tag == "Enemy") {
            // If an enemy hits, reduce shield level by 1 and destroy the enemy&#8203;:contentReference[oaicite:39]{index=39}&#8203;:contentReference[oaicite:40]{index=40}
            shieldLevel--;
            Destroy(go);
        } else if (go.tag == "PowerUp") {
            // If a PowerUp is collected, absorb it
            AbsorbPowerUp(go);
        } else {
            // Otherwise, log what hit (e.g., environment or unexpected collider)
            Debug.Log("Triggered by non-Enemy: " + go.name);
        }
    }

    public void AbsorbPowerUp(GameObject go) {
        PowerUp pu = go.GetComponent<PowerUp>();
        if (pu == null) return;
        // Apply the power-up effect based on its type
        switch (pu.type) {
            case WeaponType.shield:
                _shieldLevel++;                          // Increase shield (extra life)&#8203;:contentReference[oaicite:41]{index=41}
                break;
            default:
                // Weapon power-up picked up
                if (pu.type == weapons[0].type) {
                    // Same weapon type -> add another weapon (if slot available)&#8203;:contentReference[oaicite:42]{index=42}&#8203;:contentReference[oaicite:43]{index=43}
                    Weapon w = GetEmptyWeaponSlot();
                    if (w != null) {
                        w.SetType(pu.type);
                    }
                } else {
                    // Different weapon type -> replace all with new weapon&#8203;:contentReference[oaicite:44]{index=44}&#8203;:contentReference[oaicite:45]{index=45}
                    ClearWeapons();
                    weapons[0].SetType(pu.type);
                }
                break;
        }
        pu.AbsorbedBy(this.gameObject);  // Notify the PowerUp (and destroy it)&#8203;:contentReference[oaicite:46]{index=46}&#8203;:contentReference[oaicite:47]{index=47}
    }

    Weapon GetEmptyWeaponSlot() {
        foreach (Weapon w in weapons) {
            if (w.type == WeaponType.none) {
                return w;
            }
        }
        return null;
    }

    void ClearWeapons() {
        foreach (Weapon w in weapons) {
            w.SetType(WeaponType.none);
        }
    }

    // Property to get/set shield level with clamping
    public float shieldLevel {
        get { return _shieldLevel; }
        set {
            _shieldLevel = Mathf.Max(value, 0);
            // TODO: Add visual feedback or death handling when shield drops to 0
        }
    }
}
