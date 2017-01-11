using UnityEngine;

namespace EA4S.Scenes
{
    /// <summary>
    /// Controls the app_End scene, accessed when the game is completed.
    /// </summary>
    public class EndManager : MonoBehaviour
    {
        [Header("Scene Setup")]
        public Music SceneMusic;

        int tutorialIndex = 10;

        void Start()
        {
            AudioManager.I.PlayMusic(SceneMusic);
            SceneTransitioner.Close();
            ShowTutor();
        }

        void ShowTutor()
        {
            switch (tutorialIndex) {
                case 10:
                    tutorialIndex++;
                    WidgetSubtitles.I.DisplaySentence(Db.LocalizationDataId.Map_EndGame, 2, true, ShowTutor);
                    break;
                case 11:
                    tutorialIndex++;
                    WidgetSubtitles.I.DisplaySentence(Db.LocalizationDataId.Map_EndGame, 2, true, ShowTutor);
                    break;
                case 12:
                    tutorialIndex++;
                    WidgetSubtitles.I.DisplaySentence(Db.LocalizationDataId.Map_EndGame, 1, true, ReadyToPlay);
                    break;
            }
        }

        public void ReadyToPlay()
        {
            WidgetSubtitles.I.Close();
            AudioManager.I.PlaySfx(Sfx.GameTitle);
            ContinueScreen.Show(RestartGame, ContinueScreenMode.Button);
        }

        public void RestartGame()
        {
            NavigationManager.I.GoToScene(AppScene.Home);
        }
    }
}