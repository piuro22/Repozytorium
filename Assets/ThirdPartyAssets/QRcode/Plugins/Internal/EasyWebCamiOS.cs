using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine;
using TBEasyWebCam.CallBack;
using AOT;
using System.Threading;

namespace TBEasyWebCam
{
	public class EasyWebCamiOS:IEasyWebCam {
		
		int mPreviewWidth;
		int mPreviewHeight;
		bool mIsPlaying = false;
		
		public int previewWidth {
			get
			{
				return this.mPreviewWidth;
			}
			set
			{
				this.previewWidth = value;
			}
		}
		
		public int previewHeight {
			get
			{
				return this.mPreviewHeight;
			}
			set
			{
				this.mPreviewHeight = value;
			}
		}
		
		public bool IsPlaying
		{
			get
			{
				return mIsPlaying;
			}
		}
		
		//Color32[] pixelArr;
		Texture2D mPreview;
		byte[] buffer;
		public Texture2D WebCamPreview
		{
			get
			{
				if(this.mPreview == null)
				{
					/*
					 * Color32 blackOpaque = new Color32 (255, 20, 20, 255);

					pixelArr = new Color32[this.mPreviewWidth*this.mPreviewHeight];
					for (int i = 0; i!= pixelArr.Length; i++) {
						pixelArr[i] = blackOpaque;
					}
					*/
					TextureFormat format = TextureFormat.RGBA32;
					this.mPreview = new Texture2D(this.mPreviewWidth,this.mPreviewHeight, format, false);
				}
				return this.mPreview;
			}
		}

		public EasyWebCamiOS()
		{
			NativePluginStatic.aruRequestCamera();
			Thread.Sleep(200);
			NativePluginStatic.Init ();
			NativePluginStatic.RegisterCallbacks (EasyWebCamBase.EasyWebCamStarted, EasyWebCamBase.EasyWebCamUpdate, EasyWebCamBase.EasyWebCamStoped);
		}
		
		public void setPreviewResolution( int preWidth, int preHeight)
		{
			mPreviewWidth =  preWidth;
			mPreviewHeight =  preHeight;

			int orientation = NativePluginStatic.setPreviewResolution (preWidth, preHeight);
            Debug.Log("setPreviewResolution: is called " + orientation);

            if(orientation>0)
            {
                mPreviewWidth = preHeight;
                mPreviewHeight = preWidth;
            }
        }

		public void Play()
		{
			NativePluginStatic.Play ();
		}

		public void Init()
		{

		}

		public void Stop()
		{
			NativePluginStatic.Stop ();
		}

		public void setFocusMode(int paramode)
		{
			NativePluginStatic.setFocusMode (paramode);
		}

		public void setFlashMode(int paramode)
		{
			
		}

		public void setTorchMode(bool paramode)
		{
			NativePluginStatic.setTorchMode (paramode?1:0);
		}

		public void TakePhoto()
		{
			
		}

		public void OnPause (bool paused) {
			
		}

		public void Release () {

			NativePluginStatic.Release ();
			if (mPreview != null) {
				MonoBehaviour.Destroy (mPreview);
				mPreview = null;
			}
		}

		public void UpdateImage()
		{	
			IntPtr handle;
			int size;
			if (this.mPreview != null) {
				
				NativePluginStatic.UpdateTextureByte (out handle, out size);
				if (handle == null || size <= 0) {
					return;
				}
				buffer = buffer ?? new byte[size];

				Marshal.Copy (handle, buffer, 0, size);
				mPreview.LoadRawTextureData (buffer);
				mPreview.Apply ();
                
               
                /*
				int logCount = NativePluginStatic.ewcUpdateTexture32 (pixelArr);
				if (logCount > 0 ) {
					mPreview.SetPixels32 (pixelArr);
					mPreview.Apply ();
				}
				*/
            }
		}

		public int getEnterFrame()
		{
			return 1;
		}

		public bool isCameraUsable()
		{
			return true;
		}

		public void SwitchCamera(int mode)
		{
			NativePluginStatic.SwitchCamera (mode);
		}

        public void tapFocus()
        {

        }
        
        public void setOrientation(int so)
        {
            NativePluginStatic.setOrientation(so);
            //do somte orientation
        }


    }

}
