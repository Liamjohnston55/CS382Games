using UnityEngine;
public class Enemy : MonoBehaviour {
    [Header("Set in Inspector: Enemy")]
    public float speed = 10f;         // vertical speed (world units per second)
    public float fireRate = 0.3f;     // (Not used in base prototype; reserved for future enemy shooting)
    public float health = 1;         // hit points (not heavily used in base)
    public int score = 100;          // points earned for destroying this enemy&#8203;:contentReference[oaicite:66]{index=66}

    protected BoundsCheck bndCheck;    // reference to BoundsCheck component

    void Awake() {
        bndCheck = GetComponent<BoundsCheck>();  // cache the BoundsCheck on this object&#8203;:contentReference[oaicite:67]{index=67}
    }

    // Property to easily get or set position
    public Vector3 pos {
        get { return this.transform.position; }
        set { this.transform.position = value; }
    }

    public virtual void Move() {
        // Default movement: straight down at constant speed
        Vector3 tempPos = pos;
        tempPos.y -= speed * Time.deltaTime;
        pos = tempPos;
    }

    void Update() {
        Move();
        // Destroy the enemy if it exits the bottom of the screen
        if (bndCheck != null && !bndCheck.isOnScreen) {
            if (pos.y < -bndCheck.camHeight + bndCheck.radius) {
                // Enemy is off the bottom of the screen – destroy it&#8203;:contentReference[oaicite:68]{index=68}&#8203;:contentReference[oaicite:69]{index=69}
                Destroy(gameObject);
            }
        }
    }

    void OnCollisionEnter(Collision coll) {
        // Handle collision with a ProjectileHero (hero’s bullet)
        GameObject otherGO = coll.gameObject;
        if (otherGO.tag == "ProjectileHero") {
            Destroy(otherGO);             // destroy the projectile&#8203;:contentReference[oaicite:70]{index=70}
            Destroy(gameObject);          // destroy this enemy&#8203;:contentReference[oaicite:71]{index=71}
            // (In a more advanced game, you might trigger an explosion effect or sound here)
        } else {
            Debug.Log("Enemy hit by non-ProjectileHero: " + otherGO.name); // For debugging&#8203;:contentReference[oaicite:72]{index=72}
        }
    }
}
