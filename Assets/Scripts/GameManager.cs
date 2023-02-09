using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public List<Games> games = new List<Games>();


    private void Awake()
    {
        if(Instance!=null && Instance!=this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void OpenGame(GameType gameType, LabrinthProperties labrinthProperties)
    {
        foreach(Games game in games)
        {
            if(game.gameType == gameType)
            {
                game.gamePanel.SetActive(true);
                if (labrinthProperties != null)
                {
                    game.gamePanel.GetComponent<LabrinthController>().labrinthProperties = labrinthProperties;
                    game.gamePanel.GetComponent<LabrinthController>().Initialize();
                }
            }
        }
    }
}


[Serializable]
public class Games
{
    public GameType gameType;
    public GameObject gamePanel;
   
}