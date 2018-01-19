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

            var titleText = LocalizationManager.GetLocalizationData(LocalizationDataId.UI_Attention);
            var panelText = LocalizationManager.GetLocalizationData(LocalizationDataId.UI_AlertFinalRelease);

            if (AppManager.I.AppSettingsManager.AppVersionPrevious <= new Version(1, 0, 0, 0)) {
                EnglishText.text = "<b>" + titleText.English + "</b>\n\n" + panelText.English;
                ArabicText.text = "<b>" + titleText.Arabic + "<b/>\n\n" + panelText.Arabic;

                AppManager.I.PlayerProfileManager.DeleteAllPlayers();
            } else {
                EnglishText.text = "<b>APP UPDATE</b>\r\n\n" + "you just update from version " + AppManager.I.AppSettingsManager.AppVersionPrevious + " to " + AppConfig.AppVersion;
                ArabicText.text = "";
            }
        }

        public void OnBtnContinue()
        {
            gameObject.SetActive(false);
            if (!AppManager.I.AppSettings.OnlineAnalyticsEnabled) {
                OnlineAnalyticsRequest();
            } else {
                Close();
            }
        }

        public void OnlineAnalyticsRequest()
        {
            GlobalUI.ShowPrompt(LocalizationDataId.UI_PromptOnlineAnalytics, () => {
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