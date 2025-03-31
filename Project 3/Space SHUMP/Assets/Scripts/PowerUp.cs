using UnityEngine;
using System.Collections;

public class PowerUp : MonoBehaviour {
    [Header("Set in Inspector")]
    public Vector2 rotMinMax = new Vector2(15, 90);         // Rotation speed range 
    public Vector2 driftMinMax = new Vector2(0.25f, 2f);    // Drift speed range
    public float lifeTime = 6f;                             // time the power-up exists before fading
    public float fadeTime = 4f;                             // time over which it fades out after lifeTime

    [Header("Set Dynamically")]
    public WeaponType type;                                 // currently not in use
    public GameObject cube;                                 // Reference to the cube child 
    public Vector3 rotPerSecond;                            // Calculated rotation speed 
    public float birthTime;
    
    private Rigidbody rigid;
    private BoundsCheck bndCheck;
    private Renderer cubeRend;

    void Awake() {
        cube = transform.Find("Cube").gameObject;
        rigid = GetComponent<Rigidbody>();         
        bndCheck = GetComponent<BoundsCheck>();      
        cubeRend = cube.GetComponent<Renderer>();    

        // Set a random drift velocity (specified to the xy plane)
        Vector3 vel = Random.onUnitSphere;
        vel.z = 0;
        vel.Normalize();
        rigid.velocity = vel * Random.Range(driftMinMax.x, driftMinMax.y);

        // Reset rotation and set a random rotation speed for the cube
        transform.rotation = Quaternion.identity;
        rotPerSecond = new Vector3(
            Random.Range(rotMinMax.x, rotMinMax.y),
            Random.Range(rotMinMax.x, rotMinMax.y),
            Random.Range(rotMinMax.x, rotMinMax.y)
        );
        birthTime = Time.time;
    }

    void Start() {
        SetType(WeaponType.spread);
    }

    void Update() {
        // Spin the cube continuously
        cube.transform.rotation = Quaternion.Euler(rotPerSecond * Time.time);

        // power ups life cycle 
        float u = (Time.time - (birthTime + lifeTime)) / fadeTime;
        if (u >= 1) {
            Destroy(gameObject);
            return;
        }
        if (u > 0 && cubeRend != null) {
            Color c = cubeRend.material.color;
            c.a = 1f - u;
            cubeRend.material.color = c;
        }

        // Destroy the power up if it drifts off screen
        if (bndCheck != null && !bndCheck.isOnScreen) {
            Destroy(gameObject);
        }
    }

    // This makes sure that when the ship is hit, it collides with the object
    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Hero")) {
            AbsorbedBy(other.gameObject);
        }
    }

    public void SetType(WeaponType wt) {
        type = wt;
        WeaponDefinition def = GameManager.GetWeaponDefinition(type);
        if (cubeRend != null) {
            cubeRend.material.color = def.color;
        }
    }

    // when the power up is consumed, add the extra lives
    public void AbsorbedBy(GameObject target) {
        Hero hero = target.GetComponent<Hero>();
        if (hero != null) {
            // Increase the heros lives by 3 (max is 6)
            hero.shieldLevel += 3;
            if (hero.shieldLevel > 6) {
                hero.shieldLevel = 6;
            }

            // if the amount of lives is over three it is white, if it is under it is blue
            Renderer heroRend = target.GetComponent<Renderer>();
            if (heroRend == null) {
                heroRend = target.GetComponentInChildren<Renderer>();
            }
            if (heroRend != null) {
                if (hero.shieldLevel > 3) {
                    heroRend.material.color = Color.blue;  // powered-up color
                } else {
                    heroRend.material.color = Color.white; // default color
                }
            }
        }
        Destroy(gameObject);
    }
}
