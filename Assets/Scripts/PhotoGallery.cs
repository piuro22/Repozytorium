using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;

[System.Serializable]
public class PhotoWithAudio
{
    public Sprite photo;
    public AudioClip audioClip;
}

public class PhotoGallery : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image image1;
    [SerializeField] private Image image2;
    //[SerializeField] private TMP_InputField indexInputField; // Input field to set index
    [SerializeField] private TMP_Text photoCounterText;       // Text to display current photo/max photos
   
    [Header("Buttons")]
    [SerializeField] private Button nextButton;             // Button to show the next photo
    [SerializeField] private Button previousButton;         // Button to show the previous photo
    [SerializeField] private Button playAudioButton;        // Button to play the audio clip manually


    [Header("Gallery Settings")]
    [SerializeField] private List<PhotoWithAudio> photosWithAudio = new List<PhotoWithAudio>();
    [SerializeField] private float fadeDuration = 1f;

    private int currentIndex = 0;
    private Image activeImage;
    private Image inactiveImage;
    private AudioSource audioSource;
    [SerializeField] private bool shouldPlayAudioAutomatically;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Initialize buttons and listeners
        nextButton.onClick.AddListener(ShowNextPhoto);
        previousButton.onClick.AddListener(ShowPreviousPhoto);
        playAudioButton.onClick.AddListener(PlayAudioClipForCurrentPhoto);

        InitializeGallery();
    }

    private void InitializeGallery()
    {
        if (photosWithAudio.Count < 2)
        {
            Debug.LogError("Please add at least two photos with audio for the crossfade effect.");
            return;
        }

        image1.sprite = photosWithAudio[currentIndex].photo;
        image2.sprite = photosWithAudio[GetNextPhotoIndex()].photo;

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
        if (audioSource != null && photosWithAudio[currentIndex].audioClip != null)
        {
            audioSource.clip = photosWithAudio[currentIndex].audioClip;
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
        inactiveImage.sprite = photosWithAudio[currentIndex].photo;

        activeImage.DOFade(0f, fadeDuration);
        inactiveImage.DOFade(1f, fadeDuration).OnComplete(() =>
        {
            SwapActiveImages();
            UpdatePhotoCounterText();
        });
    }

    private int GetNextPhotoIndex()
    {
        return (currentIndex + 1) % photosWithAudio.Count;
    }

    private int GetPreviousPhotoIndex()
    {
        return (currentIndex - 1 + photosWithAudio.Count) % photosWithAudio.Count;
    }

    private void SwapActiveImages()
    {
        var temp = activeImage;
        activeImage = inactiveImage;
        inactiveImage = temp;
    }

    private void UpdatePhotoCounterText()
    {
        photoCounterText.text = $"{currentIndex + 1}/{photosWithAudio.Count}";
    }
}