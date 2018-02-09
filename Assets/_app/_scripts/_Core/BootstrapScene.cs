using Antura.Core;
using Antura.UI;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

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
                AppManager.I.AppSettingsManager.AppUpdateCheckDone();
                AppManager.I.PlayerProfileManager.DeleteAllPlayers();
                PanelAppUpdate.Init();
            } else {
                GoToHomeScene();
            }
        }

        private void GoToHomeScene()
        {
            AppManager.I.NavigationManager.GoToHome();
            //SceneManager.LoadScene(SceneHelper.GetSceneName(AppScene.Home));
        }

        public void CloseAppUpdatePanel()
        {
            GoToHomeScene();
        }

    }
}