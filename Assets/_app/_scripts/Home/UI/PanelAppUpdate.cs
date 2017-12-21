using Antura.Database;
using Antura.Scenes;
using Antura.UI;
using System;
using UnityEngine;

namespace Antura.Core
{
    public class PanelAppUpdate : MonoBehaviour
    {
        public TextRender EnglishText;
        public TextRender ArabicText;

        void Start()
        {
            if (AppManager.I.AppSettingsManager.AppVersionPrevious <= new Version(1, 0, 0, 0)) {
                EnglishText.text = "<b>THANK YOU!</b>\r\n\nThanks for downloading the final release of Antura and the letters for helping us playing the beta version. \n\nUnfortunately all player profiles that were created with the previous version are no longer compatible with the game and must be deleted. ";
                ArabicText.text = "";

                AppManager.I.PlayerProfileManager.DeleteAllPlayers();
            } else {
                EnglishText.text = "<b>APP UPDATE</b>\r\n\n" + "you just update from version " + AppManager.I.AppSettingsManager.AppVersionPrevious + " to " + AppConfig.AppVersion;
                ArabicText.text = "";
            }
        }

        public void OnBtnContinue()
        {
            gameObject.SetActive(false);
            OnlineAnalyticsRequest();
        }

        public void OnlineAnalyticsRequest()
        {
            GlobalUI.ShowPrompt(LocalizationDataId.UI_AreYouSure, () => {
                AppManager.I.AppSettingsManager.EnableOnlineAnalytics(true);
                Close();
            }, () => {
                AppManager.I.AppSettingsManager.EnableOnlineAnalytics(false);
                Close();
            });
        }

        private void Close()
        {
            (HomeScene.I as HomeScene).CloseAppUpdatePanel();
        }

    }
}