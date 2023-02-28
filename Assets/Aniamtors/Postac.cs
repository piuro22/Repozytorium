using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Postac : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private AudioClip jumpClip;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }


    public void PlayVoice()
    {
        audioSource.PlayOneShot(jumpClip);
    }
}
