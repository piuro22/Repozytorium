using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class DownloadController : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] private bool shouldCleanUnnecessaryFiles;
    [SerializeField] private bool shouldShowConsole;

    [Header("Server Related")]
    [SerializeField] private string downloadPath = "download";
    [SerializeField] private string tableLink = "http://www.skydomesoftware.usermd.net/Toki/GetTableJson.php";
    [SerializeField] private RootMp3TrackProperties downloadedList;

    [Header("Console Related")]
    [SerializeField] private ScrollRect consoleScrollRect;
    [SerializeField] private TMP_Text screenConsole;

    [Header("User Interface")]
    [SerializeField] private Image loadingSlider;
    [SerializeField] private Image globalProgressSlider;

    private float totalSizeOfFilesToDownload = 0f;
    private float globalDownloadProgress = 0f;
    private ulong downloadedBytes = 0;
    private float downloadStartTime;

    private Dictionary<string, long> fileSizeByUrl = new Dictionary<string, long>();
    private HashSet<string> encounteredUrls = new HashSet<string>();

    public event Action OnDownloadStarted;
    public event Action<float> OnDownloadSpeedUpdate;
    public event Action OnAllDownloadCompleted;
    public event Action OnDownloadFileCompleted;
    public event Action OnInternetErrorFileExistHandler;
    public event Action OnInternetErrorFileNotExistHandler;

    private void Start()
    {
        SetupProgressSliders();
        SetupConsole();
        StartCoroutine(GetTable());
        StartCoroutine(CheckTableDownloadStatus());
    }

    private void SetupProgressSliders()
    {
        loadingSlider.fillAmount = 0f;
        globalProgressSlider.fillAmount = 0f;
    }

    private void SetupConsole()
    {
        consoleScrollRect.GetComponent<CanvasGroup>().alpha = shouldShowConsole ? 1 : 0;
    }

    #region Download And Save


    /// <summary>
    /// Download Table From Server
    /// </summary>
    /// 

    public IEnumerator GetTable()
    {
        UnityWebRequest www = UnityWebRequest.Post(tableLink, new WWWForm());
        yield return www.SendWebRequest();





        if (www.isNetworkError || www.isHttpError)
        {
            PrintToConsole("Download table failed", true, Color.red);

            string subDir = Path.Combine(Application.persistentDataPath, downloadPath);
            Directory.CreateDirectory(subDir);
            string filePath = Path.Combine(subDir, "Tracks" + ".json");

            if (!File.Exists(filePath))
            {
                OnInternetErrorFileNotExistHandler?.Invoke();
            }
            else
            {
                OnInternetErrorFileExistHandler?.Invoke();
            }

        }
        else
        {
            OnDownloadStarted?.Invoke();
            downloadedList = JsonUtility.FromJson<RootMp3TrackProperties>("{\"mp3TrackProperties\":" + www.downloadHandler.text + "}");
            SaveDownloadedListToJson(downloadedList);
            CalculateTotalSizeOfFilesToDownload();
        }
        StartCoroutine(CheckFiles());
    }

    private IEnumerator CheckTableDownloadStatus()
    {
        int secondsWait = 0;
        while (!IsTableDownloaded())
        {
            secondsWait++;
            PrintToConsole($"Try to connect with host {secondsWait}", false, Color.white, true);

            if (secondsWait > 10)
            {
                PrintToConsole($"[ERROR] Unable to conect with host", false, Color.red, false);
                StopAllCoroutines();
            }
            yield return new WaitForSeconds(1f);
        }

        // Once the table is downloaded, start checking files

    }
    private bool IsTableDownloaded()
    {
        // Check if the downloadedList is not null or empty
        return downloadedList != null && downloadedList.mp3TrackProperties.Length > 0;
    }

    private void SaveDownloadedListToJson(RootMp3TrackProperties list)
    {
        string subDir = Path.Combine(Application.persistentDataPath);
        Directory.CreateDirectory(subDir);
        string messagepath = Path.Combine(subDir, "Tracks" + ".json");
        File.WriteAllText(messagepath, JsonUtility.ToJson(list));
        PrintToConsole("Download table successful");
    }

    public IEnumerator CheckFiles()
    {
        Debug.Log("4");
        foreach (Mp3TrackProperties trackProperties in downloadedList.mp3TrackProperties)
        {
            PrintToConsole($"____________________________\n{trackProperties.title}", false, Color.white);

            string audioFile = GetFilenameFromURL(trackProperties.trackAudioClipPath);
            string textureFile = GetFilenameFromURL(trackProperties.trackAudioClipTexturePath);
            string playlistTextureFile = GetFilenameFromURL(trackProperties.trackPlaylistTexturePath);

            bool shouldDownloadAudio = !FileUtils.CheckFileExist(Path.Combine(Application.persistentDataPath, downloadPath, audioFile));
            bool shouldDownloadTexture = !FileUtils.CheckFileExist(Path.Combine(Application.persistentDataPath, downloadPath, textureFile));
            bool shouldDownloadPlaylistTexture = !FileUtils.CheckFileExist(Path.Combine(Application.persistentDataPath, downloadPath, playlistTextureFile));

            if (shouldDownloadAudio)
                yield return StartCoroutine(DownloadFile(trackProperties.trackAudioClipPath));

            if (shouldDownloadTexture)
                yield return StartCoroutine(DownloadFile(trackProperties.trackAudioClipTexturePath));

            if (shouldDownloadPlaylistTexture)
                yield return StartCoroutine(DownloadFile(trackProperties.trackPlaylistTexturePath));
        }
        if (shouldCleanUnnecessaryFiles)
        {
            CleanUpFiles();
        }
        OnAllDownloadCompleted?.Invoke();
    }


    IEnumerator DownloadFile(string fileUrl)
    {
        UnityWebRequest request = UnityWebRequest.Get(fileUrl);
        var operation = request.SendWebRequest();

        long fileSize = -1;
        downloadStartTime = Time.time; // Record download start time

        ulong initialDownloadedBytes = request.downloadedBytes;
        while (!operation.isDone)
        {
            float currentDownloadProgress = request.downloadProgress;
            UpdateLoadingSlider(currentDownloadProgress);
            UpdateGlobalProgress();

            if (fileSize < 0 && !string.IsNullOrEmpty(request.GetResponseHeader("Content-Length")))
            {
                if (long.TryParse(request.GetResponseHeader("Content-Length"), out fileSize))
                {
                    PrintToConsole($"Prepare to download file from: {fileUrl} [{FileUtils.FormatFileSize(fileSize)}]\n", false, Color.cyan);
                }
            }

            float downloadSpeed = GetCurrentDownloadSpeed(request, initialDownloadedBytes);
            if (downloadSpeed > 0f)
            {
                float estimatedTimeInSeconds = ((float)totalSizeOfFilesToDownload - downloadedBytes) / downloadSpeed;
                string estimatedTimeFormatted = FormatTime(estimatedTimeInSeconds);

                PrintToConsole($"Speed: {FileUtils.FormatFileSize((long)downloadSpeed)}/s {estimatedTimeFormatted}", false, Color.yellow, true);
            }

            yield return null;
        }

        if (request.isNetworkError || request.isHttpError)
        {
            PrintToConsole($"Download file failed {request.error}", true, Color.red);
            Debug.Log(request.error + " " + fileUrl);
        }
        else
        {
            PrintToConsole($"", false, Color.yellow, true);
            SaveDownloadedFile(request.downloadHandler.data, fileUrl);
        }
    }

    private void SaveDownloadedFile(byte[] data, string fileUrl)
    {
        string subDir = Path.Combine(Application.persistentDataPath, downloadPath);
        Directory.CreateDirectory(subDir);
        string fileName = GetFilenameFromURL(fileUrl);
        string filePath = Path.Combine(subDir, fileName);

        if (!File.Exists(filePath))
        {
            File.WriteAllBytes(filePath, data);
            OnDownloadFileCompleted?.Invoke();
        }
        downloadedBytes += (ulong)data.Length;
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
        PrintToConsole($"Total size of files to download: {FileUtils.FormatFileSize((long)totalSizeOfFilesToDownload)}", false, Color.white);
    }

    private void UpdateLoadingSlider(float progress)
    {
        loadingSlider.fillAmount = progress;
    }

    private void UpdateGlobalProgress()
    {
        globalProgressSlider.fillAmount = globalDownloadProgress;
    }

    private void PrintToConsole(string message, bool isError = false, Color textColor = default, bool deleteLastLine = false)
    {
        string formattedMessage = isError ? $"\n<color=red>[Error]{message}</color>" : $"\n{message}";

        if (deleteLastLine && !string.IsNullOrEmpty(screenConsole.text))
        {
            string[] lines = screenConsole.text.Split('\n');
            if (lines.Length > 1)
            {
                List<string> newLines = new List<string>(lines);
                newLines.RemoveAt(newLines.Count - 1);
                screenConsole.text = string.Join("\n", newLines);
            }
            else
            {
                screenConsole.text = "";
            }
        }

        if (textColor != default)
        {
            formattedMessage = $"<color=#{ColorUtility.ToHtmlStringRGB(textColor)}>{formattedMessage}</color>";
        }

        screenConsole.text += formattedMessage;
        consoleScrollRect.verticalNormalizedPosition = 0;
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
            if (!FileUtils.CheckFileExist(Path.Combine(Application.persistentDataPath, downloadPath, GetFilenameFromURL(url))))
            {
                totalSizeOfFilesToDownload += fileSize;
            }
            encounteredUrls.Add(url);
        }
    }

    private void CleanUpFiles()
    {
        string tracksPath = Path.Combine(Application.persistentDataPath, downloadPath);

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
        loadingSlider.fillAmount = 1;
        globalProgressSlider.fillAmount = 1;
    }

    private bool IsFileInList(string filename, HashSet<string> validFilenames)
    {
        return validFilenames.Contains(filename);
    }

    private float GetCurrentDownloadSpeed(UnityWebRequest request, ulong initialDownloadedBytes)
    {
        ulong downloadedBytes = request.downloadedBytes - initialDownloadedBytes;
        float elapsedTime = Time.time - downloadStartTime;

        if (downloadedBytes > 1024 && elapsedTime > 1f)
        {
            float downloadSpeed = downloadedBytes / elapsedTime;
            OnDownloadSpeedUpdate?.Invoke(downloadSpeed);
            return downloadSpeed;
        }

        return 0f;
    }

    private float CalculateGlobalDownloadProgress()
    {
        float downloadedSize = 0;
        foreach (Mp3TrackProperties trackProperties in downloadedList.mp3TrackProperties)
        {
            string audioFile = GetFilenameFromURL(trackProperties.trackAudioClipPath);
            string textureFile = GetFilenameFromURL(trackProperties.trackAudioClipTexturePath);
            string playlistTextureFile = GetFilenameFromURL(trackProperties.trackPlaylistTexturePath);

            if (FileUtils.CheckFileExist(Path.Combine(Application.persistentDataPath, downloadPath, audioFile)))
                downloadedSize += GetFileSizeFromPath(audioFile);

            if (FileUtils.CheckFileExist(Path.Combine(Application.persistentDataPath, downloadPath, textureFile)))
                downloadedSize += GetFileSizeFromPath(textureFile);

            if (FileUtils.CheckFileExist(Path.Combine(Application.persistentDataPath, downloadPath, playlistTextureFile)))
                downloadedSize += GetFileSizeFromPath(playlistTextureFile);
        }

        return downloadedSize / totalSizeOfFilesToDownload;
    }

    private long GetFileSizeFromPath(string filename)
    {
        string filePath = Path.Combine(Application.persistentDataPath, downloadPath, filename);
        if (File.Exists(filePath))
        {
            FileInfo fileInfo = new FileInfo(filePath);
            return fileInfo.Length;
        }
        return 0;
    }

    private string GetFilenameFromURL(string url)
    {
        Uri uri = new Uri(url);
        return Path.GetFileName(uri.LocalPath);
    }

    private string FormatTime(float seconds)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
        return $"{timeSpan.Hours:D2}H {timeSpan.Minutes:D2}Min {timeSpan.Seconds:D2}S";
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