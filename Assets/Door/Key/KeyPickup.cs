using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeyPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private KeyCode pickupKey = KeyCode.E;

    [Header("Animation Settings")]
    [SerializeField] private float rotationSpeed = 90f;
    [SerializeField] private float bobHeight = 0.5f;
    [SerializeField] private float bobSpeed = 2f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;

    [Header("UI Prompt Settings")]
    [SerializeField] private GameObject promptUI; // Parent GameObject containing the UI elements
    [SerializeField] private TextMeshProUGUI promptText; // Text component (optional - for custom text)
    [SerializeField] private Image promptBackground; // Background image (optional)
    [SerializeField] private CanvasGroup promptCanvasGroup; // For smooth fade effects (optional)
    [SerializeField] private Vector3 promptOffset = new Vector3(0, 2f, 0); // Offset above the key
    [SerializeField] private bool alwaysFaceCamera = true;
    [SerializeField] private Vector3 promptUIScale = new Vector3(0.01f, 0.01f, 0.01f); // UI scale in world space

    [Header("Key Icon UI")]
    [SerializeField] private GameObject keyIconUI; // Key icon to show in top-right when picked up
    [Header("UI Animation (Optional)")]
    [SerializeField] private bool useUIAnimation = true;
    [SerializeField] private float fadeSpeed = 5f;
    [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0.8f, 1, 1f);

    private Transform playerTransform;

    [Header("UI Animation (Optional)")]
    private Vector3 startPosition;
    private bool canPickup = false;
    private bool isPickedUp = false;
    private bool isPromptVisible = false;
    private Coroutine uiAnimationCoroutine;

    void Start()
    {
        // Find the player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("Player not found! Make sure your player GameObject has the 'Player' tag.");
        }

        startPosition = transform.position;

        // Initialize UI
        InitializeUI();

        // Hide key icon initially
        if (keyIconUI != null)
            keyIconUI.SetActive(false);

        if (audioSource == null)
        {
            Debug.LogWarning("AudioSource not assigned! Please assign your AudioSource in the inspector.");
        }
    }

    void InitializeUI()
    {
        if (promptUI != null)
        {
            promptUI.SetActive(false);

            // Apply the custom scale
            promptUI.transform.localScale = promptUIScale;

            // Keep the text as set in the Inspector - don't modify it via script

            // Initialize canvas group for fade effects
            if (promptCanvasGroup != null)
            {
                promptCanvasGroup.alpha = 0f;
            }
        }
        else
        {
            Debug.LogWarning("PromptUI not assigned! Create a UI prompt and assign it in the inspector.");
        }
    }

    void Update()
    {
        if (isPickedUp || playerTransform == null) return;

        AnimateKey();

        // Update UI position and rotation
        UpdateUIPosition();

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= interactionRange)
        {
            if (!canPickup)
            {
                canPickup = true;
                ShowPrompt(true);
            }

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
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }

    void PickupKey()
    {
        isPickedUp = true;

        // Hide prompt immediately before disabling anything
        ShowPrompt(false);

        // Show key icon in top-right
        if (keyIconUI != null)
            keyIconUI.SetActive(true);

        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
        }

        if (KeyManager.Instance != null)
        {
            KeyManager.Instance.PickupKey();
        }
        else
        {
            Debug.LogWarning("KeyManager not found! Make sure you have a KeyManager in your scene.");
        }

        Debug.Log("Key picked up!");

        // Use coroutine to ensure UI is properly hidden before disabling
        StartCoroutine(DisableAfterUIHidden());
    }

    private System.Collections.IEnumerator DisableAfterUIHidden()
    {
        // Wait a frame to ensure UI prompt is properly hidden
        yield return null;

        // If we have an audio clip, wait for it to finish playing
        if (audioSource != null && audioSource.clip != null)
        {
            yield return new WaitForSeconds(audioSource.clip.length);
        }

        gameObject.SetActive(false);
    }

    void ShowPrompt(bool show)
    {
        if (promptUI == null) return;

        if (show == isPromptVisible) return; // Prevent unnecessary calls

        isPromptVisible = show;

        if (uiAnimationCoroutine != null)
        {
            StopCoroutine(uiAnimationCoroutine);
        }

        if (useUIAnimation && promptCanvasGroup != null)
        {
            uiAnimationCoroutine = StartCoroutine(AnimatePrompt(show));
        }
        else
        {
            // Simple show/hide without animation
            promptUI.SetActive(show);
        }
    }

    private System.Collections.IEnumerator AnimatePrompt(bool show)
    {
        promptUI.SetActive(true);

        float startAlpha = promptCanvasGroup.alpha;
        float targetAlpha = show ? 1f : 0f;
        float startScale = promptUI.transform.localScale.x;
        float targetScale = show ? 1f : 0.8f;

        float elapsed = 0f;
        float duration = 1f / fadeSpeed;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;

            // Smooth fade
            promptCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, progress);

            // Scale animation using curve
            float baseScale = promptUIScale.x; // Use the custom scale as base
            float scaleValue = Mathf.Lerp(startScale, targetScale * baseScale, scaleCurve.Evaluate(progress));
            promptUI.transform.localScale = Vector3.one * scaleValue;

            yield return null;
        }

        promptCanvasGroup.alpha = targetAlpha;
        promptUI.transform.localScale = promptUIScale * targetScale;

        if (!show)
        {
            promptUI.SetActive(false);
        }
    }

    void UpdateUIPosition()
    {
        if (promptUI != null && promptUI.activeInHierarchy)
        {
            // Position the UI above the key
            promptUI.transform.position = transform.position + promptOffset;

            // Make UI face the camera
            if (alwaysFaceCamera && Camera.main != null)
            {
                Vector3 lookDirection = Camera.main.transform.position - promptUI.transform.position;
                lookDirection.y = 0; // Keep UI upright (optional)

                if (lookDirection != Vector3.zero)
                {
                    promptUI.transform.rotation = Quaternion.LookRotation(-lookDirection);
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}