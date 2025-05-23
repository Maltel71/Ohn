using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("Animation")]
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private string openAnimationTrigger = "DoorOpening";

    [Header("Physics")]
    [SerializeField] private Rigidbody objectToActivate; // Object to make non-kinematic when door opens

    [Header("UI (Optional)")]
    [SerializeField] private GameObject promptUI; // Optional "Press E to open" prompt
    [SerializeField] private GameObject noKeyUI; // Optional "You need a key" prompt

    private Transform playerTransform;
    private bool canInteract = false;
    private bool isOpened = false;

    void Start()
    {
        // Find the player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }

        // Get animator if not assigned
        if (doorAnimator == null)
        {
            doorAnimator = GetComponent<Animator>();
        }

        // Hide UI prompts initially
        if (promptUI != null) promptUI.SetActive(false);
        if (noKeyUI != null) noKeyUI.SetActive(false);
    }

    void Update()
    {
        if (isOpened || playerTransform == null) return;

        // Check distance to player
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= interactionRange)
        {
            if (!canInteract)
            {
                canInteract = true;
                ShowPrompts(true);
            }

            // Check for interaction input
            if (Input.GetKeyDown(interactKey))
            {
                TryOpenDoor();
            }
        }
        else
        {
            if (canInteract)
            {
                canInteract = false;
                ShowPrompts(false);
            }
        }
    }

    void TryOpenDoor()
    {
        // Check if player has key
        if (KeyManager.Instance != null && KeyManager.Instance.HasKey)
        {
            OpenDoor();
        }
        else
        {
            // Show "need key" message briefly
            ShowNoKeyMessage();
        }
    }

    void OpenDoor()
    {
        if (isOpened) return;

        isOpened = true;

        // Play opening animation
        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger(openAnimationTrigger);
        }

        // Activate physics object (make it non-kinematic)
        if (objectToActivate != null)
        {
            objectToActivate.isKinematic = false;
            Debug.Log($"{objectToActivate.name} is now non-kinematic and will be affected by physics!");
        }

        // Hide prompts
        ShowPrompts(false);

        Debug.Log("Door opened!");
    }

    void ShowPrompts(bool show)
    {
        if (show && KeyManager.Instance != null && KeyManager.Instance.HasKey)
        {
            // Show "Press E to open" if player has key
            if (promptUI != null) promptUI.SetActive(true);
            if (noKeyUI != null) noKeyUI.SetActive(false);
        }
        else if (show)
        {
            // Show "You need a key" if player doesn't have key
            if (promptUI != null) promptUI.SetActive(false);
            if (noKeyUI != null) noKeyUI.SetActive(true);
        }
        else
        {
            // Hide all prompts
            if (promptUI != null) promptUI.SetActive(false);
            if (noKeyUI != null) noKeyUI.SetActive(false);
        }
    }

    void ShowNoKeyMessage()
    {
        Debug.Log("You need a key to open this door!");

        // Brief flash of the no-key message
        if (noKeyUI != null)
        {
            noKeyUI.SetActive(true);
            Invoke(nameof(HideNoKeyMessage), 2f);
        }
    }

    void HideNoKeyMessage()
    {
        if (noKeyUI != null)
        {
            noKeyUI.SetActive(false);
        }
    }

    // Visual debug in scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}