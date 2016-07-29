using UnityEngine;
using System.Collections;
using EA4S;
using ModularFramework.Core;

namespace EA4S
{
    public class MapManager : MonoBehaviour
    {

        [Header("Scene Setup")]
        public Music SceneMusic;

        void Start() {
            AudioManager.I.PlayMusic(SceneMusic);
            WidgetSubtitles.I.DisplaySentence("map_A1", 2, true, NextSentence);
        }

        public void NextSentence() {
            WidgetSubtitles.I.DisplaySentence("map_A2", 3, true, NextSentence2);
        }

        public void NextSentence2() {
            WidgetSubtitles.I.DisplaySentence("map_A3", 3, true, Ready2Play);
        }

        public void Ready2Play() {
            ContinueScreen.Show(Play, ContinueScreenMode.Button);
        }

        public void Play() {
            if(AppManager.Instance.IsAssessmentTime)
                GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("app_Assessment");
            else
                GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("app_Wheel");
        }

    }

}