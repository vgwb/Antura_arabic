using Facebook.Unity;
using UnityEngine;

namespace Antura.Core.Services.OnlineAnalytics
{
    public class FacebookManager : MonoBehaviour
    {
        public bool verbose;

        void Awake()
        {
            CheckActivation();
        }

        void OnApplicationPause(bool pauseStatus)
        {
            // Check the pauseStatus to see if we are in the foreground or background
            if (!pauseStatus)
            {
                CheckActivation();
            }
        }

        private void CheckActivation()
        {
            if (FB.IsInitialized)
            {
                Activate();
            }
            else
            {
                FB.Init(OnInitComplete, OnHideUnity);
            }
        }

        private void OnInitComplete()
        {
            if (verbose)
            {
                string logMessage = string.Format(
                    "OnInitComplete IsLoggedIn='{0}' IsInitialized='{1}'",
                    FB.IsLoggedIn,
                    FB.IsInitialized);
                Debug.Log(logMessage);
            }
            Activate();
        }

        private void Activate()
        {
            FB.ActivateApp();
            FB.LogAppEvent("testEvent");
        }

        private void OnHideUnity(bool isGameShown)
        {
            if (verbose)
            {
                string logMessage = string.Format("Success Response: OnHideUnity {0}\n", isGameShown);
                Debug.Log(logMessage);
            }
        }

    }

}