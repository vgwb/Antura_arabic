using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using EA4S;
using ModularFramework.Core;

namespace EA4S
{
    public class MoodManager : MonoBehaviour
    {
        [Header("Scene Setup")]
        public Music SceneMusic;

        void Start() {
            AudioManager.I.PlayMusic(SceneMusic);
        }

        /// <summary> 
        /// Mood selected. Values 0,1,2,3,4.
        /// </summary>
        /// <param name="_mood"></param>
        public void MoodSelected(int _mood) {
            /// - save mood
            /// - log
            /// - change scene
            AppManager.Instance.Modules.SceneModule.LoadSceneWithTransition("app_Journey");
        }

        public void StartTest() {
            LoggerEA4S.SessionID = Random.Range(10000000, 99999999).ToString();
            LoggerEA4S.Log("app", "appversion", "info", AppManager.AppVersion);
            LoggerEA4S.Log("app", "platform", "info", string.Format("{0} | (sys mem) {1} | (video mem) {2} | {3} |", SystemInfo.operatingSystem, SystemInfo.systemMemorySize, SystemInfo.graphicsMemorySize, Screen.width + "x" + Screen.height));
            LoggerEA4S.Log("app", "user", "info", LoggerEA4S.SessionID);
            SceneManager.LoadScene("Wheel");
        }
    }
}