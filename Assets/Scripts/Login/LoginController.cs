using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.Events;

public class LoginController : MonoBehaviour
{
    public TMP_InputField loginInput;
    public TMP_InputField passwordInput;
    public TMP_Text messageText;

    public string loginURL = "http://yoopieenglish.pl/Unity/GetTableJson.php"; // Replace with your server URL
    public UnityEvent OnSuccesLogin;
    public void OnLoginButtonClicked()
    {
        StartCoroutine(Login());
    }

    IEnumerator Login()
    {
        WWWForm form = new WWWForm();
        form.AddField("login", loginInput.text);
        form.AddField("password", passwordInput.text);

        UnityWebRequest www = UnityWebRequest.Post(loginURL, form);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            messageText.text = "Error: " + www.error;
        }
        else
        {
            var jsonResponse = JsonUtility.FromJson<Response>(www.downloadHandler.text);
            messageText.text = jsonResponse.message;

            if (jsonResponse.status == "success")
            {
                OnSuccesLogin.Invoke();
            }
            else
            {
                Debug.Log("Invaild Login");
            }
        }
    }

    [System.Serializable]
    public class Response
    {
        public string status;
        public string message;
    }
}