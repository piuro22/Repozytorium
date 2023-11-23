using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class MemoryGameController : MonoBehaviour
{
    public static MemoryGameController Instance { get; private set; }

    public MemoryGameProperties memoryGameProperties;

    [SerializeField] private Transform cardsParrent3x2;
    [SerializeField] private Transform cardsParrent4x2;
    [SerializeField] private Transform cardsParrent4x3;
    [SerializeField] private Transform cardsParrent5x2;
    [SerializeField] private Transform cardsParrent4x4;
    [SerializeField] private SpriteRenderer background;
    [SerializeField] private GameFinishScreen gameFinishScreen;
    private MemoryGameCardController _firstRevealCard;
    private MemoryGameCardController _secondRevealCard;
    private float _collectedPoints;
    private int requiedPointsToWinGame;
    private Sequence inCorrectRevealSequence;

    private List<MemoryGameCardController> memoryGameCardControllers3x2 = new List<MemoryGameCardController>();
    private List<MemoryGameCardController> memoryGameCardControllers4x2 = new List<MemoryGameCardController>();
    private List<MemoryGameCardController> memoryGameCardControllers4x3 = new List<MemoryGameCardController>();
    private List<MemoryGameCardController> memoryGameCardControllers5x2 = new List<MemoryGameCardController>();
    private List<MemoryGameCardController> memoryGameCardControllers4x4 = new List<MemoryGameCardController>();
    private AudioSource audioSource;
    [SerializeField] private AudioSource musicController;
    [SerializeField] private GameCanvasController gameCanvasController;
    public TMP_Text messageText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        Initialize();
    }

    private void SetupMusic()
    {
        musicController.clip = memoryGameProperties.gameMusic;
        musicController.loop = true;
        musicController.Play();
        musicController.PlayOneShot(memoryGameProperties.gameCommandAudioClip);

    }

    private void Initialize()
    {

        audioSource = GetComponent<AudioSource>();
        foreach (Transform child in cardsParrent3x2)
        {
            memoryGameCardControllers3x2.Add(child.GetComponent<MemoryGameCardController>());
        }
        cardsParrent3x2.gameObject.SetActive(false);

        foreach (Transform child in cardsParrent4x2)
        {
            memoryGameCardControllers4x2.Add(child.GetComponent<MemoryGameCardController>());
        }
        cardsParrent4x2.gameObject.SetActive(false);

        foreach (Transform child in cardsParrent4x3)
        {
            memoryGameCardControllers4x3.Add(child.GetComponent<MemoryGameCardController>());
        }
        cardsParrent4x3.gameObject.SetActive(false);

        foreach (Transform child in cardsParrent5x2)
        {
            memoryGameCardControllers5x2.Add(child.GetComponent<MemoryGameCardController>());
        }
        cardsParrent5x2.gameObject.SetActive(false);

        foreach (Transform child in cardsParrent4x4)
        {
            memoryGameCardControllers4x4.Add(child.GetComponent<MemoryGameCardController>());
        }
        cardsParrent4x4.gameObject.SetActive(false);

        SetupMemoryGame();

    }




    private void SetupMemoryGame()
    {
        if (GameManager.Instance.currentGameProperties is MemoryGameProperties)
            memoryGameProperties = GameManager.Instance.currentGameProperties as MemoryGameProperties;
        SetupMusic();
        switch (memoryGameProperties.memorySize)
        {
            case MemorySize.S3x2:
                CardIntialize(memoryGameCardControllers3x2, cardsParrent3x2);
                break;

            case MemorySize.S4x2:
                CardIntialize(memoryGameCardControllers4x2, cardsParrent4x2);
                break;

            case MemorySize.S4x3:
                CardIntialize(memoryGameCardControllers4x3, cardsParrent4x3);
                break;

            case MemorySize.S5x2:
                CardIntialize(memoryGameCardControllers5x2, cardsParrent5x2);
                break;
            case MemorySize.S4x4:
                CardIntialize(memoryGameCardControllers4x4, cardsParrent4x4);
                break;
        }
    }

    private void CardIntialize(List<MemoryGameCardController> cardControllers, Transform parrent)
    {
        int i = 0;
        parrent.gameObject.SetActive(true);

        ShuffleList(cardControllers);

        foreach (MemoryCardData memoryCardData in memoryGameProperties.GetMemoryCardsData(memoryGameProperties.memorySize))
        {
            cardControllers[i].Initialize(memoryGameProperties.cardBackTexture, memoryGameProperties.cardFrontTexture, memoryCardData);
            i++;
            cardControllers[i].Initialize(memoryGameProperties.cardBackTexture, memoryGameProperties.cardFrontTexture, memoryCardData);
            i++;
        }
        requiedPointsToWinGame = memoryGameProperties.GetMemoryCardsData(memoryGameProperties.memorySize).Count;
        background.sprite = memoryGameProperties.backGroundTexture;

    }

    private void ShuffleList<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }


    public void RevealCard(MemoryGameCardController card)
    {
        if (_firstRevealCard == null)
        {
            _firstRevealCard = card;
            audioSource.PlayOneShot(memoryGameProperties.OnFirstCardRevealSound);
           
        }
        else if (_secondRevealCard == null)
        {
            _secondRevealCard = card;
        }
        if (_firstRevealCard != null && _secondRevealCard != null)
        {
            if (CheckRevealedCards())
            {
                OnCorrectReveal();
            }
            else
            {
                OnIncorrectReveal();
            }
        }
    }

    private void OnCorrectReveal()
    {
        _firstRevealCard = null;
        _secondRevealCard = null;
        _collectedPoints++;

        if (CheckForFinish())
        {
            OnGameFinished();
        }
        else
        {
            audioSource.PlayOneShot(memoryGameProperties.OnRevealCorrectSound);
            messageText.text = "";
        }
    }

    private void OnIncorrectReveal()
    {
        if (inCorrectRevealSequence != null)
        {
            inCorrectRevealSequence.Kill();
        }
        audioSource.PlayOneShot(memoryGameProperties.OnRevealInCorrectSound);
        inCorrectRevealSequence = DOTween.Sequence();
        inCorrectRevealSequence.AppendInterval(memoryGameProperties.waitTimeOnIncorrectReveal);
        inCorrectRevealSequence.AppendCallback(() =>
        {
            _firstRevealCard.HideCard();
            _secondRevealCard.HideCard();
            _firstRevealCard = null;
            _secondRevealCard = null;
            Debug.Log("Hide");
        }).OnComplete(() =>
        {
            inCorrectRevealSequence.Kill();
            messageText.text = "";

            });
    }


    private void OnGameFinished()
    {
        audioSource.PlayOneShot(memoryGameProperties.OnGameFinishedSound);
        if (GameManager.Instance.CheckNextGameExist())
        {
            gameCanvasController.MaskScreen(true);
            GameManager.Instance.OpenNextGame();
        }
        else
        {
            gameFinishScreen.gameObject.SetActive(true);
        }
    }




    public bool CheckSequencesCompleted()
    {
        if (inCorrectRevealSequence == null)
            return true;
        if (inCorrectRevealSequence.IsPlaying())
            return false;
        else
            return true;

    }

    private bool CheckForFinish()
    {
        if (_collectedPoints == requiedPointsToWinGame)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool CheckRevealedCards()
    {
        if (_firstRevealCard.targetImage == _secondRevealCard.targetImage)
        {
            return true;
        }
        if (_firstRevealCard.alternativeImage != null)
        {
            if (_firstRevealCard.alternativeImage == _secondRevealCard.targetImage)
            {
                return true;
            }
        }
        if (_secondRevealCard.alternativeImage != null)
        {
            if (_secondRevealCard.alternativeImage == _firstRevealCard.targetImage)
            {
                return true;
            }
        }
        return false;
    }

    public void BackToChoseLevels()
    {
        // SceneManager.LoadScene("Scene Level Change");
        SceneManager.LoadScene(PlayerPrefs.GetString("LastChoseGameScene"));
    }

}
