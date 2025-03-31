using UnityEngine;

// Types of weapons (and powerups)
public enum WeaponType {
    none,
    blaster,
    spread,
    shield,
    enemyBlaster
}

[System.Serializable]
public class WeaponDefinition {
    public WeaponType type = WeaponType.none;
    public string letter;
    public Color color = Color.white;
    public GameObject projectilePrefab;
    public Color projectileColor;
    public float damageOnHit = 0;
    public float continuousDamage = 0;
    public float delayBetweenShots = 0;
    public float velocity = 20;
}
