using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AdminController : MonoBehaviour
{
    [SerializeField] private string wwwDownloadTable = "https://www.skydomesoftware.usermd.net/Toki/GetTableJson.php";
    [SerializeField] private string wwwRemoveRecord = "https://www.skydomesoftware.usermd.net/Toki/RemoveRecord.php";
    [SerializeField] private string wwwAddRecord = "https://www.skydomesoftware.usermd.net/Toki/AddRecord.php";
    [SerializeField] private string wwwUpdateRecord = "https://www.skydomesoftware.usermd.net/Toki/UpdateRecord.php";
    public RootMp3TrackProperties downloadedList;

 





    private void Start()
    {
        StartCoroutine(GetTable());
    }

    [Button]
    public void UpdateTrackInDatabase(int id, string title, string playListName, string trackAudioClipPath, string trackAudioClipTexturePath, string trackPlaylistTexturePath)
    {
        StartCoroutine(UpdateTrackCoroutine(id,title, playListName,  trackAudioClipPath, trackAudioClipTexturePath, trackPlaylistTexturePath));
    }

    IEnumerator UpdateTrackCoroutine(int id, string title, string playListName, string trackAudioClipPath, string trackAudioClipTexturePath, string trackPlaylistTexturePath)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", id.ToString());
        form.AddField("playListName", playListName);
        form.AddField("title", title);
        form.AddField("trackAudioClipPath", trackAudioClipPath);
        form.AddField("trackAudioClipTexturePath", trackAudioClipTexturePath);
        form.AddField("trackPlaylistTexturePath", trackPlaylistTexturePath);

        using (UnityWebRequest www = UnityWebRequest.Post(wwwUpdateRecord, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
            }
        }
        StartCoroutine(GetTable());
    }

    [Button]
    public void AddTrackToDatabase(string title, string playListName, string trackAudioClipPath, string trackAudioClipTexturePath, string trackPlaylistTexturePath)
    {
        StartCoroutine(AddTrackCoroutine(title, playListName, trackAudioClipPath, trackAudioClipTexturePath, trackPlaylistTexturePath));
    }

    IEnumerator AddTrackCoroutine(string title, string playListName, string trackAudioClipPath, string trackAudioClipTexturePath, string trackPlaylistTexturePath)
    {
        WWWForm form = new WWWForm();
        form.AddField("playListName", playListName);
        form.AddField("title", title);
        form.AddField("trackAudioClipPath", trackAudioClipPath);
        form.AddField("trackAudioClipTexturePath", trackAudioClipTexturePath);
        form.AddField("trackPlaylistTexturePath", trackPlaylistTexturePath);

        using (UnityWebRequest www = UnityWebRequest.Post(wwwAddRecord, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
            }
        }
        StartCoroutine(GetTable());
    }

    [Button]
    public void RemoveTrackFromDatabase(int trackId)
    {
        StartCoroutine(RemoveTrackCoroutine(trackId));
    }

    IEnumerator RemoveTrackCoroutine(int trackId)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", trackId.ToString());

        using (UnityWebRequest www = UnityWebRequest.Post(wwwRemoveRecord, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
            }
        }
        StartCoroutine(GetTable());
    }


    public IEnumerator GetTable()
    {
        UnityWebRequest www = UnityWebRequest.Post(wwwDownloadTable, new WWWForm());
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {

        }
        else
        {
            downloadedList = JsonUtility.FromJson<RootMp3TrackProperties>("{\"mp3TrackProperties\":" + www.downloadHandler.text + "}");
        }
    }
}