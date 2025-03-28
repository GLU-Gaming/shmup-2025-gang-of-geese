using System.Collections; // Add this line
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class hpsystem : MonoBehaviour
{
    [SerializeField] private GameObject[] lifePrefabs;
    [SerializeField] private int totalLives = 5;
    [SerializeField] private Image screenFlashImage; // Reference to the UI Image for screen flash
    private int currentLives;
    private bool isFlashing = false;

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

        // Ensure the screen flash image is initially transparent
        if (screenFlashImage != null)
        {
            screenFlashImage.color = new Color(1, 0, 0, 0);
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

        if (currentLives <= 0)
        {
            PlayerDeath();
        }

        // Trigger the screen flash effect
        if (!isFlashing)
        {
            StartCoroutine(ScreenFlash());
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

    private IEnumerator ScreenFlash()
    {
        isFlashing = true;
        if (screenFlashImage != null)
        {
            // Flash the screen red
            screenFlashImage.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(0.1f);
            screenFlashImage.color = new Color(1, 0, 0, 0);
            yield return new WaitForSeconds(0.1f);
        }
        isFlashing = false;
    }
}
