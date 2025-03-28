using UnityEngine;
using System.Collections.Generic;

public class InfiniteHighway : MonoBehaviour
{
    public GameObject[] roadPrefabs; // Array of possible road prefabs
    public int[] spawnWeights; // Corresponding weights for each prefab
    public int numberOfRoads = 7;
    public float roadLength = 20f;
    public float speed = 10f;
    public float recycleThreshold = 5f;

    private List<GameObject> roads;
    private float spawnZ = 0f;
    private Transform playerTransform;

    void Start()
    {
        roads = new List<GameObject>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }

        for (int i = 0; i < numberOfRoads; i++)
        {
            SpawnRoad();
        }
    }

    void Update()
    {
        MoveRoads();

        if (roads.Count > 0 && roads[0].transform.position.z < -roadLength + recycleThreshold)
        {
            RecycleRoad();
        }
    }

    void MoveRoads()
    {
        float moveAmount = speed * Time.deltaTime;

        foreach (var road in roads)
        {
            Vector3 pos = road.transform.position;
            pos.z -= moveAmount;
            road.transform.position = pos;
        }
    }

    void SpawnRoad()
    {
        GameObject selectedRoadPrefab = GetRandomRoadPrefab(); // Use weighted selection
        GameObject newRoad = Instantiate(selectedRoadPrefab, new Vector3(0f, 0f, spawnZ), Quaternion.identity);
        newRoad.transform.SetParent(transform);
        roads.Add(newRoad);
        spawnZ += roadLength;
    }

    void RecycleRoad()
    {
        if (roads.Count == 0)
        {
            return;
        }

        GameObject oldRoad = roads[0];
        roads.RemoveAt(0);
        Destroy(oldRoad);

        if (roads.Count > 0)
        {
            GameObject lastRoad = roads[roads.Count - 1];
            spawnZ = lastRoad.transform.position.z + roadLength;
        }

        SpawnRoad();
    }

    GameObject GetRandomRoadPrefab()
    {
        int totalWeight = 0;
        foreach (int weight in spawnWeights)
        {
            totalWeight += weight;
        }

        int randomValue = Random.Range(0, totalWeight);
        int cumulativeWeight = 0;

        for (int i = 0; i < roadPrefabs.Length; i++)
        {
            cumulativeWeight += spawnWeights[i];
            if (randomValue < cumulativeWeight)
            {
                return roadPrefabs[i];
            }
        }

        return roadPrefabs[0]; // Fallback
    }
}
