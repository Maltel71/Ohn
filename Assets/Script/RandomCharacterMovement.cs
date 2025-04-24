using UnityEngine;
using System.Collections;

public class RandomCharacterMovement : MonoBehaviour
{
    // Movement settings
    public float moveForce = 10f;
    public float jumpForce = 300f;
    public float maxSpeed = 3f;
    public float boundaryRadius = 10f;

    // Behavior timing (in seconds)
    public float changeDirectionInterval = 5f;
    public float pauseInterval = 10f;
    public float jumpInterval = 4f;
    public float pauseDuration = 2f;

    // Internal timers
    private float directionTimer;
    private float pauseTimer;
    private float jumpTimer;

    // Movement state
    private Vector3 moveDirection;
    private Vector3 startPosition;
    private Rigidbody rb;
    private bool isPaused = false;

    private void Start()
    {
        // Make sure we have the required components
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        // Freeze rotation on X and Z axes to keep character upright
        // but allow rotation on Y axis so character can face movement direction
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        // Ensure we have a sphere collider
        if (GetComponent<SphereCollider>() == null)
        {
            gameObject.AddComponent<SphereCollider>();
        }

        // Record starting position
        startPosition = transform.position;

        // Initial random direction
        PickNewDirection();

        // Randomize initial timers so behaviors don't all happen at once
        directionTimer = Random.Range(0f, changeDirectionInterval);
        pauseTimer = Random.Range(0f, pauseInterval);
        jumpTimer = Random.Range(0f, jumpInterval);
    }

    private void Update()
    {
        if (isPaused) return;

        // Update timers
        directionTimer -= Time.deltaTime;
        pauseTimer -= Time.deltaTime;
        jumpTimer -= Time.deltaTime;

        // Change direction on interval
        if (directionTimer <= 0)
        {
            PickNewDirection();
            directionTimer = changeDirectionInterval;
        }

        // Pause on interval
        if (pauseTimer <= 0)
        {
            StartCoroutine(Pause());
            pauseTimer = pauseInterval;
        }

        // Jump on interval if grounded
        if (jumpTimer <= 0)
        {
            // Debug info to help us understand what's happening
            Debug.Log("Jump timer expired, is grounded: " + IsGrounded());

            if (IsGrounded())
            {
                Jump();
            }

            // Reset timer regardless to avoid jump spam if grounding fails
            jumpTimer = jumpInterval;
        }

        // Check boundary
        if (Vector3.Distance(transform.position, startPosition) > boundaryRadius)
        {
            // Head back toward start position
            moveDirection = (startPosition - transform.position).normalized;
        }
    }

    private void FixedUpdate()
    {
        if (isPaused) return;

        // Apply movement force
        rb.AddForce(moveDirection * moveForce);

        // Limit horizontal speed
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        if (horizontalVelocity.magnitude > maxSpeed)
        {
            horizontalVelocity = horizontalVelocity.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(horizontalVelocity.x, rb.linearVelocity.y, horizontalVelocity.z);
        }

        // Make character face movement direction
        if (horizontalVelocity.magnitude > 0.1f)
        {
            // Create rotation that looks in the direction of movement
            Quaternion targetRotation = Quaternion.LookRotation(horizontalVelocity);

            // Smoothly rotate towards that direction
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 5f);
        }
    }

    private bool IsGrounded()
    {
        // Simple ground check
        return Physics.Raycast(transform.position, Vector3.down, GetComponent<SphereCollider>().radius + 0.1f);
    }

    private void PickNewDirection()
    {
        // Get random direction on xz plane
        float angle = Random.Range(0, 360) * Mathf.Deg2Rad;
        moveDirection = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
    }

    private void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce);
    }

    private IEnumerator Pause()
    {
        isPaused = true;

        // Look around behavior
        if (Random.value > 0.5f)
        {
            float startTime = Time.time;
            float lookDuration = pauseDuration * 0.8f;

            while (Time.time < startTime + lookDuration)
            {
                // Look in a new direction occasionally
                if (Random.value > 0.95f)
                {
                    PickNewDirection();
                }
                yield return null;
            }
        }

        yield return new WaitForSeconds(pauseDuration - (pauseDuration * 0.8f));
        isPaused = false;
    }
}