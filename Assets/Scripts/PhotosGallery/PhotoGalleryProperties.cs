using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "PhotoGalleryProperties", menuName = "ScriptableObjects/PhotoGalleryProperties", order = 1)]
public class PhotoGalleryProperties : ScriptableObject
{
    public List<PhotoWithAudio> photoWithAudios = new List<PhotoWithAudio>();
    public bool useAudio;
}

[System.Serializable]
public class PhotoWithAudio
{
    public Sprite photo;
    public AudioClip audioClip;
}