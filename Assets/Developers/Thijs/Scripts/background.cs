using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InfiniteUIBackground : MonoBehaviour
{
    public RectTransform[] backgroundPrefabs; // Array of possible background image prefabs
    public int[] spawnWeights; // Corresponding weights for each prefab
    public int numberOfBackgrounds = 7;
    public float backgroundWidth = 1920f; // Width of your background image in pixels
    public float speed = 500f; // Speed in pixels per second
    public float recycleThreshold = 200f;

    [Header("Debug")]
    public bool showDebugLogs = true;

    // The parent canvas
    private Canvas parentCanvas;
    private RectTransform canvasRect;

    private List<RectTransform> backgrounds;
    private float spawnX = 0f;

    void Start()
    {
        // Initialize
        backgrounds = new List<RectTransform>();

        // Find parent canvas
        parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas == null)
        {
            if (showDebugLogs) Debug.LogError("No parent Canvas found! This script must be on a GameObject that's a child of a Canvas.");
            return;
        }

        canvasRect = parentCanvas.GetComponent<RectTransform>();

        // Log important info
        if (showDebugLogs)
        {
            Debug.Log($"Canvas found: {parentCanvas.name}, Width: {canvasRect.rect.width}px");
            Debug.Log($"Background setup: Count={numberOfBackgrounds}, Width={backgroundWidth}px, Speed={speed}px/s");

            if (backgroundPrefabs == null || backgroundPrefabs.Length == 0)
                Debug.LogError("No background prefabs assigned!");
            else
                Debug.Log($"Background prefabs: {backgroundPrefabs.Length} assigned");
        }

        // Calculate starting position based on canvas width
        // Start just off screen to the right
        spawnX = 0;

        // Spawn initial backgrounds
        for (int i = 0; i < numberOfBackgrounds; i++)
        {
            SpawnBackground();
            if (showDebugLogs) Debug.Log($"Background {i} spawned at x: {spawnX - backgroundWidth}");
        }
    }

    void Update()
    {
        // Skip update if there was an error in Start
        if (parentCanvas == null) return;

        MoveBackgrounds();

        // Check if leftmost background needs recycling
        if (backgrounds.Count > 0)
        {
            RectTransform firstBg = backgrounds[0];
            if (firstBg.anchoredPosition.x < -backgroundWidth - recycleThreshold)
            {
                RecycleBackground();
                if (showDebugLogs) Debug.Log("Recycled background");
            }
        }
    }

    void MoveBackgrounds()
    {
        float moveAmount = speed * Time.deltaTime;

        foreach (var bg in backgrounds)
        {
            // Skip null entries (in case object was destroyed)
            if (bg == null) continue;

            Vector2 pos = bg.anchoredPosition;
            pos.x -= moveAmount;
            bg.anchoredPosition = pos;
        }
    }

    void SpawnBackground()
    {
        // Get a random prefab
        RectTransform selectedBgPrefab = GetRandomBackgroundPrefab();

        // Safety check
        if (selectedBgPrefab == null)
        {
            Debug.LogError("Failed to get a valid background prefab!");
            return;
        }

        // Create the new background
        RectTransform newBg = Instantiate(selectedBgPrefab);

        // Make it a child of this object (which should be on a Canvas)
        newBg.SetParent(transform, false);

        // Force correct size
        newBg.sizeDelta = new Vector2(backgroundWidth, newBg.sizeDelta.y);

        // Set position - anchor it to the left edge of its parent
        newBg.anchorMin = new Vector2(0, 0.5f);
        newBg.anchorMax = new Vector2(0, 0.5f);
        newBg.pivot = new Vector2(0, 0.5f);
        newBg.anchoredPosition = new Vector2(spawnX, 0f);

        // Make sure it's visible
        CanvasGroup canvasGroup = newBg.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = false;
        }

        // Enable the game object
        newBg.gameObject.SetActive(true);

        // Make sure image component is enabled if it exists
        Image image = newBg.GetComponent<Image>();
        if (image != null)
        {
            image.enabled = true;
        }

        // Add to our list
        backgrounds.Add(newBg);

        // Update spawn position for next background
        spawnX += backgroundWidth;

        if (showDebugLogs) Debug.Log($"Spawned background: {newBg.name} at x:{spawnX - backgroundWidth}");
    }

    void RecycleBackground()
    {
        if (backgrounds.Count == 0)
        {
            if (showDebugLogs) Debug.LogWarning("Tried to recycle but no backgrounds exist!");
            return;
        }

        RectTransform oldBg = backgrounds[0];
        backgrounds.RemoveAt(0);

        if (oldBg != null)
        {
            Destroy(oldBg.gameObject);
        }

        if (backgrounds.Count > 0)
        {
            RectTransform lastBg = backgrounds[backgrounds.Count - 1];
            if (lastBg != null)
            {
                spawnX = lastBg.anchoredPosition.x + backgroundWidth;
            }
        }

        SpawnBackground();
    }

    RectTransform GetRandomBackgroundPrefab()
    {
        // Safety check to avoid index out of range errors
        if (backgroundPrefabs == null || backgroundPrefabs.Length == 0)
        {
            Debug.LogError("No background prefabs assigned!");
            return null;
        }

        // If only one prefab, just return it
        if (backgroundPrefabs.Length == 1)
        {
            return backgroundPrefabs[0];
        }

        // If no weights are specified or arrays don't match, just return a random prefab
        if (spawnWeights == null || spawnWeights.Length != backgroundPrefabs.Length)
        {
            if (showDebugLogs) Debug.LogWarning("Background prefabs and spawn weights arrays don't match! Using random selection instead.");
            int randomIndex = Random.Range(0, backgroundPrefabs.Length);
            return backgroundPrefabs[randomIndex];
        }

        // Use weighted selection
        int totalWeight = 0;
        foreach (int weight in spawnWeights)
        {
            totalWeight += weight;
        }

        // If total weight is 0, use random selection
        if (totalWeight <= 0)
        {
            int randomIndex = Random.Range(0, backgroundPrefabs.Length);
            return backgroundPrefabs[randomIndex];
        }

        int randomValue = Random.Range(0, totalWeight);
        int cumulativeWeight = 0;

        for (int i = 0; i < backgroundPrefabs.Length; i++)
        {
            cumulativeWeight += spawnWeights[i];
            if (randomValue < cumulativeWeight)
            {
                return backgroundPrefabs[i];
            }
        }

        return backgroundPrefabs[0]; // Fallback
    }

    // Draw some gizmos to help visualize in editor
    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.green;
        Vector3 pos = transform.position;
        Gizmos.DrawLine(new Vector3(-recycleThreshold, pos.y - 100, 0), new Vector3(-recycleThreshold, pos.y + 100, 0));
    }
}
