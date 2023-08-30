using UnityEngine;
using System.Collections;
using TBEasyWebCam.CallBack;
using AOT;
namespace TBEasyWebCam
{
    public static class EasyWebCamBase
    {

        public static event EasyWebCamStartedDelegate OnEasyWebCamStart;

        public static event EasyWebCamUpdateDelegate OnEasyWebCamUpdate;

        public static event EasyWebCamStopedDelegate OnEasyWebCamStoped;

        public static bool isRunning = false;
        [MonoPInvokeCallback(typeof(EasyWebCamStartedDelegate))]
        public static void EasyWebCamStarted()
        {
            if (OnEasyWebCamStart != null)
            {
                OnEasyWebCamStart();
            }
            EasyWebCamBase.isRunning = true;
        }

        [MonoPInvokeCallback(typeof(EasyWebCamUpdateDelegate))]
        public static void EasyWebCamUpdate()
        {
            if (OnEasyWebCamUpdate != null)
            {
                OnEasyWebCamUpdate();
            }
        }

        [MonoPInvokeCallback(typeof(EasyWebCamStopedDelegate))]
        public static void EasyWebCamStoped()
        {
            if (OnEasyWebCamStoped != null)
            {
                OnEasyWebCamStoped();
            }
            EasyWebCamBase.isRunning = false;
        }

    }
}
