using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LevelsController : MonoBehaviour
{
    // Adres URL do skryptu PHP
    public string levelsURL = "https://yoopieenglish.pl/Unity/GetUserLevels.php";
    // Login aktualnie zalogowanego użytkownika – możesz go ustawić po zalogowaniu lub wczytać ze zapisanych danych

    // Przypisz w Inspector przyciski (CanvasGroup) odpowiadające poszczególnym poziomom
    public CanvasGroup[] levelButtons;

 public void StartGetLelvels(string userLogin)
    {
        StartCoroutine(GetLevels(userLogin));
    }

    IEnumerator GetLevels(string userLogin)
    {
        Debug.Log("GetLevels" + userLogin);
        WWWForm form = new WWWForm();
        form.AddField("login", userLogin);

        UnityWebRequest www = UnityWebRequest.Post(levelsURL, form);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log("Błąd: " + www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
            // Otrzymujemy odpowiedź JSON, np.:
            // {
            //   "status": "success",
            //   "levels": [
            //      {"id": 1, "name": "Level 1", "unlocked": true},
            //      {"id": 2, "name": "Level 2", "unlocked": false},
            //      ...
            //   ]
            // }
            LevelsResponse response = JsonUtility.FromJson<LevelsResponse>(www.downloadHandler.text);
            if (response.status == "success")
            {
                // Zakładamy, że przyciski są ułożone kolejno wg numeracji poziomów (indeks = id - 1)
                foreach (Level level in response.levels)
                {
                    int index = level.id - 1;
                    if (index >= 0 && index < levelButtons.Length)
                    {
                        // Ustawienie przezroczystości:
                        // - Jeśli poziom odblokowany: pełna przezroczystość (alpha = 1)
                        // - Jeśli zablokowany: połowa przezroczystości (alpha = 0.5)
                        levelButtons[index].alpha = level.unlocked ? 1f : 0.5f;
                        // Opcjonalnie wyłącz interakcję dla zablokowanych przycisków:
                        levelButtons[index].interactable = level.unlocked;
                    }
                }
            }
            else
            {
             //   Debug.Log("Błąd pobierania poziomów: " + response.message);
            }
        }
    }
}

[System.Serializable]
public class Level
{
    public int id;
    public string name;
    public bool unlocked;
}

[System.Serializable]
public class LevelsResponse
{
    public string status;
    public Level[] levels;
}
