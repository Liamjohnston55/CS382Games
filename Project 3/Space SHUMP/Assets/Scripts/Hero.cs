using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;  

public class Hero : MonoBehaviour {
    public static Hero S;  

    [Header("Set in Inspector")]
    public float speed = 30f;           // Movement speed
    public float rollMult = 45f;        // Banking multiplier
    public float pitchMult = 30f;       // Pitch multiplier
    public float projectileSpeed = 40f; // Speed for hero projectiles

    [Header("Set Dynamically")]
    [SerializeField] private float _shieldLevel = 3f;  // Hero starts with 3 lives (Power up will change it to 6 at max)
    public Weapon[] weapons;            // Weapon components attached to Hero children

    // Delegate for firing weapons
    public delegate void WeaponFireDelegate();
    public WeaponFireDelegate fireDelegate;

    private bool isGameOver = false;
    private Renderer heroRend;          

    private Color baseColor = Color.blue;  // blue
    private Color poweredColor = new Color(0.678f, 0.847f, 0.902f, 1f);  //  white

    void Awake() {
        if (S == null) {
            S = this;
        } 
    }

    void Start() {
        _shieldLevel = 3f;  // Initialize shield level

        // Cache the hero's Renderer so we can access it later
        heroRend = GetComponentInChildren<Renderer>();
        if (heroRend != null) {
            // Set the initial color to the blue
            heroRend.material.color = baseColor;
        } 

        // Get all Weapon components from child objects
        weapons = GetComponentsInChildren<Weapon>();
        if (weapons.Length > 0) {
            // Initialize hero's weapon to blaster at start
            weapons[0].SetType(WeaponType.blaster);
        }
        // Disable any additional weapons (none are set up but it breaks some things if this is not added)
        for (int i = 1; i < weapons.Length; i++) {
            weapons[i].SetType(WeaponType.none);
        }
    }

    void Update() {
        // Update movement based on the player's input
        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");
        Vector3 pos = transform.position;
        pos.x += xAxis * speed * Time.deltaTime;
        pos.y += yAxis * speed * Time.deltaTime;
        transform.position = pos;
        transform.rotation = Quaternion.Euler(yAxis * pitchMult, xAxis * -rollMult, 0);

        // Shooting
        if (Input.GetAxis("Jump") == 1 && fireDelegate != null) {
            fireDelegate();
        }
        
        // Update the hero's material based on shield level
        if (heroRend != null) {
            if (_shieldLevel > 3) {
                heroRend.material.color = poweredColor;
            } else {
                heroRend.material.color = baseColor;
            }
        }
    }

    void OnTriggerEnter(Collider other) {
        // If hit by an enemy or enemy projectile, take damage
        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("ProjectileEnemy")) {
            _shieldLevel--;
            Destroy(other.gameObject);
            if (_shieldLevel <= 0 && !isGameOver) {
                isGameOver = true;
                Destroy(gameObject);
                GameManager.ReloadSceneAfterDelay(5f);
            }
        }
        // If colliding with a power-up, absorb it
        else if (other.gameObject.CompareTag("PowerUp")) {
            AbsorbPowerUp(other.gameObject);
        }
    }

    public void AbsorbPowerUp(GameObject go) {
        PowerUp pu = go.GetComponent<PowerUp>();
        if (pu == null) return;
        
        // Increase shield by 3 
        _shieldLevel += 3;
        if (_shieldLevel > 6)
            _shieldLevel = 6;
        
        // Destroy the power-up after absorption.
        pu.AbsorbedBy(this.gameObject);
    }

    public float shieldLevel {
        get { return _shieldLevel; }
        set { _shieldLevel = Mathf.Max(value, 0); }
    }
}
