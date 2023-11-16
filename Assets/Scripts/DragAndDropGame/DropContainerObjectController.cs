using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropContainerObjectController : MonoBehaviour
{
    public DragAndDropObjectController dragAndDropObjectController;
    private PolygonCollider2D polygonCollider2D;
    public SpriteRenderer spriteRenderer;
    private RaycastHit2D hit;
    private void Start()
    {
        polygonCollider2D = GetComponent<PolygonCollider2D>();
    }

    private void Update()
    {
        if (polygonCollider2D == null)
        {
            return;
        }
        if (!dragAndDropObjectController.dragAndDropGameProperties.containerFillTransparency)
        {
            if (!Input.GetMouseButton(0))
            {
                polygonCollider2D.enabled = true;
            }
            else
            {
                polygonCollider2D.enabled = false;
            }
        }

    }
    private void LateUpdate()
    {

        if (dragAndDropObjectController.dragAndDropGameProperties.containerFillTransparency)
        {
            if (Input.GetMouseButtonDown(0))
            {
                hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);



                if (hit.collider != null && hit.collider.gameObject == gameObject)
                {
                    if (!dragAndDropObjectController.isLockedBySequence)
                    {
                        dragAndDropObjectController.isCorrect = true;
                        dragAndDropObjectController.dragAndDropGameController.CheckSequence();
                        spriteRenderer.DOFade(1, 0.5f);
                    }
                   
                }
            }
        }

    }
}