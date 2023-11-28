using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotosRotateGameController : MonoBehaviour
{
    public static PhotosRotateGameController Instance { get; private set; }


    public PhotosRotateGameProperties gameProperties;
    public SpriteRenderer background;
    public TMP_Text textCommand;
    public SpriteGrid grid2x2;
    public SpriteGrid grid2x3;
    public SpriteGrid grid3x3;
    public SpriteGrid grid3x4;
    public SpriteGrid grid4x4;
    public GameObject endPanel;
    public AudioSource musicAudioSource;

    public AudioSource audioSource;
    private SpriteGrid currentUsingGrid;
    private Dictionary<SpriteRenderer, Vector3> originalScales = new Dictionary<SpriteRenderer, Vector3>();
    private DG.Tweening.Sequence endGameSequence;
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }
    private void Start()
    {
        Initialize();
        DOTween.Init();
    }

    private void Initialize()
    {

        if (Application.isPlaying)
        {
            if(GameManager.Instance!=null)
            if (GameManager.Instance.currentGameProperties is PhotosRotateGameProperties)
                gameProperties = GameManager.Instance.currentGameProperties as PhotosRotateGameProperties;
        }
        musicAudioSource.clip = gameProperties.gameMusic;
        musicAudioSource.Play();
        audioSource = GetComponent<AudioSource>();
        if (gameProperties.gameCommandAudioClip != null)
        {
            audioSource.PlayOneShot(gameProperties.gameCommandAudioClip);
        }
        background.sprite = Sprite.Create(gameProperties.background, new Rect(0.0f, 0.0f, gameProperties.background.width, gameProperties.background.height), new Vector2(0.5f, 0.5f),100f);
        textCommand.text = gameProperties.commandText;
        DeactivateAllGrids();
        StartCoroutine(ActivateGridCoroutine());
    
    }

    IEnumerator ActivateGridCoroutine()
    {
        yield return new WaitForSeconds(gameProperties.delayBetweenCommandAndInitializeGrid);
        textCommand.text = "";
        switch (gameProperties.grid)
        {
            case PhotosRotateGameType.grid2x2:
                ActivateGrid(grid2x2);
                break;
            case PhotosRotateGameType.grid2x3:
                ActivateGrid(grid2x3);
                break;
            case PhotosRotateGameType.grid3x3:
                ActivateGrid(grid3x3);
                break;
            case PhotosRotateGameType.grid3x4:
                ActivateGrid(grid3x4);
                break;
            case PhotosRotateGameType.grid4x4:
                ActivateGrid(grid4x4);
                break;
            default:
                Debug.LogError("Unsupported game type");
                break;
        }
    }

    private void DeactivateAllGrids()
    {
        grid2x2.SetActive(false);
        grid2x3.SetActive(false);
        grid3x3.SetActive(false);
        grid3x4.SetActive(false);
        grid4x4.SetActive(false);
    }
    private void ActivateGrid(SpriteGrid grid)
    {
        currentUsingGrid = grid;
        grid.SetActive(true);
        SetSpritesImages(grid.photosRotateObjectControllers);
    }

    private void SetSpritesImages(List<PhotosRotateObjectController> photorRotateObjectController)
    {


        if (gameProperties.images.Count < photorRotateObjectController.Count)
        {
            Debug.LogError("Not enough images for the selected grid size.");
            return;
        }

        for (int i = 0; i < photorRotateObjectController.Count; i++)
        {
            // Store original scale
            originalScales[photorRotateObjectController[i].spriteRenderer] = photorRotateObjectController[i].transform.localScale;

            Sprite previousSprite = photorRotateObjectController[i].spriteRenderer.sprite;

            // Create the new sprite
            Sprite newSprite = gameProperties.images[i];

            // Assign the new sprite to the sprite renderer
            photorRotateObjectController[i].spriteRenderer.sprite = newSprite;

            // Calculate the scale factor required to maintain the size of the sprite renderer
            float widthScale = 1f;
            float heightScale = 1f;

            if (previousSprite != null)
            {
                widthScale = previousSprite.bounds.size.x / newSprite.bounds.size.x;
                heightScale = previousSprite.bounds.size.y / newSprite.bounds.size.y;
            }

            // Set initial scale to zero
            photorRotateObjectController[i].transform.localScale = Vector3.zero;

            // Rotate immediately
            photorRotateObjectController[i].transform.eulerAngles = new Vector3(0, 0, RandomRotate());
            photorRotateObjectController[i].AddComponent<BoxCollider2D>();
            // Create a tween for this sprite with a random delay
            float randomDelay = UnityEngine.Random.Range(0f, 0.5f); // Random delay, adjust max value as needed
            Vector3 originalScale = originalScales[photorRotateObjectController[i].spriteRenderer];
            Vector3 targetScale = new Vector3(widthScale * originalScale.x, heightScale * originalScale.y, originalScale.z);
            photorRotateObjectController[i].transform.DOScale(targetScale, gameProperties.scaleDuration)
                .SetEase(Ease.OutBack)
                .SetDelay(randomDelay); // Set the random start delay
        }
    }


    public int RandomRotate()
    {
        float total = 0;
        foreach (var angleChance in gameProperties.angleChances)
        {
            total += angleChance.chance;
        }

        float randomPoint = UnityEngine.Random.value * total;

        foreach (var angleChance in gameProperties.angleChances)
        {
            if (randomPoint < angleChance.chance)
                return angleChance.angle;
            else
                randomPoint -= angleChance.chance;
        }

        return 0;
    }

    public void CheckGameReady()
    {
        foreach(PhotosRotateObjectController photo in currentUsingGrid.photosRotateObjectControllers)
        {
            if (photo.transform.localEulerAngles.z != 0)
            {
                Debug.Log($"{photo.gameObject.name} rotation = {photo.transform.localEulerAngles.z}", photo.gameObject );
                return;
            }
              
        }
        OnGameReady();
    }

    public void OnGameReady()
    {

        if (endGameSequence != null) endGameSequence.Kill();
        endGameSequence = DOTween.Sequence();
        endGameSequence.AppendCallback(() => audioSource.PlayOneShot(gameProperties.soundOnEndGame));
        endGameSequence.AppendInterval(gameProperties.soundOnEndGame.length);
        endGameSequence.AppendCallback(() => { endPanel.SetActive(true); });

    }
    public void BackToChoseLevels()
    {
        // SceneManager.LoadScene("Scene Level Change");
        SceneManager.LoadScene(PlayerPrefs.GetString("LastChoseGameScene"));
    }

    [System.Serializable]
    public class SpriteGrid
    {
        public GameObject parentObject;
        public List<PhotosRotateObjectController> photosRotateObjectControllers;

        public void SetActive(bool active)
        {
            parentObject.SetActive(active);
        }
    }
}


