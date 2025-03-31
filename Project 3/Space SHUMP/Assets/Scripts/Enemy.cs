using UnityEngine;
public class Enemy : MonoBehaviour {
    [Header("Set in Inspector: Enemy")]
    public float speed = 10f;         // vertical speed 
    public float fireRate = 0.3f;     // how fast you get to shoot
    public float health = 1;         // hit points 
    public int score = 100;          // points earned for destroying this enemy

    protected BoundsCheck bndCheck;    // reference to BoundsCheck component

    void Awake() {
        bndCheck = GetComponent<BoundsCheck>();  // cache the BoundsCheck on this object
    }

    // Property to easily get or set position
    public Vector3 pos {
        get { return this.transform.position; }
        set { this.transform.position = value; }
    }

    public virtual void Move() {
        // Go straight down 
        Vector3 tempPos = pos;
        tempPos.y -= speed * Time.deltaTime;
        pos = tempPos;
    }

    void Update() {
        Move();
        // Destroy the enemy if it exits the bottom of the screen
        if (bndCheck != null && !bndCheck.isOnScreen) {
            if (pos.y < -bndCheck.camHeight + bndCheck.radius) {
                // when the enemy is off screen destroy it before it destroys your computer 
                Destroy(gameObject);
            }
        }
    }

    void OnCollisionEnter(Collision coll) {
        // Handle collision with a ProjectileHero 
        GameObject otherGO = coll.gameObject;
        if (otherGO.tag == "ProjectileHero") {
            Destroy(otherGO);             // destroy the projectile
            Destroy(gameObject);          // destroy this enemy
            
        }
    }
}
