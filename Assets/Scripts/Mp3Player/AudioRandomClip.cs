using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioRandomClip : MonoBehaviour
{
    void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }
    public List<AudioClip> audioClips;
    public float repeatInterval = 5.0f; // Time interval between repetitions
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private bool shouldPlay = false;

    private void Update()
    {
        if (shouldPlay && !audioSource.isPlaying)
        {
            PlayRandomAudioClip();
        }
    }

    private void PlayRandomAudioClip()
    {
        int randomIndex = Random.Range(0, audioClips.Count);
        AudioClip randomClip = audioClips[randomIndex];
        audioSource.clip = randomClip;
        audioSource.Play();
    }
}