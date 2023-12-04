using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

public class PhotoGalleryController : MonoBehaviour
{
    [SerializeField] public PhotoGalleryProperties gameProperties;

    
    public static PhotoGalleryController Instance { get; private set; }
    [Header("UI References")]
    [SerializeField] private Image image1;
    [SerializeField] private Image image2;
    //[SerializeField] private TMP_InputField indexInputField; // Input field to set index
    [SerializeField] private TMP_Text photoCounterText;       // Text to display current photo/max photos
   
    [Header("Buttons")]
    [SerializeField] private Button nextButton;             // Button to show the next photo
    [SerializeField] private Button previousButton;         // Button to show the previous photo
    [SerializeField] private Button playAudioButton;        // Button to play the audio clip manually




    [SerializeField] private float fadeDuration = 1f;

    private int currentIndex = 0;
    private Image activeImage;
    private Image inactiveImage;
    private AudioSource audioSource;
    [SerializeField] private bool shouldPlayAudioAutomatically;
    public GameObject finishScreen;
    //t³o
    public SpriteRenderer background;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;



        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        // Initialize buttons and listeners
        nextButton.onClick.AddListener(ShowNextPhoto);
        previousButton.onClick.AddListener(ShowPreviousPhoto);
        if (gameProperties.useAudio)
        {
            playAudioButton.onClick.AddListener(PlayAudioClipForCurrentPhoto);
        }
        else
        {
            playAudioButton.gameObject.SetActive(false);
        }

      
    }
    private void Start()
    {
        InitializeGallery();
       
    }
    
    private void InitializeGallery()
    {

        if (gameProperties.photoWithAudios.Count < 2)
        {
            Debug.LogError("Please add at least two photos with audio for the crossfade effect.");
            return;
        }
        //t³o
        background.sprite = gameProperties.background;

        image1.sprite = gameProperties.photoWithAudios[currentIndex].photo;
        image2.sprite = gameProperties.photoWithAudios[GetNextPhotoIndex()].photo;

        image1.color = Color.white;
        image2.color = new Color(1, 1, 1, 0);

        activeImage = image1;
        inactiveImage = image2;

        if (shouldPlayAudioAutomatically)
        {
            PlayAudioClipForCurrentPhoto();
        }
        
        UpdatePhotoCounterText();
    }

    public void PlayAudioClipForCurrentPhoto()
    {
        if (audioSource != null && gameProperties.photoWithAudios[currentIndex].audioClip != null)
        {
            audioSource.clip = gameProperties.photoWithAudios[currentIndex].audioClip;
            audioSource.Play();
        }
    }

    public void ShowNextPhoto()
    {
        currentIndex = GetNextPhotoIndex();
        CrossfadeToNewPhoto();
        if (shouldPlayAudioAutomatically)
        {
            PlayAudioClipForCurrentPhoto();
        }
        if (currentIndex == gameProperties.photoWithAudios.Count-1)
          Invoke("FinishGame", 2);
    }

    public void ShowPreviousPhoto()
    {
        currentIndex = GetPreviousPhotoIndex();
        CrossfadeToNewPhoto();
        if(shouldPlayAudioAutomatically)
        {
            PlayAudioClipForCurrentPhoto();
        }
       
    }

    private void CrossfadeToNewPhoto()
    {
        inactiveImage.sprite = gameProperties.photoWithAudios[currentIndex].photo;

        activeImage.DOFade(0f, fadeDuration);
        inactiveImage.DOFade(1f, fadeDuration).OnComplete(() =>
        {
            SwapActiveImages();
            UpdatePhotoCounterText();
        });
    }

    private int GetNextPhotoIndex()
    {
      
        return (currentIndex + 1) % gameProperties.photoWithAudios.Count;
    }

    private int GetPreviousPhotoIndex()
    {
        return (currentIndex - 1 + gameProperties.photoWithAudios.Count) % gameProperties.photoWithAudios.Count;
    }

    private void SwapActiveImages()
    {
        var temp = activeImage;
        activeImage = inactiveImage;
        inactiveImage = temp;
    }

    private void UpdatePhotoCounterText()
    {
        photoCounterText.text = $"{currentIndex + 1}/{gameProperties.photoWithAudios.Count}";
    }
    public void FinishGame()
    {
        finishScreen.SetActive(true);
    }
    public void BackToChoseLevels()
    {
        SceneManager.LoadScene(PlayerPrefs.GetString("LastChoseGameScene"));
    }
}