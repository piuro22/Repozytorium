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
    [SerializeField] private ScrollRect consoleScrollRect;
    [SerializeField] private TMP_Text screenConsole;
    [SerializeField] private RootMp3TrackProperties downloadedList;
    [SerializeField] private bool shouldCleanUnnecessaryFiles;

    [SerializeField] private Image loadingSlider;
    [SerializeField] private Image globalProgressSlider;
    private float currentDownloadProgress = 0f;
    private float globalDownloadProgress = 0f;
    private long totalSizeOfFilesToDownload = 0;
    private float downloadStartTime;
    private Dictionary<string, long> fileSizeByUrl = new Dictionary<string, long>();
    private HashSet<string> encounteredUrls = new HashSet<string>();

    public delegate void DownloadCompletedHandler();
    public event DownloadCompletedHandler OnDownloadCompleted;

    public delegate void InternetErrorHandler();
    public event InternetErrorHandler OnInternetErrorHandler;


    private void Start()
    {
        Application.targetFrameRate = 60;
        loadingSlider.fillAmount = 0f;
        globalProgressSlider.fillAmount = 0f;
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
            OnInternetErrorHandler.Invoke();
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

            if (!FileUtils.CheckFileExist(Path.Combine(Application.persistentDataPath, "Tracks", audioFile)))
            {
                yield return StartCoroutine(DownloadFile(trackProperties.trackAudioClipPath));
            }
            else
            {
                PrintToConsole($"File already exist: {audioFile}", false, Color.green);
            }
            if (!FileUtils.CheckFileExist(Path.Combine(Application.persistentDataPath, "Tracks", textureFile)))
            {
                yield return StartCoroutine(DownloadFile(trackProperties.trackAudioClipTexturePath));
            }
            else
            {
                PrintToConsole($"File already exist: {textureFile}", false, Color.green);
            }

            if (!FileUtils.CheckFileExist(Path.Combine(Application.persistentDataPath, "Tracks", playlistTextureFile)))
            {
                yield return StartCoroutine(DownloadFile(trackProperties.trackPlaylistTexturePath));
            }
            else
            {
                PrintToConsole($"File already exist: {playlistTextureFile}", false, Color.green);
            }
        }
        if (shouldCleanUnnecessaryFiles)
        {
            CleanUpFiles();
        }
        OnDownloadCompleted.Invoke();
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
        downloadStartTime = Time.time; // Record download start time
        ulong initialDownloadedBytes = 0;

        while (!operation.isDone)
        {
            currentDownloadProgress = request.downloadProgress;
            UpdateLoadingSlider();
            UpdateGlobalProgress();

            if (fileSize < 0 && !string.IsNullOrEmpty(request.GetResponseHeader("Content-Length")))
            {
                if (long.TryParse(request.GetResponseHeader("Content-Length"), out fileSize))
                {
                    PrintToConsole($"Prepare to download file from: {fileUrl} [{ FileUtils.FormatFileSize(fileSize)}]\n", false, Color.cyan);
                }
            }

            // Calculate download speed only after a certain threshold
            ulong downloadedBytes = request.downloadedBytes - initialDownloadedBytes;
            float elapsedTime = Time.time - downloadStartTime;

            // Add a minimum threshold for download speed calculation
            if (downloadedBytes > 1024 && elapsedTime > 1f)
            {
                float downloadSpeed = downloadedBytes / elapsedTime;
                long remainingBytes = fileSize - (long)(request.downloadedBytes - initialDownloadedBytes);
                float estimatedTime = remainingBytes / downloadSpeed;
                string estimatedTimeFormatted = FormatTime(estimatedTime);

                PrintToConsole($"Download speed: {FileUtils.FormatFileSize((long)downloadSpeed)}/s  {(currentDownloadProgress * 100).ToString("0.00")}%  Estimated time: {estimatedTimeFormatted}", false, Color.yellow, true);
            }

            yield return null;
        }


        if (request.isNetworkError || request.isHttpError)
        {
            PrintToConsole($"Download file failed {request.error}", true, Color.red);
            OnInternetErrorHandler.Invoke();
            Debug.Log(request.error + " " + fileUrl);
        }
        else
        {
            PrintToConsole($"", false, Color.yellow, true);
            SaveDownloadedFile(request.downloadHandler.data, fileUrl);
        }

        // Calculate and display final download speed
        float finalDownloadTime = Time.time - downloadStartTime;
        ulong totalDownloadedBytes = request.downloadedBytes - initialDownloadedBytes;
        float finalDownloadSpeed = totalDownloadedBytes / finalDownloadTime;
        PrintToConsole($"Avarage downloadspeed: {FileUtils.FormatFileSize((long)finalDownloadSpeed)}/s", false, Color.yellow);
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
        loadingSlider.fillAmount = 1;
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
        PrintToConsole($"Total size of files to download: { FileUtils.FormatFileSize(totalSizeOfFilesToDownload)}", false, Color.white);
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

            if (FileUtils.CheckFileExist(Path.Combine(Application.persistentDataPath, "Tracks", audioFile)))
                downloadedFiles++;

            if (FileUtils.CheckFileExist(Path.Combine(Application.persistentDataPath, "Tracks", textureFile)))
                downloadedFiles++;

            if (FileUtils.CheckFileExist(Path.Combine(Application.persistentDataPath, "Tracks", playlistTextureFile)))
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
        loadingSlider.fillAmount = currentDownloadProgress;
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
            // Split the current text by newlines and remove the last line.
            string[] lines = screenConsole.text.Split('\n');
            if (lines.Length > 1)
            {
                string[] newLines = new string[lines.Length - 1];
                for (int i = 0; i < newLines.Length; i++)
                {
                    newLines[i] = lines[i];
                }
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
            if (!FileUtils.CheckFileExist(Path.Combine(Application.persistentDataPath, "Tracks", GetFilenameFromURL(url))))
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
        loadingSlider.fillAmount = 1;
        globalProgressSlider.fillAmount = 1;
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

    private string FormatTime(float seconds)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
        return $"{timeSpan.Hours:D2}H {timeSpan.Minutes:D2}Min {timeSpan.Seconds:D2}S";
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