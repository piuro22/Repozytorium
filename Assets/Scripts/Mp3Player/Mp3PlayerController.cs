using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.IO;
using System.Collections;
using System.Linq;
using System;

public class Mp3PlayerController : MonoBehaviour
{
    public static Mp3PlayerController Instance { get; private set; }

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Slider timeSlider;
    [SerializeField] private TMP_Text currentTimeText;
    [SerializeField] private TMP_Text trackTitleText;
    [SerializeField] private Image playPauseImage;
    [SerializeField] private Sprite playSprite;
    [SerializeField] private Sprite pauseSprite;

    [SerializeField] private Button playButton;
    [SerializeField] private Button stopButton;
    [SerializeField] private Button nextTrackButton;
    [SerializeField] private Button previousTrackButton;

    [SerializeField] private Transform trackButtonParent;
    [SerializeField] private GameObject trackButtonPrefab;

    private List<AudioClip> trackList = new List<AudioClip>();
    private List<string> trackPaths = new List<string>();
    private List<Button> trackButtons = new List<Button>();

    private int currentTrackIndex = 0;
    private bool isPlaying = false;

    [SerializeField] private string jsonFilePath = "Tracks.json";
    [SerializeField]
    private RootMp3TrackProperties downloadedList;

    [SerializeField] private string downloadPath = "download";
    [SerializeField] private string jsonFileName = "Tracks.json";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeUI();

        // Load JSON tracks
        LoadTracksFromJson();

        // Load playlist from saved QR code
        GetFromSave();
    }

    private void Update()
    {
        if (isPlaying && audioSource.isPlaying)
        {
            // Update the slider value based on the audio playback progress
            timeSlider.value = audioSource.time / audioSource.clip.length;
            UpdateTimeDisplay();
        }
    }


    private void InitializeUI()
    {
        playButton.onClick.AddListener(TogglePlayPause);
        stopButton.onClick.AddListener(StopPlayback);
        nextTrackButton.onClick.AddListener(PlayNextTrack);
        previousTrackButton.onClick.AddListener(PlayPreviousTrack);
        timeSlider.onValueChanged.AddListener(Seek);
    }

    private void GetFromSave()
    {
        if (!PlayerPrefs.HasKey("QRCode"))
        {
            Debug.LogWarning("No playlist saved. Loading default tracks.");
            return;
        }

        string playListSave = PlayerPrefs.GetString("QRCode");
        PlayerPrefs.DeleteKey("QRCode");

        string[] parts = playListSave.Split('/');
        if (parts.Length >= 2)
        {
            string playlistName = parts[1];
            Debug.Log($"Attempting to load playlist: {playlistName}");
            LoadPlaylist(playlistName);
        }

        if (parts.Length >= 3)
        {
            string trackTitle = parts[2];
            Debug.Log($"Attempting to play track: {trackTitle}");
            PlayTrackByTitle(trackTitle);
        }
        else if (trackList.Count > 0)
        {
            PlayTrack(0);
        }
    }

    private void LoadTracksFromJson()
    {
        string filePath = Path.Combine(Application.persistentDataPath, jsonFileName);

        if (!File.Exists(filePath))
        {
            Debug.LogError($"JSON file not found at path: {filePath}");
            return;
        }

        string jsonContent = File.ReadAllText(filePath);
        downloadedList = JsonUtility.FromJson<RootMp3TrackProperties>(jsonContent);

        if (downloadedList == null || downloadedList.mp3TrackProperties == null)
        {
            Debug.LogError("Failed to parse JSON or no tracks available.");
            return;
        }

        Debug.Log($"Loaded {downloadedList.mp3TrackProperties.Length} tracks from JSON.");
    }

    private IEnumerator LoadAudioClip(string filePath)
    {
        using (var www = UnityEngine.Networking.UnityWebRequestMultimedia.GetAudioClip($"file://{filePath}", AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                AudioClip clip = UnityEngine.Networking.DownloadHandlerAudioClip.GetContent(www);
                trackList.Add(clip);
                Debug.Log("Add Filepath" + filePath);
                trackPaths.Add(filePath);

                string fileName = Path.GetFileNameWithoutExtension(filePath);
                CreateTrackButton(fileName);
            }
            else
            {
                Debug.LogError($"Failed to load track: {filePath}, Error: {www.error}");
            }
        }
    }

    public void LoadPlaylist(string playlistName)
    {
        if (downloadedList == null || downloadedList.mp3TrackProperties == null)
        {
            Debug.LogWarning("Track list is empty. Ensure JSON is loaded before calling this method.");
            return;
        }

        Debug.Log($"Loading playlist: {playlistName}");
        ClearCurrentTracks();

        // Filter tracks by playlist name
        var filteredTracks = new List<Mp3TrackProperties>();
        foreach (var track in downloadedList.mp3TrackProperties)
        {
            if (track.playListName.Equals(playlistName, StringComparison.OrdinalIgnoreCase))
            {
                filteredTracks.Add(track);
            }
        }

        // Apply natural sorting on titles
        filteredTracks.Sort((a, b) => NaturalSortComparer(a.title, b.title));
        foreach (var track in filteredTracks)
        {
            Debug.Log($"Creating button for track: ID = {track.id}, Title = {track.title}");
            StartCoroutine(LoadAudioClipFromUrl(track.trackAudioClipPath, track.title));
        }
    }

    // Natural sorting comparer for strings
    private int NaturalSortComparer(string a, string b)
    {
        int i = 0, j = 0;
        while (i < a.Length && j < b.Length)
        {
            if (char.IsDigit(a[i]) && char.IsDigit(b[j]))
            {
                // Compare numbers
                int numA = 0, numB = 0;

                // Parse numbers from the strings
                while (i < a.Length && char.IsDigit(a[i]))
                {
                    numA = numA * 10 + (a[i] - '0');
                    i++;
                }
                while (j < b.Length && char.IsDigit(b[j]))
                {
                    numB = numB * 10 + (b[j] - '0');
                    j++;
                }

                // Compare numeric parts
                int result = numA.CompareTo(numB);
                if (result != 0)
                    return result;
            }
            else
            {
                // Compare characters
                int result = a[i].CompareTo(b[j]);
                if (result != 0)
                    return result;

                i++;
                j++;
            }
        }

        // Compare lengths if both strings are similar
        return a.Length - b.Length;
    }

    private IEnumerator LoadAudioClipFromUrl(string url, string trackTitle)
    {
        using (var www = UnityEngine.Networking.UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                AudioClip clip = UnityEngine.Networking.DownloadHandlerAudioClip.GetContent(www);
                trackList.Add(clip);

                CreateTrackButton(trackTitle);
            }
            else
            {
                Debug.LogError($"Failed to load track from URL: {url}, Error: {www.error}");
            }
        }
    }


    private void ClearCurrentTracks()
    {
        trackList.Clear();
        foreach (Transform child in trackButtonParent)
        {
            Destroy(child.gameObject);
        }
        trackButtons.Clear();
    }

    public void PlayTrackByTitle(string trackTitle)
    {
        int trackIndex = trackList.FindIndex(track => track.name.Equals(trackTitle, System.StringComparison.OrdinalIgnoreCase));

        if (trackIndex != -1)
        {
            PlayTrack(trackIndex);
        }
        else
        {
            Debug.LogWarning($"Track with title '{trackTitle}' not found.");
        }
    }

    public void PlayTrack(int trackIndex)
    {
        if (trackIndex < 0 || trackIndex >= trackList.Count) return;

        currentTrackIndex = trackIndex;
        audioSource.clip = trackList[trackIndex];
        StartPlayback();
        UpdateTrackInfo();
    }

    private void TogglePlayPause()
    {
        if (isPlaying)
        {
            PausePlayback();
        }
        else
        {
            StartPlayback();
        }
    }

    private void StartPlayback()
    {
        if (audioSource.clip == null && trackList.Count > 0)
        {
            audioSource.clip = trackList[currentTrackIndex];
        }

        if (audioSource.clip != null)
        {
            audioSource.Play();
            isPlaying = true;
            playPauseImage.sprite = pauseSprite;

            // Update the slider maximum value
            timeSlider.maxValue = 1f; // Slider represents normalized playback (0.0 to 1.0)
            timeSlider.value = 0f;   // Reset slider position
        }
    }

    private void PausePlayback()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
            isPlaying = false;
            playPauseImage.sprite = playSprite;
        }
    }

    private void StopPlayback()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
            isPlaying = false;
            audioSource.time = 0;
            playPauseImage.sprite = playSprite;

            // Reset slider value
            timeSlider.value = 0;
            UpdateTimeDisplay();
        }
    }

    private void Seek(float value)
    {
        if (audioSource.clip != null)
        {
            audioSource.time = value * audioSource.clip.length;
        }
    }

    private void UpdateTrackInfo()
    {
        if (trackList.Count > 0 && currentTrackIndex < trackList.Count)
        {
            AudioClip currentTrack = trackList[currentTrackIndex];
            trackTitleText.text = currentTrack.name;
        }
    }

    private void CreateTrackButton(string trackName)
    {
        GameObject newTrackButton = Instantiate(trackButtonPrefab, trackButtonParent);
        TrackButtonController trackButtonController = newTrackButton.GetComponent<TrackButtonController>();

        if (trackButtonController != null)
        {
            trackButtonController.Initialize(trackName);
        }

        Button button = newTrackButton.GetComponent<Button>();
        int trackIndex = trackButtons.Count;
        button.onClick.AddListener(() => PlayTrack(trackIndex));
        trackButtons.Add(button);
    }

    private void PlayNextTrack()
    {
        if (trackList.Count == 0) return;

        currentTrackIndex = (currentTrackIndex + 1) % trackList.Count;
        PlayTrack(currentTrackIndex);
    }

    private void PlayPreviousTrack()
    {
        if (trackList.Count == 0) return;

        currentTrackIndex = (currentTrackIndex - 1 + trackList.Count) % trackList.Count;
        PlayTrack(currentTrackIndex);
    }

    private void UpdateTimeDisplay()
    {
        if (audioSource.clip != null)
        {
            currentTimeText.text = $"{FormatTime(audioSource.time)} / {FormatTime(audioSource.clip.length)}";
        }
    }
    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        return $"{minutes:00}:{seconds:00}";
    }
}
