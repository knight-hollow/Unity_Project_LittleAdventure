using UnityEngine;

// Define the faction or "camp" of an object (Player or Enemy)
// Used to determine who can damage whom
public enum CampType
{
    Player,
    Enemy
}

public class DamageCaster : MonoBehaviour
{
    // Reference to the BoxCollider used as the hit detection area
    public BoxCollider _box;

    // Reference to the enemy’s base component (used when this belongs to an enemy)
    public EnemyBase enemyBase;

    // Indicates which camp this DamageCaster belongs to (Player or Enemy)
    public CampType currentCamp;

    // Called when the script instance is being loaded
    private void Awake()
    {
        // Get the BoxCollider component attached to the same GameObject
        _box = GetComponent<BoxCollider>();

        // Disable it by default; it will be enabled only when needed (e.g., during attack frames)
        _box.enabled = false;

        // Get the EnemyBase component from the parent object (if any)
        enemyBase = transform.parent.GetComponent<EnemyBase>();
    }

    void Start()
    {
        // Optional: initialization logic can go here
    }

    void Update()
    {
        // Update logic can be added here if needed (currently unused)
    }

    // Triggered when this collider overlaps another collider marked as "Is Trigger"
    private void OnTriggerEnter(Collider other)
    {
        // If this DamageCaster belongs to the Player
        if (currentCamp == CampType.Player)
        {
            // Check if the object hit has the "Enemy" tag
            if (other.CompareTag("Enemy"))
            {
                Debug.Log("Enemy Detected");

                // Deal damage to the enemy using the player's attack power
                other.gameObject
                     .GetComponent<EnemyBase>()
                     .Hurt(Player.Instance.attackPower);
            }
        }
        // If this DamageCaster belongs to an Enemy
        else if (currentCamp == CampType.Enemy)
        {
            // Check if the object hit has the "Player" tag
            if (other.CompareTag("Player"))
            {
                Debug.Log("Player Detected");

                // Check if references exist before applying damage
                if (enemyBase == null)
                    Debug.LogError("enemyBase is NULL!");

                if (Player.Instance == null)
                    Debug.LogError("Player.Instance is NULL!");

                // Deal damage to the player using the enemy’s attack value
                Player.Instance.Hurt(enemyBase.Damage);
            }
        }
    }
}
