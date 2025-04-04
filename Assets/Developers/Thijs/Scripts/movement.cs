using UnityEngine;

public class movement : MonoBehaviour
{
    public float xRange = 5f;
    public float horizontalSpeed = 5f;

    [Header("Car Specifications")]
    public float baseMotorForce = 15f;
    public float accelerationForce = 5f;
    public float brakeForce = 3f;
    public float maxSpeed = 100f;
    public float minSpeed = 20f;
    public float reverseMaxSpeed = 30f;

    private Rigidbody rb;
    private float accelerationInput;
    private float horizontalInput;
    private float currentBrakeForce;
    private bool isReversing = false;
    private float currentSpeedMultiplier = 1f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            if ((rb.constraints & RigidbodyConstraints.FreezePositionZ) != 0)
            {
                rb.constraints &= ~RigidbodyConstraints.FreezePositionZ;
                Debug.Log("Unfroze Z-axis constraint to allow forward movement");
            }

            rb.linearDamping = 5f;
        }
        else
        {
            Debug.LogError("No Rigidbody component found! Please add a Rigidbody to this GameObject.");
        }
    }

    void Update()
    {
        GetInput();
        //Debug.Log($"Speed Multiplier: {currentSpeedMultiplier}, Current Speed: {rb.linearVelocity.magnitude}");ws
    }

    private void FixedUpdate()
    {
        HandleHorizontalMovement();
        ApplyBoundaries();
        LimitSpeed();
    }

    void GetInput()
    {
        horizontalInput = 0f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            horizontalInput = 1f;
        }
        else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            horizontalInput = -1f;
        }

        accelerationInput = 0f;
        currentBrakeForce = 0f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            currentSpeedMultiplier += 0.05f;
            currentSpeedMultiplier = Mathf.Clamp(currentSpeedMultiplier, 0.5f, 2.0f);
            accelerationInput = 1f;
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            currentSpeedMultiplier -= 0.05f;
            currentSpeedMultiplier = Mathf.Clamp(currentSpeedMultiplier, 0.5f, 2.0f);
            float forwardVelocity = Vector3.Dot(rb.linearVelocity, transform.forward);
            if (forwardVelocity < minSpeed && currentSpeedMultiplier < 0.6f)
            {
                currentSpeedMultiplier = 0.6f;
            }
        }
    }

    private void HandleHorizontalMovement()
    {
        if (horizontalInput != 0)
        {
            Vector3 horizontalVelocity = Vector3.right * horizontalInput * horizontalSpeed;
            Vector3 currentVelocity = rb.linearVelocity;
            Vector3 targetVelocity = new Vector3(horizontalVelocity.x, currentVelocity.y, currentVelocity.z);
            Vector3 velocityChange = targetVelocity - currentVelocity;
            velocityChange.y = 0;
            velocityChange.z = 0;
            rb.AddForce(velocityChange, ForceMode.VelocityChange);
        }
    }

    void ApplyBoundaries()
    {
        Vector3 position = transform.position;
        position.x = Mathf.Clamp(position.x, -xRange, xRange);
        transform.position = position;
    }

    private void LimitSpeed()
    {
        float currentSpeed = rb.linearVelocity.magnitude;
        float adjustedMaxSpeed = maxSpeed * currentSpeedMultiplier;
        adjustedMaxSpeed = Mathf.Clamp(adjustedMaxSpeed, minSpeed, maxSpeed);
        if (currentSpeed > adjustedMaxSpeed)
        {
            float brakeSpeed = currentSpeed - adjustedMaxSpeed;
            Vector3 normalizedVelocity = rb.linearVelocity.normalized;
            Vector3 brakeVelocity = normalizedVelocity * brakeSpeed;
            rb.AddForce(-brakeVelocity * 2);
        }
    }

   
}
