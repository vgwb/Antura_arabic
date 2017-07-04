using EA4S.Core;
using EA4S.Database;
using EA4S.UI;
using UnityEngine;
using System;
using System.IO;
using EA4S.Debugging;

namespace EA4S.Scenes
{
    public class ReservedAreaScene : SceneBase
    {
        [Header("References")]
        public TextRender SupportText;

        protected override void Start()
        {
            base.Start();

            GlobalUI.ShowPauseMenu(false);
            GlobalUI.ShowBackButton(true);

            SupportText.text = AppConstants.AppVersion;
        }

        #region Buttons
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

        private int clickCounter = 0;
        public void OnClickEnableDebugPanel()
        {
            clickCounter++;
            if (clickCounter >= 3) {
                if (!DebugManager.I.DebugPanelEnabled) {
                    DebugManager.I.EnableDebugPanel();
                }
            }
        }
        #endregion
        
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
            string destPath;
            var pdfTemp = Resources.Load("Pdf/" + filename, typeof(TextAsset)) as TextAsset;
            destPath = Application.persistentDataPath + "/" + filename;

            File.WriteAllBytes(destPath, pdfTemp.bytes);
            Debug.Log("Copied " + pdfTemp.name + " to " + destPath + " , File size : " + pdfTemp.bytes.Length);
            Application.OpenURL(destPath);
        }

        /// <summary>
        /// exports all databases found in 
        /// </summary>
        public void OnExportJoinedDatabase()
        {
            string errorString = "";
            if (AppManager.I.DB.ExportJoinedDatabase(out errorString))
            {
                string dbPath = DBService.GetDatabaseFilePath(AppConstants.GetJoinedDatabaseFilename(), AppConstants.DbJoinedFolder);
                GlobalUI.ShowPrompt("", "The joined DB is here:\n" + dbPath);
            }
            else {
                GlobalUI.ShowPrompt("", "Could not export the joined database.\n");
            }
        }

        /// <summary>
        /// Imports a set of database
        /// </summary>
        public void OnImportDatabases()
        {
            AppManager.I.PlayerProfileManager.ImportAllPlayerProfiles();
            AppManager.I.NavigationManager.ReloadScene();
        }
    }
}