using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class HighlightObjectGameController : MonoBehaviour
{
    public HighlightObjectGameScriptable highlightObjectGameScriptable;
    public HighlightObjectController highlightObjectPrefab;
    private List<HighlightObjectController> spawnedObjects = new List<HighlightObjectController>();
    public Action objectClickedAction;
    private bool objectWasClicked;
    private AudioSource audioSource;
    public GameObject GameEndScreen;
    [SerializeField] private SpriteRenderer background;
    [SerializeField] private AudioSource musicController;
    [SerializeField] private GameCanvasController gameCanvasController;
    public AudioClip tempAudioClipOnSingleSequenceStart;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        SetupGame();
        objectClickedAction += ObjectClicked;
        background.sprite = highlightObjectGameScriptable.backGroundTexture;
    }

    private void SetupMusic()
    {
        musicController.clip = highlightObjectGameScriptable.gameMusic;
        musicController.loop = true;
        musicController.Play();
        musicController.PlayOneShot(highlightObjectGameScriptable.gameCommandAudioClip);
    }
    private void ObjectClicked()
    {
        Debug.Log("ObjectClicked triggered");
        objectWasClicked = true;
    }


    private IEnumerator ResetClickFlag()
    {
        yield return null; // Wait for the next frame
        objectWasClicked = false; // Reset the flag
    }


    [Button]
    private void SetupGame()
    {
        if (GameManager.Instance.currentGameProperties is HighlightObjectGameScriptable)
            highlightObjectGameScriptable = GameManager.Instance.currentGameProperties as HighlightObjectGameScriptable;
        SetupMusic();

        int index = 0;
        foreach (HiglightObject higlightObject in highlightObjectGameScriptable.higlightObjects)
        {
            HighlightObjectController spawnedObject = Instantiate(highlightObjectPrefab);
            spawnedObject.transform.position = higlightObject.position;
            spawnedObject._texture = higlightObject.texture;
            spawnedObject.transform.localScale = higlightObject.scale;
            spawnedObject.index = index;
            spawnedObject.higlightObjectProperites = higlightObject;
            spawnedObject.highlightObjectGameController = this;
            spawnedObject.SetupSprite();
            spawnedObjects.Add(spawnedObject);
            index++;
        }
        StartSequence();
    }

    private void StartSequence()
    {
        StartCoroutine(GameSequence());
    }

    IEnumerator GameSequence()
    {
        yield return new WaitForSeconds(highlightObjectGameScriptable.gameCommandAudioClip.length + 0.5f);

        foreach (HighlightGameSequence gameSequence in highlightObjectGameScriptable.gameSequence)
        {
            foreach (HighlightObjectController spawnedObject in spawnedObjects)
            {
                spawnedObject.isLocked = true;
            }

            foreach (HighlightObjectController spawnedObject in spawnedObjects)
            {
                foreach (ObjectsToHighlight objectsToHighlight in gameSequence.objectsToHighlights)
                {
                    if (spawnedObject.higlightObjectProperites.id == objectsToHighlight.ObjectID)
                    {
                        if (gameSequence.waitForClick)
                        {
                            spawnedObject.isLocked = false;
                            spawnedObject.shouldCheckClickedAction = true;
                        }

                        if (gameSequence.audioClipOnSingleSequenceStart != null)
                        {
                            tempAudioClipOnSingleSequenceStart = gameSequence.audioClipOnSingleSequenceStart;
                            audioSource.PlayOneShot(gameSequence.audioClipOnSingleSequenceStart);
                        }

                        spawnedObject.HiglightObject();
                    }
                }
            }

            if (gameSequence.waitForClick)
            {
                Debug.Log("Waiting for object to be clicked...");
                yield return new WaitUntil(() => objectWasClicked); // Wait for object to be clicked
                objectWasClicked = false; // Reset flag after click

                // Find the clicked object
                HighlightObjectController clickedObject = spawnedObjects.Find(obj => obj.WasClicked);

                if (clickedObject != null && clickedObject.audioSource.isPlaying)
                {
                    Debug.Log($"Waiting for {clickedObject.name}'s audio to finish...");
                    yield return new WaitUntil(() => !clickedObject.audioSource.isPlaying); // Wait for the audio to stop
                }

                if (gameSequence.audioClipOnClick)
                {
                    audioSource.PlayOneShot(gameSequence.audioClipOnClick);
                    yield return new WaitUntil(() => !audioSource.isPlaying); // Wait for game sequence audio to finish
                }
            }

            yield return new WaitForSeconds(gameSequence.SequenceTime);

            foreach (HighlightObjectController spawnedObject in spawnedObjects)
            {
                spawnedObject.isLocked = true;
                spawnedObject.DeselectObject();
                spawnedObject.shouldCheckClickedAction = false;
                spawnedObject.WasClicked = false;
            }
        }

        if (GameManager.Instance.CheckNextGameExist())
        {
            gameCanvasController.MaskScreen(true);
            GameManager.Instance.OpenNextGame();
        }
        else
        {
            GameEndScreen.SetActive(true);
        }
    }


    public void BackToChoseLevels()
    {
        SceneManager.LoadScene(PlayerPrefs.GetString("LastChoseGameScene"));
    }
    public void Question()
    {
    if(tempAudioClipOnSingleSequenceStart != null)
        audioSource.PlayOneShot(tempAudioClipOnSingleSequenceStart);
    }
   
}
