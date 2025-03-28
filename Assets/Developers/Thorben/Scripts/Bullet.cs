using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed = 20f;
    public float bulletLifetime = 5f;

    private Rigidbody rb;

    private ScoreSystem scoreSystem;

    void Start()
    {
        scoreSystem = FindFirstObjectByType<ScoreSystem>();

        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Use AddForce instead of directly setting velocity
            rb.useGravity = false;
            rb.AddForce(-transform.right * bulletSpeed, ForceMode.VelocityChange);
        }
        Destroy(gameObject, bulletLifetime);
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            // Normalize the velocity to maintain consistent speed
            Vector3 currentVelocity = rb.linearVelocity;
            Vector3 desiredVelocity = -transform.right * bulletSpeed;

            // Smoothly correct the velocity
            rb.linearVelocity = Vector3.Lerp(currentVelocity, desiredVelocity, Time.fixedDeltaTime * 10f);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            scoreSystem?.AddScore(10);
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}