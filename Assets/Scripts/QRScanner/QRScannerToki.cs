using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QRScannerToki : MonoBehaviour
{
    public GameObject qrHandler;


    private void Awake()
    {
        qrHandler.SetActive(true);
    }


    private void Start()
    {

    }

    [Button]
    public void OnDecodeFinished(string dataText)
    {
        PlayerPrefs.SetString("QRCode", dataText);

        string[] parts = dataText.Split('/');

        // Check if the first part is "OpenScene"
        if (parts.Length >= 2 && parts[0] == "Mp3Player")
        {
            SceneManager.LoadScene(parts[0]);
        }

    }

    // Mp3Player/NazwaPlaylisty
}
