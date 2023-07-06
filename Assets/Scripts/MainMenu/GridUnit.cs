using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GridUnit : MonoBehaviour
{
   [SerializeField] private TMP_Text titleText;

    public void Initialize(Unit unit)
    {
        titleText.SetText(unit.unitName);
        GetComponent<Button>().onClick.AddListener(delegate () { SceneManager.LoadScene(unit.sceneToLoadName); });
    }
}
