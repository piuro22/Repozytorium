using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    public Animator cubeAnimator;



    [Button]
    public void StartRotate()
    {
        cubeAnimator.SetTrigger("StartPlay");
    }


    [Button]
    public void StartJump()
    {
        cubeAnimator.SetTrigger("Jump");
    }
}
