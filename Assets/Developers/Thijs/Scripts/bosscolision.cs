using UnityEngine;

public class BossCollisionHandler : MonoBehaviour
{
    [Header("Collision Settings")]
    [SerializeField] private float contactDamage = 25f;
    [SerializeField] private float knockbackForce = 10f;
    [SerializeField] private float damageInterval = 1.0f;
    [SerializeField] private LayerMask playerLayer;

    [Header("Effects")]
    [SerializeField] private GameObject collisionEffect;

    // References
    private Boss bossController;

    // Internal state
    private float lastDamageTime = 0f;

    private void Start()
    {
        bossController = GetComponent<Boss>();
        if (bossController == null)
        {
            Debug.LogError("BossCollisionHandler: No Boss component found on this GameObject!");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        HandlePlayerCollision(collision.gameObject);
    }

    private void OnCollisionStay(Collision collision)
    {
        // For continuous collision, only apply damage based on interval
        if (Time.time >= lastDamageTime + damageInterval)
        {
            HandlePlayerCollision(collision.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        HandlePlayerCollision(other.gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        // For continuous trigger overlap, only apply damage based on interval
        if (Time.time >= lastDamageTime + damageInterval)
        {
            HandlePlayerCollision(other.gameObject);
        }
    }

    private void HandlePlayerCollision(GameObject collidedObject)
    {
        // Check if the collided object is the player
        if (((1 << collidedObject.layer) & playerLayer) != 0 || collidedObject.CompareTag("Player"))
        {
            lastDamageTime = Time.time;

            // Try to get player health component
            PlayerHealth playerHealth = collidedObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(contactDamage);
                Debug.Log($"Player collided with boss and took {contactDamage} contact damage!");
            }

            // Try to apply knockback
            Rigidbody playerRb = collidedObject.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                Vector3 knockbackDirection = (collidedObject.transform.position - transform.position).normalized;
                playerRb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
            }

            // Spawn collision effect if available
            if (collisionEffect != null)
            {
                Vector3 contactPoint = collidedObject.transform.position;
                GameObject effect = Instantiate(collisionEffect, contactPoint, Quaternion.identity);
                Destroy(effect, 2f);
            }

            // Notify the boss controller if needed
            if (bossController != null)
            {
                // You can add a method to Boss.cs to handle player contact if desired
                // Example: bossController.OnPlayerContact();
            }
        }
    }

    // Optional: Method to modify collision damage based on boss phase
    public void UpdateContactDamage(float newDamage)
    {
        contactDamage = newDamage;
    }
}
