using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ReverbDistanceController : MonoBehaviour
{
    public Transform player; // Reference to the player transform
    public float maxDistance = 20f; // Maximum distance for full reverb effect
    public float minDistance = 1f; // Minimum distance where reverb starts

    // Audio mixer references
    public AudioMixer audioMixer; // Reference to your audio mixer
    public string volumeParameterName = "ReverbVolumeParameter"; // Parameter name for volume in mixer (from your screenshot)

    // Curve to control the effect based on distance
    public AnimationCurve volumeCurve = AnimationCurve.Linear(0, -80, 1, 0); // Curve for volume effect (min to max dB)

    // Update frequency
    public float updateInterval = 0.2f; // How often to update the audio effects

    private float normalizedDistance; // Distance value normalized between 0-1

    void Start()
    {
        if (player == null)
        {
            // Try to find the player if not assigned
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

            if (player == null)
            {
                Debug.LogError("Player transform not assigned and couldn't be found automatically!");
                enabled = false; // Disable the script if no player is found
                return;
            }
        }

        StartCoroutine(UpdateAudioEffects());
    }

    private IEnumerator UpdateAudioEffects()
    {
        while (true)
        {
            UpdateEffectsBasedOnDistance();
            yield return new WaitForSeconds(updateInterval);
        }
    }

    void UpdateEffectsBasedOnDistance()
    {
        if (player == null || audioMixer == null) return;

        // Calculate distance between player and this object
        float distance = Vector3.Distance(player.position, transform.position);

        // Normalize distance between min and max values (0 = close, 1 = far)
        normalizedDistance = Mathf.Clamp01((distance - minDistance) / (maxDistance - minDistance));

        // Set volume amount based on curve - as player moves further away, volume increases
        float volumeLevel = volumeCurve.Evaluate(normalizedDistance);
        audioMixer.SetFloat(volumeParameterName, volumeLevel); // Volume in dB

        // Debug info
        Debug.Log($"Distance: {distance:F2}, Normalized: {normalizedDistance:F2}, Volume Level: {volumeLevel:F2}dB");
    }

    // Optional: Visualize the effect range in the editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, minDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }
}