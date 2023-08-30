using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine;
using TBEasyWebCam.CallBack;

public static class  NativePlugin  {

	private const string LIBRARY_NAME = "EasyWebCam";

	[DllImport(LIBRARY_NAME)]
	public static extern void Init();

	[DllImport(LIBRARY_NAME)]
	public static extern void Close();

	[DllImport(LIBRARY_NAME)]
	public static extern int ewcUpdateTextureGL(int textureID);

	[DllImport(LIBRARY_NAME)]
	public static extern bool ewcUpdateTexture32(IntPtr colors32);

	[DllImport(LIBRARY_NAME)]
	public static extern bool ewcUpdateTexture (out IntPtr buffer);

	[DllImport(LIBRARY_NAME)]
	public static extern void updateParam(int width,int height,int camMode);

	[DllImport(LIBRARY_NAME)]
	public static extern float addFloat(float a,float b);

	[DllImport(LIBRARY_NAME)]
	public static extern int getHeight();
	
	[DllImport(LIBRARY_NAME)]
	public static extern int getWidth();

	[DllImport(LIBRARY_NAME)]
	public static extern int getTestSize();

	[DllImport(LIBRARY_NAME)]
	public static extern bool StartRunning ();

	[DllImport(LIBRARY_NAME)]
	public static extern bool StopRunning ();
    
	[DllImport(LIBRARY_NAME)]
	public static extern bool RegisterCallbacks(EasyWebCamStartedDelegate start,
	                                            EasyWebCamUpdateDelegate update,
	                                           	EasyWebCamStopedDelegate stop);

    [DllImport(LIBRARY_NAME)]
    public static extern void setUnityResolution(int w,int h);

    [DllImport(LIBRARY_NAME)]
    public static extern void setUnityCameraOrientation(int cm);

    [DllImport(LIBRARY_NAME)]
    public static extern void convertUnityPixels32(IntPtr colors32);

    [DllImport(LIBRARY_NAME)]
    public static extern void stopUnityCamera();

    [DllImport(LIBRARY_NAME)]
    public static extern void Release();
    
    public static int LogStr = 200;

}
