using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public List<Games> games = new List<Games>();
    public UnityEngine.Object gameProporties;

    private void Awake()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        if (Instance!=null && Instance!=this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void OpenGame(GameType gameType, UnityEngine.Object _gameProperties)
    {
        gameProporties = _gameProperties;
        foreach (Games game in games)
        {
            if(game.gameType == gameType)
            {
                SceneManager.LoadScene(game.sceneName, LoadSceneMode.Single);
            }
        }
    }
}


[Serializable]
public class Games
{
    public GameType gameType;
    public string sceneName;
}