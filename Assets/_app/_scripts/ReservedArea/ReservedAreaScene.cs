using EA4S.Core;
using EA4S.UI;
using UnityEngine;
using System.IO;

namespace EA4S.Scenes
{
    public class ReservedAreaScene : MonoBehaviour
    {
        [Header("References")]
        public TextRender SupportText;

        void Start()
        {
            GlobalUI.ShowPauseMenu(false);
            GlobalUI.ShowBackButton(true);

            SupportText.text = AppConstants.AppVersion + " " + "OPEN BETA";
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
            OpenPDF("TestPDF");
        }

        #region RATE
        public void OnOpenRateApp()
        {
            GlobalUI.ShowPrompt(Database.LocalizationDataId.UI_Prompt_rate, DoOpenRateApp, DoNothing);
        }

        void DoOpenRateApp()
        {
            Debug.Log("On DEVICE it will open the app page on the proper store");
            if (Application.platform == RuntimePlatform.IPhonePlayer) {
                Application.OpenURL(AppConstants.UrlStoreiOSApple);
                // IOSNativeUtility.RedirectToAppStoreRatingPage();
            } else if (Application.platform == RuntimePlatform.Android) {
                Application.OpenURL(AppConstants.UrlStoreAndroidGoogle);
                // AndroidNativeUtility.OpenAppRatingPage("");
            }
            //GlobalUI.ShowPrompt("", "Rate app");
        }
        #endregion

        #region SUPPORT FORM
        public void OnOpenSupportForm()
        {
            GlobalUI.ShowPrompt(Database.LocalizationDataId.UI_Prompt_bugreport, DoOpenSupportForm, DoNothing);
        }

        void DoOpenSupportForm()
        {
            AppManager.I.OpenSupportForm();
        }
        #endregion

        public void OnOpenRecomment()
        {
            // GlobalUI.ShowPrompt("", "How to Recommend Antura");
        }

        void DoNothing()
        {

        }

        public void OpenPDF(string filename)
        {
            //string sourcePath = System.IO.Path.Combine(Application.streamingAssetsPath, filename);
            string destPath = System.IO.Path.Combine(Application.persistentDataPath, filename + ".pdf");
            TextAsset pdfTemp = Resources.Load("Pdf/" + filename, typeof(TextAsset)) as TextAsset;
            File.WriteAllBytes(destPath, pdfTemp.bytes);
            Application.OpenURL(destPath);
        }

    }
}