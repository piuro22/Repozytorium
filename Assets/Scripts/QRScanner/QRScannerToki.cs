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

    // Mp3Player/NazwaPlaylisty
}
