using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class hpsystem : MonoBehaviour
{
    // Array to hold the 5 life UI prefabs
    [SerializeField] private GameObject[] lifePrefabs;

    // Total number of lives
    [SerializeField] private int totalLives = 5;

    // Current number of lives
    private int currentLives;

    void Start()
    {
        // Initialize current lives to total lives
        currentLives = totalLives;

        // Ensure we have exactly 5 life prefabs
        if (lifePrefabs.Length != 5)
        {
            Debug.LogError("Life prefabs array must contain exactly 5 elements!");
        }

        // Log the initial state of life prefabs
        for (int i = 0; i < lifePrefabs.Length; i++)
        {
            Debug.Log($"Life Prefab {i} initial state: {lifePrefabs[i].name} - Active: {lifePrefabs[i].activeSelf}");
        }

        // Ensure all life prefabs are visible at start
        //ResetLifePrefabs();
    }

    // Method to be called when player is hit by a bullet
    public void TakeDamage()
    {
        currentLives--;

        if (currentLives >= 0 && currentLives < lifePrefabs.Length)
        {
            // Access the RawImage component directly
            var rawImage = lifePrefabs[currentLives].GetComponent<RawImage>();
            if (rawImage != null)
            {
                rawImage.enabled = false;
            }

            // Also disable the GameObject if needed
            lifePrefabs[currentLives].SetActive(false);
        }
    }

 
    void PlayerDeath()
    {
        SceneManager.LoadScene(2);
        Debug.Log("Player has died! Game Over!");
    }

    public void AddLife()
    {
        if (currentLives < totalLives)
        {
            currentLives++;
            lifePrefabs[currentLives - 1].SetActive(true);
        }
    }
}