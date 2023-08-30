using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AOT;
using UnityEngine.Android;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace TBEasyWebCam
{
    /// <summary>
    /// Frame orientation.
    /// </summary>
    public enum CameraOrientation : int
    {
        /// <summary>
        /// Landscape left.
        /// </summary>
        LandscapeLeft = 0,
        /// <summary>
        /// Portrait.
        /// </summary>
        Portrait = 1,
        /// <summary>
        /// Landscape right.
        /// </summary>
        LandscapeRight = 2,
        /// <summary>
        /// Portrait upside down.
        /// </summary>
        PortraitUpsideDown = 3
    }
    
    public enum CameraMode
    {
        Rear=0,
        Front=1,
        None
    }

    public enum FocusMode
    {
        Off = 0,
        TapToFocus = 1,
        AutoFocus = 2,
        SoftFocus = 4,
        MacroFocus = 8
    }

    public enum ResolutionMode : byte
    {
        HD = 1,
        FullHD = 2,
        HighestResolution = 4,
        MediumResolution = 8,
        LowestResolution = 16,
        CustomResolution = 32
    }

    public enum FlashMode
    {
        Auto,
        On,
        Off
    }

    public enum TorchMode
    {
        Off = 0,
        On
    }

}

public sealed class CameraSetting 
{

    
    public static Task<int> RequestCameraPermissions()
    {
        // Request
        var permissionTask = new TaskCompletionSource<int>();
        if (Application.platform == RuntimePlatform.Android)
        {
            var requester = new GameObject("MediaDeviceQuery Permissions Helper").AddComponent<CameraPermissionHelper>();
            requester.StartCoroutine(RequestAndroid(requester));
        }

        return permissionTask.Task;
        // Define Android request
        IEnumerator RequestAndroid(CameraPermissionHelper requester)
        {
            var permission = Permission.Camera;
            if (!Permission.HasUserAuthorizedPermission(permission))
                Permission.RequestUserPermission(permission);
            yield return new WaitUntil(() => Permission.HasUserAuthorizedPermission(permission));
            permissionTask.SetResult(1);
            MonoBehaviour.Destroy(requester.gameObject);
        }
    }
    
    private sealed class CameraPermissionHelper : MonoBehaviour
    {
        void Awake() => DontDestroyOnLoad(this.gameObject);
    }

}
