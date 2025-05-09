using UnityEngine;

[RequireComponent(typeof(CharacterController))]
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

    private CharacterController controller;
    private Transform cameraTransform;
    private Vector3 moveDirection;
    private float verticalVelocity;
    private bool isGrounded;
    private Quaternion targetRotation;

    // Platform movement
    private Transform platformParent;
    private Vector3 lastPlatformPosition;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        cameraTransform = Camera.main.transform;
        targetRotation = transform.rotation;

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
        if (Input.GetKeyDown(toggleCursorLockKey))
        {
            ToggleCursorLock();
        }

        // Custom ground check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        // Platform-based grounding
        bool canJump = isGrounded;
        if (platformParent != null)
        {
            Vector3 platformDelta = platformParent.position - lastPlatformPosition;
            canJump = canJump || platformDelta.y > 0;
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

        // Apply movement speed
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;
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

        // Smooth rotation - calculate target direction first
        if (moveDirection.x != 0 || moveDirection.z != 0)
        {
            targetRotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0, moveDirection.z));
        }

        // Apply smooth rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
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