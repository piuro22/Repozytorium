using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;
using TBEasyWebCam;
using System.Threading.Tasks;

public class EasyWebCamUnity : IEasyWebCam
{
    int mPreviewWidth;
    int mPreviewHeight;
    bool mIsPlaying = false;

    Texture2D mPreview;
    int orientation = 0;
    WebCamTexture unityWebCamera;

    string deviceName;
    bool isActive = false;
    int cameraWidth = 0;
    int cameraHeight = 0;
    int cameraMode = 0;
    bool isRunning = false;
    int platform = 0;//0:无平台，1：android平台，2：iOS平台
    public Texture2D WebCamPreview {
        get
        {
            if (this.mPreview == null)
            {
                TextureFormat format = TextureFormat.RGBA32;
                this.mPreview = new Texture2D(this.mPreviewWidth, this.mPreviewHeight, format, false);
            }
            return this.mPreview;
        }
    }

    public int previewWidth {
        get {
            return mPreviewWidth;
        }
        set{
            mPreviewWidth = value;
        }
    }

    public int previewHeight
    {
        get
        {
            return mPreviewHeight;
        }
        set{
            mPreviewHeight = value;
        }
    }
    
    public bool IsPlaying {
        get
        {
            return mIsPlaying;
        }
    }

    public EasyWebCamUnity(int cm)
    {
        this.platform = cm;
        if (cm == 1)
        {
            NativePlugin.RegisterCallbacks(EasyWebCamBase.EasyWebCamStarted, EasyWebCamBase.EasyWebCamUpdate, EasyWebCamBase.EasyWebCamStoped);
        }
        else if(cm == 2)
        {
            NativePluginStatic.RegisterCallbacks(EasyWebCamBase.EasyWebCamStarted, EasyWebCamBase.EasyWebCamUpdate, EasyWebCamBase.EasyWebCamStoped);
        }
    }

    public int getEnterFrame()
    {
        return 0;
    }

    public void Init()
    {
        
    }

    public bool isCameraUsable()
    {
        return true;
    }

    public void OnPause(bool paused)
    {
        
    }

    public async void Play()
    {
        if(!isActive)
        {
            int waitAcitve = await waitToolActive();
        }
        
        int waited = await waitCameraInit();

        if (waited > 0)
        {
            if (unityWebCamera != null)
            {
                unityWebCamera.Play();
                mIsPlaying = true;
                isRunning = true;
                //  EasyWebCam.EasyWebCamStarted();
            }
        }
    }

    IEnumerator waitimeToPlay()
    {
        yield return new WaitForEndOfFrame();
        while(!isActive)
        {
            yield return new WaitForEndOfFrame();
        }
    }

    public void Stop()
    {
        if (unityWebCamera != null)
        {
            unityWebCamera.Stop();
            mIsPlaying = false;
            isRunning = false;
            if (this.platform == 1) {
                NativePlugin.stopUnityCamera();
            }
            else if (this.platform == 2) {
                NativePluginStatic.stopUnityCamera();
            }
        }
    }
    
    public void Release()
    {
        if (this.platform == 1)
        {
            NativePlugin.Release();
        }
        else if (this.platform == 2)
        {
            NativePluginStatic.Release();
        }
        }

    public void setFlashMode(int paramode)
    {
        
    }

    public void setFocusMode(int paramode)
    {
        unityWebCamera.autoFocusPoint = new Vector2(0.5f, 0.5f);
    }

    public void setOrientation(int paramode)
    {
        orientation = paramode;
    }

    public void setPreviewResolution(int width, int height)
    {
        cameraWidth = width;
        cameraHeight = height;
        this.initResolution();
    }
    

    async  void initResolution()
    {
        GetDeviceName(this.cameraMode);
#if UNITY_ANDROID
        unityWebCamera = new WebCamTexture(this.deviceName, cameraWidth, cameraHeight);
        unityWebCamera.Play();
#elif UNITY_IOS
        int iosCameraResult = await requestIosCamera();
        if (iosCameraResult > 0)
        {
            
        }
        else
        {
            return;
        }
#endif

        int waited = await waitCameraInit();
        if (waited > 0)
        {
            
        }

        if (this.platform == 1)
        {
            NativePlugin.setUnityResolution(cameraWidth, cameraHeight);
        }
        else if (this.platform == 2)
        {
            NativePluginStatic.setUnityResolution(cameraWidth, cameraHeight);
        }

        if (orientation % 2 != 0)
        {
            mPreviewWidth = unityWebCamera.height > unityWebCamera.width ? unityWebCamera.width : unityWebCamera.height;
            mPreviewHeight = unityWebCamera.width > unityWebCamera.height ? unityWebCamera.width : unityWebCamera.height;
        }
        else
        {
            mPreviewWidth = unityWebCamera.width > unityWebCamera.height ? unityWebCamera.width : unityWebCamera.height;
            mPreviewHeight = unityWebCamera.height > unityWebCamera.width ? unityWebCamera.width : unityWebCamera.height;
        }

        TextureFormat format = TextureFormat.RGBA32;
        this.mPreview = new Texture2D(this.mPreviewWidth, this.mPreviewHeight, format, false);
    }

    Task<int> requestIosCamera()
    {
        var permissionTask = new TaskCompletionSource<int>();
        var requester = new GameObject("Ios Camera Permissions Helper").AddComponent<IosCameraPermissionHelper>();
        requester.StartCoroutine(callIosCamera());

        return permissionTask.Task;

        // Define Android request

        IEnumerator callIosCamera()
        {
            yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
            if (Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                unityWebCamera = new WebCamTexture(this.deviceName, cameraWidth, cameraHeight);
                unityWebCamera.Play();

                permissionTask.SetResult(1);
                Debug.Log(deviceName);
            }
            else
            {
                permissionTask.SetResult(-1);
            }
        }

    }

    Task<int> waitCameraInit()
    {
           // Request
        var permissionTask = new TaskCompletionSource<int>();
        var requester = new GameObject("MediaDeviceQuery Permissions Helper").AddComponent<CameraPermissionHelper>();
        requester.StartCoroutine(RequestInit());

        return permissionTask.Task;

        // Define Android request
        IEnumerator RequestInit()
        {
            while(!isActive)
            {
                yield return new WaitForEndOfFrame();
                if(unityWebCamera.width>100)
                {
                    unityWebCamera.Stop();
                    isActive = true;
                    yield return new WaitForEndOfFrame();
                }
            }
            permissionTask.SetResult(1);
            MonoBehaviour.Destroy(requester.gameObject);
        }
    }


    Task<int> waitToolActive()
    {
        var permissionTask = new TaskCompletionSource<int>();
        var requester = new GameObject("Tool ActivePermissions Helper").AddComponent<ToolActiveCameraPermissionHelper>();
        requester.StartCoroutine(waitActive());

        return permissionTask.Task;

        // Define Android request

        IEnumerator waitActive()
        {
            float ptime = Time.time;

            while (!isActive)
            {
                yield return new WaitForEndOfFrame();
                float ctime = Time.time;
                if(ctime-ptime>500)
                {
                    permissionTask.SetResult(-1);
                }
            }
            permissionTask.SetResult(1);
        }

    }


    private sealed class CameraPermissionHelper : MonoBehaviour
    {
      //  void Awake() => DontDestroyOnLoad(this.gameObject);
    }

    private sealed class IosCameraPermissionHelper : MonoBehaviour
    {
        //  void Awake() => DontDestroyOnLoad(this.gameObject);
    }

    private sealed class ToolActiveCameraPermissionHelper : MonoBehaviour
    {
        //  void Awake() => DontDestroyOnLoad(this.gameObject);
    }


    public void setTorchMode(bool paramode)
    {
        
    }
    
    public void SwitchCamera(int mode)
    {
        if(this.cameraMode == mode)
        {
            return;
        }
        
        bool isSwitchEnable = GetDeviceName(mode);
        if(!isSwitchEnable)
        {
            return;
        }
        mIsPlaying = false;
        isActive = false;
        unityWebCamera.Stop();
        
        if (this.platform == 1)
        {
            NativePlugin.stopUnityCamera();
        }
        else if (this.platform == 2)
        {
            NativePluginStatic.stopUnityCamera();
        }

        unityWebCamera = null;
        this.cameraMode = mode;
       
        unityWebCamera = new WebCamTexture(this.deviceName, cameraWidth, cameraHeight);
        unityWebCamera.Play();
        if(isRunning)
        {
            this.Play();
        }
    }

    public void TakePhoto()
    {
       
    }

    public void tapFocus()
    {
        unityWebCamera.autoFocusPoint = new Vector2(0.5f, 0.5f);
    }

    public void UpdateImage()
    {
        if(unityWebCamera!=null && mIsPlaying)
        {
            Color32[] pixels32 = unityWebCamera.GetPixels32();
          
            if (this.platform == 1)
            {
                NativePlugin.setUnityCameraOrientation(unityWebCamera.videoRotationAngle);
            }
            else if (this.platform == 2)
            {
                NativePluginStatic.setUnityCameraOrientation(unityWebCamera.videoRotationAngle);
            }
            convertUntiyTexture(pixels32);
            
            mPreview.SetPixels32(pixels32);
            mPreview.Apply();
        }
        
    }

    void convertUntiyTexture([In, Out]Color32[] colors32)
    {
        GCHandle handle = GCHandle.Alloc(colors32, GCHandleType.Pinned);
        IntPtr address = handle.AddrOfPinnedObject();
        if (this.platform == 1) {
            NativePlugin.convertUnityPixels32(address);
        }
        else if (this.platform == 2) {
            NativePluginStatic.convertUnityPixels32(address);
        }

            handle.Free();
        }

    public bool GetDeviceName(int cmode)
    {
        if (cmode == (int)CameraMode.Front)
        {
            int i = 0;
            for (i = 0; i != WebCamTexture.devices.Length; i++)
            {
                if (WebCamTexture.devices[i].isFrontFacing)
                {
                    this.deviceName = WebCamTexture.devices[i].name;
                    break;
                }
            }
            if (i == WebCamTexture.devices.Length)
            {
                return false;
            }
        }
        else
        {

            if (WebCamTexture.devices.Length < 1)
            {
                return false;
            }
            this.deviceName = WebCamTexture.devices[0].name;
        }
        return true;
    }


}
