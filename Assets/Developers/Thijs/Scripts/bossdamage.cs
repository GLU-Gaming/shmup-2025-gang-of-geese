using UnityEngine;

public class bossdamage : MonoBehaviour
{

    public hpsystem playerHpSystemInstance;
     

    void Start()
    {
        playerHpSystemInstance = FindAnyObjectByType<hpsystem>();
    }

    void Update()
    {

    }


     void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Apply damage to the player
            if (playerHpSystemInstance != null)
            {
                playerHpSystemInstance.TakeDamage();
            }
            else
            {
                Debug.LogError("Player HP System is null!");
            }

            // Destroy the bullet upon hitting the player
            
            Destroy(gameObject);
        }
        else
        {
            // Destroy bullet on any other collision
            Destroy(gameObject);
        }

    }
}