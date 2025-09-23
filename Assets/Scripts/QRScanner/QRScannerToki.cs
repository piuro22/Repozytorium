using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QRScannerToki : MonoBehaviour
{
    public QRCodeDecodeController codeDecodeController;
    public GameObject qrHandler;


    private void Awake()
    {

        StartCoroutine(Delay());
    }


    IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.5f);
        qrHandler.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        codeDecodeController.Reset();
        codeDecodeController.enabled = true;
    }

    private void Start()
    {

    }
    /*
    [Button]
    public void OnDecodeFinished(string dataText)
    {
     
        codeDecodeController.Reset();
        PlayerPrefs.SetString("QRCode", dataText);

        string[] parts = dataText.Split('/');

        // Check if the first part is "OpenScene"
        if (parts.Length >= 2 && parts[0] == "Mp3Player")
        {
            SceneManager.LoadScene(parts[0]);
        }
        Debug.Log(dataText);
    }

    // Mp3Player/NazwaPlaylisty*/
    [Button]
    public void OnDecodeFinished(string dataText)
    {
        codeDecodeController.Reset();
        PlayerPrefs.SetString("QRCode", dataText);

        string[] parts = dataText.Split('/');

        if (parts.Length >= 2 && parts[0] == "Mp3Player")
        {
            // Stop camera before changing scene
            if (codeDecodeController.e_DeviceController != null)
                codeDecodeController.e_DeviceController.StopWork();

            // Optionally, disable the QR code controller
            codeDecodeController.enabled = false;

            // Load scene with a short delay to ensure camera is released
            StartCoroutine(LoadSceneWithDelay(parts[0]));
            return;
        }
        Debug.Log(dataText);
    }

    private IEnumerator LoadSceneWithDelay(string sceneName)
    {
        yield return new WaitForSeconds(0.2f);
        SceneManager.LoadScene(sceneName);
    }

}
