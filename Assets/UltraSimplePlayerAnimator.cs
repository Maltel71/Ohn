using UnityEngine;

[RequireComponent(typeof(Animator))]
public class UltraSimplePlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterController controller;

    // Animation parameter names as strings (safer than hashed IDs)
    private const string SPEED_PARAM = "Speed";
    private const string JUMP_PARAM = "Jump";
    private const string GROUNDED_PARAM = "Grounded";
    private const string FREEFALL_PARAM = "FreeFall";
    private const string MOTION_SPEED_PARAM = "MotionSpeed";
    private const string IS_MOVING_PARAM = "IsMoving"; // New parameter for platform fix

    // Optional Ground check reference
    [SerializeField] private Transform groundCheckTransform;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    // Input detection threshold (to distinguish from slight movements)
    [SerializeField] private float inputThreshold = 0.1f;
    [SerializeField] private bool debugMode = false;

    private void Awake()
    {
        // Get components if not already assigned
        if (animator == null)
            animator = GetComponent<Animator>();

        if (controller == null)
            controller = GetComponent<CharacterController>();

        // Get references from the player controller if needed
        if (groundCheckTransform == null)
        {
            var playerController = GetComponent<MinimalThirdPersonController>();
            if (playerController != null && playerController.groundCheck != null)
            {
                groundCheckTransform = playerController.groundCheck;
                groundCheckRadius = playerController.groundCheckRadius;
                groundLayer = playerController.groundLayer;
            }
        }

        // Safety check - make sure parameters exist
        EnsureParameterExists(SPEED_PARAM);
        EnsureParameterExists(JUMP_PARAM);
        EnsureParameterExists(GROUNDED_PARAM);
        EnsureParameterExists(FREEFALL_PARAM);
        EnsureParameterExists(MOTION_SPEED_PARAM);
        EnsureParameterExists(IS_MOVING_PARAM);
    }

    private void EnsureParameterExists(string parameterName)
    {
        // Check if parameter exists in animator
        AnimatorControllerParameter[] parameters = animator.parameters;
        bool exists = false;

        foreach (var param in parameters)
        {
            if (param.name == parameterName)
            {
                exists = true;
                break;
            }
        }

        if (!exists)
        {
            Debug.LogWarning($"Animator parameter '{parameterName}' not found in controller. Animations may not work correctly.");

            // Special warning for IsMoving parameter since it's critical for the platform fix
            if (parameterName == IS_MOVING_PARAM)
            {
                Debug.LogError($"The '{IS_MOVING_PARAM}' parameter is missing! You need to add a bool parameter " +
                              $"named '{IS_MOVING_PARAM}' to your animator for the platform movement fix to work.");
            }
        }
    }

    private void Update()
    {
        UpdateAnimator();
    }

    private void UpdateAnimator()
    {
        // Get player input for movement
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector2 inputVector = new Vector2(horizontalInput, verticalInput);
        float inputMagnitude = inputVector.magnitude;

        // Determine if player is actively trying to move (key input)
        bool isPlayerInputMoving = inputMagnitude > inputThreshold;

        // Calculate speed based on horizontal velocity
        Vector3 horizontalVelocity = new Vector3(controller.velocity.x, 0, controller.velocity.z);
        float speed = horizontalVelocity.magnitude;

        // For animation purposes, calculate speed based on input when actively moving
        float speedParam = isPlayerInputMoving ?
            (Input.GetKey(KeyCode.LeftShift) ? 10f : 5f) * inputMagnitude :
            0f;

        // Check if grounded
        bool isGrounded = false;
        if (groundCheckTransform != null)
        {
            isGrounded = Physics.CheckSphere(groundCheckTransform.position, groundCheckRadius, groundLayer);
        }
        else
        {
            // Fallback - assume grounded if vertical velocity is minimal
            isGrounded = controller.isGrounded || Mathf.Abs(controller.velocity.y) < 0.1f;
        }

        // Check for jumping
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            TrySetTrigger(JUMP_PARAM);
        }

        // Check for freefall
        bool isFreeFalling = !isGrounded && controller.velocity.y < -1.0f;

        // Set animator parameters - with safety checks
        TrySetFloat(SPEED_PARAM, speedParam);
        TrySetBool(GROUNDED_PARAM, isGrounded);
        TrySetBool(FREEFALL_PARAM, isFreeFalling);
        TrySetFloat(MOTION_SPEED_PARAM, Input.GetKey(KeyCode.LeftShift) ? 1.5f : 1.0f);

        // This is the key parameter for the platform fix!
        TrySetBool(IS_MOVING_PARAM, isPlayerInputMoving);

        // Debug info
        if (debugMode)
        {
            Debug.Log($"Input: {inputMagnitude:F2}, IsMoving: {isPlayerInputMoving}, " +
                     $"Speed: {speedParam:F2}, Actual Velocity: {speed:F2}, " +
                     $"Grounded: {isGrounded}");
        }
    }

    // Safe parameter setting methods to prevent errors
    private void TrySetFloat(string paramName, float value)
    {
        try
        {
            animator.SetFloat(paramName, value);
        }
        catch (System.Exception)
        {
            // Parameter might not exist - already logged warning at start
        }
    }

    private void TrySetBool(string paramName, bool value)
    {
        try
        {
            animator.SetBool(paramName, value);
        }
        catch (System.Exception)
        {
            // Parameter might not exist - already logged warning at start
        }
    }

    private void TrySetTrigger(string paramName)
    {
        try
        {
            animator.SetTrigger(paramName);
        }
        catch (System.Exception)
        {
            // Parameter might not exist - already logged warning at start
        }
    }
}