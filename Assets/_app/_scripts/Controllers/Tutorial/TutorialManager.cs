using UnityEngine;
using System.Collections;
using EA4S;
using ModularFramework.Core;

namespace EA4S
{
    public class TutorialManager : MonoBehaviour
    {

        public MinigamesCode CurrentGameCode;

        int tutorialIndex = 10;

        void Start()
        {
            AudioManager.I.PlayMusic(Music.Silence);

            if (CurrentGameCode == MinigamesCode.FastCrowd) {
                tutorialIndex = 10;
            } else if (CurrentGameCode == MinigamesCode.DontWakeUp) {
                tutorialIndex = 20;
            } else if (CurrentGameCode == MinigamesCode.Balloons) {
                tutorialIndex = 30;
            }
            ShowTutor();
        }

        public void ShowTutor()
        {
            switch (tutorialIndex) {
                case 10:
                    tutorialIndex++;
                    WidgetSubtitles.I.DisplaySentence("game_fastcrowd_intro1", 2, true, ShowTutor);
                    break;
                case 11:
                    tutorialIndex++;
                    WidgetSubtitles.I.DisplaySentence("game_fastcrowd_intro2", 2, true, ShowTutor);
                    break;
                case 12:
                    tutorialIndex++;
                    WidgetSubtitles.I.DisplaySentence("game_fastcrowd_intro3", 3, true, Ready2Play);
                    break;
                case 15:
                    tutorialIndex++;
                    WidgetSubtitles.I.DisplaySentence("game_fastcrowd_A_intro1", 2, true, ShowTutor);
                    break;
                case 16:
                    tutorialIndex++;
                    WidgetSubtitles.I.DisplaySentence("game_fastcrowd_A_intro2", 2, true, Ready2Play);
                    break;
                case 20:
                    tutorialIndex++;
                    WidgetSubtitles.I.DisplaySentence("game_dontwake_intro1", 2, true, ShowTutor);
                    break;
                case 21:
                    tutorialIndex++;
                    WidgetSubtitles.I.DisplaySentence("game_dontwake_intro1", 2, true, ShowTutor);
                    break;
                case 22:
                    tutorialIndex++;
                    WidgetSubtitles.I.DisplaySentence("game_dontwake_intro1", 2, true, Ready2Play);
                    break;
                case 30:
                    tutorialIndex++;
                    WidgetSubtitles.I.DisplaySentence("game_balloons_intro1", 2, true, ShowTutor);
                    break;
                case 31:
                    tutorialIndex++;
                    WidgetSubtitles.I.DisplaySentence("game_balloons_intro2", 2, true, ShowTutor);
                    break;
                case 32:
                    tutorialIndex++;
                    WidgetSubtitles.I.DisplaySentence("game_balloons_intro3", 2, true, Ready2Play);
                    break;

            }
        }

        public void Ready2Play()
        {
            ContinueScreen.Show(Play, ContinueScreenMode.Button);
        }

        public void Play()
        {
            if (CurrentGameCode == MinigamesCode.FastCrowd) {
                GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("game_FastCrowd");
            } else if (CurrentGameCode == MinigamesCode.DontWakeUp) {
                GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("game_DontWakeUp");
            } else if (CurrentGameCode == MinigamesCode.Balloons) {
                GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("game_Balloons");
            }
        }

    }
}
