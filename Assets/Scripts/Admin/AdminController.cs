using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AdminController : MonoBehaviour
{
    [SerializeField] private string tableLink = "http://www.skydomesoftware.usermd.net/Toki/GetTableJson.php";

    public RootMp3TrackProperties downloadedList;

    public int id;
    public string title;
    public string playListName;
    public string trackAudioClipPath;
    public string trackAudioClipTexturePath;
    public string trackPlaylistTexturePath;





    private void Start()
    {
        StartCoroutine(GetTable());
    }

    [Button]
    public void UpdateTrackInDatabase()
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

        using (UnityWebRequest www = UnityWebRequest.Post("http://www.skydomesoftware.usermd.net/Toki/UpdateRecord.php", form))
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
    public void AddTrackToDatabase()
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

        using (UnityWebRequest www = UnityWebRequest.Post("http://www.skydomesoftware.usermd.net/Toki/AddRecord.php", form))
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

        using (UnityWebRequest www = UnityWebRequest.Post("http://www.skydomesoftware.usermd.net/Toki/RemoveRecord.php", form))
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
        UnityWebRequest www = UnityWebRequest.Post(tableLink, new WWWForm());
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