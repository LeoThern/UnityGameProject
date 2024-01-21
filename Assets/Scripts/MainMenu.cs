using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync("Stage1");
    }


    

    public void openCredits()
    {
        SceneManager.LoadSceneAsync("Credits");
    }

    public void quitGame()
    {
        Application.Quit();
    }
 
    public void quitCredits()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }
}
