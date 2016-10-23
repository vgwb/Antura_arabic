using UnityEngine;
using System.Collections;
using EA4S;
using ModularFramework.Core;

namespace EA4S
{
    public class HomeManager : MonoBehaviour
    {
        [Header("Scene Setup")]
        public Music SceneMusic;

        void Start()
        {
            GlobalUI.ShowPauseMenu(false);
            AudioManager.I.PlayMusic(SceneMusic);
            AudioManager.I.PlaySfx(Sfx.GameTitle);

            SceneTransitioner.Close();
        }

        public void Play()
        {
            GlobalUI.ShowPauseMenu(true);

            AppManager.Instance.ResetProgressionData();

            /// Reset log session
            EA4S.LoggerEA4S.SessionID = Random.Range(10000000, 99999999).ToString();
            LoggerEA4S.Log("app", "appversion", "info", Config.AppVersion);
            LoggerEA4S.Log("app", "platform", "info", string.Format("{0} | (sys mem) {1} | (video mem) {2} | {3} |", SystemInfo.operatingSystem, SystemInfo.systemMemorySize, SystemInfo.graphicsMemorySize, Screen.width + "x" + Screen.height));
            LoggerEA4S.Log("app", "user", "info", LoggerEA4S.SessionID);
            LoggerEA4S.Save();

            GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("app_Mood");
        }
    }
}