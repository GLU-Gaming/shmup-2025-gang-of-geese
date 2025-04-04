using UnityEngine;
using TMPro;

public class WinSceneManager : MonoBehaviour
{
    public TextMeshProUGUI finalScoreText;

    void Start()
    {
        if (finalScoreText != null)
        {
            finalScoreText.text = ScoreSystem.finalScore.ToString();
        }
    }
}
