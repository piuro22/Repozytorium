using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class LoadingAppController : MonoBehaviour
{
    [SerializeField] private DownloadController downloadController;
    [SerializeField] private GameObject downloadCompletedButton;
    [SerializeField] private TMP_Text loadingText;
    private void Awake()
    {

        Application.targetFrameRate = 60;
        downloadCompletedButton.SetActive(false);
    }

    private void Start()
    {
        // Subscribe to the event
        downloadController.OnDownloadStarted += HandleDownloadStarted;
        downloadController.OnDownloadCompleted += HandleDownloadCompleted;
        downloadController.OnInternetErrorHandler += HandleNoInternetConnection;
    }


    private void HandleNoInternetConnection()
    {
        loadingText.SetText("Brak połączenia z internetem");
    }

    private void HandleDownloadStarted()
    {
        loadingText.SetText("Pobieranie treści...");
    }

    private void HandleDownloadCompleted()
    {
        loadingText.SetText("Ładowanie zakończone");
        downloadCompletedButton.SetActive(true);
    }

    private void OnDestroy()
    {
        downloadController.OnInternetErrorHandler -= HandleNoInternetConnection;
        downloadController.OnDownloadCompleted -= HandleDownloadCompleted;
    }
}
