using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class hpsystem : MonoBehaviour
{
    [SerializeField] private GameObject[] lifePrefabs;
    [SerializeField] private int totalLives = 5;
    private int currentLives;

    void Start()
    {
        currentLives = totalLives;

        if (lifePrefabs.Length != 5)
        {
            Debug.LogError("Life prefabs array must contain exactly 5 elements!");
        }

        for (int i = 0; i < lifePrefabs.Length; i++)
        {
            Debug.Log($"Life Prefab {i} initial state: {lifePrefabs[i].name} - Active: {lifePrefabs[i].activeSelf}");
        }
    }

    public void TakeDamage()
    {
        currentLives--;

        if (currentLives >= 0 && currentLives < lifePrefabs.Length)
        {
            var rawImage = lifePrefabs[currentLives].GetComponent<RawImage>();
            if (rawImage != null)
            {
                rawImage.enabled = false;
            }

            lifePrefabs[currentLives].SetActive(false);
        }

        // Call the static method to damage all players
        Player.DamageAllPlayers();
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


