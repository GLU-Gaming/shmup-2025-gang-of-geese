using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class Wave
{
   
    public string waveName;
    
    public GameObject[] enemyPrefabs;
  
    public float spawnDelay = 1f;

    public bool isBossWave = false;
}
public class enemyspawner : MonoBehaviour
{


    [Header("Wave Configuration")]
    [Tooltip("Array of wave arrays. Each inner array represents a sequence of waves.")]
    public Wave[] waveSequences;

    [Header("Spawn Settings")]
    [Tooltip("Box Collider defining spawn area")]
    public BoxCollider spawnArea;
    [Tooltip("Spawn offset to prevent enemies from spawning exactly on top of each other")]
    [Range(0f, 2f)]
    public float spawnOffset = 0.5f;
    [Tooltip("Fixed Y position for spawning")]
    public float fixedYPosition = 0f;

    [Header("Debugging")]
    public bool showSpawnAreaGizmos = true;

    private int currentSequenceIndex = 0;
    private int currentEnemyIndex = 0;
    private bool isSpawning = false;

    private void Start()
    {
        // Validate spawn area
        if (spawnArea == null)
        {
            Debug.LogError("No spawn area BoxCollider assigned to EnemySpawner!");
            enabled = false;
            return;
        }

        // Start spawning waves
        StartCoroutine(SpawnWaveSequences());
    }

    private IEnumerator SpawnWaveSequences()
    {
        isSpawning = true;
        while (currentSequenceIndex < waveSequences.Length)
        {
            Wave waveSequence = waveSequences[currentSequenceIndex];
            currentEnemyIndex = 0;

            while (currentEnemyIndex < waveSequence.enemyPrefabs.Length)
            {
                //Wave currentWave = waveSequence[currentEnemyIndex];
                //Debug.Log("Starting wave: " + currentWave.waveName + " in sequence " + currentSequenceIndex);

                // Spawn all enemies in the current wave
                //yield return StartCoroutine(SpawnWave(currentWave));
                if (waveSequence.enemyPrefabs == null || waveSequence.enemyPrefabs.Length == 0)
                {
                    Debug.LogError("No enemy prefabs assigned to wave: " + waveSequence.waveName);
                    yield break;
                }

                foreach (GameObject enemyPrefab in waveSequence.enemyPrefabs)
                {
                    currentEnemyIndex++;
                    SpawnEnemy(enemyPrefab);
                    yield return new WaitForSeconds(waveSequence.spawnDelay);
                }

                //Debug.Log("Wave Completed! Waiting before next wave...");
                //yield return new WaitForSeconds(2f); // Wait between waves
            }

            //zolang de enemies nog leven
                //wacht

            currentSequenceIndex++;
            Debug.Log("Wave Sequence Completed! Waiting before next sequence...");
            yield return new WaitForSeconds(3f); // Wait between sequences
        }

        Debug.Log("All wave sequences completed!");
        isSpawning = false;
    }

    private IEnumerator SpawnWave(Wave wave)
    {
        if (wave.enemyPrefabs == null || wave.enemyPrefabs.Length == 0)
        {
            Debug.LogError("No enemy prefabs assigned to wave: " + wave.waveName);
            yield break;
        }

        foreach (GameObject enemyPrefab in wave.enemyPrefabs)
        {
            SpawnEnemy(enemyPrefab);
            yield return new WaitForSeconds(wave.spawnDelay);
        }
    }

    private void SpawnEnemy(GameObject enemyPrefab)
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("Attempted to spawn null enemy prefab!");
            return;
        }

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
        GameObject spawnedEnemy = Instantiate(enemyPrefab, worldSpawnPosition, Quaternion.Euler(0, 0, 0));
        Rigidbody enemyRigidbody = spawnedEnemy.GetComponent<Rigidbody>();
        if (enemyRigidbody != null)
        {
            enemyRigidbody.freezeRotation = true;
        }
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
