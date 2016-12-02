using UnityEngine;

namespace EA4S
{
    public class MoodManager : MonoBehaviour
    {
        [Header("Scene Setup")]
        public Music SceneMusic;

        void Start()
        {
            NavigationManager.I.CurrentScene = AppScene.Mood;


            AudioManager.I.PlayMusic(SceneMusic);
            GlobalUI.ShowPauseMenu(false);

            if ((AppManager.I.Player.CurrentJourneyPosition.PlaySession) < 2) {
                KeeperManager.I.PlayDialog(Db.LocalizationDataId.Mood_Question_2);
            } else {
                int rnd = Random.Range(1, 3);
                switch (rnd) {
                    case 1:
                        KeeperManager.I.PlayDialog(Db.LocalizationDataId.Mood_Question_1);
                        break;
                    case 3:
                        KeeperManager.I.PlayDialog(Db.LocalizationDataId.Mood_Question_3);
                        break;
                    default:
                        KeeperManager.I.PlayDialog(Db.LocalizationDataId.Mood_Question_2);
                        break;
                }
            }
        }

        /// <summary> 
        /// Mood selected. Values 0,1,2,3,4.
        /// </summary>
        /// <param name="_mood"></param>
        public void MoodSelected(int _mood)
        {
            AppManager.I.Teacher.logAI.LogMood(_mood);
            AudioManager.I.PlaySfx(Sfx.UIButtonClick);
            Invoke("exitScene", 0.5f);
        }

        void exitScene()
        {
            AppManager.I.Player.MoodLastVisit = System.DateTime.Today.ToString();
            AppManager.I.Player.Save();
            NavigationManager.I.GoToScene(AppScene.Map);
        }
    }
}