using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PhotoGalleryProperties", menuName = "ScriptableObjects/PhotoGalleryProperties", order = 1)]
public class PhotoGalleryProperties : ScriptableObject
{
    [SerializeField, LabelText("Tło")]
    public Sprite background;

 
    public List<PhotoWithAudio> photoWithAudios = new List<PhotoWithAudio>();


    public bool useAudio;

    public Sprite Background => background;
    public IReadOnlyList<PhotoWithAudio> PhotoWithAudios => photoWithAudios;
    public bool UseAudio => useAudio;
}

[System.Serializable]
public class PhotoWithAudio
{
    public Sprite photo;
    public AudioClip audioClip;
}