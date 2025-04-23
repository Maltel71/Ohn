using UnityEngine;
using System.Collections;

public class RandomCharacterMovement : MonoBehaviour
{
    // Movement settings
    public float moveForce = 10f;
    public float jumpForce = 300f;
    public float maxSpeed = 3f;
    public float boundaryRadius = 10f;

    // Behavior chances (per second)
    [Range(0, 1)]
    public float chanceToChangeDirection = 0.3f;
    [Range(0, 1)]
    public float chanceToPause = 0.2f;
    [Range(0, 1)]
    public float chanceToJump = 0.1f;

    // Movement state
    private Vector3 moveDirection;
    private Vector3 startPosition;
    private Rigidbody rb;
    private bool isPaused = false;

    // Timers for random events
    private float directionChangeTimer = 0f;
    private float pauseTimer = 0f;
    private float jumpTimer = 0f;

    private void Start()
    {
        // Make sure we have the required components
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }

        // Ensure we have a sphere collider
        if (GetComponent<SphereCollider>() == null)
        {
            gameObject.AddComponent<SphereCollider>();
        }

        // Record starting position
        startPosition = transform.position;

        // Initial random direction
        PickNewDirection();
    }

    private void Update()
    {
        // Handle random decision timers
        directionChangeTimer -= Time.deltaTime;
        pauseTimer -= Time.deltaTime;
        jumpTimer -= Time.deltaTime;

        // Check for random behavior changes
        if (directionChangeTimer <= 0 && !isPaused)
        {
            directionChangeTimer = 1f / chanceToChangeDirection;
            if (Random.value < chanceToChangeDirection * Time.deltaTime * 10)
            {
                PickNewDirection();
            }
        }

        if (pauseTimer <= 0 && !isPaused)
        {
            pauseTimer = 1f / chanceToPause;
            if (Random.value < chanceToPause * Time.deltaTime * 10)
            {
                StartCoroutine(Pause());
            }
        }

        if (jumpTimer <= 0 && IsGrounded() && !isPaused)
        {
            jumpTimer = 1f / chanceToJump;
            if (Random.value < chanceToJump * Time.deltaTime * 10)
            {
                Jump();
            }
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
        float duration = Random.Range(1f, 3f);

        // Look around behavior
        if (Random.value > 0.5f)
        {
            // Just look in random directions
            float lookTime = duration * 0.8f;
            float startTime = Time.time;

            while (Time.time < startTime + lookTime)
            {
                if (Random.value > 0.95f)
                {
                    // Look in a new direction occasionally
                    PickNewDirection();
                }
                yield return null;
            }
        }

        yield return new WaitForSeconds(Random.Range(0.2f, 0.5f));
        isPaused = false;
    }
}