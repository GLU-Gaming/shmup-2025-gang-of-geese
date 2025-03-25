using UnityEngine;
using System.Collections.Generic;

public class InfiniteHighway : MonoBehaviour
{
    public GameObject roadPrefab;        // The road segment prefab
    public int numberOfRoads = 7;        // Number of road segments to instantiate at once (increased for smoother experience)
    public float roadLength = 20f;       // Length of each road segment
    public float speed = 10f;            // Speed of the road moving
    public float recycleThreshold = 5f;  // Threshold to recycle road before it completely disappears

    private List<GameObject> roads;      // List of current road segments
    private float spawnZ = 0f;           // The z position to spawn the next road
    private Transform playerTransform;   // Reference to player transform for adaptive spawning (optional)

    void Start()
    {
        roads = new List<GameObject>();

        // Find player (if you have one)
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }

        // Initialize with sequential road segments
        for (int i = 0; i < numberOfRoads; i++)
        {
            SpawnRoad();
        }
    }

    void Update()
    {
        MoveRoads();

        // Check if the first road segment has moved past the threshold
        // We recycle it before it's completely gone to avoid visible gaps
        if (roads.Count > 0 && roads[0].transform.position.z < -roadLength + recycleThreshold)
        {
            RecycleRoad();
        }

        // Optional: Adapt speed based on game state
        // For example: speed = Mathf.Lerp(speed, targetSpeed, Time.deltaTime);
    }

    void MoveRoads()
    {
        // Use Time.fixedDeltaTime for smoother physics-based movement
        float moveAmount = speed * Time.deltaTime;

        foreach (var road in roads)
        {
            // Using position directly instead of Translate for more precise positioning
            Vector3 pos = road.transform.position;
            pos.z -= moveAmount;
            road.transform.position = pos;
        }
    }

    void SpawnRoad()
    {
        // Instantiate the new road at the current spawnZ position
        GameObject newRoad = Instantiate(roadPrefab, new Vector3(0f, 0f, spawnZ), Quaternion.identity);
        newRoad.transform.SetParent(transform); // Parent to this object for better hierarchy management
        roads.Add(newRoad);

        // Update the spawn position for the next road, ensuring perfect alignment
        spawnZ += roadLength;
    }

    void RecycleRoad()
    {
        if (roads.Count == 0)
        {
            return; // Exit if there are no roads to recycle
        }

        // Remove the old road from the front of the list
        GameObject oldRoad = roads[0];
        roads.RemoveAt(0);

        // Either destroy or object pool
        Destroy(oldRoad);

        if (roads.Count > 0)
        {
            // Calculate new spawn position based on the position of the last road
            // Using exact positions for perfect alignment
            GameObject lastRoad = roads[roads.Count - 1];
            spawnZ = lastRoad.transform.position.z + roadLength;
        }

        // Spawn a new road at the updated position
        SpawnRoad();
    }
}
