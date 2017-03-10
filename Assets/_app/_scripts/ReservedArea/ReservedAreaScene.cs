using EA4S.Core;
using EA4S.UI;
using UnityEngine;

namespace EA4S.Scenes
{
    public class ReservedAreaScene : MonoBehaviour
    {

        void Start()
        {
            GlobalUI.ShowPauseMenu(false);
            GlobalUI.ShowBackButton(true);
        }

        public void OnOpenUrlWebsite()
        {
            Application.OpenURL(AppConstants.UrlWebsite);
        }

        public void OnOpenUrlPrivacy()
        {
            Application.OpenURL(AppConstants.UrlPrivacy);
        }

        public void OnOpenCommunityTelegram()
        {
            Application.OpenURL(AppConstants.UrlCommunityTelegram);
        }

        public void OnOpenCommunityFacebook()
        {
            Application.OpenURL(AppConstants.UrlCommunityFacebook);
        }

        public void OnOpenInstallInstructions()
        {
            GlobalUI.ShowPrompt("", "Install instructions");
        }

        public void OnOpenRateApp()
        {
            //GlobalUI.ShowPrompt("", "Rate app");
        }

        public void OnOpenRecomment()
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer) {
                Application.OpenURL(AppConstants.UrlStoreiOSApple);
            } else if (Application.platform == RuntimePlatform.Android) {
                Application.OpenURL(AppConstants.UrlStoreAndroidGoogle);
            }
            // GlobalUI.ShowPrompt("", "How to Recommend Antura");
        }

    }
}