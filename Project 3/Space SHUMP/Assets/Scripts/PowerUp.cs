using UnityEngine;
using System.Collections;

public class PowerUp : MonoBehaviour {
    [Header("Set in Inspector")]
    public Vector2 rotMinMax = new Vector2(15, 90);   // rotation speed range (Euler angles per second)
    public Vector2 driftMinMax = new Vector2(0.25f, 2f); // drift speed range
    public float lifeTime = 6f;      // seconds the power-up exists before fading
    public float fadeTime = 4f;      // seconds over which it will fade out after lifeTime

    [Header("Set Dynamically")]
    public WeaponType type;         // The WeaponType this PowerUp grants
    public GameObject cube;         // Reference to the cube child
    public TextMesh letter;         // Reference to the TextMesh showing the letter
    public Vector3 rotPerSecond;    // Euler rotation speed (calculated)
    public float birthTime;
    
    private Rigidbody rigid;
    private BoundsCheck bndCheck;
    private Renderer cubeRend;

    void Awake() {
        // Find child references
        cube = transform.Find("Cube").gameObject;
        letter = GetComponent<TextMesh>();
        rigid = GetComponent<Rigidbody>();
        bndCheck = GetComponent<BoundsCheck>();
        cubeRend = cube.GetComponent<Renderer>();
        // Set a random velocity for drifting
        Vector3 vel = Random.onUnitSphere;    // random direction (length 1)&#8203;:contentReference[oaicite:116]{index=116}
        vel.z = 0;                            // flatten to XY plane&#8203;:contentReference[oaicite:117]{index=117}
        vel.Normalize();                      // normalize to length 1 (since Random.onUnitSphere gave unit length already)&#8203;:contentReference[oaicite:118]{index=118}
        vel *= Random.Range(driftMinMax.x, driftMinMax.y);  // scale to random speed between min and max&#8203;:contentReference[oaicite:119]{index=119}
        rigid.linearVelocity = vel;
        // Set the rotation of the cube to identity (no rotation)
        transform.rotation = Quaternion.identity;
        // Choose a random rotation speed for the cube
        rotPerSecond = new Vector3(Random.Range(rotMinMax.x, rotMinMax.y),
                                   Random.Range(rotMinMax.x, rotMinMax.y),
                                   Random.Range(rotMinMax.x, rotMinMax.y));
        birthTime = Time.time;
    }

    void Update() {
        // Spin the cube continuously
        cube.transform.rotation = Quaternion.Euler(rotPerSecond * Time.time);
        // Fade out over time after lifeTime has passed
        float u = (Time.time - (birthTime + lifeTime)) / fadeTime;
        // If u < 0, the power-up is still in its lifeTime phase (no fading yet)
        if (u >= 1) {
            Destroy(this.gameObject);
            return;
        }
        if (u > 0) {
            // u goes from 0 to 1 as the powerUp fades
            Color c = cubeRend.material.color;
            c.a = 1f - u;                         // decrease alpha
            cubeRend.material.color = c;
            // Also fade the letter, but slower (half the rate)
            c = letter.color;
            c.a = 1f - u * 0.5f;
            letter.color = c;
        }
        // If the power-up has drifted off screen, destroy it
        if (!bndCheck.isOnScreen) {
            Destroy(gameObject);
        }
    }

    public void SetType(WeaponType wt) {
        // Assign the type of this PowerUp and adjust visuals accordingly
        type = wt;
        WeaponDefinition def = GameManager.GetWeaponDefinition(type);
        // Set cube color and letter text to match the weapon type
        cubeRend.material.color = def.color;
        letter.text = def.letter;   // e.g., "S", "B", or "O"
    }

    public void AbsorbedBy(GameObject target) {
        // This function is called when the power-up is collected by the hero
        // We could add effects here (like a sound or animation). For now, just destroy.
        Destroy(this.gameObject);
    }
}
