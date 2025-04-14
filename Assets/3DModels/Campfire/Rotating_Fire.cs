using UnityEngine;

public class RotatingObject : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("Maximum rotation speed in degrees per second")]
    public Vector3 maxRotationSpeed = new Vector3(30f, 30f, 30f);

    [Tooltip("How quickly the rotation changes direction")]
    [Range(0.1f, 5.0f)]
    public float smoothTime = 1.0f;

    [Tooltip("Interval between random rotation changes (seconds)")]
    [Range(1.0f, 10.0f)]
    public float changeInterval = 3.0f;

    // Current rotation velocity
    private Vector3 currentRotationVelocity;

    // Target rotation velocity
    private Vector3 targetRotationVelocity;

    // Timer for changing rotation
    private float changeTimer;

    // Velocity smoothing variables
    private Vector3 velocitySmoothDampX = Vector3.zero;
    private Vector3 velocitySmoothDampY = Vector3.zero;
    private Vector3 velocitySmoothDampZ = Vector3.zero;

    private void Start()
    {
        // Initialize with a random rotation target
        SetNewRandomRotationTarget();
    }

    private void Update()
    {
        // Update timer and change rotation if needed
        changeTimer -= Time.deltaTime;
        if (changeTimer <= 0)
        {
            SetNewRandomRotationTarget();
            changeTimer = changeInterval;
        }

        // Smoothly interpolate current rotation velocity toward target
        currentRotationVelocity.x = Mathf.SmoothDamp(currentRotationVelocity.x, targetRotationVelocity.x, ref velocitySmoothDampX.x, smoothTime);
        currentRotationVelocity.y = Mathf.SmoothDamp(currentRotationVelocity.y, targetRotationVelocity.y, ref velocitySmoothDampY.y, smoothTime);
        currentRotationVelocity.z = Mathf.SmoothDamp(currentRotationVelocity.z, targetRotationVelocity.z, ref velocitySmoothDampZ.z, smoothTime);

        // Apply rotation
        transform.Rotate(currentRotationVelocity * Time.deltaTime);
    }

    private void SetNewRandomRotationTarget()
    {
        // Generate random values between -1 and 1 for each axis
        targetRotationVelocity = new Vector3(
            Random.Range(-1f, 1f) * maxRotationSpeed.x,
            Random.Range(-1f, 1f) * maxRotationSpeed.y,
            Random.Range(-1f, 1f) * maxRotationSpeed.z
        );
    }
}