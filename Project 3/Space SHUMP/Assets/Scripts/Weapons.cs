using UnityEngine;

public class Weapon : MonoBehaviour {
    [Header("Set Dynamically")]
    public WeaponType type = WeaponType.none;
    public WeaponDefinition def;
    public GameObject collar;      // Child object to show weapon muzzle
    private Renderer collarRend;
    private float lastShotTime;

    // Static transform for parenting all projectiles (to keep hierarchy neat)
    static private Transform PROJECTILE_ANCHOR;

    void Awake() {
        // Find the collar child and its Renderer
        collar = transform.Find("Collar").gameObject;
        collarRend = collar.GetComponent<Renderer>();

        // Create a global parent for all Projectiles if it doesn’t exist
        if (PROJECTILE_ANCHOR == null) {
            GameObject go = new GameObject("_ProjectileAnchor");
            PROJECTILE_ANCHOR = go.transform;
        }

        // If attached to the Hero, subscribe Fire() to hero’s fireDelegate
        GameObject rootGO = transform.root.gameObject; 
        Hero hero = rootGO.GetComponent<Hero>();
        if (hero != null) {
            hero.fireDelegate += Fire;
        }

        // Initialize weapon type (this also hides the GameObject if type=none)
        SetType(type);
    }

    public void SetType(WeaponType wt) {
        type = wt;
        if (type == WeaponType.none) {
            // Deactivate if no weapon
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);
        def = GameManager.GetWeaponDefinition(type);  // Look up stats in GameManager
        collarRend.material.color = def.color;        // Visually indicate weapon color
        lastShotTime = 0;
    }

    public void Fire() {
        // If weapon is inactive or we haven’t waited the delayBetweenShots, do nothing
        if (!gameObject.activeInHierarchy) return;
        if (Time.time - lastShotTime < def.delayBetweenShots) return;

        // Calculate projectile velocity
        Vector3 vel = Vector3.up * def.velocity;
        // Flip direction if this weapon is oriented downward (e.g. on an enemy)
        if (transform.up.y < 0) {
            vel.y = -vel.y;
        }

        // Spawn projectiles depending on type
        switch(type) {
            case WeaponType.blaster:
                MakeOneProjectile(vel, 0);
                break;

            case WeaponType.spread:
                // Middle shot
                MakeOneProjectile(vel, 0);
                // +10° shot
                MakeOneProjectile(vel, 10);
                // -10° shot
                MakeOneProjectile(vel, -10);
                break;
        }

        lastShotTime = Time.time;
    }

    // Helper to instantiate and set up a single projectile
    private void MakeOneProjectile(Vector3 baseVel, float angle) {
        // Instantiate the projectile prefab from def
        GameObject projGO = Instantiate(def.projectilePrefab) as GameObject;

        // Tag & layer depends on whether attached to Hero or Enemy
        if (transform.root.CompareTag("Hero")) {
            projGO.tag = "ProjectileHero";
            projGO.layer = LayerMask.NameToLayer("ProjectileHero");
        } else {
            projGO.tag = "ProjectileEnemy";
            projGO.layer = LayerMask.NameToLayer("ProjectileEnemy");
        }

        // Position the projectile at the collar
        projGO.transform.position = collar.transform.position;
        projGO.transform.SetParent(PROJECTILE_ANCHOR, true);

        // Set up the Projectile script
        Projectile p = projGO.GetComponent<Projectile>();
        p.type = type;

        // Apply angle if we’re doing a spread shot
        if (angle != 0) {
            projGO.transform.rotation = Quaternion.AngleAxis(angle, Vector3.back);
            // Multiply velocity by this rotation
            baseVel = projGO.transform.rotation * baseVel;
        }

        // Finally, set the velocity
        p.rigid.linearVelocity = baseVel;
    }
}
