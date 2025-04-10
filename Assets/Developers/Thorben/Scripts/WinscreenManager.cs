using UnityEngine;
using TMPro; // Import TextMesh Pro namespace

public class WinscreenManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText; // Use TextMeshProUGUI instead of Text

    void Start()
    {
        // Retrieve the final score and display it as a number
        int finalScore = PlayerPrefs.GetInt("FinalScore", 0);
        scoreText.text = finalScore.ToString(); // Display only the number
    }
}
