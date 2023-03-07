using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class DragAndDropObjectController : MonoBehaviour
{
    public DragAndDropGameProperties dragAndDropGameProperties;
    public DragAndDropObjectProperties dragAndDropObjectProperties;
    public SpriteRenderer spriteRenderer;
    private Vector3 offset;
    private bool dragging = false;
    private RaycastHit2D hit;
    private RaycastHit2D dropHit;
    public LayerMask dropLayerMask;
    private PolygonCollider2D polygonCollider2D;

    private void Start()
    {
        polygonCollider2D = GetComponent<PolygonCollider2D>();
    }


    private void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                OnDragStart();
            }
        }
        Drag();
    }

    private void OnDragStart()
    {
        offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        dragging = true;
    }
    private void OnDragToContainer(DropContainerObjectController container)
    {
        transform.DOMove(container.transform.position, 0.5f);
        transform.DOScale(container.transform.localScale, 0.5f);
        transform.DORotate(container.transform.eulerAngles, 0.5f);
    }

    private void OnDragToWrongContainer()
    {
        transform.DOShakeRotation(dragAndDropGameProperties.onWrongContainerShakeDuration, dragAndDropGameProperties.onWrongContainerShakePower).OnComplete(() => SnapToStartPosition());
       
    }

    private void SnapToStartPosition()
    {
        transform.DOMove(dragAndDropObjectProperties.startPosition, 0.5f);
    }



    private void Drag()
    {
        if (dragging)
        {
            polygonCollider2D.enabled = false;
            Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;
            transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
            if (Input.GetMouseButtonUp(0))
            {

                dragging = false;
                dropHit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, ~dropLayerMask);

                if (polygonCollider2D != null)
                {
                    polygonCollider2D.enabled = true;

                }

                if (dropHit.collider == null)
                {
                    SnapToStartPosition();
                    return;
                }
                if (!dropHit.collider.GetComponent<DropContainerObjectController>())
                {
                    SnapToStartPosition();
                    return;
                }
                DropContainerObjectController container = dropHit.collider.GetComponent<DropContainerObjectController>();
                if (container.dragAndDropObjectController == this)
                {
                    OnDragToContainer(container);
                }
                else
                {
                    OnDragToWrongContainer();
                }
            }
        }
    }

   


}