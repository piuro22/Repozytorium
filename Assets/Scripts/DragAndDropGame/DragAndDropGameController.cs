using DrawTextureExt;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class DragAndDropGameController : MonoBehaviour
{
    public static DragAndDropGameController Instance { get; private set; }

    public DragAndDropGameProperties dragAndDropGameProperties;
    public GameObject dragAndDropObjectPrefab;
    public GameObject dropContainerPrefab;
    public AudioSource audioSource;
    public List<DragAndDropObjectController> dragAndDropObjectControllers = new List<DragAndDropObjectController>();
    private int currentSequenceStep;
    public TMP_Text messageText;
    [SerializeField] private GameFinishScreen gameFinishScreen;
    [SerializeField] private GameCanvasController gameCanvasController;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
        Initialize();
    }

    [Button]
    private void Initialize()
    {
        if (Application.isPlaying)
        {
            if (GameManager.Instance.currentGameProperties is DragAndDropGameProperties)
                dragAndDropGameProperties = GameManager.Instance.currentGameProperties as DragAndDropGameProperties;
        }

        audioSource.PlayOneShot(dragAndDropGameProperties.gameCommandAudioClip);
        messageText.text = dragAndDropGameProperties.commandText;
        gameFinishScreen.gameObject.SetActive(false);
        foreach (DragAndDropObjectProperties dragAndDrop in dragAndDropGameProperties.objects)
        {
            DragAndDropObjectController dragObject = Instantiate(dragAndDropObjectPrefab.GetComponent<DragAndDropObjectController>());

            if (dragAndDrop.useRandomPositionForDragObjects)
            {
                dragObject.transform.position = new Vector3(
                 Random.Range(dragAndDrop.startPositionMinMaxX.x, dragAndDrop.startPositionMinMaxX.y),
                 Random.Range(dragAndDrop.startPositionMinMaxY.x, dragAndDrop.startPositionMinMaxY.y), 0);

            }
            else
            {
                dragObject.transform.position = dragAndDrop.startPosition;
            }

            if (dragAndDrop.useRandomRotationForDragObjects)
            {
                dragObject.transform.eulerAngles = new Vector3(0, 0, Random.Range(dragAndDrop.rotationMinMaxAngle.x, dragAndDrop.rotationMinMaxAngle.y));

            }
            else
            {
                dragObject.transform.eulerAngles = new Vector3(0, 0, dragAndDrop.rotationAngle);
            }

            dragObject.transform.localScale = dragAndDrop.scale;

            Texture2D dragTexture = duplicateTexture(dragAndDrop.texture);
            dragObject.GetComponent<SpriteRenderer>().sprite = Sprite.Create(dragTexture, new Rect(0.0f, 0.0f, dragTexture.width, dragTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
            dragObject.gameObject.AddComponent<PolygonCollider2D>();
            dragObject.name = dragAndDrop.texture.name;
            dragObject.dragAndDropGameProperties = dragAndDropGameProperties;
            dragObject.dragAndDropObjectProperties = dragAndDrop;
            dragObject.dragAndDropGameController = this;
            dragObject.id = dragAndDrop.id;
            dragAndDropObjectControllers.Add(dragObject);
            DropContainerObjectController dropContainerObject = Instantiate(dropContainerPrefab.GetComponent<DropContainerObjectController>());
            if (dragAndDrop.useRandomPositionForContainerObjects)
            {
                dropContainerObject.transform.position = new Vector3(
                 Random.Range(dragAndDrop.targetPositionMinMaxX.x, dragAndDrop.targetPositionMinMaxX.y),
                 Random.Range(dragAndDrop.targetPositionMinMaxY.x, dragAndDrop.targetPositionMinMaxY.y), 0);
            }
            else
            {
                dropContainerObject.transform.position = dragAndDrop.targetPosition;
            }

            if (dragAndDrop.useRandomRotationForContainerObjects)
            {
                dropContainerObject.transform.eulerAngles = new Vector3(0, 0, Random.Range(dragAndDrop.targetRotationMinMaxAngle.x, dragAndDrop.targetRotationMinMaxAngle.y));
            }
            else
            {
                dropContainerObject.transform.localEulerAngles = new Vector3(0, 0, dragAndDrop.targetRotationAngle);
            }
            dropContainerObject.transform.localScale = dragAndDrop.targetScale;

            if (dragAndDropGameProperties.useOtherTextureForContainer)
            {
                Texture2D dropContainerTexture = duplicateTexture(dragAndDrop.alternativeTargetTexture);
                dropContainerObject.GetComponent<SpriteRenderer>().sprite = Sprite.Create(dropContainerTexture, new Rect(0.0f, 0.0f, dropContainerTexture.width, dropContainerTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
                dropContainerObject.spriteRenderer.color = new Color(1, 1, 1, dragAndDropGameProperties.containerTransparency);

            }
            else
            {
                dropContainerObject.spriteRenderer.sprite = Sprite.Create(dragTexture, new Rect(0.0f, 0.0f, dragTexture.width, dragTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
               
            }
            if (dragAndDropGameProperties.overrideContainerColor)
            {
                dropContainerObject.spriteRenderer.material.EnableKeyword("HITEFFECT_ON");
                dropContainerObject.spriteRenderer.material.SetColor("_HitEffectColor", dragAndDropGameProperties.containerColor);
            }
            else
            {
                dropContainerObject.spriteRenderer.material.DisableKeyword("HITEFFECT_ON");
            }
            dropContainerObject.spriteRenderer.color = new Color(1, 1, 1, dragAndDropGameProperties.containerTransparency);



            dropContainerObject.dragAndDropObjectController = dragObject;
            dropContainerObject.gameObject.AddComponent<PolygonCollider2D>();
        }

        if (dragAndDropGameProperties.useSequence)
        {
            foreach (DragAndDropObjectController dragAndDropObjectController in dragAndDropObjectControllers)
            {
                dragAndDropObjectController.isLockedBySequence = true;
            }
        }
        else
        {
            foreach (DragAndDropObjectController dragAndDropObjectController in dragAndDropObjectControllers)
            {
                dragAndDropObjectController.isLockedBySequence = false;
            }
        }
        StartCoroutine(WaitForCommandEnd());
    }

    IEnumerator WaitForCommandEnd()
    {
        yield return new WaitForSeconds(dragAndDropGameProperties.gameCommandAudioClip.length);
        CheckSequence();

    }


    public void CheckSequence()
    {
        if (dragAndDropGameProperties.useSequence)
        {
            foreach (DragAndDropObjectController dragAndDropObjectController in dragAndDropObjectControllers)
            {
                if (dragAndDropGameProperties.dragAndDropGameSequences.Count > currentSequenceStep)
                {
                    if (dragAndDropObjectController.id == dragAndDropGameProperties.dragAndDropGameSequences[currentSequenceStep].objectID)
                    {
                        dragAndDropObjectController.isLockedBySequence = false;
                    }
                    else
                    {
                        dragAndDropObjectController.isLockedBySequence = true;
                        messageText.text = dragAndDropGameProperties.dragAndDropGameSequences[currentSequenceStep].textMessage;
                        audioSource.PlayOneShot(dragAndDropGameProperties.dragAndDropGameSequences[currentSequenceStep].dialogOnSequenceStart);
                    }
                }
            }
            currentSequenceStep++;

        }
        int correctDrags = 0;
        foreach (DragAndDropObjectController dragAndDropObjectController in dragAndDropObjectControllers)
        {
            if (dragAndDropObjectController.isCorrect)
            {
                correctDrags++;
            }
            if(correctDrags == dragAndDropObjectControllers.Count)
            {

                if (GameManager.Instance.CheckNextGameExist())
                {
                    gameCanvasController.MaskScreen(true);
                    GameManager.Instance.OpenNextGame();
                }
                else
                {
                    gameFinishScreen.gameObject.SetActive(true);
                }
            }
        }
    }

    Texture2D duplicateTexture(Texture2D source)
    {
        RenderTexture renderTex = RenderTexture.GetTemporary(
                    source.width,
                    source.height,
                    0,
                    RenderTextureFormat.ARGB32,
                    RenderTextureReadWrite.Linear);

        Graphics.Blit(source, renderTex);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        Texture2D readableText = new Texture2D(source.width, source.height);
        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableText.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);
        return readableText;

     
    }
}
