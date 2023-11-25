using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "PhotosRotateGameProperties", menuName = "ScriptableObjects/PhotosRotateGameProperties", order = 1)]
public class PhotosRotateGameProperties : ScriptableObject
{
    [LabelText("Siatka")]
    public PhotosRotateGameType grid;
    [LabelText("Obrazki")]
    public List<Texture2D> images;
    [LabelText("Szanse na rotację ustawione od 0-1")]
    public List<AngleChance> angleChances;
    [LabelText("Przerwa między poleceniem gry a pojawieniem się kostek")]
    public float delayBetweenCommandAndInitializeGrid;
    [LabelText("Dźwięk na skończoną grę")]
    public AudioClip soundOnEndGame;

    [BoxGroup("Animacja przy Pojawieniu się obrazka")]
    [LabelText("Czas trwania animacji")]
    public float scaleDuration = 0.25f;




    [BoxGroup("Animacja przy kliknięciu")]
    [LabelText("Czas trwania obrotu")]
    public float rotateDuration = 0.25f;

    [BoxGroup("Animacja przy kliknięciu")]
    [LabelText("Krzywa animacji obrotu")]
    public AnimationCurve rotateCurve;

    [BoxGroup("Animacja przy kliknięciu")]
    [LabelText("Dźwięk przy kliknięciu")]
    public AudioClip onPhotoClickAudioClip;







    [BoxGroup("Ogólne")]
    [LabelText("Tło")]
    public Texture2D background;


    [BoxGroup("Ogólne")]
    [LabelText("Muzyka gry")]
    public AudioClip gameMusic;

    [BoxGroup("Ogólne")]
    [LabelText("Dźwięk polecenia do gry")]
    public AudioClip gameCommandAudioClip;

    [BoxGroup("Ogólne")]
    [LabelText("Polecenie w formie tekstu")]
    public string commandText;


}


[System.Serializable]
public struct AngleChance
{
    [LabelText("Kąt")]
    public int angle;
    [LabelText("Szansa (ustawić między 0-1)")]
    public float chance; // Chance should be a value between 0 and 1
}


public enum PhotosRotateGameType
{
    grid2x2,
    grid2x3,
    grid3x3,
    grid3x4,
    grid4x4,
}