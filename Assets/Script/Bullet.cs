using UnityEngine;

public class Bullet : MonoBehaviour
{
    // The amount of damage this bullet deals when it hits a target
    public float damage;

    // Bullet movement speed (units per second)
    private float speed = 5f;

    // Direction in which the bullet will travel (normalized vector)
    public Vector3 direction;

    // Maximum lifetime of the bullet before it gets destroyed (in seconds)
    public float timeout = 5f;

    // Internal timer to keep track of how long the bullet has existed
    public float timer = 0f;

    void Start()
    {
        // Initialization logic can be placed here if needed
    }

    // Called once per frame
    void Update()
    {
        // Move the bullet along its direction in world space
        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        // Increment the lifetime counter
        timer += Time.deltaTime;

        // Destroy the bullet after it exceeds its allowed lifetime
        if (timer >= timeout)
        {
            Destroy(gameObject);
        }
    }

    // Set the amount of damage this bullet will deal
    public void SetDamage(float d)
    {
        damage = d;
    }

    // Trigger event when the bullet collides with another collider marked as "Is Trigger"
    private void OnTriggerEnter(Collider other)
    {
        // Check if the collided object has the "Player" tag
        if (other.CompareTag("Player"))
        {
            // Apply damage to the player via its Hurt() method
            Player.Instance.Hurt(damage);

            // Destroy the bullet upon impact
            Destroy(gameObject);
        }
    }

    // Initialize the bullet's properties (direction and damage)
    public void Init(Vector3 d, float damage)
    {
        // Assign the damage value
        SetDamage(damage);

        // Set the bullet's travel direction
        direction = d;

        // Rotate the bullet so that it faces the direction it is moving
        transform.rotation = Quaternion.LookRotation(direction);

        // Adjust local rotation (useful if the 3D model is not aligned correctly by default)
        transform.Rotate(90f, 0, 0, Space.Self);
    }
}
