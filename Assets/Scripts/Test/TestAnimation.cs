using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class TestAnimation : MonoBehaviour
{
    [SerializeField] private float shakePower;
    [SerializeField] private float shakeDouration;

    private Sequence testAnimation;


    void OnMouseDown()
    {
        OnClick();
    }

    private void OnClick()
    {
        DestroySequence();
        testAnimation = DOTween.Sequence();
        testAnimation.Append(transform.DOShakePosition(shakeDouration, shakePower));
    }


    private void DestroySequence()
    {
        if (testAnimation != null)
        {
            testAnimation.Kill();
        }
    }
}
