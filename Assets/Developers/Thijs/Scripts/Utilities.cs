using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Utilities : MonoBehaviour
{

    public void GoToGame()
    {

        SceneManager.LoadScene("game");

    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit");
    }


    public void GoToGameStart()
    {

        SceneManager.LoadScene("start screen");

    }
}

