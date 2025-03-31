using UnityEngine;

public class Weapon : MonoBehaviour {
    [Header("Set Dynamically")]
    public WeaponType type = WeaponType.none;
    public WeaponDefinition def;
    public GameObject collar;      // Child object to show the weapon
    private Renderer collarRend;
    private float lastShotTime;

    // This is used so I can actually see what enemys are spawning (do not remove unless you want 20+ bullets on the hierarchy)
    static private Transform PROJECTILE_ANCHOR;

    void Awake() {
        collar = transform.Find("Collar").gameObject;
        collarRend = collar.GetComponent<Renderer>();

        // Create a global parent for all projectiles if it doesn't exist (stops errors do not remove :)
        if (PROJECTILE_ANCHOR == null) {
            GameObject go = new GameObject("_ProjectileAnchor");
            PROJECTILE_ANCHOR = go.transform;
        }

        GameObject rootGO = transform.root.gameObject;
        Hero hero = rootGO.GetComponent<Hero>();
        if (hero != null) {
            hero.fireDelegate += Fire;
        } 

        SetType(type);

        // Make the enemy shoot
        if (rootGO.CompareTag("Enemy") && type != WeaponType.none && def != null && def.delayBetweenShots > 0) {
            InvokeRepeating("Fire", 1f, def.delayBetweenShots);
        }
    }

    public void SetType(WeaponType wt) {
        type = wt;
        if (type == WeaponType.none) {
            gameObject.SetActive(false);
            return;
        }
        gameObject.SetActive(true);
        def = GameManager.GetWeaponDefinition(type);
        if (collarRend != null) {
            collarRend.material.color = def.color;
        }
        lastShotTime = 0;
    }


    public void Fire() {
        // If weapon is inactive or we haven't waited the delayBetweenShots, do nothing
        if (!gameObject.activeInHierarchy) return;
        if (Time.time - lastShotTime < def.delayBetweenShots) return;

        Vector3 vel;
        // If this weapon belongs to an enemy, use Vector3.down as the base velocity.
        if (transform.root.CompareTag("Enemy")) {
            vel = Vector3.down * def.velocity;
        } else {
            vel = Vector3.up * def.velocity;
        }
        
        // (No need for additional flipping logic now.)

        // Spawn projectiles depending on type
        switch (type) {
            case WeaponType.blaster:
                MakeOneProjectile(vel, 0);
                break;
            case WeaponType.spread:
                // For spread, fire three projectiles
                MakeOneProjectile(vel, 0);
                MakeOneProjectile(vel, 10);
                MakeOneProjectile(vel, -10);
                break;
        }
        lastShotTime = Time.time;

        if (transform.root.CompareTag("Enemy")) {
            vel = Vector3.down * def.velocity; 
        } 
        else {
            vel = Vector3.up * def.velocity;
        }
    }


    private void MakeOneProjectile(Vector3 baseVel, float angle) {
        // Instantiate the projectile prefab from def
        GameObject projGO = Instantiate(def.projectilePrefab) as GameObject;

        // Set tag & layer 
        if (transform.root.CompareTag("Hero")) {
            projGO.tag = "ProjectileHero";
            projGO.layer = LayerMask.NameToLayer("ProjectileHero");
        } else {
            projGO.tag = "ProjectileEnemy";
            projGO.layer = LayerMask.NameToLayer("ProjectileEnemy");
        }

        // Position the projectile at the weapon
        projGO.transform.position = collar.transform.position;
        projGO.transform.SetParent(PROJECTILE_ANCHOR, true);

        Projectile p = projGO.GetComponent<Projectile>();
        p.type = type;

        if (angle != 0) {
            projGO.transform.rotation = Quaternion.AngleAxis(angle, Vector3.back);
            baseVel = projGO.transform.rotation * baseVel;
        }

        p.rigid.linearVelocity = baseVel;
    }
}
