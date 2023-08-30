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
        downloadCompletedButton.SetActive(false);
    }

    private void Start()
    {
        // Subscribe to the event
        downloadController.OnDownloadCompleted += HandleDownloadCompleted;
        downloadController.OnInternetErrorHandler += HandleNoInternetConnection;
    }


    private void HandleNoInternetConnection()
    {
        loadingText.SetText("Brak połączenia z internetem");
    }

    private void HandleDownloadCompleted()
    {
        downloadCompletedButton.SetActive(true);
    }

    private void OnDestroy()
    {
        downloadController.OnInternetErrorHandler -= HandleNoInternetConnection;
        downloadController.OnDownloadCompleted -= HandleDownloadCompleted;
    }
}
