// Enemy goes from one side to the other side of the screen as it moves towards the player
using UnityEngine;

public class Enemy_2 : Enemy {
    [Header("Set in Inspector: Enemy_2")]
    public float sinEccentricity = 0.6f; // how curvy the path is
    public float lifeTime = 10f;         // how long it takes to traverse from p0 to p1

    [Header("Set Dynamically: Enemy_2")]
    public Vector3 p0, p1;  // start and end points
    private float birthTime;

    void Start() {
        // Place p0 off the left edge, p1 off the right 
        p0 = Vector3.zero;
        p0.x = -bndCheck.camWidth - bndCheck.radius;
        p0.y = Random.Range(-bndCheck.camHeight, bndCheck.camHeight);

        p1 = Vector3.zero;
        p1.x =  bndCheck.camWidth + bndCheck.radius;
        p1.y = Random.Range(-bndCheck.camHeight, bndCheck.camHeight);

        // Swap half the time so some enemies travel right-to-left
        if (Random.value > 0.5f) {
            Vector3 temp = p0;
            p0 = p1;
            p1 = temp;
        }

        birthTime = Time.time;
    }

    public override void Move() {
        float u = (Time.time - birthTime) / lifeTime;
        if (u > 1) {
            // when done kill self
            Destroy(gameObject);
            return;
        }

        // apply some easing with a sine wave
        u = u + sinEccentricity * Mathf.Sin(u * Mathf.PI * 2);

        // linear interpolation from p0 to p1
        Vector3 tempPos = (1 - u) * p0 + u * p1;
        pos = tempPos;
    }
}
