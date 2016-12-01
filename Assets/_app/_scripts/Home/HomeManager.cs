using UnityEngine;
using System.Collections;

namespace EA4S
{

    public class HomeManager : MonoBehaviour
    {
        [Header("Scene Setup")]
        public Music SceneMusic;

        void Start()
        {
            NavigationManager.I.CurrentScene = AppScene.Home;

            GlobalUI.ShowPauseMenu(true, PauseMenuType.StartScreen);
            AudioManager.I.PlayMusic(SceneMusic);
            AudioManager.I.PlaySfx(Sfx.GameTitle);
        }

        public void Play()
        {
            GlobalUI.ShowPauseMenu(true);

            LogManager.I.InitNewSession();
            LogManager.I.LogInfo(InfoEvent.AppPlay, JsonUtility.ToJson(new AppInfoParameters()));

            if (AppManager.I.Player.MoodLastVisit == System.DateTime.Today.ToString())
                NavigationManager.I.GoToScene(AppScene.Map);
            else
                NavigationManager.I.GoToScene(AppScene.Mood);
        }
    }
}