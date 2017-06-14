using EA4S.Core;
using EA4S.UI;
using UnityEngine;
using System;
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
            GlobalUI.ShowPrompt("", "Opening a PDF with the Install instructions.\nIf the document doesn't open, please install a PDF viewer app and retry!");
            OpenPDF(AppConstants.PdfAndroidInstall);
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
            (AppManager.Instance as AppManager).OpenSupportForm();
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
            string destPath;
            TextAsset pdfTemp = Resources.Load("Pdf/" + filename, typeof(TextAsset)) as TextAsset;
            if (Application.platform == RuntimePlatform.Android) {
                //destPath = Application.persistentDataPath + "/Data/" + filename;
                destPath = Application.persistentDataPath + "/" + filename;
            } else {
                destPath = Application.persistentDataPath + "/" + filename;
                //destPath =  System.IO.Path.Combine(Application.persistentDataPath, filename);
            }

            File.WriteAllBytes(destPath, pdfTemp.bytes);
            Debug.Log("Copied " + pdfTemp.name + " to " + destPath + " , File size : " + pdfTemp.bytes.Length);
            Application.OpenURL(destPath);

            //var sourceFilename = System.IO.Path.Combine(Application.streamingAssetsPath, filename);
            //var savePath = System.IO.Path.Combine(Application.persistentDataPath, filename);

            //var myPDF = File.ReadAllBytes(sourceFilename);
            //File.WriteAllBytes(savePath, myPDF);
            //Debug.Log("Copied " + sourceFilename + " to " + savePath + " , bytes downloaded, File size : " + myPDF.Length);

            //Application.OpenURL(savePath);
        }
    }
}