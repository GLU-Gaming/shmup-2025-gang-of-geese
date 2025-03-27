using UnityEngine;

public class bounderies : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        // Check if the entering object has the tag "Enemy"
        if (other.gameObject.CompareTag("Enemy"))
        {
            // Get the Rigidbody of the enemy
            Rigidbody enemyRigidbody = other.GetComponent<Rigidbody>();

            // Check if the enemy has a Rigidbody
            if (enemyRigidbody != null)
            {
                // Stop the enemy's movement by setting its velocity to zero
                enemyRigidbody.linearVelocity = Vector3.zero;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Check if the entering object has the tag "Enemy"
        if (other.gameObject.CompareTag("Enemy"))
        {
            // Get the Rigidbody of the enemy
            Rigidbody enemyRigidbody = other.GetComponent<Rigidbody>();

            // Check if the enemy has a Rigidbody
            if (enemyRigidbody != null)
            {
                // Stop the enemy's movement by setting its velocity to zero
                enemyRigidbody.linearVelocity = Vector3.zero;
            }
        }
    }
}

