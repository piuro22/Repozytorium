using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PhotoGalleryProperties", menuName = "ScriptableObjects/PhotoGalleryProperties", order = 1)]
public class PhotoGalleryProperties : ScriptableObject
{
    [SerializeField, LabelText("Tło")]
    public Sprite background;
    [SerializeField] public bool shouldPlayAudioAutomatically;

    public List<PhotoWithAudio> photoWithAudios = new List<PhotoWithAudio>();


    public bool useAudio;
    public bool useScreenClick;

    public Sprite Background => background;
    public IReadOnlyList<PhotoWithAudio> PhotoWithAudios => photoWithAudios;
    public bool UseAudio => useAudio;

    [InfoBox("Muzyka gry")]
    public AudioClip gameMusic;
    [LabelText("Dźwięk polecenia do gry")]
    public AudioClip gameCommandAudioClip;
}


[System.Serializable]
public class PhotoWithAudio
{
    public Sprite photo;
    public AudioClip audioClip;
}