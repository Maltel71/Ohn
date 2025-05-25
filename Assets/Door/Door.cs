using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("Animation")]
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private string openAnimationTrigger = "DoorOpening";

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;

    [Header("Physics")]
    [SerializeField] private Rigidbody objectToActivate;

    [Header("UI Prompts")]
    [SerializeField] private GameObject openPromptCanvas;
    [SerializeField] private GameObject needKeyPromptCanvas;
    [SerializeField] private GameObject keyIconUI;

    private Transform playerTransform;
    private bool canInteract = false;
    private bool isOpened = false;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }

        if (doorAnimator == null)
        {
            doorAnimator = GetComponent<Animator>();
        }

        if (openPromptCanvas != null) openPromptCanvas.SetActive(false);
        if (needKeyPromptCanvas != null) needKeyPromptCanvas.SetActive(false);
    }

    void Update()
    {
        if (isOpened || playerTransform == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= interactionRange)
        {
            if (!canInteract)
            {
                canInteract = true;
                ShowAppropriatePrompt(true);
            }

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
                ShowAppropriatePrompt(false);
            }
        }
    }

    void TryOpenDoor()
    {
        if (KeyManager.Instance != null && KeyManager.Instance.HasKey)
        {
            OpenDoor();
        }
        else
        {
            ShowNoKeyMessage();
        }
    }

    void OpenDoor()
    {
        if (isOpened) return;

        isOpened = true;

        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
        }

        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger(openAnimationTrigger);
        }

        if (objectToActivate != null)
        {
            objectToActivate.isKinematic = false;
            Debug.Log($"{objectToActivate.name} is now non-kinematic and will be affected by physics!");
        }

        if (openPromptCanvas != null) openPromptCanvas.SetActive(false);
        if (needKeyPromptCanvas != null) needKeyPromptCanvas.SetActive(false);
        if (keyIconUI != null) keyIconUI.SetActive(false);

        Debug.Log("Door opened!");
    }

    void ShowAppropriatePrompt(bool show)
    {
        if (show && KeyManager.Instance != null && KeyManager.Instance.HasKey)
        {
            if (openPromptCanvas != null) openPromptCanvas.SetActive(true);
            if (needKeyPromptCanvas != null) needKeyPromptCanvas.SetActive(false);
        }
        else if (show)
        {
            if (openPromptCanvas != null) openPromptCanvas.SetActive(false);
            if (needKeyPromptCanvas != null) needKeyPromptCanvas.SetActive(true);
        }
        else
        {
            if (openPromptCanvas != null) openPromptCanvas.SetActive(false);
            if (needKeyPromptCanvas != null) needKeyPromptCanvas.SetActive(false);
        }
    }

    void ShowNoKeyMessage()
    {
        Debug.Log("You need a key to open this door!");

        if (needKeyPromptCanvas != null)
        {
            needKeyPromptCanvas.SetActive(true);
            Invoke(nameof(HideNoKeyMessage), 2f);
        }
    }

    void HideNoKeyMessage()
    {
        if (needKeyPromptCanvas != null)
        {
            needKeyPromptCanvas.SetActive(false);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}