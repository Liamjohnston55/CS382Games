// Enemy zig-zags horizontally as it moves towards the player
using UnityEngine;

public class Enemy_1 : Enemy {
    [Header("Set in Inspector: Enemy_1")]
    public float waveFrequency = 2f; // # of seconds for one full sine wave cycle
    public float waveWidth = 4f;     // how far left & right it moves
    public float waveRotY = 45f;     // how much it tilts left/right

    private float x0;                // initial x position
    private float birthTime;

    void Start() {
        x0 = pos.x;
        birthTime = Time.time;
    }

    public override void Move() {
        // 1) Start with the current position
        Vector3 tempPos = pos;

        // 2) Calculate how long this enemy has existed
        float age = Time.time - birthTime;
        // 3) Convert that to a value between 0..2π based on waveFrequency
        float theta = Mathf.PI * 2 * (age / waveFrequency);
        float sin = Mathf.Sin(theta);

        // 4) Offset x by the sine wave
        tempPos.x = x0 + waveWidth * sin;
        pos = tempPos;

        // 5) Tilt the ship (rotating around the Y-axis)
        Vector3 rot = new Vector3(0, sin * waveRotY, 0);
        transform.rotation = Quaternion.Euler(rot);

        // 6) Still move down normally (via base.Move())
        base.Move();
    }
}
