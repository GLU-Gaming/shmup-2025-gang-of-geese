using UnityEngine;

public class bossdamage : MonoBehaviour
{

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public hpsystem playerHpSystemInstance;

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