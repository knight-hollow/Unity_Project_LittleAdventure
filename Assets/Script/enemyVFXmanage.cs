using UnityEngine;
using UnityEngine.VFX;

/// <summary>
/// Controls visual effects (VFX) related to enemy actions,
/// such as footsteps and attack effects.
/// This script is typically attached to an enemy GameObject.
/// </summary>
public class enemyVFXmanage : MonoBehaviour
{
    // Visual effect that plays when the enemy walks or runs
    public VisualEffect footstep;

    // Visual effect that plays when the enemy performs an attack
    public VisualEffect attackVFK;

    // Reference to the enemy’s base component (for health, state checks, etc.)
    public EnemyBase enemyBase;

    private void Awake()
    {
        // Cache the EnemyBase component attached to this GameObject
        enemyBase = GetComponent<EnemyBase>();
    }

    void Start()
    {
        // Additional initialization logic could be added here if needed
    }

    void Update()
    {
        // Currently unused — could be used for continuous VFX control or state checking
    }

    /// <summary>
    /// Plays the footstep visual effect.
    /// Usually triggered by animation events during walk/run animations.
    /// </summary>
    public void FootStepVFX()
    {
        footstep.Play();
    }

    /// <summary>
    /// Plays the attack visual effect.
    /// Usually triggered by animation events during attack animations.
    /// </summary>
    public void AttackVFX()
    {
        attackVFK.Play();
    }
}
