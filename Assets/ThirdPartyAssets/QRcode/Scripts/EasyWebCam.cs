using UnityEngine;
using System.Collections;
using AOT;
using TBEasyWebCam.CallBack;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using TBEasyWebCam;

using UnityEngine.Android;
using System.Threading.Tasks;

namespace TBEasyWebCam
{
	public class EasyWebCam : MonoBehaviour {

//        public static event Action onQuit;
//        public static event Action<bool> onPause;

        public static IEasyWebCam easyWebCamInterface;

		public static ResolutionMode mCamResolution = ResolutionMode.MediumResolution;
        public static FocusMode mFocusMode = FocusMode.AutoFocus;
		public static bool isActive = false;
       
        private float time = 0f;
        private int count = 0;

        public static bool isRunning = false;
        
        public static event EasyWebCamStartedDelegate OnEasyWebCamStart;

        public static event EasyWebCamUpdateDelegate OnEasyWebCamUpdate;

        public static event EasyWebCamStopedDelegate OnEasyWebCamStoped;
        
        public static bool isUseNativeCamera = true;

        public static Texture2D WebCamPreview
		{
			get
			{
				if(easyWebCamInterface != null)
				{
                    return easyWebCamInterface.WebCamPreview;
				}
				else
				{
                    return null;
				}
			}
		}

		EasyWebCam()
		{
            EasyWebCamBase.OnEasyWebCamStart += EasyWebCamStarted;
            EasyWebCamBase.OnEasyWebCamStoped += EasyWebCamStoped;
        }
        
        //static async void initEasyWebCam()
        async void Awake()
        {
            
#if UNITY_EDITOR

#elif UNITY_ANDROID
             int granted = await CameraSetting.RequestCameraPermissions();
            if(granted<=0)
            {
                return;
            }

			isActive = true;
            if(isUseNativeCamera)
            {
                easyWebCamInterface = new EasyWebCamAndroid ();
            }
            else
            {
                easyWebCamInterface = new EasyWebCamUnity(1);
            }
			
            // Set orientation
            switch (Screen.orientation)
            {
                case ScreenOrientation.LandscapeLeft: easyWebCamInterface.setOrientation((int)CameraOrientation.LandscapeLeft); break;
                case ScreenOrientation.Portrait: easyWebCamInterface.setOrientation((int)CameraOrientation.Portrait); break;
                case ScreenOrientation.LandscapeRight: easyWebCamInterface.setOrientation((int)CameraOrientation.LandscapeRight); break;
                case ScreenOrientation.PortraitUpsideDown: easyWebCamInterface.setOrientation((int)CameraOrientation.PortraitUpsideDown); break;
            }
            
#elif UNITY_IOS

			isActive = true;

            if(isUseNativeCamera)
            {
                easyWebCamInterface = new EasyWebCamiOS ();
            }
            else
            {
                easyWebCamInterface = new EasyWebCamUnity(2);
            }


            switch (Screen.orientation)
            {
                case ScreenOrientation.LandscapeLeft: easyWebCamInterface.setOrientation((int)CameraOrientation.LandscapeLeft); break;
                case ScreenOrientation.Portrait: easyWebCamInterface.setOrientation((int)CameraOrientation.Portrait); break;
                case ScreenOrientation.LandscapeRight: easyWebCamInterface.setOrientation((int)CameraOrientation.LandscapeRight); break;
                case ScreenOrientation.PortraitUpsideDown: easyWebCamInterface.setOrientation((int)CameraOrientation.PortraitUpsideDown); break;
            }

#endif
            setPreviewResolution(mCamResolution);
        }
        
		void Start()
		{

		}
        
     
		void Update()
		{
			if ( easyWebCamInterface != null && isRunning) {
				easyWebCamInterface.UpdateImage();
			}
            else if(easyWebCamInterface!=null && easyWebCamInterface.IsPlaying &&!isUseNativeCamera  )
            {
                easyWebCamInterface.UpdateImage();
            }
		}


        /// <summary>
        /// Play this instance.
        /// </summary>
		
        /// <summary>
        /// Play this instance.
        /// </summary>
        public static async void Play()
		{
#if UNITY_EDITOR

#elif UNITY_ANDROID

            int granted = await CameraSetting.RequestCameraPermissions();
            
            if(granted<=0)
            {
                return;
            }

            bool isActive = await RequestEasyWebCamInterface();
            if (!isActive)
            {
                return;
            }
			if (easyWebCamInterface != null) {
				easyWebCamInterface.Play ();
			}
#elif UNITY_IOS
			if (easyWebCamInterface != null) {
				easyWebCamInterface.Play ();
			}
#endif
        }


        /// <summary>
        /// Stop this instance.
        /// </summary>
        public static void Stop()
		{
#if UNITY_EDITOR

#elif UNITY_ANDROID || UNITY_IOS
			if (easyWebCamInterface != null) {
			    easyWebCamInterface.Stop ();
			}
            isRunning = false;
#endif

        }

        /// <summary>
        /// Sets the preview resolution.
        /// </summary>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        public static void setPreviewResolution(ResolutionMode resolutionMode)
		{
			if (easyWebCamInterface != null) {
				int preWidth = 0;
				int preHeight = 0;
                Utilities.Dimensions(resolutionMode,out preWidth,out preHeight);
				easyWebCamInterface.setPreviewResolution (preWidth, preHeight);
			}
		}

		/// <summary>
		/// Takes the photo.
		/// </summary>
		public static void TakePhoto()
		{
			if (easyWebCamInterface != null) {
				easyWebCamInterface.TakePhoto ();
			}
		}

		/// <summary>
		/// Sets the focus mode.
		/// </summary>
		/// <param name="paramode">Paramode.</param>
		public static void setFocusMode(FocusMode paramode)
		{

			if (!isRunning) {
				return;
			}
			if (easyWebCamInterface != null) {
				easyWebCamInterface.setFocusMode((int)paramode);
			}
            mFocusMode = paramode;
		}

        public static void tapFocus()
        {
            if (!isRunning)
            {
                return;
            }
            if (easyWebCamInterface != null)
            {
                easyWebCamInterface.tapFocus();
            }
        }

		/// <summary>
		/// Sets the flash mode.
		/// </summary>
		/// <param name="paramode">Paramode.</param>
		public static void setFlashMode(FlashMode paramode)
		{

			if (!isRunning) {
			return ;
			}
			if (easyWebCamInterface != null) {
				easyWebCamInterface.setFocusMode((int)paramode);
			}
		}

		/// <summary>
		/// Sets the torch mode.
		/// </summary>
		/// <param name="paramode">Paramode.</param>
		public static void setTorchMode(TorchMode paramode)
		{
			if (!isRunning) {
				return ;
			}
			if (easyWebCamInterface != null) {
                bool ison = (int)paramode > 0 ? true : false;
                easyWebCamInterface.setTorchMode(ison);
			}
		}

		public static void SwitchCamera(CameraMode mode)
		{
            if (easyWebCamInterface != null) {
				easyWebCamInterface.SwitchCamera((int)(mode));
			}
		}

		public static int Width()
		{
            if (easyWebCamInterface != null) {
				return easyWebCamInterface.previewWidth;
			}
            else
            {
                Debug.LogError(" Device width is " + easyWebCamInterface.previewWidth);
            }
			return 0;
		}

		public static int Height()
		{
			if (easyWebCamInterface != null) {
				return easyWebCamInterface.previewHeight;
			}
			return 0;
		}

		public static int getFrame()
		{
			if (easyWebCamInterface != null) {
				return easyWebCamInterface.getEnterFrame ();
			} else {
				return -1;
			}
		}
		
		private void OnPause(bool isPaused)
		{
			if (easyWebCamInterface != null) {
				easyWebCamInterface.OnPause (isPaused);
			}
		}

		private void OnRelease()
		{
			if (easyWebCamInterface != null) {
				easyWebCamInterface.Release ();
			}
		}
		
		public static void Release()
		{
			if (easyWebCamInterface != null) {
				easyWebCamInterface.Release ();
			}
		}

        public static Task<bool> RequestEasyWebCamInterface()
        {
            // Request
            var permissionTask = new TaskCompletionSource<bool>();
            if (Application.platform == RuntimePlatform.Android)
            {
                var requester = new GameObject("EasyWebCam Active Helper").AddComponent<EasyWebCamCheckHelper>();
                requester.StartCoroutine(RequestEasyWebCamActive(requester));
            }

            return permissionTask.Task;
            // Define Request the Component if enabled
            IEnumerator RequestEasyWebCamActive(EasyWebCamCheckHelper requester)
            {
                yield return new WaitUntil(() => isActive);
                permissionTask.SetResult(true);
                MonoBehaviour.Destroy(requester.gameObject);
            }
        }

        private sealed class EasyWebCamCheckHelper : MonoBehaviour
        {
            void Awake() => DontDestroyOnLoad(this.gameObject);
        }
        
        bool isDoubleClick()
        {
            count++;
            if (count == 1)
            {
                time = Time.time;

            }
            if (2 == count && Time.time - time <= 0.5f)
            { 
                count = 0;
                return true;
            }
            if (Time.time - time > 0.5f)
            {
                count = 0;
            }
            return false;
        }
        
        public static void EasyWebCamStarted()
        {
            if (OnEasyWebCamStart != null)
            {
                OnEasyWebCamStart();
            }
            isRunning = true;
        }
        
        public static void EasyWebCamUpdate()
        {
            if (OnEasyWebCamUpdate != null)
            {
                OnEasyWebCamUpdate();
            }
        }
        
        public static void EasyWebCamStoped()
        {
            if (OnEasyWebCamStoped != null)
            {
                OnEasyWebCamStoped();
            }
            isRunning = false;
        }
        
        void OnApplicationPause(bool paused)
        {
            OnPause(paused);
        }

        void OnApplicationQuit()
        {
           OnRelease();
        }
        void OnDestroy()
        {
            OnRelease();
        }
    }
}
