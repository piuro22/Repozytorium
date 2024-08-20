#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
#endif
using UnityEngine;

public class ScreenRotation : MonoBehaviour
{
    [System.Serializable]
    public enum RotationType
    {
        Portrait,
        LandscapeLeft,
        LandscapeRight,
        PortraitUpsideDown
    }

    public RotationType rotationType = RotationType.Portrait;
    private float delayInSeconds = 0.1f;

    private void Start()
    {
        Invoke("ApplyResolution", delayInSeconds);
    }

    private void ApplyResolution()
    {
        // Example resolutions for a typical Android device
        int width = 1080;
        int height = 1920;

        switch (rotationType)
        {
            case RotationType.Portrait:
#if UNITY_EDITOR
                GameViewUtils.AddAndSelectCustomSize(GameViewUtils.GameViewSizeType.FixedResolution, GameViewSizeGroupType.Standalone, height, width, "1080x1920 Portrait");
#endif
                Screen.orientation = ScreenOrientation.Portrait;
                break;
            case RotationType.LandscapeLeft:
            case RotationType.LandscapeRight:
#if UNITY_EDITOR
                GameViewUtils.AddAndSelectCustomSize(GameViewUtils.GameViewSizeType.FixedResolution, GameViewSizeGroupType.Standalone, width, height, "1920x1080 Landscape");
#endif
                Screen.orientation = rotationType == RotationType.LandscapeLeft
                    ? ScreenOrientation.LandscapeLeft
                    : ScreenOrientation.LandscapeRight;
                break;
            case RotationType.PortraitUpsideDown:
#if UNITY_EDITOR
                GameViewUtils.AddAndSelectCustomSize(GameViewUtils.GameViewSizeType.FixedResolution, GameViewSizeGroupType.Standalone, height, width, "1080x1920 Portrait Upside Down");
#endif
                Screen.orientation = ScreenOrientation.PortraitUpsideDown;
                break;
        }

        Debug.Log($"Resolution set to {Screen.width}x{Screen.height} in {rotationType} mode.");
    }
}
