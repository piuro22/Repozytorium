/// <summary>
/// write by 52cwalk,if you have some question ,please contract lycwalk@gmail.com
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TBEasyWebCam;
using System.Runtime.InteropServices;
using System;

public class DeviceCameraController : MonoBehaviour {

    public DeviceCamera dWebCam
    {
        get
        { 
            return webcam;
        }
    }

    private DeviceCamera webcam;
    public RawImage previewImage;
    public AspectRatioFitter previewAspectFitter;
    
    public bool isUseEasyWebCam = true;
    CameraMode cameraMode = CameraMode.Rear;
    public bool isPlaying
    {
        get{
            if (webcam != null) {
                return webcam.isPlaying ();
            } else {
                return false;
            }
        }
    }
    
    void Start()
    {
        if (previewImage == null)
        {
            Debug.LogError("Error: Preview Image Can't Be Null ");
            return;
        }
        webcam = new DeviceCamera (isUseEasyWebCam);
        EasyWebCam.OnEasyWebCamStart += PreviewStart;
        StartWork();
    }
    
    void PreviewStart()
    {

        if (previewImage != null && webcam != null)
        {
            previewImage.texture = webcam.preview;
            previewAspectFitter.aspectRatio = webcam.Width() * 1.0f / (float)webcam.Height();
#if UNITY_ANDROID||UNITY_IOS
            Vector3 scale = previewImage.gameObject.GetComponent<RectTransform>().localScale;
            if (this.cameraMode == CameraMode.Front)
            {
                scale.Set(-1 * scale.x, scale.y, scale.z);
            }
            else
            {

                scale.Set(Math.Abs( scale.x ), scale.y, scale.z);
            }
            previewImage.gameObject.GetComponent<RectTransform>().localScale = scale;
#endif
        }
    }
    

    // Update is called once per frame  
    void Update()  
    {
        
    }
    
    /// <summary>
    /// start the work.
    /// </summary>
    public void StartWork()
    {
        if (this.webcam != null) {
            this.webcam.Play ();
        }
    }

    /// <summary>
    /// Stops the work.
    /// when you need to leave current scene ,you must call this func firstly
    /// </summary>
    public void StopWork()
    {
        if (this.webcam != null && isPlaying)
        {
            this.webcam.Stop();
        }
        if (previewImage != null)
        {
            previewImage.texture = null;
        }
    }

    public void tapFocus()
    {
        EasyWebCam.tapFocus();
    }
    
    public void swithCamera()
    {
        cameraMode = cameraMode == CameraMode.Rear ? CameraMode.Front : CameraMode.Rear;
        EasyWebCam.SwitchCamera(cameraMode);
    }
    
    bool isTorchOn = false;
    public void toggleTorch()
    {
       isTorchOn = !isTorchOn;
       EasyWebCam.setTorchMode(isTorchOn? TorchMode.On: TorchMode.Off);
    }
    
}


