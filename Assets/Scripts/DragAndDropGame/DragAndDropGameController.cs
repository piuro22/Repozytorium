using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class DragAndDropGameController : MonoBehaviour
{
    public DragAndDropGameProperties dragAndDropGameProperties;
    public GameObject dragAndDropObjectPrefab;
    public GameObject dropContainerPrefab;




    private void Start()
    {
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
           
            if (dragAndDrop.alternativeTargetTexture != null)
            {
                Texture2D dropContainerTexture = duplicateTexture(dragAndDrop.alternativeTargetTexture);
                dropContainerObject.GetComponent<SpriteRenderer>().sprite = Sprite.Create(dropContainerTexture, new Rect(0.0f, 0.0f, dropContainerTexture.width, dropContainerTexture.height), new Vector2(0.5f, 0.5f), 100.0f);

            }
            else
            {
                dropContainerObject.GetComponent<SpriteRenderer>().sprite = Sprite.Create(dragTexture, new Rect(0.0f, 0.0f, dragTexture.width, dragTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
                dropContainerObject.GetComponent<SpriteRenderer>().color = Color.black;
            }
            dropContainerObject.dragAndDropObjectController = dragObject;
            dropContainerObject.gameObject.AddComponent<PolygonCollider2D>();
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