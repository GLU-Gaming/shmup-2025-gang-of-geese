using UnityEngine;
using TMPro;

public class ScoreSystem : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    private int score = 0;

    void Awake()
    {
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
            scoreText.text = score.ToString();
        }
    }
}

