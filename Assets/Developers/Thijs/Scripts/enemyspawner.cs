
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
    public Wave[] waveSequences;

    [Header("Spawn Settings")]
    public BoxCollider spawnArea;
    [Range(0f, 2f)]
    public float spawnOffset = 0.5f;
    public float fixedYPosition = 0f;

    [Header("Target Settings")]
    public Transform[] targetTransforms;
    public float enemyMoveSpeed = 3f;

    [Header("Debugging")]
    public bool showSpawnAreaGizmos = true;

    private int currentSequenceIndex = 0;
    private int currentEnemyIndex = 0;
    private bool isSpawning = false;
    private int spawnCount = 0;  // Track the number of enemies spawned

    private void Start()
    {
        if (spawnArea == null)
        {
            Debug.LogError("No spawn area BoxCollider assigned to EnemySpawner!");
            enabled = false;
            return;
        }

        if (targetTransforms == null || targetTransforms.Length == 0)
        {
            Debug.LogWarning("No target transforms assigned! Enemies won't know where to move.");
        }

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
                if (waveSequence.enemyPrefabs == null || waveSequence.enemyPrefabs.Length == 0)
                {
                    Debug.LogError("No enemy prefabs assigned to wave: " + waveSequence.waveName);
                    yield break;
                }

                GameObject enemyPrefab = waveSequence.enemyPrefabs[currentEnemyIndex];
                SpawnEnemy(enemyPrefab);
                currentEnemyIndex++;
                yield return new WaitForSeconds(waveSequence.spawnDelay);
            }

            while (GameObject.FindGameObjectsWithTag("Enemy").Length > 0)
            {
                yield return null;
            }

            currentSequenceIndex++;
            Debug.Log("Wave Sequence Completed! Waiting before next sequence...");
            yield return new WaitForSeconds(3f);
        }

        Debug.Log("All wave sequences completed!");
        isSpawning = false;
    }

    private void SpawnEnemy(GameObject enemyPrefab)
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("Attempted to spawn null enemy prefab!");
            return;
        }

        Vector3 localMin = spawnArea.center - spawnArea.size / 2;
        Vector3 localMax = spawnArea.center + spawnArea.size / 2;

        Vector3 localSpawnPosition = new Vector3(
            Random.Range(localMin.x, localMax.x),
            fixedYPosition,
            Random.Range(localMin.z, localMax.z)
        );

        Vector3 worldSpawnPosition = spawnArea.transform.TransformPoint(localSpawnPosition);

        Vector3 randomOffset = Random.insideUnitSphere * spawnOffset;
        randomOffset.y = 0;
        worldSpawnPosition += randomOffset;

        GameObject spawnedEnemy = Instantiate(enemyPrefab, worldSpawnPosition, Quaternion.identity);

        Rigidbody enemyRigidbody = spawnedEnemy.GetComponent<Rigidbody>();
        if (enemyRigidbody != null)
        {
            enemyRigidbody.freezeRotation = true;
        }

        EnemyMovement movement = spawnedEnemy.GetComponent<EnemyMovement>();
        if (movement == null)
        {
            movement = spawnedEnemy.AddComponent<EnemyMovement>();
        }

        if (movement != null && targetTransforms != null && targetTransforms.Length > 0)
        {
            // Determine the target based on the spawn count
            int targetIndex = spawnCount % targetTransforms.Length;  // Use modulo to cycle through targets
            Transform target = targetTransforms[targetIndex];

            movement.SetTarget(target, enemyMoveSpeed);

            spawnCount++;  // Increment the spawn count for the next enemy
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!showSpawnAreaGizmos || spawnArea == null) return;
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.matrix = spawnArea.transform.localToWorldMatrix;
        Gizmos.DrawCube(spawnArea.center, spawnArea.size);

        if (targetTransforms != null && targetTransforms.Length > 0 && targetTransforms[0] != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, targetTransforms[0].position);
            Gizmos.DrawSphere(targetTransforms[0].position, 0.5f);
        }
    }
}
