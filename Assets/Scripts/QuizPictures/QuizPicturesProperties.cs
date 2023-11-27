using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuizPicuresProperties", menuName = "ScriptableObjects/QuizPicuresProperties", order = 1)]

public class QuizPicturesProperties : ScriptableObject
{
    [LabelText("Muzyka")]
    public AudioClip musicClip;
    [LabelText("Dźwięk zła odpowiedź")]
    public AudioClip badAnswerSound;
    [LabelText("Dźwięk dobra odpowiedź")]
    public AudioClip goodAnswerSound;


    public List<QuizPicturesQuestionProperties> questions;

}
[Serializable]
public class QuizPicturesQuestionProperties
{
    [LabelText("Pojedyncze obrazki")]
    public List<SingleQuizPictures> singleQuizPictures = new List<SingleQuizPictures>();
    [LabelText("pytanie w formie tekstu")]
    public string questionText;
    [LabelText("pytanie w formie dźwiękowej")]
    public AudioClip questionClip;

}

[Serializable]
public class SingleQuizPictures
{
    [LabelText("poprawna odpowiedź")]
    public bool isCorrect = false;
    [LabelText("obrazek")]
    public Sprite picture;
    [LabelText("Dodatkowy dźwięk obrazka")]
    public AudioClip additionalPictureSound;
    [LabelText("Rozmiar obrazka")]
    public Vector2 pictureSize;
}