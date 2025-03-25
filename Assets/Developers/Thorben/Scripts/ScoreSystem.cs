using UnityEngine;
using TMPro;

public class ScoreSystem : MonoBehaviour
{
    public static ScoreSystem instance; 
    public TextMeshProUGUI scoreText; 
    private int score = 0; 

    void Awake()
    {
        // Ensure there's only one instance of ScoreSystem
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateScoreText();
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateScoreText();
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }
}