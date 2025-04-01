using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [Tooltip("List of enemy prefabs to spawn")]
    public GameObject[] enemyPrefabs;

    [Tooltip("Spawn chances for each prefab (0-100)")]
    public int[] spawnChances;

    [Tooltip("Box Collider defining spawn area")]
    public BoxCollider spawnArea;

    [Tooltip("Minimum delay between enemy spawns")]
    [Range(0.5f, 5f)]
    public float minSpawnDelay = 1f;

    [Tooltip("Maximum delay between enemy spawns")]
    [Range(0.5f, 5f)]
    public float maxSpawnDelay = 3f;

    [Tooltip("Number of enemies to spawn in each wave")]
    [Range(1, 20)]
    public int enemiesPerWave = 5;

    [Tooltip("Total number of waves to spawn")]
    [Range(1, 10)]
    public int totalWaves = 3;

    [Tooltip("Spawn offset to prevent enemies from spawning exactly on top of each other")]
    [Range(0f, 2f)]
    public float spawnOffset = 0.5f;

    [Tooltip("Fixed Y position for spawning")]
    public float fixedYPosition = 0f;

    [Header("Debugging")]
    public bool showSpawnAreaGizmos = true;

    private int currentWave = 0;
    private int enemiesSpawnedInCurrentWave = 0;

    private void Start()
    {
        // Validate spawn area
        if (spawnArea == null)
        {
            Debug.LogError("No spawn area BoxCollider assigned to EnemySpawner!");
            enabled = false;
            return;
        }

        // Check if spawn chances match prefabs
        if (spawnChances.Length != enemyPrefabs.Length)
        {
            Debug.LogWarning("Spawn chances array length doesn't match enemy prefabs array length!");
        }

        // Start spawning waves
        StartCoroutine(SpawnWaves());
    }

    private IEnumerator SpawnWaves()
    {
        while (currentWave < totalWaves)
        {
            // Reset enemies spawned in current wave
            enemiesSpawnedInCurrentWave = 0;

            // Spawn enemies for current wave
            while (enemiesSpawnedInCurrentWave < enemiesPerWave)
            {
                // Spawn an enemy
                SpawnEnemy();

                // Wait for next spawn
                float spawnDelay = Random.Range(minSpawnDelay, maxSpawnDelay);
                yield return new WaitForSeconds(spawnDelay);
            }

            // Move to next wave
            currentWave++;

            // Optional: Add a longer pause between waves
            yield return new WaitForSeconds(2f);
        }

        // Optionally, you can add logic for what happens after all waves are spawned
        Debug.Log("All waves completed!");
    }

    private void SpawnEnemy()
    {
        // Check if we have enemy prefabs
        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            Debug.LogError("No enemy prefabs assigned to spawn!");
            return;
        }

        // Select a prefab based on spawn chances
        GameObject enemyToSpawn = SelectEnemyBasedOnChance();

        // Get spawn area local bounds
        Vector3 localMin = spawnArea.center - spawnArea.size / 2;
        Vector3 localMax = spawnArea.center + spawnArea.size / 2;

        // Generate random position within the spawn area's local bounds
        Vector3 localSpawnPosition = new Vector3(
            Random.Range(localMin.x, localMax.x),
            fixedYPosition, // Use fixed Y position
            Random.Range(localMin.z, localMax.z)
        );

        // Convert local position to world space
        Vector3 worldSpawnPosition = spawnArea.transform.TransformPoint(localSpawnPosition);

        // Add small random offset to prevent spawning in exact same position
        Vector3 randomOffset = Random.insideUnitSphere * spawnOffset;
        randomOffset.y = 0; // Keep Y position fixed
        worldSpawnPosition += randomOffset;

        // Instantiate the enemy
        GameObject spawnedEnemy = Instantiate(enemyToSpawn, worldSpawnPosition, Quaternion.Euler(0, 0, 0));

        Rigidbody enemyRigidbody = spawnedEnemy.GetComponent<Rigidbody>();
        if (enemyRigidbody != null)
        {
            enemyRigidbody.freezeRotation = true;
        }

        enemiesSpawnedInCurrentWave++;
    }

    private GameObject SelectEnemyBasedOnChance()
    {
        // If no spawn chances set, use default random selection
        if (spawnChances == null || spawnChances.Length == 0)
        {
            return enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        }

        // Get a random number from 0 to 100
        int randomValue = Random.Range(0, 101);

        // Find which enemy should spawn based on chance
        int currentChanceValue = 0;

        for (int i = 0; i < enemyPrefabs.Length && i < spawnChances.Length; i++)
        {
            currentChanceValue += spawnChances[i];

            if (randomValue <= currentChanceValue)
            {
                return enemyPrefabs[i];
            }
        }

        // Default fallback to first enemy
        return enemyPrefabs[0];
    }

    // Visualize spawn area in Scene view
    private void OnDrawGizmosSelected()
    {
        if (!showSpawnAreaGizmos || spawnArea == null) return;

        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.matrix = spawnArea.transform.localToWorldMatrix;
        Gizmos.DrawCube(spawnArea.center, spawnArea.size);
    }
}