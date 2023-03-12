using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class GameCanvasController : MonoBehaviour
{
    public CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
        MaskScreen(false);


    }

    public void MaskScreen(bool state)
    {
        if (state == true)
        {
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
            canvasGroup.DOFade(1, 1);
        }
        else
        {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
            canvasGroup.DOFade(0, 1);
        }
    }
}
