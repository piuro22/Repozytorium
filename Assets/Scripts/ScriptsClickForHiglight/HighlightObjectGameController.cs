using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public void ShuffleSequence()
    {
        // Loguj zawartoœæ listy przed tasowaniem
        Debug.Log("Przed tasowaniem: " + string.Join(", ", highlightObjectGameScriptable.higlightObjects.Select(obj => obj.id)));

        for (int i = 0; i < highlightObjectGameScriptable.higlightObjects.Count; i++)
        {
            int rand = UnityEngine.Random.Range(i, highlightObjectGameScriptable.higlightObjects.Count);
            var temp = highlightObjectGameScriptable.higlightObjects[i];
            highlightObjectGameScriptable.higlightObjects[i] = highlightObjectGameScriptable.higlightObjects[rand];
            highlightObjectGameScriptable.higlightObjects[rand] = temp;
        }

        // Loguj zawartoœæ listy po tasowaniu
        Debug.Log("Po tasowaniu: " + string.Join(", ", highlightObjectGameScriptable.higlightObjects.Select(obj => obj.id)));

        Debug.Log("Lista higlightObjects zosta³a przetasowana.");
    }

    //copailot 2
    /*  public void ShuffleSequence()
      {
          var list = highlightObjectGameScriptable.higlightObjects;
          for (int i = 0; i < list.Count; i++)
          {
              int rand = UnityEngine.Random.Range(i, list.Count); // Losuj indeks od i do koñca listy
              var temp = list[i];
              list[i] = list[rand];
              list[rand] = temp; // Zamieñ elementy miejscami
          }

          Debug.Log("Lista higlightObjects zosta³a przetasowana.");
      }*/
    //copailot

    /*private void SetupMusic()
     {
         musicController.clip = highlightObjectGameScriptable.gameMusic;
         musicController.loop = true;
         musicController.Play();
         musicController.PlayOneShot(highlightObjectGameScriptable.gameCommandAudioClip);
     }*/

    //copailot
    private IEnumerator SetupMusic()
    {
        // Zablokuj interakcje dla wszystkich obiektów
        foreach (var spawnedObject in spawnedObjects)
        {
            spawnedObject.isLocked = true;
        }

        // Odtwórz dŸwiêk polecenia gry, jeœli istnieje
        if (highlightObjectGameScriptable.gameCommandAudioClip != null)
        {
            audioSource.PlayOneShot(highlightObjectGameScriptable.gameCommandAudioClip);

            // Poczekaj, a¿ dŸwiêk polecenia siê skoñczy
            yield return new WaitWhile(() => audioSource.isPlaying);
        }

        // Odtwórz muzykê w tle
        musicController.clip = highlightObjectGameScriptable.gameMusic;
        musicController.loop = true;
        musicController.Play();

        // Odblokuj interakcje dla wszystkich obiektów
        foreach (var spawnedObject in spawnedObjects)
        {
            spawnedObject.isLocked = false;
        }
    }
    //copailot
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
        Debug.Log("SetupGame zosta³o wywo³ane.");
        if (GameManager.Instance.currentGameProperties is HighlightObjectGameScriptable)
            highlightObjectGameScriptable = GameManager.Instance.currentGameProperties as HighlightObjectGameScriptable;
        ShuffleSequence(); // Przetasuj listê higlightObjects copailot 2
        SetupMusic();

        int index = 0;
        /* foreach (HiglightObject higlightObject in highlightObjectGameScriptable.higlightObjects)
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
         }*/
        foreach (HiglightObject higlightObject in highlightObjectGameScriptable.higlightObjects) //copailot
        {
            HighlightObjectController spawnedObject = Instantiate(highlightObjectPrefab);
            spawnedObject.transform.position = higlightObject.position;
            spawnedObject._texture = higlightObject.texture;
            spawnedObject.transform.localScale = higlightObject.scale;
            spawnedObject.index = higlightObject.id;
            spawnedObject.higlightObjectProperites = higlightObject;
            spawnedObject.highlightObjectGameController = this;
            spawnedObject.SetupSprite();
            spawnedObjects.Add(spawnedObject);
        }
        StartCoroutine(SetupMusic()); // Uruchom coroutine dla muzyki i dŸwiêku polecenia
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
