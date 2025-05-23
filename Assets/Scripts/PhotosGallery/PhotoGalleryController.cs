using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class PhotoGalleryController : MonoBehaviour
{
    [Header("Gallery Properties")]
    [SerializeField] private PhotoGalleryProperties gameProperties;

    [Header("UI References")]
    [SerializeField] private Image image1;
    [SerializeField] private Image image2;
    [SerializeField] private Image backgroundUI;

    [SerializeField] private TMP_Text photoCounterText;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;
    [SerializeField] private Button playAudioButton;
    [SerializeField] private GameObject finishScreen;
    [SerializeField] private SpriteRenderer background;
    [SerializeField] private AudioSource musicController;
    [Header("Settings")]
    [SerializeField] private float fadeDuration = 1f;
    public AudioSource commandAudioSource;

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
        // Create a new AudioSource for the command audio if it doesn't already exist


     
        // Play the command audio
        commandAudioSource.clip = gameProperties.gameCommandAudioClip;
        commandAudioSource.Play();

        // Configure the main music controller but do not start it yet
        musicController.clip = gameProperties.gameMusic;
        musicController.loop = true;
    }


    private void Start()
    {
        SetupMusic();
        StartCoroutine(WaitForCommandAudioToFinish());
    }
    private IEnumerator WaitForCommandAudioToFinish()
    {
        // Ensure the command audio clip is playing
        if (commandAudioSource != null && commandAudioSource.isPlaying)
        {
            yield return new WaitWhile(() => commandAudioSource.isPlaying);
        }

        // Start background music after command audio finishes
        musicController.Play();

        // Proceed to set up the gallery
        if (gameProperties.photoWithAudios.Count < 2)
        {
            Debug.LogError("Insufficient photos with audio.");
            yield break;
        }

        ShuffleGallery();  // Chat GPT

        background.sprite = gameProperties.background;
        SetupInitialImages();

        if (gameProperties.shouldPlayAudioAutomatically)
            PlayAudioClipForCurrentPhoto();

        UpdatePhotoCounterText();
        UpdateNavigationButtons(); // Ensure buttons are updated initially
    }

    private void ShuffleGallery()   //Chat gpt
    {
        var list = gameProperties.photoWithAudios;
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            var temp = list[i];
            list[i] = list[rand];
            list[rand] = temp;
        }
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
        
        if (gameProperties.useScreenClick)
        {
            nextButton.gameObject.SetActive(false);
            previousButton.gameObject.SetActive(false);
            photoCounterText.gameObject.SetActive(false);
            image1.GetComponent<Button>().onClick.AddListener(ShowNextPhoto);
            backgroundUI.GetComponent<Button>().onClick.AddListener(ShowNextPhoto);
            image2.GetComponent<Button>().onClick.AddListener(ShowNextPhoto);
        }
        else
        {

            nextButton.onClick.AddListener(ShowNextPhoto);
            previousButton.onClick.AddListener(ShowPreviousPhoto);
        }

        if (gameProperties.useAudio)
            playAudioButton.onClick.AddListener(PlayAudioClipForCurrentPhoto);
        else
            playAudioButton.gameObject.SetActive(false);
    }

    private void InitializeGallery()
    {
        SetupMusic();
        if (gameProperties.photoWithAudios.Count < 2)
        {
            Debug.LogError("Insufficient photos with audio.");
            return;
        }

        background.sprite = gameProperties.background;
        SetupInitialImages();

        if (gameProperties.shouldPlayAudioAutomatically)
            PlayAudioClipForCurrentPhoto();

        UpdatePhotoCounterText();
        UpdateNavigationButtons(); // Ensure buttons are updated initially
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
        if (audioSource.isPlaying) return;

        if (currentIndex < gameProperties.photoWithAudios.Count - 1)
        {
            currentIndex++;
            CrossfadeToNewPhoto();
            if (gameProperties.shouldPlayAudioAutomatically)
                PlayAudioClipForCurrentPhoto();

            UpdateNavigationButtons();
        }
        else
        {
            CheckEndOfGallery(); // Call this if the last photo is reached
        }
    }

    private void ShowPreviousPhoto()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            CrossfadeToNewPhoto();
            if (gameProperties.shouldPlayAudioAutomatically)
                PlayAudioClipForCurrentPhoto();

            UpdateNavigationButtons();
        }
    }
    private Sequence crossSequence;
    private void CrossfadeToNewPhoto()
    {
        inactiveImage.sprite = gameProperties.photoWithAudios[currentIndex].photo;

        if (crossSequence != null) crossSequence.Kill();
        crossSequence = DOTween.Sequence();
        crossSequence.Append(activeImage.DOFade(0f, fadeDuration));
        crossSequence.Join(inactiveImage.DOFade(1f, fadeDuration).OnComplete(SwapActiveImages));
    }
    private void UpdateNavigationButtons()
    {
        // Disable the "Previous" button if at the first photo
        previousButton.interactable = currentIndex > 0;

        // Disable the "Next" button if at the last photo
        nextButton.interactable = currentIndex < gameProperties.photoWithAudios.Count - 1;
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
        {
            finishScreen.SetActive(true);
            nextButton.interactable = false; // Disable the "Next" button
        }
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