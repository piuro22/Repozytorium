using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using System.IO;
using UnityEngine.Networking;
using System.Net;
using System.Collections;

public class Mp3PlayerController : MonoBehaviour
{
    public static Mp3PlayerController Instance { get; private set; }
    [SerializeField] AudioSource audioSource;
    [SerializeField] Slider timeSlider;
    [SerializeField] TMP_Text currentTimeText;
    [SerializeField] TMP_Text totalTimeText;
    [SerializeField] TMP_Text trackTitleText;
    [SerializeField] Image trackImage;
    [SerializeField] Image playlistImage;
    [SerializeField] private Image playPauseImage;
    [SerializeField] private Sprite playSprite;
    [SerializeField] private Sprite pauseSprite;

    [SerializeField] private Button playButton;
    [SerializeField] private Button stopButton;
    [SerializeField] private Button nextTrackButton;
    [SerializeField] private Button previousTrackButton;
    // [SerializeField] private Button slowDownButton;
    // [SerializeField] private Button speedUpButton;

    [SerializeField, ReadOnly] private bool isPlaying = false;
    [SerializeField, ReadOnly] private bool isPaused = false;
    [SerializeField, ReadOnly] private int currentTrack = 0;

    [SerializeField] AudioClip exampleAudioClip;

    [SerializeField] private RootMp3TrackProperties downloadedList;

    [SerializeField] private List<Mp3TrackProperties> trackList = new List<Mp3TrackProperties>();

    [SerializeField] private Transform trackButtonParrent;

    [SerializeField] private GameObject trackButton;


    private string totalClipTime;
    private List<TrackButtonController> trackButtons = new List<TrackButtonController>();
    enum DownloadType { track, trackTexture, playlistTexture };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        Initialize();
        LoadJson();
        LoadPlaylist("PlayLista 1");
    }

    public void LoadPlaylist(string playListName)
    {
        foreach (Mp3TrackProperties mp3TrackProperties in downloadedList.mp3TrackProperties)
        {
            if (playListName == mp3TrackProperties.playListName)
            {
                trackList.Add(mp3TrackProperties);
                GameObject newTrackButton = Instantiate(trackButton, trackButtonParrent);
                TrackButtonController trackButtonController = newTrackButton.GetComponent<TrackButtonController>();
                trackButtonController.Initialize(mp3TrackProperties);
                trackButtons.Add(trackButtonController);
            }
        }
    }

    public void PlayTrack(string trackTitle)
    {
        foreach (Mp3TrackProperties mp3TrackProperties in trackList)
        {
            if (mp3TrackProperties.title == trackTitle)
            {
                currentTrack = trackList.IndexOf(mp3TrackProperties);
                InitializeTrack(mp3TrackProperties);
                break;
            }
        }
    }

    public void PlayTrack(int trackIndex)
    {
        InitializeTrack(trackList[trackIndex]);
    }

    public void LoadJson()
    {
        string subDir = Path.Combine(Application.persistentDataPath);
        Directory.CreateDirectory(subDir);
        string messagepath = Path.Combine(subDir, "Tracks" + ".json");

        if (!File.Exists(messagepath))
        {
            File.WriteAllText(messagepath, "");
        }
        string jsonString = File.ReadAllText(messagepath);

        if (jsonString != "")
        {
            downloadedList = JsonUtility.FromJson<RootMp3TrackProperties>(jsonString);
        }
    }


    public static Texture2D LoadImageFromFile(string filePath)
    {
        string path = Path.Combine(Application.persistentDataPath, "Tracks", filePath);

        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(path))
        {
            fileData = File.ReadAllBytes(path);
            tex = new Texture2D(2, 2, TextureFormat.RGB24, false);
            tex.LoadImage(fileData);
        }
        return tex;
    }
    private IEnumerator LoadAudioClipFromFile(string filePath, Action<AudioClip> callback)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + Path.Combine(Application.persistentDataPath, "Tracks", filePath), AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                AudioClip audioClip = DownloadHandlerAudioClip.GetContent(www);
                callback?.Invoke(audioClip);
            }
            else
            {
                Debug.LogError("Error loading audio clip: " + www.error);
                callback?.Invoke(null);
            }
        }
    }




    private void Initialize()
    {
        playButton.onClick.AddListener(PlayPause);
        stopButton.onClick.AddListener(Stop);
        nextTrackButton.onClick.AddListener(NextTrack);
        previousTrackButton.onClick.AddListener(PreviousTrack);
        // slowDownButton.onClick.AddListener(SlowDown);
        // speedUpButton.onClick.AddListener(SpeedUp);
        isPlaying = false;
        isPaused = true;
        audioSource.playOnAwake = false;
        // InitializeTrack(playList.mp3TrackProperties[currentTrack]);
    }

    private string GetSavePath()
    {
        return Application.persistentDataPath;
    }




    private void Update()
    {

        currentTimeText.text = $"{FormatTime(audioSource.time)}/{totalClipTime}";
        if (isPlaying && !isPaused)
        {
            timeSlider.value = audioSource.time;
            if (Mathf.Approximately(timeSlider.value, timeSlider.maxValue))
            {
                Stop();
            }
        }
    }

    private void InitializeTrack(Mp3TrackProperties track)
    {
        trackTitleText.text = track.title;
        Texture2D trackTexture = LoadImageFromFile(GetFilenameFromURL(track.trackAudioClipTexturePath));
        trackImage.sprite = Sprite.Create(trackTexture, new Rect(0, 0, trackTexture.width, trackTexture.height), Vector2.one * 0.5f);
        Texture2D playlistTexture = LoadImageFromFile(GetFilenameFromURL(track.trackPlaylistTexturePath));
        playlistImage.sprite = Sprite.Create(playlistTexture, new Rect(0, 0, playlistTexture.width, playlistTexture.height), Vector2.one * 0.5f);
        
        StartCoroutine(LoadAudioClipFromFile(GetFilenameFromURL(track.trackAudioClipPath), (AudioClip loadedAudioClip) =>
        {
            audioSource.clip = loadedAudioClip;
            timeSlider.value = 0;
            audioSource.time = 0;
            currentTimeText.text = FormatTime(0);
            totalClipTime = FormatTime(audioSource.clip.length);
            timeSlider.maxValue = audioSource.clip.length;
            if(isPlaying)
            {
                Stop();
                PlayPause();
            }
            else
            {
                Stop();
            }
            for (int i = 0; i < trackButtons.Count; i++)
            {
                if(i == currentTrack)
                {
                    trackButtons[i].HighLightButton(true);
                }
                else
                {
                    trackButtons[i].HighLightButton(false);
                }
            }

        }));


    }


    private void PlayPause()
    {
        if (!isPlaying && isPaused)
        {
            playPauseImage.sprite = pauseSprite;
            audioSource.Play();
            isPaused = false;
            isPlaying = true;
            return;
        }
        if (isPlaying && !isPaused)
        {
            playPauseImage.sprite = playSprite;
            audioSource.Pause();
            isPaused = true;
            isPlaying = false;
            return;
        }
    }

    private void Stop()
    {
        audioSource.Stop();
        isPlaying = false;
        isPaused = true;
        timeSlider.value = 0;
        audioSource.time = 0;
        currentTimeText.text = FormatTime(0);
    }

    private void NextTrack()
    {
        if (currentTrack == trackList.Count-1)
            currentTrack = 0;
        else
            currentTrack++;
        timeSlider.value = 0;
        PlayTrack(currentTrack);
    }

    private void PreviousTrack()
    {
        if (currentTrack == 0)
            currentTrack = trackList.Count-1;
        else
            currentTrack--;
        timeSlider.value = 0;
        PlayTrack(currentTrack);
    }



    #region trackSpeed
    private void SlowDown()
    {
        if (audioSource.pitch > 0.5f) // Limit the slowdown to 50% of original pitch
        {
            audioSource.pitch -= 0.1f;
        }
    }

    private void SpeedUp()
    {
        if (audioSource.pitch < 2.0f) // Limit the speedup to 200% of original pitch
        {
            audioSource.pitch += 0.1f;
        }
    }
    #endregion

    #region Slider
    public void OnSliderDragStart()
    {
        if (isPlaying && !isPaused)
        {
            PlayPause();
        }
    }

    public void OnSliderDragEnd()
    {
        if (isPaused)
        {
            audioSource.time = timeSlider.value;
            PlayPause();
            isPaused = false;
        }
    }
    #endregion

    private string GetFilenameFromURL(string url)
    {
        Uri uri = new Uri(url);
        string filename = Path.GetFileName(uri.LocalPath);
        return filename;
    }

    private string FormatTime(float time)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(time);
        return string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
    }

    [Button]
    private void ShowPersistentDataPath()
    {
        System.Diagnostics.Process.Start("explorer.exe", "/select," + Application.persistentDataPath.Replace("/", "\\"));
    }
}