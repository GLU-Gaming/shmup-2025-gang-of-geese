using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class hpsystem : MonoBehaviour
{
    [SerializeField] private GameObject[] lifePrefabs;
    [SerializeField] private int totalLives = 5;
    [SerializeField] private Image screenFlashImage;
    [SerializeField] private ScreenShake screenShake;
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

        if (!isFlashing)
        {
            StartCoroutine(ScreenFlash());
        }

        if (screenShake != null)
        {
            Debug.Log("Shaking screen");
            StartCoroutine(screenShake.Shake(0.2f, 0.1f));
        }
    }

    void PlayerDeath()
    {
        ScoreSystem scoreSystem = Object.FindFirstObjectByType<ScoreSystem>();
        if (scoreSystem != null)
        {
            scoreSystem.SetFinalScore();
        }
        Utilities utils = FindFirstObjectByType<Utilities>();
        utils.GoToGameOver();        
        //SceneManager.LoadScene(2);
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
            screenFlashImage.color = new Color32(0xcd, 0x45, 0x45, 0x50);
            yield return new WaitForSeconds(0.2f);
            screenFlashImage.color = new Color32(0xcd, 0x45, 0x45, 0x00);
            yield return new WaitForSeconds(0.2f);
        }
        isFlashing = false;
    }
}
