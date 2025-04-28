using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [Tooltip("How much force to apply when launching the player")]
    [SerializeField] private float launchForce = 10f;

    [Tooltip("Direction to launch the player (will be normalized)")]
    [SerializeField] private Vector3 launchDirection = Vector3.up;

    [Tooltip("Optional particle effect to play when triggered")]
    [SerializeField] private ParticleSystem activationEffect;

    [Tooltip("Optional sound effect to play when triggered")]
    [SerializeField] private AudioSource launchSound;

    [Tooltip("Cool-down time between launches (in seconds)")]
    [SerializeField] private float cooldownTime = 0.5f;

    private bool canLaunch = true;
    private float cooldownTimer = 0f;

    private void Awake()
    {
        // Make sure launch direction is normalized
        launchDirection = launchDirection.normalized;
    }

    private void Update()
    {
        // Handle cooldown timer
        if (!canLaunch)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0)
            {
                canLaunch = true;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryLaunch(collision.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        TryLaunch(other.gameObject);
    }

    private void TryLaunch(GameObject obj)
    {
        // Check if it's the player and we can launch
        if (!canLaunch) return;

        // Check if object has the tag "Player" or you can use a specific player script check
        if (obj.CompareTag("Player"))
        {
            // Get the Rigidbody component from the player
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Apply launch force to the player
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z); // Reset vertical velocity
                rb.AddForce(launchDirection * launchForce, ForceMode.Impulse);

                // Play effects if assigned
                if (activationEffect != null)
                {
                    activationEffect.Play();
                }

                if (launchSound != null)
                {
                    launchSound.Play();
                }

                // Set cooldown
                canLaunch = false;
                cooldownTimer = cooldownTime;
            }
            else
            {
                Debug.LogWarning("Player object doesn't have a Rigidbody component!");
            }
        }
    }
}