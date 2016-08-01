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

        void Start()
        {
            AudioManager.I.PlayMusic(SceneMusic);

            Debug.Log("MapManager PlaySession " + AppManager.Instance.PlaySession);
            if ((AppManager.Instance.PlaySession) > 2) {
                WidgetSubtitles.I.DisplaySentence("mood_how_do_you_feel");
            } else {
                WidgetSubtitles.I.DisplaySentence("mood_how_are_you_today");
            }
        }

        /// <summary> 
        /// Mood selected. Values 0,1,2,3,4.
        /// </summary>
        /// <param name="_mood"></param>
        public void MoodSelected(int _mood)
        {
            if (AppManager.Instance.PlaySession > 2) 
                // End mood eval
                LoggerEA4S.Log("app", "mood", "end", _mood.ToString());
            else
                // start mood eval
                LoggerEA4S.Log("app", "mood", "start", _mood.ToString());

            LoggerEA4S.Save();
            AudioManager.I.PlaySfx(Sfx.UIButtonClick);
            Invoke("exitMoodScene", 0.5f);
        }

        void exitMoodScene()
        {
            // if we just did Assestment then go home
            if (AppManager.Instance.PlaySession > 2) {
                GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("app_Start");
            } else {
                GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("app_Journey");
            }
        }


    }
}