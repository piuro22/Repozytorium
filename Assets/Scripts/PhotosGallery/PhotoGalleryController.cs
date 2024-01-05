using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;

public class PhotoGalleryController : MonoBehaviour
{
    [Header("Gallery Properties")]
    [SerializeField] private PhotoGalleryProperties gameProperties;

    [Header("UI References")]
    [SerializeField] private Image image1;
    [SerializeField] private Image image2;
    [SerializeField] private TMP_Text photoCounterText;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;
    [SerializeField] private Button playAudioButton;
    [SerializeField] private GameObject finishScreen;
    [SerializeField] private SpriteRenderer background;
    [SerializeField] private AudioSource musicController;

    [Header("Settings")]
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private bool shouldPlayAudioAutomatically;

    private AudioSource audioSource;
    private int currentIndex = 0;
    private Image activeImage, inactiveImage;

    public static PhotoGalleryController Instance { get; private set; }

    private void Awake()
    {
        SetupSingleton();
        InitializeComponents();
        SetupButtons();
    }
    private void SetupMusic()
    {
        musicController.clip = gameProperties.gameMusic;
        musicController.loop = true;
        musicController.Play();
        musicController.PlayOneShot(gameProperties.gameCommandAudioClip);

    }

    private void Start()
    {
        
        InitializeGallery();
    }

    private void SetupSingleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void InitializeComponents()
    {
       
        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();

        if (Application.isPlaying)
        {
            if (GameManager.Instance != null)
                if (GameManager.Instance.currentGameProperties is PhotoGalleryProperties)
                    gameProperties = GameManager.Instance.currentGameProperties as PhotoGalleryProperties;
        }
    }

    private void SetupButtons()
    {
        nextButton.onClick.AddListener(ShowNextPhoto);
        previousButton.onClick.AddListener(ShowPreviousPhoto);

        if (gameProperties.useAudio)
            playAudioButton.onClick.AddListener(PlayAudioClipForCurrentPhoto);
        else
            playAudioButton.gameObject.SetActive(false);
    }

    private void InitializeGallery()
    {
        if (gameProperties.photoWithAudios.Count < 2)
        {
            Debug.LogError("Insufficient photos with audio.");
            return;
        }
        SetupMusic();
        background.sprite = gameProperties.background;
        SetupInitialImages();

        if (shouldPlayAudioAutomatically)
            PlayAudioClipForCurrentPhoto();

        UpdatePhotoCounterText();
    }

    private void SetupInitialImages()
    {
        image1.sprite = gameProperties.photoWithAudios[currentIndex].photo;
        image2.sprite = gameProperties.photoWithAudios[GetNextPhotoIndex()].photo;
        image1.color = Color.white;
        image2.color = new Color(1, 1, 1, 0);

        activeImage = image1;
        inactiveImage = image2;
    }

    public void PlayAudioClipForCurrentPhoto()
    {
        if (audioSource != null && gameProperties.photoWithAudios[currentIndex].audioClip != null)
        {
            audioSource.clip = gameProperties.photoWithAudios[currentIndex].audioClip;
            audioSource.Play();
        }
    }

    private void ShowNextPhoto()
    {
        currentIndex = GetNextPhotoIndex();
        CrossfadeToNewPhoto();
        if (shouldPlayAudioAutomatically)
            PlayAudioClipForCurrentPhoto();

        CheckEndOfGallery();
    }

    private void ShowPreviousPhoto()
    {
        currentIndex = GetPreviousPhotoIndex();
        CrossfadeToNewPhoto();
        if (shouldPlayAudioAutomatically)
            PlayAudioClipForCurrentPhoto();
    }

    private void CrossfadeToNewPhoto()
    {
        inactiveImage.sprite = gameProperties.photoWithAudios[currentIndex].photo;

        activeImage.DOFade(0f, fadeDuration);
        inactiveImage.DOFade(1f, fadeDuration).OnComplete(SwapActiveImages);
    }

    private int GetNextPhotoIndex() => (currentIndex + 1) % gameProperties.photoWithAudios.Count;

    private int GetPreviousPhotoIndex() => (currentIndex - 1 + gameProperties.photoWithAudios.Count) % gameProperties.photoWithAudios.Count;

    private void SwapActiveImages()
    {
        (activeImage, inactiveImage) = (inactiveImage, activeImage);
        UpdatePhotoCounterText();
    }

    private void UpdatePhotoCounterText()
    {
        photoCounterText.text = $"{currentIndex + 1}/{gameProperties.photoWithAudios.Count}";
    }

    private void CheckEndOfGallery()
    {
        if (currentIndex == gameProperties.photoWithAudios.Count - 1)
            Invoke("FinishGame", 4);
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