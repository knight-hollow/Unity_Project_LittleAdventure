using UnityEngine;
using UnityEngine.VFX;

/// <summary>
/// Manages all player-related visual effects (VFX),
/// including footsteps, healing particles, and attack blade effects.
/// This script works closely with the Player controller and animation events.
/// </summary>
public class PlayerVFXManage : MonoBehaviour
{
    // --- Visual Effects References ---

    // Footstep effect (played while the player is walking or running)
    public VisualEffect footStep;

    // Healing effect (played when the player restores HP)
    public VisualEffect Heal;

    // Blade slash effects (played during attack animations)
    public ParticleSystem blade01;
    public ParticleSystem blade02;
    public ParticleSystem blade03;

    // --- State Control ---
    public bool isPlayingFoot = false; // Tracks whether the footstep VFX is currently active

    void Start()
    {
        // Initialization (not currently needed)
    }

    void Update()
    {
        // XOR (exclusive OR) condition ensures this runs only when state changes:
        // → If isPlayingFoot ≠ Player.Instance.ismove, toggle footstep VFX
        if (isPlayingFoot ^ Player.Instance.ismove)
        {
            Update_FootStep(Player.Instance.ismove);
            isPlayingFoot = !isPlayingFoot; // Flip the tracking state
        }
    }

    /// <summary>
    /// Starts or stops the footstep VFX depending on the player's movement state.
    /// </summary>
    /// <param name="state">True if the player is moving, false if idle.</param>
    private void Update_FootStep(bool state)
    {
        if (state)
        {
            footStep.Play(); // Begin footstep particles
        }
        else
        {
            footStep.Stop(); // Stop footstep particles when idle
        }
    }

    // --- Attack Effect Controls ---

    /// <summary>
    /// Plays the first attack blade VFX (used for combo stage 1).
    /// </summary>
    public void PlayerAttackVFX01()
    {
        blade01.Play();
    }

    /// <summary>
    /// Plays the second attack blade VFX (used for combo stage 2).
    /// </summary>
    public void PlayerAttackVFX02()
    {
        blade02.Play();
    }

    /// <summary>
    /// Plays the third attack blade VFX (used for combo stage 3).
    /// </summary>
    public void PlayerAttackVFX03()
    {
        blade03.Play();
    }
}
