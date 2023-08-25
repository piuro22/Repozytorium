using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class UnitController : MonoBehaviour
{
    void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }
    public Button startButton;

    public void Initialize(Unit unit)
    {
        startButton.onClick.AddListener(delegate () { SceneManager.LoadScene(unit.sceneToLoadName); });
    }
}
