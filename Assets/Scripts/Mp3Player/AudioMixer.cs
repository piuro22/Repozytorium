using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMixer : MonoBehaviour
{
    public List<AudioProperties> audioSources = new List<AudioProperties>();
    public AnimationCurve fadeCurve; // Krzywa kontrolująca przygłoszenie/wyciszenie
    public float fadeDuration = 1.0f; // Czas trwania przygłoszenia/wyciszenia

    private Coroutine currentFadeCoroutine; // Aktualnie działający proces przygłoszenia/wyciszenia

    public void Start()
    {
        foreach (AudioProperties audioSource in audioSources)
        {
            audioSource.audioSource.clip = audioSource.audioClip;
        }
    }

    // Rozpocznij proces przygłoszenia/wyciszenia
    public void StartFade(bool fadeIn, AudioSource targetSource)
    {
        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
        }

        currentFadeCoroutine = StartCoroutine(FadeAudio(fadeIn, targetSource));
    }

    // Coroutine realizujący przygłoszanie/wyciszanie
    private IEnumerator FadeAudio(bool fadeIn, AudioSource targetSource)
    {
        float timer = 0f;
        float startVolume = targetSource.volume;
        float endVolume = fadeIn ? 1.0f : 0.0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeDuration;
            float curveValue = fadeCurve.Evaluate(t);

            targetSource.volume = Mathf.Lerp(startVolume, endVolume, curveValue);
            yield return null;
        }

        targetSource.volume = endVolume;
        currentFadeCoroutine = null;
    }

    // Klasa opisująca właściwości źródła dźwięku
    [System.Serializable]
    public class AudioProperties
    {
        public AudioSource audioSource;
        public AudioClip audioClip;
    }
}


