using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using Sirenix.OdinInspector;
using UnityEngine.UI;

public class DownloadController : MonoBehaviour
{
    [SerializeField] private TMP_Text screenConsole;
    [SerializeField] private RootMp3TrackProperties downloadedList;

    [SerializeField] private Slider loadingSlider;
    [SerializeField] private Slider globalProgressSlider;
    private float currentDownloadProgress = 0f;
    private float globalDownloadProgress = 0f;
    private long totalSizeOfFilesToDownload = 0;
    private Dictionary<string, long> fileSizeByUrl = new Dictionary<string, long>();
    private HashSet<string> encounteredUrls = new HashSet<string>();



    private void Start()
    {
        loadingSlider.value = 0f;
        globalProgressSlider.value = 0f;
        StartCoroutine(GetTable());
    }

    #region Download And Save
    [Obsolete]
    public IEnumerator GetTable()
    {
        UnityWebRequest www = UnityWebRequest.Post("http://www.skydomesoftware.usermd.net/Toki/GetTableJson.php", new WWWForm());
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            PrintToConsole("Download table failed", true, Color.red);
            Debug.Log(www.error);
        }
        else
        {
            downloadedList = JsonUtility.FromJson<RootMp3TrackProperties>("{\"mp3TrackProperties\":" + www.downloadHandler.text + "}");
            SaveDownloadedListToJson(downloadedList);
            CalculateTotalSizeOfFilesToDownload();
        }
        StartCoroutine(CheckFiles());
    }

    public IEnumerator CheckFiles()
    {
        foreach (Mp3TrackProperties trackProperties in downloadedList.mp3TrackProperties)
        {
            PrintToConsole($"____________________________\n{ trackProperties.title}", false, Color.white);

            yield return new WaitUntil(() => CanContinueCheck());

            string audioFile = GetFilenameFromURL(trackProperties.trackAudioClipPath);
            string textureFile = GetFilenameFromURL(trackProperties.trackAudioClipTexturePath);
            string playlistTextureFile = GetFilenameFromURL(trackProperties.trackPlaylistTexturePath);

            if (!CheckFileExist(audioFile, true))
                yield return StartCoroutine(DownloadFile(trackProperties.trackAudioClipPath));

            if (!CheckFileExist(textureFile, true))
                yield return StartCoroutine(DownloadFile(trackProperties.trackAudioClipTexturePath));

            if (!CheckFileExist(playlistTextureFile, true))
                yield return StartCoroutine(DownloadFile(trackProperties.trackPlaylistTexturePath));
        }
        CleanUpFiles();
    }

    private void SaveDownloadedListToJson(RootMp3TrackProperties list)
    {
        string subDir = Path.Combine(Application.persistentDataPath);
        Directory.CreateDirectory(subDir);
        string messagepath = Path.Combine(subDir, "Tracks" + ".json");
        File.WriteAllText(messagepath, JsonUtility.ToJson(list));
        PrintToConsole("Download table successful");
    }

    IEnumerator DownloadFile(string fileUrl)
    {
        UnityWebRequest request = UnityWebRequest.Get(fileUrl);
        var operation = request.SendWebRequest();

        long fileSize = -1;
        while (!operation.isDone)
        {
            currentDownloadProgress = request.downloadProgress;
            UpdateLoadingSlider();
            UpdateGlobalProgress();

            if (fileSize < 0 && !string.IsNullOrEmpty(request.GetResponseHeader("Content-Length")))
            {
                if (long.TryParse(request.GetResponseHeader("Content-Length"), out fileSize))
                {
                    PrintToConsole($"Prepare to download file from: {fileUrl} [{FormatFileSize(fileSize)}]", false, Color.cyan);
                }
            }

            yield return null;
        }

        if (request.isNetworkError || request.isHttpError)
        {
            PrintToConsole($"Download table failed {request.error}", true, Color.red);
            Debug.Log(request.error + " " + fileUrl);
        }
        else
        {
            SaveDownloadedFile(request.downloadHandler.data, fileUrl);
        }
    }

    private void SaveDownloadedFile(byte[] data, string fileUrl)
    {
        string subDir = Path.Combine(Application.persistentDataPath, "Tracks");
        Directory.CreateDirectory(subDir);
        string fileName = GetFilenameFromURL(fileUrl);
        string filePath = Path.Combine(subDir, fileName);

        if (!File.Exists(filePath))
        {
            File.WriteAllBytes(filePath, data);
            PrintToConsole($"Downloaded completed: {fileName}", false, Color.green);
        }
        loadingSlider.value = 1;
        globalDownloadProgress = CalculateGlobalDownloadProgress();
        UpdateGlobalProgress();
    }


    #endregion

    private void CalculateTotalSizeOfFilesToDownload()
    {
        foreach (Mp3TrackProperties trackProperties in downloadedList.mp3TrackProperties)
        {
            AddFileSizeFromUrlIfUnique(trackProperties.trackAudioClipPath);
            AddFileSizeFromUrlIfUnique(trackProperties.trackAudioClipTexturePath);
            AddFileSizeFromUrlIfUnique(trackProperties.trackPlaylistTexturePath);
        }
        PrintToConsole($"Total size of files to download: {FormatFileSize(totalSizeOfFilesToDownload)}", false, Color.white);
    }

  

   

  

    private bool CheckFileExist(string file, bool writeMessage = false)
    {
        string filePath = Path.Combine(Application.persistentDataPath, "Tracks", file);
        bool exists = File.Exists(filePath);

        if (writeMessage)
        {
            if (exists)
                PrintToConsole($"{file} is already Downloaded", false, Color.green);
            else
                PrintToConsole($"{file} does not exist in device memory", false, Color.yellow);
        }
        return exists;
    }

    private float CalculateGlobalDownloadProgress()
    {
        int totalFiles = downloadedList.mp3TrackProperties.Length * 3; // Assuming 3 files per track
        int downloadedFiles = 0;

        foreach (Mp3TrackProperties trackProperties in downloadedList.mp3TrackProperties)
        {
            string audioFile = GetFilenameFromURL(trackProperties.trackAudioClipPath);
            string textureFile = GetFilenameFromURL(trackProperties.trackAudioClipTexturePath);
            string playlistTextureFile = GetFilenameFromURL(trackProperties.trackPlaylistTexturePath);

            if (CheckFileExist(audioFile))
                downloadedFiles++;

            if (CheckFileExist(textureFile))
                downloadedFiles++;

            if (CheckFileExist(playlistTextureFile))
                downloadedFiles++;
        }

        return (float)downloadedFiles / totalFiles;
    }

    private bool CanContinueCheck()
    {
        return true;
    }

    private string GetFilenameFromURL(string url)
    {
        Uri uri = new Uri(url);
        string filename = Path.GetFileName(uri.LocalPath);
        return filename;
    }

    private void UpdateLoadingSlider()
    {
        loadingSlider.value = currentDownloadProgress;
    }

    private void UpdateGlobalProgress()
    {
        globalProgressSlider.value = globalDownloadProgress;
    }

    private void PrintToConsole(string message, bool isError = false, Color textColor = default)
    {
        string formattedMessage = isError ? $"\n<color=red>[Error]{message}</color>" : $"\n{message}";
        if (textColor != default)
        {
            formattedMessage = $"<color=#{ColorUtility.ToHtmlStringRGB(textColor)}>{formattedMessage}</color>";
        }
        screenConsole.text += formattedMessage;
    }

  

    private string FormatFileSize(long size)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        int order = 0;
        double formattedSize = size;

        while (formattedSize >= 1024 && order < sizes.Length - 1)
        {
            order++;
            formattedSize /= 1024;
        }

        return $"{formattedSize:F2} {sizes[order]}";
    }

    private long GetFileSizeFromUrl(string fileUrl)
    {
        if (fileSizeByUrl.ContainsKey(fileUrl))
        {
            return fileSizeByUrl[fileUrl];
        }
        else
        {
            UnityWebRequest request = UnityWebRequest.Head(fileUrl);
            var operation = request.SendWebRequest();

            while (!operation.isDone) { }

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.LogError($"Error getting file size from URL: {fileUrl}\n{request.error}");
                return 0;
            }

            if (request.GetResponseHeader("Content-Length") != null && long.TryParse(request.GetResponseHeader("Content-Length"), out long fileSize))
            {
                fileSizeByUrl[fileUrl] = fileSize;
                return fileSize;
            }
            else
            {
                Debug.LogError($"Unable to retrieve file size from URL: {fileUrl}");
                return 0;
            }
        }
    }

    private void AddFileSizeFromUrlIfUnique(string url)
    {
        if (!encounteredUrls.Contains(url))
        {
            long fileSize = GetFileSizeFromUrl(url);
            if (!CheckFileExist(GetFilenameFromURL(url)))
            {
                totalSizeOfFilesToDownload += fileSize;
            }
            encounteredUrls.Add(url);
        }
    }

    private void CleanUpFiles()
    {
        string tracksPath = Path.Combine(Application.persistentDataPath, "Tracks");

        if (downloadedList != null)
        {
            HashSet<string> validFilenames = new HashSet<string>();
            foreach (Mp3TrackProperties trackProperties in downloadedList.mp3TrackProperties)
            {
                validFilenames.Add(GetFilenameFromURL(trackProperties.trackAudioClipPath));
                validFilenames.Add(GetFilenameFromURL(trackProperties.trackAudioClipTexturePath));
                validFilenames.Add(GetFilenameFromURL(trackProperties.trackPlaylistTexturePath));
            }

            string[] allFiles = Directory.GetFiles(tracksPath);
            foreach (string filePath in allFiles)
            {
                string filename = Path.GetFileName(filePath);
                if (!IsFileInList(filename, validFilenames))
                {
                    File.Delete(filePath);
                    PrintToConsole($"Delete file that is not in database: {filename}", false, Color.red);
                }
            }
        }
        loadingSlider.value = 1;
        globalProgressSlider.value = 1;
    }

    private bool IsFileInList(string filename, HashSet<string> validFilenames)
    {
        return validFilenames.Contains(filename);
    }

    [Button]
    private void ShowPersistentDataPath()
    {
        System.Diagnostics.Process.Start("explorer.exe", "/select," + Application.persistentDataPath.Replace("/", "\\"));
    }
}

[Serializable]
public class RootMp3TrackProperties
{
    public Mp3TrackProperties[] mp3TrackProperties;
}

[Serializable]
public class Mp3TrackProperties
{
    public int id;
    public string playListName;
    public string title;
    public string trackAudioClipPath;
    public string trackAudioClipTexturePath;
    public string trackPlaylistTexturePath;
}