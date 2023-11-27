using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }


    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (Application.isPlaying)
        {
            if (GameManager.Instance != null)
                if (GameManager.Instance.currentGameProperties is QuizPicturesProperties)
                    gameProperties = GameManager.Instance.currentGameProperties as QuizPicturesProperties;
        }

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

}
