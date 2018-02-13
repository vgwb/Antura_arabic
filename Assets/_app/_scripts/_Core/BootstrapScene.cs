using Antura.Core;
using Antura.UI;
using System;
using UnityEngine;

namespace Antura.Scenes
{
    /// <summary>
    /// Controls the _Start scene, providing an entry point for all users prior to having selected a player profile. 
    /// </summary>
    public class BootstrapScene : SceneBase
    {

        [Header("References")]
        public PanelAppUpdate PanelAppUpdate;

        protected override void Start()
        {
            if (AppManager.I.AppSettingsManager.IsAppJustUpdatedFromOldVersion()) {
                Debug.Log("Updating from Old version");
                AppManager.I.AppSettingsManager.AppUpdateCheckDone();
                if (AppManager.I.AppSettings.SavedPlayers != null) {
                    AppManager.I.PlayerProfileManager.DeleteAllPlayers();
                    PanelAppUpdate.Init();
                } else {
                    GoToHomeScene();
                }
            } else {
                GoToHomeScene();
            }
        }

        private void GoToHomeScene()
        {
            AppManager.I.NavigationManager.GoToHome();
        }

        public void CloseAppUpdatePanel()
        {
            GoToHomeScene();
        }

    }
}