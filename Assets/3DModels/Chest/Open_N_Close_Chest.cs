using UnityEngine;

public class Open_N_Close_Chest : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private Animator animator;
    [SerializeField] private string closedIdleAnimName = "ClosedIdle";
    [SerializeField] private string openingAnimName = "Opening";
    [SerializeField] private string openIdleAnimName = "OpenIdle";
    [SerializeField] private string closingAnimName = "Closing";

    [Header("Interaction Settings")]
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private KeyCode interactionKey = KeyCode.E;

    // State tracking
    private bool isOpen = false;
    private bool isTransitioning = false;

    private void Start()
    {
        // If animator is not assigned, try to get it from this game object
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        // Start in closed idle state
        animator.Play(closedIdleAnimName);
    }

    private void Update()
    {
        // Check for player interaction
        if (Input.GetKeyDown(interactionKey) && PlayerIsInRange() && !isTransitioning)
        {
            ToggleChest();
        }
    }

    private void ToggleChest()
    {
        isTransitioning = true;

        if (!isOpen)
        {
            // Open the chest
            animator.Play(openingAnimName);
            Invoke("OnOpeningComplete", GetAnimationClipLength(openingAnimName));
        }
        else
        {
            // Close the chest
            animator.Play(closingAnimName);
            Invoke("OnClosingComplete", GetAnimationClipLength(closingAnimName));
        }
    }

    private void OnOpeningComplete()
    {
        isOpen = true;
        isTransitioning = false;
        animator.Play(openIdleAnimName);
    }

    private void OnClosingComplete()
    {
        isOpen = false;
        isTransitioning = false;
        animator.Play(closedIdleAnimName);
    }

    private float GetAnimationClipLength(string clipName)
    {
        // Find the animation clip in the animator controller
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
            {
                return clip.length;
            }
        }

        // Default to 1 second if clip not found
        Debug.LogWarning($"Animation clip '{clipName}' not found! Using default length of 1 second.");
        return 1f;
    }

    private bool PlayerIsInRange()
    {
        // If you have a player tag, you can use this method to check distance
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            return Vector3.Distance(transform.position, player.transform.position) <= interactionDistance;
        }

        // If no player found, always return true (for testing)
        return true;
    }

    // Optional: Add visual indicator for interaction
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}