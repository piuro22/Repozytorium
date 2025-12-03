using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuizPucturesGameController : MonoBehaviour
{
    public static QuizPucturesGameController Instance { get; private set; }

    public QuizPicturesProperties gameProperties;
    public List<QuizPicturesObjectController> quizPicturesObjectControllers;

    public AudioSource audioSource;
    public AudioSource voiceAudioSource;

    public int badAnswered;
    public int goodAnswered;

    public GameObject finishScreen;
    public TMP_Text questionText;

    private int currentQuizIndex;
    public SpriteRenderer background;

    public AudioClip QuizAudioClipOnSingleSequenceStart;

    // ⭐ szablon 4 ramek z SCENY
    public GameObject templateUI;

    // ⭐ blokada kliknięć
    public bool canClick = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    private void Start()
    {
        StartCoroutine(StartSequence());
    }

    // 🔥 Główna sekwencja startowa
    private IEnumerator StartSequence()
    {
        Initialize();  // obrazki pokazują się od razu

        // 🔊 1. Polecenie gry
        if (gameProperties.gameCommandAudioClip != null)
        {
            canClick = false; // 🚫 blokada kliknięć podczas polecenia

            (voiceAudioSource ?? audioSource)
                .PlayOneShot(gameProperties.gameCommandAudioClip);

            yield return new WaitForSeconds(gameProperties.gameCommandAudioClip.length);
        }

        // 🔊 2. dźwięk pytania
        if (QuizAudioClipOnSingleSequenceStart != null)
        {
            canClick = false; // nadal zablokowane
            audioSource.PlayOneShot(QuizAudioClipOnSingleSequenceStart);

            yield return new WaitForSeconds(QuizAudioClipOnSingleSequenceStart.length);
        }

        // 🔓 3. Teraz można klikać w ramki
        canClick = true;
    }

    private void Initialize()
    {
        if (Application.isPlaying)
        {
            if (GameManager.Instance != null)
                if (GameManager.Instance.currentGameProperties is QuizPicturesProperties)
                    gameProperties = GameManager.Instance.currentGameProperties as QuizPicturesProperties;
        }

        background.sprite = gameProperties.background;

        // ⭐ wyłączamy szablon sceny (4 ramki)
        if (templateUI != null)
            templateUI.SetActive(false);

        if (gameProperties.enableShuffle)
            ShuffleQuestions();

        SetupQuizPart(0);
    }

    private void ShuffleQuestions()
    {
        for (int i = 0; i < gameProperties.questions.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, gameProperties.questions.Count);
            var temp = gameProperties.questions[i];
            gameProperties.questions[i] = gameProperties.questions[randomIndex];
            gameProperties.questions[randomIndex] = temp;
        }
    }

    public void ResetPictures()
    {
        foreach (var pictureObject in quizPicturesObjectControllers)
            pictureObject.gameObject.SetActive(false);
    }

    public void SetupQuizPart(int index)
    {
        ResetPictures();

        QuizPicturesQuestionProperties quiz = gameProperties.questions[index];

        // ⭐ NIE odtwarzamy tutaj dźwięku pytania — tylko zapamiętujemy
        QuizAudioClipOnSingleSequenceStart = quiz.questionClip;

        questionText.text = quiz.questionText;

        for (int i = 0; i < quiz.singleQuizPictures.Count; i++)
        {
            var obj = quizPicturesObjectControllers[i];

            obj.transform.localScale = Vector3.zero;
            obj.transform.DOScale(Vector3.one, 0.25f);

            obj.gameObject.SetActive(true);
            obj.SingleQuizPicture = quiz.singleQuizPictures[i];
            obj.quizPucturesGameController = this;
            obj.SetupPicture();
        }
    }

    public void OnGoodAnswer()
    {
        goodAnswered++;

        canClick = false;  // ✋ blokujemy kliknięcia na czas dźwięku

        if (currentQuizIndex < gameProperties.questions.Count - 1)
        {
            currentQuizIndex++;
            SetupQuizPart(currentQuizIndex);

            // dźwięk nowego pytania
            if (QuizAudioClipOnSingleSequenceStart != null)
            {
                audioSource.PlayOneShot(QuizAudioClipOnSingleSequenceStart);
                StartCoroutine(UnlockAfter(QuizAudioClipOnSingleSequenceStart.length));
            }
        }
        else FinishGame();
    }

    public void OnBadAnswer()
    {
        badAnswered++;
    }

    public void FinishGame()
    {
        finishScreen.SetActive(true);
    }

    public void BackToChoseLevels()
    {
        SceneManager.LoadScene(PlayerPrefs.GetString("LastChoseGameScene"));
    }

    public void QuizQuestion()
    {
        if (QuizAudioClipOnSingleSequenceStart != null)
            audioSource.PlayOneShot(QuizAudioClipOnSingleSequenceStart);
    }

    // 🔓 Odblokowanie kliknięć po czasie
    private IEnumerator UnlockAfter(float time)
    {
        yield return new WaitForSeconds(time);
        canClick = true;
    }
}





/*using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class QuizPucturesGameController : MonoBehaviour
{
    public static QuizPucturesGameController Instance { get; private set; }
    public QuizPicturesProperties gameProperties;
    public List<QuizPicturesObjectController> quizPicturesObjectControllers;
    public AudioSource audioSource;
    public int badAnswered;
    public int goodAnswered;
    public GameObject finishScreen;
    public TMP_Text questionText;
    //  public List<QuizTwoPicturesObjectController> quizTwoPicturesObjectControllers = new List<QuizTwoPicturesObjectController>();
    private int currentQuizIndex;
    public SpriteRenderer background;
    public AudioClip QuizAudioClipOnSingleSequenceStart;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }


    private void Start()
    {
        Initialize();
    }
    private void ShuffleQuestions()
    {
        for (int i = 0; i < gameProperties.questions.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, gameProperties.questions.Count);
            var temp = gameProperties.questions[i];
            gameProperties.questions[i] = gameProperties.questions[randomIndex];
            gameProperties.questions[randomIndex] = temp;
        }
        Debug.Log("Pytania zostały przetasowane.");
    }
    /*
    private void Initialize()
    {
        if (Application.isPlaying)
        {
            if (GameManager.Instance != null)
                if (GameManager.Instance.currentGameProperties is QuizPicturesProperties)
                    gameProperties = GameManager.Instance.currentGameProperties as QuizPicturesProperties;
        }
        background.sprite = gameProperties.background;
        SetupQuizPart(0);
    }*/
/* private void Initialize()
 {
     if (Application.isPlaying)
     {
         if (GameManager.Instance != null)
             if (GameManager.Instance.currentGameProperties is QuizPicturesProperties)
                 gameProperties = GameManager.Instance.currentGameProperties as QuizPicturesProperties;
     }

     background.sprite = gameProperties.background;

     // Przetasuj pytania
     ShuffleQuestions();

     // Rozpocznij od pierwszego pytania
     SetupQuizPart(0);
 }

 public void ResetPictures()
 {
     foreach (QuizPicturesObjectController pictureObject in quizPicturesObjectControllers)
     {
         pictureObject.gameObject.SetActive(false);
     }
 }


 public void SetupQuizPart(int index)
 {
     ResetPictures();

     QuizPicturesQuestionProperties quizPicturesQuestionProperties = gameProperties.questions[index];

     for (int i = 0; i < quizPicturesQuestionProperties.singleQuizPictures.Count; i++)
     {
         audioSource.PlayOneShot(quizPicturesQuestionProperties.questionClip);
         QuizAudioClipOnSingleSequenceStart = quizPicturesQuestionProperties.questionClip;

         questionText.text = quizPicturesQuestionProperties.questionText;
         quizPicturesObjectControllers[i].transform.localScale = Vector3.zero;
         quizPicturesObjectControllers[i].transform.DOScale(Vector3.one, 0.25f);
         quizPicturesObjectControllers[i].gameObject.SetActive(true);
         quizPicturesObjectControllers[i].SingleQuizPicture = quizPicturesQuestionProperties.singleQuizPictures[i];
         quizPicturesObjectControllers[i].quizPucturesGameController = this;
         quizPicturesObjectControllers[i].SetupPicture();

     }
 }



 public void OnGoodAnswer()
 {
     goodAnswered++;
     if (currentQuizIndex< gameProperties.questions.Count-1)
     {
         currentQuizIndex++;
         SetupQuizPart(currentQuizIndex);
         Debug.Log("Next game");
     }

     else
     {
         FinishGame();
     }
 }
 public void OnBadAnswer()
 {
     badAnswered++;
 }
 public void FinishGame()
 {
     finishScreen.SetActive(true);
 }
 public void BackToChoseLevels()
 {
     SceneManager.LoadScene(PlayerPrefs.GetString("LastChoseGameScene"));
 }
 public void QuizQuestion()
 {
     if (QuizAudioClipOnSingleSequenceStart != null)
         audioSource.PlayOneShot(QuizAudioClipOnSingleSequenceStart);
 }

}
*/