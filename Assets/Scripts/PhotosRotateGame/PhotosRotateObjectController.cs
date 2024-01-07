using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; // Add this for DOTween

public class PhotosRotateObjectController : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    private bool isRotating = false; // Flag to track if rotation is in progress
    private PhotosRotateGameController gameController;
    public bool canRotate;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        gameController = PhotosRotateGameController.Instance;
    }

    void Update()
    {
        if (!canRotate) return;
        // Check if the left mouse button was clicked and rotation is not in progress
        if (Input.GetMouseButtonDown(0) && !isRotating)
        {
            // Convert mouse position to world position
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Check if the mouse is over the sprite
            if (GetComponent<Collider2D>().OverlapPoint(mousePos))
            {
                // Start rotation and set isRotating to true
                isRotating = true;
                PhotosRotateGameController.Instance.audioSource.PlayOneShot(PhotosRotateGameController.Instance.gameProperties.onPhotoClickAudioClip);
                // Use DOTween to smoothly rotate the sprite by 90 degrees around the Z axis
                transform.DOLocalRotate(new Vector3(0, 0, transform.localEulerAngles.z + 90), gameController.gameProperties.rotateDuration).SetEase(gameController.gameProperties.rotateCurve)
                        .OnComplete(() =>
                        {
                            isRotating = false;
                            CorrectRotation();
                            gameController.CheckGameReady();
                        });
            }
        }
    }

    // Function to correct the rotation to the nearest 90-degree angle
    private void CorrectRotation()
    {
        var zRotation = transform.localEulerAngles.z;
        var correctedRotation = Mathf.Round(zRotation / 90) * 90;
        transform.localEulerAngles = new Vector3(0, 0, correctedRotation);
    }
}