using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.Events;
using System.Collections;

public class LoginController : MonoBehaviour
{
    public TMP_InputField loginInput;
    public TMP_InputField passwordInput;
    public TMP_Text messageText;

    public string loginURL = "https://yoopieenglish.pl/Unity/GetTableJson.php"; // Replace with your server URL
    public UnityEvent OnSuccesLogin;
    [SerializeField] private LevelsController levelController;
    private const string CREDENTIALS_FILE_NAME = "userCredentials.json";

    private void Start()
    {
        TryAutoLogin();
    }


    public void OnLogoutButtonClicked()
    {
        string path = Path.Combine(Application.persistentDataPath, CREDENTIALS_FILE_NAME);
        if (File.Exists(path))
        {
            File.Delete(path);
            messageText.text = "Logged out successfully!";
        }
        else
        {
            messageText.text = "No credentials found to logout!";
        }
    }

    public void OnLoginButtonClicked()
    {
        StartCoroutine(Login(loginInput.text, passwordInput.text));
    }

    IEnumerator Login(string login, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("login", login);
        form.AddField("password", password);

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
                SaveCredentials(login, password);
                levelController.StartGetLelvels(login);
            }
            else
            {
                Debug.Log("Invalid Login");
            }
        }
    }

    void SaveCredentials(string login, string password)
    {
        UserCredentials credentials = new UserCredentials
        {
            Login = login,
            Password = password
        };

        string json = JsonUtility.ToJson(credentials);
        string path = Path.Combine(Application.persistentDataPath, CREDENTIALS_FILE_NAME);
        File.WriteAllText(path, json);
    }

    void TryAutoLogin()
    {
        string path = Path.Combine(Application.persistentDataPath, CREDENTIALS_FILE_NAME);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            UserCredentials credentials = JsonUtility.FromJson<UserCredentials>(json);
            StartCoroutine(Login(credentials.Login, credentials.Password));
        }
    }

    [System.Serializable]
    public class Response
    {
        public string status;
        public string message;
    }

    [System.Serializable]
    public class UserCredentials
    {
        public string Login;
        public string Password;
    }
}