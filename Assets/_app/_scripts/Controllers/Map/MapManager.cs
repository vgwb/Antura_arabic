using UnityEngine;
using System.Collections;
using EA4S;

namespace EA4S
{
    public class MapManager : MonoBehaviour
    {

        [Header("Scene Setup")]
        public Music SceneMusic;

        void Start() {
            AudioManager.I.PlayMusic(SceneMusic);

            WidgetSubtitles.I.DisplaySentence("map_A2", 2, true, NextSentence);
        }


        public void NextSentence() {
            WidgetSubtitles.I.DisplaySentence("map_A3", 3, true);
        }

    }

}