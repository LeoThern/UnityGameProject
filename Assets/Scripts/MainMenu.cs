using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public int currentScene = 0;
    public int test = 0;
    public int scoreFirstPlayer;
    public int scoreSecondPlayer;

    public Image winBanner;
    public Sprite Blue;
    public Sprite Orange;

    private void Start()
    {
        scoreFirstPlayer = PlayerPrefs.GetInt("scoreFirstPlayer");
        scoreSecondPlayer = PlayerPrefs.GetInt("scoreSecondPlayer");
        currentScene = PlayerPrefs.GetInt("scene");
        if(currentScene == 0)
        {
            PlayerPrefs.SetInt("scoreFirstPlayer", 0);
            PlayerPrefs.SetInt("scoreSecondPlayer", 0);
        }
        if(currentScene == 4)
        {
            winScreen();
        }
    }

    public void PlayGame()
    {
        switchScene();
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
        PlayerPrefs.SetInt("scene", 0);
        SceneManager.LoadSceneAsync("MainMenu");
    }

    public void switchScene()
    {
 
        switch (currentScene)
        {
            case 0:
                PlayerPrefs.SetInt("scene", 1);
                SceneManager.LoadSceneAsync("Stage1");
                break;
            case 1:
                PlayerPrefs.SetInt("scene", 2);
                SceneManager.LoadSceneAsync("Stage2");
                break;
            case 2:
                PlayerPrefs.SetInt("scene", 3);
                SceneManager.LoadSceneAsync("Stage3");
                break;

            case 3:
                
                PlayerPrefs.SetInt("scene", 4);
                SceneManager.LoadSceneAsync("VictoryScreen");
                break;
            

        }
    }

    public void increaseScore(int playerId)
    {
        if (playerId == 1)
        {
            scoreSecondPlayer++;
            PlayerPrefs.SetInt("scoreSecondPlayer", scoreSecondPlayer);
        }else if (playerId == 2)
        {
            scoreFirstPlayer++;
            PlayerPrefs.SetInt("scoreFirstPlayer", scoreFirstPlayer);
        }
        
    }
    public void winScreen()
    {
        if(scoreSecondPlayer > scoreFirstPlayer)
        {
            winBanner.sprite = Blue;
        }
        else
        {
            winBanner.sprite = Orange;
        }
    }
}
