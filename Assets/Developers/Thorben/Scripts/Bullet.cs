using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed = 20f;  // The speed of the bullet
    public float bulletLifetime = 5f;  // The lifetime of the bullet in seconds

    void Start()
    {
        // Add velocity to the bullet
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = transform.forward * -bulletSpeed; // Reverse the direction
        }
        // Destroy the bullet after a certain amount of seconds
        Destroy(gameObject, bulletLifetime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Add score when hitting an enemy
            ScoreSystem.instance.AddScore(10);
            // Destroy the enemy
            Destroy(collision.gameObject);
            // Destroy the bullet
            Destroy(gameObject);
        }
    }
}
