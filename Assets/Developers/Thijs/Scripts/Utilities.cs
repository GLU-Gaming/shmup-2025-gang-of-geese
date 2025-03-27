using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Utilities : MonoBehaviour
{

    public void GoToGame()
    {

SceneManager.LoadScene(1);

    }
    public void GoToMenu()
    {

        SceneManager.LoadScene(0);

    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit");
    }
    public void TestFunctie()
    {
        Debug.Log("De knop werkt!");
    }
}
