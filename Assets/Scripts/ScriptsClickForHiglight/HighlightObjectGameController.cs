using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HighlightObjectGameController : MonoBehaviour
{
    public HighlightObjectGameScriptable highlightObjectGameScriptable;
    public HighlightObjectController highlightObjectPrefab;
    private List<HighlightObjectController> spawnedObjects = new List<HighlightObjectController>();
    public Action objectClickedAction;
    private bool objectWasClicked;
    private AudioSource audioSource;
    public GameObject GameEndScreen;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        SetupGame();
        objectClickedAction += ObjectClicked;
    }
    private void ObjectClicked()
    {
        objectWasClicked = true;
    }





    [Button]
    private void SetupGame()
    {

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
        yield return new WaitForSeconds(1);
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
                            audioSource.PlayOneShot(gameSequence.audioClipOnSingleSequenceStart);
                        }
                        spawnedObject.HiglightObject();

                    }




                    //    if (spawnedObject.higlightObjectProperites.id == objectsToHighlight.ObjectID)
                    //    {

                    //        if (gameSequence.waitForClick)
                    //        {
                    //            if (objectsToHighlight.shouldPlayAudio)
                    //            {
                    //                spawnedObject.PlayAudio();
                    //            }
                    //            spawnedObject.HiglightObject();


                    //        }

                    //        if(!gameSequence.waitForClick)
                    //        spawnedObject.Play(objectsToHighlight.shouldPlayAudio);
                    //    }
                    //}
                    //yield return new WaitUntil(() => spawnedObject.WasClicked);
                }


            }
            if (gameSequence.waitForClick)
            {
                yield return new WaitUntil(() => objectWasClicked);
                objectWasClicked = false;
                if (gameSequence.audioClipOnClick)
                {
                    audioSource.PlayOneShot(gameSequence.audioClipOnClick);
                }
            }
         


            yield return new WaitForSeconds(gameSequence.SequenceTime);
            foreach (HighlightObjectController spawnedObject in spawnedObjects)
            {
                spawnedObject.DeselectObject();
                spawnedObject.shouldCheckClickedAction = false;
                spawnedObject.WasClicked = false;
            }
        }

        GameEndScreen.SetActive(true);
    }
}
