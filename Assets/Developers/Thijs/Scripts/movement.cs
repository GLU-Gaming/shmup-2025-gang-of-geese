using UnityEngine;

public class movement : MonoBehaviour
{
    public float xRange = 5f; // Horizontal movement boundaries
    public float horizontalSpeed = 5f; // Speed of horizontal movement

    [Header("Car Specifications")]
    public float baseMotorForce = 15f; // Base forward force (auto-movement)
    public float accelerationForce = 5f; // Additional force when accelerating
    public float brakeForce = 3f;
    public float maxSpeed = 100f;
    public float minSpeed = 20f; // Minimum speed the car will maintain
    public float reverseMaxSpeed = 30f;

    // Private variables
    private Rigidbody rb;
    private float accelerationInput;
    private float horizontalInput;
    private float currentBrakeForce;
    private bool isReversing = false;
    private float currentSpeedMultiplier = 1f; // Multiplier for speed control

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Make sure rigidbody is properly configured
        if (rb != null)
        {
            // Check for frozen position constraints
            if ((rb.constraints & RigidbodyConstraints.FreezePositionZ) != 0)
            {
                // Unfreeze Z-axis movement if it was frozen
                rb.constraints &= ~RigidbodyConstraints.FreezePositionZ;
                Debug.Log("Unfroze Z-axis constraint to allow forward movement");
            }

            // Set appropriate drag values
            rb.linearDamping = 0.5f;
        }
        else
        {
            Debug.LogError("No Rigidbody component found! Please add a Rigidbody to this GameObject.");
        }
    }

    void Update()
    {
        // Get input
        GetInput();

        // Debug output for troubleshooting
        Debug.Log($"Speed Multiplier: {currentSpeedMultiplier}, Current Speed: {rb.linearVelocity.magnitude}");
    }

    private void FixedUpdate()
    {


        // Apply horizontal movement
        HandleHorizontalMovement();

        // Apply boundaries
        ApplyBoundaries();

        // Limit speed
        LimitSpeed();
    }

    void GetInput()
    {
        // Get horizontal input
        horizontalInput = 0f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            horizontalInput = 1f;
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            horizontalInput = -1f;
        }

        // Get acceleration and braking input
        accelerationInput = 0f;
        currentBrakeForce = 0f;

        // Increase speed
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            // Increase speed multiplier (clamped to reasonable limits)
            currentSpeedMultiplier += 0.05f;
            currentSpeedMultiplier = Mathf.Clamp(currentSpeedMultiplier, 0.5f, 2.0f);
            accelerationInput = 1f;
        }

        // Decrease speed
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            // Decrease speed multiplier (clamped to reasonable limits)
            currentSpeedMultiplier -= 0.05f;
            currentSpeedMultiplier = Mathf.Clamp(currentSpeedMultiplier, 0.5f, 2.0f);

            // Apply brakes if below minimum speed
            float forwardVelocity = Vector3.Dot(rb.linearVelocity, transform.forward);
            if (forwardVelocity < minSpeed && currentSpeedMultiplier < 0.6f)
            {
                currentSpeedMultiplier = 0.6f; // Prevent going too slow
            }
        }
    }

    private void HandleHorizontalMovement()
    {
        if (horizontalInput != 0)
        {
            // Calculate desired horizontal velocity
            Vector3 horizontalVelocity = Vector3.right * horizontalInput * horizontalSpeed;

            // Only apply horizontal velocity (keep current Y and Z velocity)
            Vector3 currentVelocity = rb.linearVelocity;
            Vector3 targetVelocity = new Vector3(horizontalVelocity.x, currentVelocity.y, currentVelocity.z);

            // Apply horizontal movement using forces
            Vector3 velocityChange = targetVelocity - currentVelocity;
            velocityChange.y = 0;
            velocityChange.z = 0;

            rb.AddForce(velocityChange, ForceMode.VelocityChange);
        }
    }

    void ApplyBoundaries()
    {
        // Clamp position within boundaries
        Vector3 position = transform.position;
        position.x = Mathf.Clamp(position.x, -xRange, xRange);
        transform.position = position;
    }

    private void LimitSpeed()
    {
        float currentSpeed = rb.linearVelocity.magnitude;

        // Calculate dynamic max speed based on multiplier
        float adjustedMaxSpeed = maxSpeed * currentSpeedMultiplier;

        // Ensure we respect the absolute maximum
        adjustedMaxSpeed = Mathf.Clamp(adjustedMaxSpeed, minSpeed, maxSpeed);

        // Apply speed limit
        if (currentSpeed > adjustedMaxSpeed)
        {
            float brakeSpeed = currentSpeed - adjustedMaxSpeed;
            Vector3 normalizedVelocity = rb.linearVelocity.normalized;
            Vector3 brakeVelocity = normalizedVelocity * brakeSpeed;

            rb.AddForce(-brakeVelocity * 2); // Apply limiting force
        }
    }
}