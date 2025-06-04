using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class MinimalThirdPersonController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 10f;
    public float jumpForce = 7f;
    public float gravity = -20f;
    public float pushPower = 2f;
    public float turnSpeed = 10f; // Controls how fast the player turns (higher = faster)

    [Header("Cursor Settings")]
    public bool lockCursorOnStart = true;
    public KeyCode toggleCursorLockKey = KeyCode.Escape;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Audio Settings")]
    public AudioClip[] footstepSounds;
    public AudioClip[] runningSounds; // Optional: separate sounds for running
    public AudioClip[] landingSounds;
    [Range(0f, 1f)]
    public float footstepVolume = 0.5f;
    [Range(0f, 1f)]
    public float landingVolume = 0.7f;
    public float walkStepInterval = 0.5f; // Time between footsteps when walking
    public float runStepInterval = 0.3f;  // Time between footsteps when running
    [Range(0f, 2f)]
    public float minPitchVariation = 0.8f;
    [Range(0f, 2f)]
    public float maxPitchVariation = 1.2f;

    private CharacterController controller;
    private Transform cameraTransform;
    private AudioSource audioSource;
    private Vector3 moveDirection;
    private float verticalVelocity;
    private bool isGrounded;
    private bool wasGrounded; // Track previous grounded state for landing detection
    private Quaternion targetRotation;

    // Platform movement
    private Transform platformParent;
    private Vector3 lastPlatformPosition;

    // Audio timing
    private float stepTimer;
    private bool isMoving;
    private bool wasMoving;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
        cameraTransform = Camera.main.transform;
        targetRotation = transform.rotation;

        // Initialize grounded state
        wasGrounded = true;

        // Create ground check if not assigned
        if (groundCheck == null)
        {
            GameObject checkObj = new GameObject("GroundCheck");
            groundCheck = checkObj.transform;
            groundCheck.parent = transform;
            groundCheck.localPosition = new Vector3(0, -controller.height / 2, 0);
        }

        // Lock and hide cursor on start if enabled
        if (lockCursorOnStart)
        {
            LockCursor();
        }
    }

    private void Update()
    {
        // Handle cursor lock toggling
      /*  if (Input.GetKeyDown(toggleCursorLockKey))
        {
            ToggleCursorLock();
        }*/

        // Store previous grounded state for landing detection
        wasGrounded = isGrounded;

        // Custom ground check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        // Platform-based grounding
        bool canJump = isGrounded;
        if (platformParent != null)
        {
            Vector3 platformDelta = platformParent.position - lastPlatformPosition;
            canJump = canJump || platformDelta.y > 0;
        }

        // Landing sound detection
        if (isGrounded && !wasGrounded && verticalVelocity < -1f)
        {
            PlayLandingSound();
        }

        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }

        // Get input for movement
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calculate movement direction relative to camera
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        moveDirection = forward * verticalInput + right * horizontalInput;

        // Check if player is moving
        wasMoving = isMoving;
        isMoving = moveDirection.magnitude > 0.1f && isGrounded;

        // Apply movement speed
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isRunning ? sprintSpeed : walkSpeed;
        moveDirection *= currentSpeed;

        // Handle jumping - using our combined ground check
        if (canJump && Input.GetKeyDown(KeyCode.Space))
        {
            verticalVelocity = jumpForce;
        }

        // Apply gravity
        verticalVelocity += gravity * Time.deltaTime;
        Vector3 motion = moveDirection;
        motion.y = verticalVelocity;

        // Apply movement
        controller.Move(motion * Time.deltaTime);

        // Handle footstep audio
        HandleFootstepAudio(isRunning);

        // Smooth rotation - calculate target direction first
        if (moveDirection.x != 0 || moveDirection.z != 0)
        {
            targetRotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0, moveDirection.z));
        }

        // Apply smooth rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
    }

    private void HandleFootstepAudio(bool isRunning)
    {
        if (isMoving)
        {
            stepTimer += Time.deltaTime;

            float currentStepInterval = isRunning ? runStepInterval : walkStepInterval;

            if (stepTimer >= currentStepInterval)
            {
                PlayFootstepSound(isRunning);
                stepTimer = 0f;
            }
        }
        else
        {
            // Reset timer when not moving
            stepTimer = 0f;
        }
    }

    private void PlayFootstepSound(bool isRunning)
    {
        AudioClip[] soundArray;

        // Use running sounds if available and running, otherwise use footstep sounds
        if (isRunning && runningSounds != null && runningSounds.Length > 0)
        {
            soundArray = runningSounds;
        }
        else if (footstepSounds != null && footstepSounds.Length > 0)
        {
            soundArray = footstepSounds;
        }
        else
        {
            return; // No sounds available
        }

        // Play random sound from the array with pitch variation
        AudioClip randomClip = soundArray[Random.Range(0, soundArray.Length)];
        if (randomClip != null)
        {
            float randomPitch = Random.Range(minPitchVariation, maxPitchVariation);
            audioSource.pitch = randomPitch;
            audioSource.PlayOneShot(randomClip, footstepVolume);
        }
    }

    private void PlayLandingSound()
    {
        if (landingSounds != null && landingSounds.Length > 0)
        {
            AudioClip randomLandingClip = landingSounds[Random.Range(0, landingSounds.Length)];
            if (randomLandingClip != null)
            {
                audioSource.PlayOneShot(randomLandingClip, landingVolume);
            }
        }
    }

    // New cursor lock methods
    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void ToggleCursorLock()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            UnlockCursor();
        }
        else
        {
            LockCursor();
        }
    }

    private void LateUpdate()
    {
        // Handle platform movement
        if (platformParent != null)
        {
            // Apply platform movement to player
            Vector3 platformDelta = platformParent.position - lastPlatformPosition;
            if (platformDelta.magnitude > 0.001f)
            {
                controller.Move(platformDelta);
            }
            lastPlatformPosition = platformParent.position;
        }
    }

    // Handle pushing rigidbodies
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody hitRigidbody = hit.collider.attachedRigidbody;

        if (hitRigidbody != null && !hitRigidbody.isKinematic)
        {
            Vector3 pushDir = hit.point - transform.position;
            pushDir.y = 0;
            hitRigidbody.AddForce(pushDir.normalized * pushPower, ForceMode.Impulse);
        }
    }

    // Public method to set platform parent
    public void SetPlatform(Transform platform)
    {
        platformParent = platform;
        if (platform != null)
        {
            lastPlatformPosition = platform.position;
        }
    }

    // Public method to clear platform parent
    public void ClearPlatform()
    {
        platformParent = null;
    }

    // Visual debugging
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}