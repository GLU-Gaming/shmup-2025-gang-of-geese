using UnityEngine;
using System.Collections;

public class TrapSpawner : MonoBehaviour
{
    public GameObject trapPrefab;
    public float spawnInterval = 5f;
    public Vector3 spawnArea = new Vector3(10f, 0f, 10f);

    void Start()
    {
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

        // Instantiate the trap at the spawner's position plus a random offset
        GameObject trap = Instantiate(trapPrefab, transform.position + randomPosition, Quaternion.identity);

        // Get the Rigidbody component from the instantiated trap
        Rigidbody trapRb = trap.GetComponent<Rigidbody>();

        // If the trap doesn't have a Rigidbody, add one
        if (trapRb == null)
        {
            trapRb = trap.AddComponent<Rigidbody>();
        }

        // Configure the Rigidbody as needed (e.g., isKinematic, collision settings)
        trapRb.isKinematic = true; // If you don't want the trap to be affected by physics
        trapRb.useGravity = false; // If you don't want the trap to be affected by gravity
    }
}
