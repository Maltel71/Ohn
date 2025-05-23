using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private KeyCode pickupKey = KeyCode.E;

    [Header("Animation Settings")]
    [SerializeField] private float rotationSpeed = 90f; // degrees per second
    [SerializeField] private float bobHeight = 0.5f;
    [SerializeField] private float bobSpeed = 2f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource; // Assign your separate AudioSource here

    [Header("UI (Optional)")]
    [SerializeField] private GameObject promptUI; // Optional UI prompt like "Press E to pick up"

    private Transform playerTransform;
    private Vector3 startPosition;
    private bool canPickup = false;
    private bool isPickedUp = false;

    void Start()
    {
        // Find the player - adjust the tag if needed
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("Player not found! Make sure your player GameObject has the 'Player' tag.");
        }

        // Store the starting position for bobbing animation
        startPosition = transform.position;

        // Hide prompt UI initially
        if (promptUI != null)
            promptUI.SetActive(false);

        // Validate audio source
        if (audioSource == null)
        {
            Debug.LogWarning("AudioSource not assigned! Please assign your AudioSource in the inspector.");
        }
    }

    void Update()
    {
        if (isPickedUp || playerTransform == null) return;

        // Animate the key (rotation and bobbing)
        AnimateKey();

        // Check distance to player
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= interactionRange)
        {
            if (!canPickup)
            {
                canPickup = true;
                ShowPrompt(true);
            }

            // Check for pickup input
            if (Input.GetKeyDown(pickupKey))
            {
                PickupKey();
            }
        }
        else
        {
            if (canPickup)
            {
                canPickup = false;
                ShowPrompt(false);
            }
        }
    }

    void AnimateKey()
    {
        // Rotate the key
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);

        // Bob up and down
        float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }

    void PickupKey()
    {
        isPickedUp = true;

        // Play pickup sound
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
        }

        // Hide prompt
        ShowPrompt(false);

        // Notify the KeyManager that a key was picked up
        if (KeyManager.Instance != null)
        {
            KeyManager.Instance.PickupKey();
        }
        else
        {
            Debug.LogWarning("KeyManager not found! Make sure you have a KeyManager in your scene.");
        }

        Debug.Log("Key picked up!");

        // Hide the key object
        gameObject.SetActive(false);

        // Alternative: Destroy the object after a short delay to let sound finish
        // Destroy(gameObject, audioSource.clip != null ? audioSource.clip.length : 0f);
    }

    void ShowPrompt(bool show)
    {
        if (promptUI != null)
        {
            promptUI.SetActive(show);
        }
    }

    // Optional: Visual debug in scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}