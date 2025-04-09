using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Utilities : MonoBehaviour
{

    public void GoToGame()
    {
        AudioManager.Instance.PlayGameTheme();
        SceneManager.LoadScene(1);

    }
    public void GoToMenu()
    {

        AudioManager.Instance.PlayStartSchermTheme();
        SceneManager.LoadScene(0);

    }

    public void GoToControls()
    {

        SceneManager.LoadScene(3);

    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit");
    }

    public void GoToWinScreen()
    {
        AudioManager.Instance.PlayWinTheme();
        AudioManager.Instance.PlayVictoryHonk();
        SceneManager.LoadScene(4);
    }
    public void GoToGameOver()
    {
        AudioManager.Instance.PlayGameTheme();
        AudioManager.Instance.PlaySadTrombone();
        SceneManager.LoadScene(2);
    }
    public void GoToCredits()
    {
        
        SceneManager.LoadScene(5);
    }

    public void TestFunctie()
    {
        Debug.Log("De knop werkt!");
    }


}
