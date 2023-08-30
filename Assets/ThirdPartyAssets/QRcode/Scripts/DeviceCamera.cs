using UnityEngine;
using System.Collections;
using TBEasyWebCam;

public class DeviceCamera {
	
    public Texture preview
	{
		get
		{
			#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
			return EasyWebCam.WebCamPreview;
			#else
			return webcamera;
			#endif
		}
	}

    WebCamTexture webcamera;
    public DeviceCamera (bool isUseEWC = true)
	{
#if (UNITY_ANDROID || UNITY_IOS)  && !UNITY_EDITOR
        EasyWebCam.isUseNativeCamera = isUseEWC;
        var webCamObject = new GameObject("EasyWebCamLib").AddComponent<EasyWebCam>();
#else
        webcamera = new WebCamTexture (640, 480);
#endif
	}
    
    public int getRotationAngle()
    {
        return this.webcamera.videoRotationAngle;
    }
    
	/// <summary>
	/// open the camera
	/// </summary>
	public void Play()
	{
		if (isPlaying()) {
			return;
		}
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
	    EasyWebCam.Play();
#else
        webcamera.Play ();
        EasyWebCam.EasyWebCamStarted();
#endif
	}

	/// <summary>
	/// Stop this camera.
	/// </summary>
	public void Stop()
	{
		if (!isPlaying()) {
			return;
		}
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
		EasyWebCam.Stop();
#else
        webcamera.Stop ();
        EasyWebCam.EasyWebCamStoped();
#endif
	}
	/// <summary>
	/// Gets the size of the webcam
	/// </summary>
	/// <returns>The size.</returns>
	public Vector2 getSize()
	{
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
		return new Vector2(EasyWebCam.Width(), EasyWebCam.Height()); 
#else
		return new Vector2(webcamera.width, webcamera.height); 
#endif
	}

	/// <summary>
	/// get the width of the camera
	/// </summary>
	public int Width()
	{
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
		return EasyWebCam.Width(); 
#else
		return webcamera.width; 
#endif
	}

	/// <summary>
	/// get the height of the camera
	/// </summary>
	public int Height()
	{
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
			return  EasyWebCam.Height(); 
#else
		return webcamera.height; 
#endif
	}

	/// <summary>
	/// get status of the camera
	/// </summary>
	/// <returns><c>true</c>, if playing was ised, <c>false</c> otherwise.</returns>
	public bool isPlaying()
	{
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
		return EasyWebCam.isRunning;
			
#else
        return webcamera.isPlaying; 
#endif
	}

	/// <summary>
	///  get the Pixels of the camera image
	/// </summary>
	/// <returns>The pixels.</returns>
	public Color[] GetPixels()
	{
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
			return EasyWebCam.WebCamPreview.GetPixels();

#else
		return webcamera.GetPixels(); 
#endif
	}

	/// <summary>
	/// get the pixels of the camera image by using the target rect range
	/// </summary>
	/// <returns>The pixels.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	/// <param name="targetWidth">Target width.</param>
	/// <param name="targetHeight">Target height.</param>
	public Color[] GetPixels(int x,int y,int targetWidth,int targetHeight)
	{
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
			return EasyWebCam.WebCamPreview.GetPixels(x,y,targetWidth,targetHeight); 
#else
		return webcamera.GetPixels(x,y,targetWidth,targetHeight); 
#endif
	}

	/// <summary>
	/// Gets the pixels32 of the camera
	/// </summary>
	/// <returns>The pixels32.</returns>
	public Color32[] GetPixels32()
	{
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
	    return EasyWebCam.WebCamPreview.GetPixels32();
#else
		return webcamera.GetPixels32(); 
#endif
	}
}
