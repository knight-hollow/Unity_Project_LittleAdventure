using UnityEngine;
using System.Collections;

/// <summary>
/// Base class for all enemy types.
/// Provides shared logic for health, damage, attacks, animations, and death handling.
/// </summary>
public class EnemyBase : MonoBehaviour
{
    // Reference to the Animator component controlling enemy animations
    public Animator _anim;

    // Attack timing variables
    public float attackTimer = 0;             // Tracks time remaining before next attack
    public float attackInterval = 5f;         // Total cooldown duration between attacks
    public float attackConsumerTime = 2.3f;   // Active attack phase duration (time before attack resets)

    // Attack state flags
    public bool isAttacking = false;          // Whether the enemy is currently attacking
    public bool canAttack = true;             // Whether the enemy is ready to attack again

    // Base damage dealt per attack
    public float Damage;

    // Rotation speed when facing the player
    public float rotationSpeed = 3f;

    // Movement flag (used for animation transitions)
    public bool isMoving = false;

    // Health system
    public float hp = 100f;       // Current health
    public float maxHp = 100f;    // Maximum health
    public bool isDead = false;   // Death state flag

    /// <summary>
    /// Called when the enemy takes damage.
    /// Reduces health, plays hurt animation, and checks for death.
    /// </summary>
    /// <param name="damage">The amount of incoming damage.</param>
    public void Hurt(float damage)
    {
        // Ignore damage if already dead
        if (isDead)
        {
            return;
        }

        // Subtract health and ensure it doesn't go below zero
        hp = Mathf.Max(0, hp - damage);

        // Trigger "Hurt" animation to show the reaction
        _anim.SetTrigger("Hurt");

        // Check if enemy has died after taking damage
        if (hp <= 0)
        {
            Dead();
        }
    }

    /// <summary>
    /// Handles the enemy's death process:
    /// - Marks as dead
    /// - Plays death animation
    /// - Updates spawner count
    /// - Rewards the player
    /// - Schedules destruction
    /// </summary>
    public void Dead()
    {
        // Set death flag
        isDead = true;

        // Update animator to play death animation
        _anim.SetBool("isDead", isDead);

        // Notify the spawner that one enemy has been eliminated
        Spawner.Instance.remainEnemies -= 1;

        // Reward the player with coins
        Player.Instance.AddCoin(50);

        // Destroy the enemy object after a short delay (to allow animation to finish)
        StartCoroutine(DestroyAfterDelay(5f));
    }

    /// <summary>
    /// Coroutine to destroy the enemy object after a delay.
    /// Allows time for the death animation to play.
    /// </summary>
    /// <param name="delay">Time in seconds before destruction.</param>
    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
