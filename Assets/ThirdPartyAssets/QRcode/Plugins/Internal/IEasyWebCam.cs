using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine;
using TBEasyWebCam.CallBack;
using AOT;

public interface  IEasyWebCam{

	Texture2D WebCamPreview
	{
		get;
	}
	
	int previewWidth {
		get;
		set;
	}
	
	int previewHeight {
		get;
		set;
	}
	
	bool IsPlaying
	{
		get;
	}
	
	void Init();
	void Play();
	
	void Stop();

	void UpdateImage();
	
	void setPreviewResolution(int width, int height);
	 
	void TakePhoto();


	void setFocusMode(int paramode);

	void setFlashMode(int paramode);

	void setTorchMode(bool paramode);

    void setOrientation(int paramode);

    int getEnterFrame();

	bool isCameraUsable();
	void OnPause (bool paused);
	void Release () ;

	void SwitchCamera(int mode);
    void tapFocus();
}
