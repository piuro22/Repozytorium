using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;
using TBEasyWebCam.CallBack;

public static class NativePluginStatic {

#if UNITY_IOS
     private const string LIBRARY_NAME = "__Internal";
#else
    private const string LIBRARY_NAME = "EasyWebCam";
#endif

    public static int ewcUpdateTexture32([In, Out]Color32[] colors32)
	{
		int log = -200;
		GCHandle handle = GCHandle.Alloc(colors32, GCHandleType.Pinned);
		IntPtr address = handle.AddrOfPinnedObject();
		log = UpdateTexture32 (address);
		handle.Free();
		return log;
	}
    
    [DllImport(LIBRARY_NAME)]
    public static extern void aruRequestCamera();
    
    [DllImport(LIBRARY_NAME)]
    public static extern int Init();

    [DllImport(LIBRARY_NAME)]
    public static extern void Play();

    [DllImport(LIBRARY_NAME)]
    public static extern void Stop();

    [DllImport(LIBRARY_NAME)]
    public static extern void setFocusMode(int mode);

    [DllImport(LIBRARY_NAME)]
    public static extern void setTorchMode(int mode);

    [DllImport(LIBRARY_NAME)]
    public static extern void SwitchCamera(int mode);

    [DllImport(LIBRARY_NAME)]
    public static extern int UpdateTexture32 (System.IntPtr  colors32);

    [DllImport(LIBRARY_NAME)]
    public static extern int UpdateTextureByte (out System.IntPtr  colors32, out int  length);

    [DllImport(LIBRARY_NAME)]
    public static extern bool RegisterCallbacks(EasyWebCamStartedDelegate start,
		EasyWebCamUpdateDelegate update,
		EasyWebCamStopedDelegate stop);

    [DllImport(LIBRARY_NAME)]
    public static extern int setPreviewResolution(int w,int h);

    [DllImport(LIBRARY_NAME)]
    public static extern void SaveImageToAlbum (string iPath);

    [DllImport(LIBRARY_NAME)]
    public static extern void setOrientation(int so);

    [DllImport(LIBRARY_NAME)]
    public static extern void Release();

    // setting the unity camera
    [DllImport(LIBRARY_NAME)]
    public static extern void setUnityResolution(int w,int h);

    [DllImport(LIBRARY_NAME)]
    public static extern void setUnityCameraOrientation(int cm);

    [DllImport(LIBRARY_NAME)]
    public static extern void convertUnityPixels32(IntPtr colors32);

    [DllImport(LIBRARY_NAME)]
    public static extern void stopUnityCamera();


}
