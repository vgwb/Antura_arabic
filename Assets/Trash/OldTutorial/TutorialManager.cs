using UnityEngine;
using System.Collections;
using EA4S;
using ModularFramework.Core;

namespace EA4S
{
    public class TutorialManager : MonoBehaviour
    {
        public MiniGameCode CurrentGameCode;

        bool isAnimationComplete;
        bool isDialogsComplete;
        bool continueShown;

        int tutorialIndex = 10;

        void Start()
        {
            AudioManager.I.PlayMusic(Music.Silence);

            SceneTransitioner.Close();

            if (CurrentGameCode == MiniGameCode.FastCrowd_letter) {
                tutorialIndex = 10;
            } else if (CurrentGameCode == MiniGameCode.FastCrowd_words) {
                tutorialIndex = 15;
            } else if (CurrentGameCode == MiniGameCode.DontWakeUp) {
                tutorialIndex = 20;
            } else if (CurrentGameCode == MiniGameCode.Balloons_spelling) {
                tutorialIndex = 30;
            }
            //ShowTutor();
        }

        //public void ShowTutor()
        //{
        //    switch (tutorialIndex) {
        //        case 10:
        //            tutorialIndex++;
        //            WidgetSubtitles.I.DisplaySentence("game_fastcrowd_intro1", 2, true, ShowTutor);
        //            break;
        //        case 11:
        //            tutorialIndex++;
        //            WidgetSubtitles.I.DisplaySentence("game_fastcrowd_intro2", 2, true, ShowTutor);
        //            break;
        //        case 12:
        //            tutorialIndex++;
        //            WidgetSubtitles.I.DisplaySentence("game_fastcrowd_intro3", 3, true, DialogsComplete);
        //            break;
        //        case 15:
        //            tutorialIndex++;
        //            WidgetSubtitles.I.DisplaySentence("game_fastcrowd_A_intro1", 2, true, ShowTutor);
        //            break;
        //        case 16:
        //            tutorialIndex++;
        //            WidgetSubtitles.I.DisplaySentence("game_fastcrowd_A_intro2", 2, true, DialogsComplete);
        //            break;
        //        case 20:
        //            tutorialIndex++;
        //            WidgetSubtitles.I.DisplaySentence("game_dontwake_intro1", 2, true, ShowTutor);
        //            break;
        //        case 21:
        //            tutorialIndex++;
        //            WidgetSubtitles.I.DisplaySentence("game_dontwake_intro2", 2, true, ShowTutor);
        //            break;
        //        case 22:
        //            tutorialIndex++;
        //            WidgetSubtitles.I.DisplaySentence("game_dontwake_intro3", 2, true, DialogsComplete);
        //            break;
        //        case 30:
        //            tutorialIndex++;
        //            WidgetSubtitles.I.DisplaySentence("game_balloons_intro1", 2, true, ShowTutor);
        //            break;
        //        case 31:
        //            tutorialIndex++;
        //            WidgetSubtitles.I.DisplaySentence("game_balloons_intro2", 2, true, ShowTutor);
        //            break;
        //        case 32:
        //            tutorialIndex++;
        //            WidgetSubtitles.I.DisplaySentence("game_balloons_intro3", 2, true, DialogsComplete);
        //            break;
        //    }
        //}

        public void DialogsComplete()
        {
            isDialogsComplete = true;
            Ready2Play();
        }

        public void OnTutorialComplete()
        {
            isAnimationComplete = true;
            Ready2Play();
        }

        void Ready2Play()
        {
            // TODO both dialog and animation should be compelted before allowing continue
            if ((isAnimationComplete && isDialogsComplete) && !continueShown) {
                continueShown = true;
                ContinueScreen.Show(Play, ContinueScreenMode.Button);
            }
        }

        public void Play()
        {
            if (CurrentGameCode == MiniGameCode.FastCrowd_letter || CurrentGameCode == MiniGameCode.FastCrowd_words) {
                GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("game_FastCrowd");
            } else if (CurrentGameCode == MiniGameCode.DontWakeUp) {
                GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("game_DontWakeUp");
            } else if (CurrentGameCode == MiniGameCode.Balloons_spelling) {
                GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("game_Balloons");
            }
        }

    }
}
