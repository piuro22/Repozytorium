using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using EPOOutline;
using UnityEngine.UIElements;

public class DragAndDropObjectController : MonoBehaviour
{
    public DragAndDropGameProperties dragAndDropGameProperties;
    public DragAndDropObjectProperties dragAndDropObjectProperties;
    public DragAndDropGameController dragAndDropGameController;
    public DropContainerObjectController dropContainerObjectController;
    public Outlinable outlinable;
    public SpriteRenderer spriteRenderer;
    private Vector3 offset;
    private bool dragging = false;
    private RaycastHit2D hit;
    private RaycastHit2D dropHit;
    public LayerMask dropLayerMask;
    private PolygonCollider2D polygonCollider2D;
    public int id;
    public bool isLockedBySequence = false;
    public bool isCorrect = false;
    private Sequence highlightSequence;
   
    private void Start()
    {
        polygonCollider2D = GetComponent<PolygonCollider2D>();
        outlinable.OutlineParameters.BlurShift = 0;
        outlinable.OutlineParameters.DilateShift = 0;
        outlinable.OutlineParameters.Color = dragAndDropGameProperties.onWrongContainerOutlineColor;
        
    }


    private void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {


            hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);



            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {

                if (dragAndDropGameProperties.containerFillTransparency)
                {
                    if (isLockedBySequence)
                    {
                        OnStartDragWrongObject();
                        return;
                    }
                    else
                    {
                        dropContainerObjectController.spriteRenderer.DOFade(1, 0.5f);
                        dragAndDropGameController.audioSource.PlayOneShot(dragAndDropGameProperties.onGoodContainerAudioClip);
                        isCorrect = true;
                        dragAndDropGameController.CheckSequence();
                        return;
                    }
                }
                else
                {





                    if (!dragAndDropGameProperties.snapOnClick)
                    {
                        if (isLockedBySequence)
                        {
                            OnStartDragWrongObject();
                            return;
                        }
                        else
                        {
                            OnDragStart();
                            return;
                        }



                    }
                    else
                    {
                        if (isLockedBySequence)
                        {
                            OnStartDragWrongObject();
                            return;
                        }

                        if (dragAndDropGameProperties.useOtherTextureForContainer == true)
                        {
                            transform.DOMove(dropContainerObjectController.transform.position + (Vector3)dragAndDropObjectProperties.additionalOffset, dragAndDropGameProperties.snapToContainterTime);
                            transform.DOScale(dragAndDropObjectProperties.endScaleObjectIfAlternativeTexture, dragAndDropGameProperties.snapToContainterTime);
                            transform.DORotate(new Vector3(0, 0, dragAndDropObjectProperties.endRotationObjectIfAlternativeTexture), dragAndDropGameProperties.snapToContainterTime).OnComplete(() => dragAndDropGameController.CheckSequence());
                        }
                        else
                        {
                            transform.DOScale(dropContainerObjectController.transform.localScale, dragAndDropGameProperties.snapToContainterTime);
                            transform.DOMove(dropContainerObjectController.transform.position, dragAndDropGameProperties.snapToContainterTime);
                            transform.DORotate(dropContainerObjectController.transform.eulerAngles, dragAndDropGameProperties.snapToContainterTime).OnComplete(() => dragAndDropGameController.CheckSequence());
                        }
                        dragAndDropGameController.audioSource.PlayOneShot(dragAndDropGameProperties.onGoodContainerAudioClip);
                        polygonCollider2D.enabled = false;
                        isCorrect = true;
                    }
                }
            }

        }
        Drag();
    }

    private void OnDragStart()
    {
        offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        dragging = true;
    }
    private void OnDragToCorrectContainer(DropContainerObjectController container)
    {

        if (dragAndDropGameProperties.useOtherTextureForContainer == true)
        {
            transform.DOMove(container.transform.position + (Vector3)dragAndDropObjectProperties.additionalOffset, dragAndDropGameProperties.snapToContainterTime);
            transform.DOScale(dragAndDropObjectProperties.endScaleObjectIfAlternativeTexture, dragAndDropGameProperties.snapToContainterTime);
            transform.DORotate(new Vector3(0, 0, dragAndDropObjectProperties.endRotationObjectIfAlternativeTexture), dragAndDropGameProperties.snapToContainterTime).OnComplete(() => dragAndDropGameController.CheckSequence());
        }
        else
        {
            transform.DOScale(container.transform.localScale, dragAndDropGameProperties.snapToContainterTime);
            transform.DOMove(container.transform.position, dragAndDropGameProperties.snapToContainterTime);
            transform.DORotate(container.transform.eulerAngles, dragAndDropGameProperties.snapToContainterTime).OnComplete(() => dragAndDropGameController.CheckSequence());
        }



        dragAndDropGameController.audioSource.PlayOneShot(dragAndDropGameProperties.onGoodContainerAudioClip);
        polygonCollider2D.enabled = false;
        isCorrect = true;


    }

    private void OnDragToWrongContainer()
    {

        transform.DOShakeRotation(dragAndDropGameProperties.onWrongContainerShakeDuration, dragAndDropGameProperties.onWrongContainerShakePower).OnComplete(() => SnapToStartPosition());
        dragAndDropGameController.audioSource.PlayOneShot(dragAndDropGameProperties.onWrongContainerAudioClip);
        HighlightObject();
    }

    private void SnapToStartPosition()
    {
        transform.DOMove(dragAndDropObjectProperties.startPosition, 0.5f);
        HighlightObject();
    }

    private void OnStartDragWrongObject()
    {
        transform.DOShakeRotation(dragAndDropGameProperties.onWrongContainerShakeDuration, dragAndDropGameProperties.onWrongContainerShakePower);
        dragAndDropGameController.audioSource.PlayOneShot(dragAndDropGameProperties.onWrongObjectAudioClip);
    }

    public void HighlightObject()
    {
        if (highlightSequence != null) highlightSequence.Kill();
        highlightSequence = DOTween.Sequence();
        highlightSequence.AppendCallback(() => outlinable.enabled = true);
        highlightSequence.Append(DOTween.To(() => outlinable.OutlineParameters.DilateShift, x => outlinable.OutlineParameters.DilateShift = x, dragAndDropGameProperties.onWrongContainerOutlineWidth, dragAndDropGameProperties.onWrongContainerOutlineFadeTime));
        highlightSequence.Join(DOTween.To(() => outlinable.OutlineParameters.BlurShift, x => outlinable.OutlineParameters.BlurShift = x, dragAndDropGameProperties.onWrongContainerOutlineWidth, dragAndDropGameProperties.onWrongContainerOutlineFadeTime));
        highlightSequence.Append(DOTween.To(() => outlinable.OutlineParameters.DilateShift, x => outlinable.OutlineParameters.DilateShift = x, 0, dragAndDropGameProperties.onWrongContainerOutlineFadeTime));
        highlightSequence.Join(DOTween.To(() => outlinable.OutlineParameters.BlurShift, x => outlinable.OutlineParameters.BlurShift = x, 0, dragAndDropGameProperties.onWrongContainerOutlineFadeTime));
        highlightSequence.AppendCallback(() => outlinable.enabled = false);
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
                    OnDragToCorrectContainer(container);
                }
                else
                {
                    OnDragToWrongContainer();
                }
            }
        }
    }




}