using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Plays random sounds from a list at random intervals with pitch variation.
/// </summary>
public class RandomSoundPlayer : MonoBehaviour
{
    [Header("Sound Settings")]
    [Tooltip("List of audio clips to play randomly")]
    public AudioClip[] soundClips;

    [Tooltip("Audio source component to play sounds through")]
    public AudioSource audioSource;

    [Header("Timing Settings")]
    [Tooltip("Minimum time between sounds in seconds")]
    public float minTimeBetweenSounds = 5f;

    [Tooltip("Maximum time between sounds in seconds")]
    public float maxTimeBetweenSounds = 15f;

    [Header("Pitch Settings")]
    [Tooltip("Minimum pitch multiplier (1 = normal pitch)")]
    [Range(0.5f, 1.5f)]
    public float minPitch = 0.8f;

    [Tooltip("Maximum pitch multiplier (1 = normal pitch)")]
    [Range(0.5f, 1.5f)]
    public float maxPitch = 1.2f;

    [Header("Volume Settings (Optional)")]
    [Tooltip("Random volume variation (0 = constant volume)")]
    [Range(0f, 0.5f)]
    public float volumeVariation = 0f;

    // Internal variables
    private bool isPlaying = false;
    private Coroutine soundRoutine;

    private void Awake()
    {
        // If no audio source assigned, try to get one on this game object
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();

            // If still null, add a new audio source
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                Debug.Log("RandomSoundPlayer: Added AudioSource component automatically");
            }
        }
    }

    private void Start()
    {
        // Validate the configuration
        if (soundClips == null || soundClips.Length == 0)
        {
            Debug.LogWarning("RandomSoundPlayer: No sound clips assigned!");
            return;
        }

        // Start playing sounds
        StartSoundSequence();
    }

    /// <summary>
    /// Starts the sequence of random sounds
    /// </summary>
    public void StartSoundSequence()
    {
        if (!isPlaying)
        {
            isPlaying = true;
            soundRoutine = StartCoroutine(PlaySoundsRandomly());
        }
    }

    /// <summary>
    /// Stops the sequence of random sounds
    /// </summary>
    public void StopSoundSequence()
    {
        if (isPlaying && soundRoutine != null)
        {
            isPlaying = false;
            StopCoroutine(soundRoutine);
            soundRoutine = null;
        }
    }

    /// <summary>
    /// Coroutine to play sounds at random intervals
    /// </summary>
    private IEnumerator PlaySoundsRandomly()
    {
        while (isPlaying)
        {
            // Wait for a random time
            float waitTime = Random.Range(minTimeBetweenSounds, maxTimeBetweenSounds);
            yield return new WaitForSeconds(waitTime);

            if (!isPlaying) break;

            // Play a random sound with pitch variation
            PlayRandomSound();
        }
    }

    /// <summary>
    /// Plays a random sound from the list with pitch variation
    /// </summary>
    public void PlayRandomSound()
    {
        if (soundClips.Length == 0 || audioSource == null) return;

        // Select a random sound clip
        int randomIndex = Random.Range(0, soundClips.Length);
        AudioClip clipToPlay = soundClips[randomIndex];

        if (clipToPlay != null)
        {
            // Apply random pitch
            audioSource.pitch = Random.Range(minPitch, maxPitch);

            // Apply volume variation if enabled
            if (volumeVariation > 0)
            {
                float baseVolume = audioSource.volume;
                audioSource.volume = baseVolume * (1f - volumeVariation + Random.Range(0f, volumeVariation * 2f));
            }

            // Play the sound
            audioSource.PlayOneShot(clipToPlay);
        }
    }

    private void OnDisable()
    {
        // Clean up when disabled
        StopSoundSequence();
    }
}