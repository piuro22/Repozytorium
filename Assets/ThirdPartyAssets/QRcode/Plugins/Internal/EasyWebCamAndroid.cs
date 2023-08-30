using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine;
using TBEasyWebCam.CallBack;
using AOT;

namespace TBEasyWebCam
{
    public class EasyWebCamAndroid : IEasyWebCam
    {

        AndroidJavaObject ECM_NOBJ;

        int mPreviewWidth;
        int mPreviewHeight;
        bool mIsPlaying = false;

        public int previewWidth
        {
            get
            {
                return this.mPreviewWidth;
            }
            set
            {
                this.previewWidth = value;
            }
        }

        public int previewHeight
        {
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

        Color32[] pixelArr;
        Texture2D mPreview;

        public Texture2D WebCamPreview
        {
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

        public EasyWebCamAndroid()
        {
            NativePlugin.Init();
            AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.ToolBar.EasyWebCam.EasyWebCam");
            ECM_NOBJ = androidJavaClass.CallStatic<AndroidJavaObject>("instance", new object[0]);

            NativePlugin.RegisterCallbacks(EasyWebCamBase.EasyWebCamStarted, EasyWebCamBase.EasyWebCamUpdate, EasyWebCamBase.EasyWebCamStoped);
        }

        public void setPreviewResolution(int preWidth, int preHeight)
        {
            if (ECM_NOBJ == null)
            {
                return;
            }
            AndroidJavaObject androidJavaObject = ECM_NOBJ.Call<AndroidJavaObject>("setPreviewResolution", new object[]{
                preWidth,preHeight
            });
            if (androidJavaObject.GetRawObject().ToInt32() != 0)
            {
                int[] array = AndroidJNIHelper.ConvertFromJNIArray<int[]>(androidJavaObject.GetRawObject());
                mPreviewWidth = array[0];
                mPreviewHeight = array[1];
            }
        }

        public void Play()
        {
            // NativePlugin.LogStr = 100051;
            if (ECM_NOBJ == null)
            {
                return;
            }
            //  NativePlugin.LogStr = 100050;
            ECM_NOBJ.Call("openCamera", new object[0]);
        }

        public void Init()
        {

        }

        public void Stop()
        {
            ECM_NOBJ.Call("Stop", new object[0]);
        }


        public void setFocusMode(int paramode)
        {
            ECM_NOBJ.Call<bool>("tapToFocus", new object[0] { });
            //	ECM_NOBJ.Call("setFocusMode", new object[1]{paramode});
        }

        public void setFlashMode(int paramode)
        {
            ECM_NOBJ.Call("setFlash", new object[1] { paramode });
        }

        public void setTorchMode(bool paramode)
        {
            if (ECM_NOBJ == null)
            {
                return;
            }
            ECM_NOBJ.Call("setTorchEnabled", new object[1] { paramode });
        }

        public void tapFocus()
        {
            ECM_NOBJ.Call<bool>("tapToFocus", new object[0] { });
        }


        public void TakePhoto()
        {
            ECM_NOBJ.Call("TakePhoto", new object[0]);
        }

        public void OnPause(bool paused)
        {

            ECM_NOBJ.Call("onPause", new object[1] { paused });
        }

        public void Release()
        {
            if (ECM_NOBJ == null)
            {
                return;
            }
            ECM_NOBJ.Call("Release", new object[0]);
            if (mPreview != null)
            {
                MonoBehaviour.Destroy(mPreview);
                mPreview = null;
            }
            if (pixelArr != null)
            {
                pixelArr = null;
            }
        }

        public void SwitchCamera(int mode)
        {
            if (ECM_NOBJ == null)
            {
                return;
            }
            ECM_NOBJ.Call("SwitchCamera", mode);
        }

        public void UpdateImage()
        {

            if (this.mPreview != null)
            {

                IntPtr bufferhandle;
                if (NativePlugin.ewcUpdateTexture(out bufferhandle))
                {

                }
                this.mPreview.LoadRawTextureData(bufferhandle, this.mPreviewWidth * this.mPreviewHeight * 4);
                this.mPreview.Apply();
            }
        }

        public static bool ewcUpdateTexture32([In, Out]Color32[] colors32)
        {
            bool ok;
            GCHandle handle = GCHandle.Alloc(colors32, GCHandleType.Pinned);
            IntPtr address = handle.AddrOfPinnedObject();
            ok = NativePlugin.ewcUpdateTexture32(address);
            handle.Free();
            return ok;
        }

        public void setOrientation(int so)
        {
            if (ECM_NOBJ == null)
            {
                return;
            }
            ECM_NOBJ.Call("setOrientation", new object[1] { so });
        }

        public int getEnterFrame()
        {
            return ECM_NOBJ.Call<int>("getEnterFrame", new object[0]);
        }

        public bool isCameraUsable()
        {
            AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.ToolBar.EasyWebCam.EasyWebCam");
            return androidJavaClass.CallStatic<bool>("isCameraUsable", new object[0]);
        }
    }

}
