using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public List<MainMenuCard> mainMenuCards = new List<MainMenuCard>();

    private void Awake()
    {
        GameViewUtils.SetSize(8);
    }

    private void Start()
    {

        SetupScene();
    }

    private void SetupScene()
    {
        foreach (MainMenuCard mainMenuCard in mainMenuCards)
        {
            mainMenuCard.Card.anchoredPosition = Vector4.zero;
        }

        SwitchCard("Register");
    }

    public void OnRegisterClick()
    {
        SwitchCard("LevelChange");
    }
    public void OnLevelChangeClick()
    {
        SwitchCard("Activities");
    }
    public void OnGameActivityClick()
    {
        GameViewUtils.SetSize(7);
        OpenScene("Scene Level Change");

    }




    public void OpenScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }


    private void SwitchCard(string cardName)
    {
        foreach(MainMenuCard mainMenuCard in mainMenuCards)
        {
           if(mainMenuCard.name!= cardName)
            {
                mainMenuCard.Card.gameObject.SetActive(false);
            }
            else
            {
                mainMenuCard.Card.gameObject.SetActive(true);
              
            }
        }

    }

}
[Serializable]
public class MainMenuCard
{
    public string name;
    public RectTransform Card;
}
