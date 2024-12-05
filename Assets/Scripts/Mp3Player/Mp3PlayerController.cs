using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.IO;
using System.Collections;

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

    [SerializeField] private string downloadPath = "download";

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
        LoadTracksFromDirectory();
        UpdateTrackInfo();
    }

    private void InitializeUI()
    {
        playButton.onClick.AddListener(TogglePlayPause);
        stopButton.onClick.AddListener(StopPlayback);
        nextTrackButton.onClick.AddListener(PlayNextTrack);
        previousTrackButton.onClick.AddListener(PlayPreviousTrack);
        timeSlider.onValueChanged.AddListener(Seek);
    }

    private void LoadTracksFromDirectory()
    {
        string directoryPath = Path.Combine(Application.persistentDataPath, downloadPath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
            Debug.LogWarning($"Directory created at: {directoryPath}. Add MP3 files to this folder.");
            return;
        }

        string[] files = Directory.GetFiles(directoryPath, "*.mp3");

        foreach (string file in files)
        {
            StartCoroutine(LoadAudioClip(file));
        }
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
                trackPaths.Add(filePath);

                // Extract the file name without extension
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                Debug.Log("File: " + fileName);

                // Use the file name for the track button
                CreateTrackButton(fileName);
            }
            else
            {
                Debug.LogError($"Failed to load track: {filePath}, Error: {www.error}");
            }
        }
    }

    private void CreateTrackButton(string trackName)
    {
        GameObject newTrackButton = Instantiate(trackButtonPrefab, trackButtonParent);
        TMP_Text buttonText = newTrackButton.GetComponentInChildren<TMP_Text>();
        buttonText.text = trackName;

        Button button = newTrackButton.GetComponent<Button>();
        int trackIndex = trackButtons.Count;
        button.onClick.AddListener(() => PlayTrack(trackIndex));
        trackButtons.Add(button);
    }

    private void Update()
    {
        if (isPlaying && audioSource.isPlaying)
        {
            timeSlider.value = audioSource.time / audioSource.clip.length;
            UpdateTimeDisplay();
        }
    }

    private void UpdateTimeDisplay()
    {
        if (audioSource.clip != null)
        {
            currentTimeText.text = $"{FormatTime(audioSource.time)}/{FormatTime(audioSource.clip.length)}";
        }
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        return $"{minutes:00}:{seconds:00}";
    }

    private void UpdateTrackInfo()
    {
        if (trackList.Count > 0 && currentTrackIndex < trackList.Count)
        {
            AudioClip currentTrack = trackList[currentTrackIndex];
            trackTitleText.text = currentTrack.name;
            timeSlider.value = 0;
        }
    }

    public void TogglePlayPause()
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
            timeSlider.value = 0;
        }
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

    public void PlayTrack(int trackIndex)
    {
        if (trackIndex < 0 || trackIndex >= trackList.Count) return;

        currentTrackIndex = trackIndex;
        audioSource.clip = trackList[trackIndex];
        StartPlayback();
        UpdateTrackInfo();
    }

    private void Seek(float value)
    {
        if (audioSource.clip != null)
        {
            audioSource.time = value * audioSource.clip.length;
            UpdateTimeDisplay();
        }
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
}
