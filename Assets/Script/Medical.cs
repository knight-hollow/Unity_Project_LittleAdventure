using UnityEngine;

/// <summary>
/// Represents a medical item (health pickup).
/// When the player collides with it, the player recovers health,
/// and the item is destroyed.
/// </summary>
public class Medical : MonoBehaviour
{
    void Start()
    {
        // Initialization logic (currently unused)
    }

    void Update()
    {
        // Could be used later for animation or rotation effects (e.g., spinning pickup)
    }

    /// <summary>
    /// Trigger event called when another collider enters this object's trigger zone.
    /// </summary>
    /// <param name="other">The collider that entered the trigger.</param>
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is the player
        if (other.CompareTag("Player"))
        {
            // Restore 50 health points to the player
            Player.Instance.AddHealth(50f);

            // Remove this medical item from the scene
            Destroy(gameObject);
        }
    }
}
