using UnityEngine;
using System.Collections;

/// <summary>
/// Controls a gate that smoothly opens by moving vertically over time.
/// The gate only opens once when triggered.
/// </summary>
public class Gate : MonoBehaviour
{
    // Target vertical movement amount (relative to current position)
    // Example: -2.5f means the gate will move downward by 2.5 units
    public float TargetY = -2.5f;

    // Duration of the opening animation (in seconds)
    public float duration = 2f;

    // Flag to ensure the gate only opens once
    private bool isOpened = false;

    private void Awake()
    {
        // Initialization logic (if needed later)
    }

    void Start()
    {
        // Optional startup logic
    }

    void Update()
    {
        // Could be used to test or trigger OpenGate() manually for debugging
    }

    /// <summary>
    /// Public method to open the gate.
    /// Can be called from other scripts, animation events, or triggers.
    /// </summary>
    public void OpenGate()
    {
        // Prevent reopening if the gate is already open
        if (!isOpened)
        {
            isOpened = true;
            StartCoroutine(OpenGateAnim());
        }
    }

    /// <summary>
    /// Coroutine that smoothly moves the gate from its current position
    /// to the target position over a specified duration.
    /// </summary>
    IEnumerator OpenGateAnim()
    {
        float currentDuration = 0f;

        // Store initial and target positions
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + Vector3.up * TargetY;

        // Gradually interpolate between start and target positions
        while (currentDuration < duration)
        {
            currentDuration += Time.deltaTime;

            // Linearly interpolate (Lerp) between positions
            transform.position = Vector3.Lerp(startPos, targetPos, currentDuration / duration);

            // Wait for the next frame
            yield return null;
        }

        // Optional: ensure gate fully reaches the target position
        transform.position = targetPos;
    }
}
