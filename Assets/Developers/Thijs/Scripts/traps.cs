using UnityEngine;

public class traps : MonoBehaviour
{
    private hpsystem playerHpSystem;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Find the HP system component
        playerHpSystem = Object.FindFirstObjectByType<hpsystem>();
        if (playerHpSystem == null)
        {
            Debug.LogError("No HP system found in the scene!");
        }
    }


   

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Apply damage to the player
            if (playerHpSystem != null)
            {
                playerHpSystem.TakeDamage();

            }
            else
            {
                Debug.LogError("Player HP System is null!");
            }

            // Destroy the bullet upon hitting the player
            Destroy(gameObject);
        }

        //Destroy bullet on any other collision
        else
        {
            Destroy(gameObject);
        }

    }
}
