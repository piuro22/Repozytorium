using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenRotation : MonoBehaviour
{
    [Serializable]
    public enum RotationType
    {
        Portrait,
        Landscape,
        PortraitUpsideDown,
        LandscapeRight,
        LandscapeLeft
    }

    public RotationType rotationType = RotationType.Portrait;

    private void Start()
    {
        ApplyRotation();
    }

    private void ApplyRotation()
    {
        switch (rotationType)
        {
            case RotationType.Portrait:
                Screen.orientation = ScreenOrientation.Portrait;
                break;
            case RotationType.PortraitUpsideDown:
                Screen.orientation = ScreenOrientation.PortraitUpsideDown;
                break;
            case RotationType.LandscapeRight:
                Screen.orientation = ScreenOrientation.LandscapeRight;
                break;
            case RotationType.LandscapeLeft:
                Screen.orientation = ScreenOrientation.LandscapeLeft;
                break;
        }
    }

    public void SetRotation(RotationType newRotationType)
    {
        rotationType = newRotationType;
        ApplyRotation();
    }

    // Overloaded method to be called from UnityEvent
    public void SetRotation(int newRotationType)
    {
        rotationType = (RotationType)newRotationType;
        ApplyRotation();
    }
}
