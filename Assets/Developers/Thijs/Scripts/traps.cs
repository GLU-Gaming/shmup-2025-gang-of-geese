using UnityEngine;
using System.Collections;
public class traps : MonoBehaviour
{
    private hpsystem playerHpSystem;
    public float damageAmount = 10f;
    public float pushForce = 5f;
    public float warningDuration = 2f;
    public GameObject warningIndicator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Find the HP system component
        playerHpSystem = Object.FindFirstObjectByType<hpsystem>();
        if (playerHpSystem == null)
        {
            Debug.LogError("No HP system found in the scene!");
        }

        StartCoroutine(ActivateTrap());
    }
    IEnumerator ActivateTrap()
    {
        // Show warning indicator
        if (warningIndicator != null)
        {
            warningIndicator.SetActive(true);
            yield return new WaitForSeconds(warningDuration);
            warningIndicator.SetActive(false);
        }

        // Activate trap collision
        GetComponent<Collider>().enabled = true;
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

            // Push the player back
            Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                Vector3 pushDirection = collision.gameObject.transform.position - transform.position;
                pushDirection.Normalize();
                playerRb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
            }

            // Deactivate the trap
            gameObject.SetActive(false);
        }
    }
}

public class TrapSpawner : MonoBehaviour
{
    private hpsystem playerHpSystem;
    public GameObject trapPrefab;
    public float spawnInterval = 5f;
    public Vector3 spawnArea = new Vector3(10f, 0f, 10f);

    void Start()
    {
        playerHpSystem = Object.FindFirstObjectByType<hpsystem>();
        if (playerHpSystem == null)
        {
            Debug.LogError("No HP system found in the scene!");
        }
        StartCoroutine(SpawnTraps());
    }

    IEnumerator SpawnTraps()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnTrap();
        }
    }

    void SpawnTrap()
    {
        Vector3 randomPosition = new Vector3(
            Random.Range(-spawnArea.x / 2, spawnArea.x / 2),
            0,
            Random.Range(-spawnArea.z / 2, spawnArea.z / 2)
        );

        Instantiate(trapPrefab, transform.position + randomPosition, Quaternion.identity);
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
