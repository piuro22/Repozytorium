using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameFinishScreen : MonoBehaviour
{
   public void OnBackButtonClicked()
    {
        SceneManager.LoadScene("Scene Level Change");
    }
}
