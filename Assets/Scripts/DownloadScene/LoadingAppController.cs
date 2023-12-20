using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LoadingAppController : MonoBehaviour
{
    public SwitchScene switchscene;
    [SerializeField] private DownloadController downloadController;
    [SerializeField] private GameObject downloadCompletedButton;
    [SerializeField] private TMP_Text loadingText;

    private string downloadSpeed;

    [Header("User Interface")]
    [SerializeField] private Image loadingSlider;
    [SerializeField] private Image globalProgressSlider;

    [Header("Console")]
    [SerializeField] private ScrollRect consoleScrollRect;
    [SerializeField] private TMP_Text screenConsole;
    [SerializeField] private CanvasGroup concoleCanvasGroup;

    private void Awake()
    {

        Application.targetFrameRate = 60;
        downloadCompletedButton.SetActive(false);
    }

    private void Start()
    {
        // Subscribe to the event
        downloadController.OnDownloadStarted += HandleDownloadStarted;
        downloadController.OnDownloadSpeedUpdate += HandleDownloadSpeed;
        downloadController.OnAllDownloadCompleted += HandleAllDownloadCompleted;
        downloadController.OnInternetErrorFileExistHandler += HandleOnInternetErrorFileExistHandler;
        downloadController.OnInternetErrorFileNotExistHandler += HandleOnInternetErrorFileNotExistHandler;
    }



    private void HandleOnInternetErrorFileNotExistHandler()
    {
        loadingText.SetText("Połącz aplikację z internetem");
        downloadCompletedButton.SetActive(false);
    }

    private void HandleOnInternetErrorFileExistHandler()
    {
        loadingText.SetText("Brak połączenia z internetem");
        downloadCompletedButton.SetActive(true);
    }

    private void HandleDownloadStarted()
    {
        loadingText.SetText("Pobieranie treści...");
        downloadCompletedButton.SetActive(false);
    }

    private void HandleDownloadSpeed(float speed)
    {
        downloadSpeed = $"{ FileUtils.FormatFileSize((long)speed)}/s";
    }

    private void HandleAllDownloadCompleted()
    {
        loadingText.SetText("Ładowanie zakończone");
        downloadCompletedButton.SetActive(true);
        switchscene.LoadScene("Scene Login");
    }

    private void OnDestroy()
    {
        downloadController.OnDownloadStarted -= HandleDownloadStarted;
        downloadController.OnDownloadSpeedUpdate -= HandleDownloadSpeed;
        downloadController.OnAllDownloadCompleted -= HandleAllDownloadCompleted;
        downloadController.OnInternetErrorFileExistHandler -= HandleOnInternetErrorFileExistHandler;
        downloadController.OnInternetErrorFileNotExistHandler -= HandleOnInternetErrorFileNotExistHandler;
    }
}
