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
  

    public List<UnityEngine.Object> gamePropertiesList = new List<UnityEngine.Object>();
    private GameType gameType;
    public int currentGame;
    public UnityEngine.Object currentGameProperties;

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
        DontDestroyOnLoad(this.gameObject);
    }
    public void OpenNextGame()
    {
        StartCoroutine(OpenNextGameCoroutine());
    }

    IEnumerator OpenNextGameCoroutine()
    {
        yield return new WaitForSeconds(1);
        currentGame++;
        currentGameProperties = gamePropertiesList[currentGame];
        OpenGame(gameType);
    }

    public bool CheckNextGameExist()
    {
        if (gamePropertiesList.Count-1 > currentGame)
            return true;
        else
            return false;
    }

    public void OnGameFirstStart(GameType _gameType, List< UnityEngine.Object> _gameProperties)
    {
        currentGame = 0;
        gameType = _gameType;
        gamePropertiesList = _gameProperties;
        currentGameProperties = gamePropertiesList[0];
        OpenGame(gameType);
    }


    public void OpenGame(GameType _gameType)
    {
        gameType = _gameType;
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