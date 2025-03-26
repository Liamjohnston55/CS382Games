// Enemy randomly picks three points spaced apart and moves along the dotted line between the points
using UnityEngine;

public class Enemy_3 : Enemy {
    [Header("Set in Inspector: Enemy_3")]
    public float lifeTime = 5f;

    private Vector3[] points; // p0, p1, p2 for a quadratic Bezier curve
    private float birthTime;

    void Start() {
        // Allocate an array of 3 points
        points = new Vector3[3];

        // p0 above the screen
        points[0] = new Vector3(
            Random.Range(-bndCheck.camWidth, bndCheck.camWidth),
            bndCheck.camHeight + bndCheck.radius,
            0);

        // p2 below the screen
        points[2] = new Vector3(
            Random.Range(-bndCheck.camWidth, bndCheck.camWidth),
            -bndCheck.camHeight - bndCheck.radius,
            0);

        // p1 is some midpoint between top & bottom
        points[1] = new Vector3(
            Random.Range(-bndCheck.camWidth, bndCheck.camWidth),
            Random.Range(-0.5f * bndCheck.camHeight, 0),
            0);

        // Swap p0 and p2 half the time to vary direction
        if (Random.value > 0.5f) {
            Vector3 temp = points[0];
            points[0] = points[2];
            points[2] = temp;
        }

        birthTime = Time.time;
    }

    public override void Move() {
        float u = (Time.time - birthTime) / lifeTime;
        if (u > 1) {
            Destroy(gameObject);
            return;
        }

        // add some ease-in/out
        u = u - 0.2f * Mathf.Sin(u * Mathf.PI * 2);

        // Quadratic Bezier interpolation
        // (1-u)^2 * p0 + 2u(1-u)*p1 + u^2 * p2
        Vector3 tempPos =
              (1 - u) * (1 - u) * points[0]
            + 2 * u * (1 - u) * points[1]
            + u * u * points[2];

        pos = tempPos;
    }
}
