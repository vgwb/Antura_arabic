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
            /// Reset log session
            EA4S.LoggerEA4S.SessionID = Random.Range(10000000, 99999999).ToString();
            LoggerEA4S.Log("app", "appversion", "info", AppManager.AppVersion);
            LoggerEA4S.Log("app", "platform", "info", string.Format("{0} | (sys mem) {1} | (video mem) {2} | {3} |", SystemInfo.operatingSystem, SystemInfo.systemMemorySize, SystemInfo.graphicsMemorySize, Screen.width + "x" + Screen.height));
            LoggerEA4S.Log("app", "user", "info", LoggerEA4S.SessionID);
            LoggerEA4S.Save();

            AppManager.Instance.ResetProgressionData();


            AudioManager.I.PlayMusic(SceneMusic);
            AudioManager.I.PlaySfx(Sfx.GameTitle);
            ContinueScreen.Show(Play, ContinueScreenMode.Button);
        }

        public void Play()
        {
            GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("app_Mood");
        }



    }
}