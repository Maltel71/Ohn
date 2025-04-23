using UnityEngine;

// This needs to inherit from MonoBehaviour to work as a Unity component
public class Fire_Effect_Light : MonoBehaviour
{
    [Header("Flicker Settings")]
    [Tooltip("The minimum intensity the light can reach")]
    public float minIntensity = 0.5f;

    [Tooltip("The maximum intensity the light can reach")]
    public float maxIntensity = 1.5f;

    [Tooltip("How quickly the light intensity changes (higher = faster changes)")]
    public float flickerSpeed = 0.1f;

    [Header("Color Variation")]
    [Tooltip("Enable color variation to simulate fire color changes")]
    public bool enableColorVariation = true;

    [Tooltip("Base color of the fire")]
    public Color baseColor = new Color(1.0f, 0.6f, 0.1f);

    [Tooltip("How much the color can vary from the base")]
    [Range(0.0f, 0.5f)]
    public float colorVariation = 0.1f;

    // Reference to the light component
    private Light pointLight;

    // Variables for smooth noise generation
    private float noiseOffset;

    private void Start()
    {
        // Get the Light component
        pointLight = GetComponent<Light>();

        if (pointLight == null)
        {
            Debug.LogError("Fire_Effect_Light script must be attached to a GameObject with a Light component!");
            enabled = false;
            return;
        }

        // Set a random starting point for our noise
        noiseOffset = Random.Range(0f, 1000f);
    }

    private void Update()
    {
        // Calculate the new intensity using Perlin noise for smooth transitions
        float noise = Mathf.PerlinNoise(noiseOffset, Time.time * flickerSpeed);

        // Map the noise (0-1 range) to our intensity range
        float newIntensity = Mathf.Lerp(minIntensity, maxIntensity, noise);

        // Apply the new intensity
        pointLight.intensity = newIntensity;

        // If color variation is enabled, vary the color slightly
        if (enableColorVariation)
        {
            // Create subtle color variations
            float r = baseColor.r + (Mathf.PerlinNoise(noiseOffset + 42, Time.time * flickerSpeed) * colorVariation * 2 - colorVariation);
            float g = baseColor.g + (Mathf.PerlinNoise(noiseOffset + 84, Time.time * flickerSpeed) * colorVariation * 2 - colorVariation);
            float b = baseColor.b + (Mathf.PerlinNoise(noiseOffset + 126, Time.time * flickerSpeed) * colorVariation * 2 - colorVariation);

            // Ensure values stay in valid range
            r = Mathf.Clamp01(r);
            g = Mathf.Clamp01(g);
            b = Mathf.Clamp01(b);

            // Apply the new color
            pointLight.color = new Color(r, g, b, 1.0f);
        }
    }
}