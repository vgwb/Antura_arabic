using UnityEngine;
using System.Collections;
using ModularFramework.Core;

namespace EA4S
{
    [RequireComponent(typeof(RewardsAnimator))]
    public class RewardsManager : MonoBehaviour
    {
        [Header("References")]
        public Antura AnturaController;

        int tutorialIndex = 10;

        void Start()
        {
            // Navigation manager 
            NavigationManager.I.CurrentScene = AppScene.Rewards;

            AudioManager.I.PlayMusic(Music.Theme4);
            Debug.Log("RewardsManager playsession: " + AppManager.I.Player.CurrentJourneyPosition.PlaySession);

            // here we set the Rewards base on current progression level (playsession -1 because Rewards appear when playsession is already incremented)
            if ((AppManager.I.Player.CurrentJourneyPosition.PlaySession - 1) == 1) {
                AppManager.I.Player.AnturaCurrentPreset = 1;

                tutorialIndex = 10;
                //LoggerEA4S.Log("app", "Reward", "get_reward", "1");
                LogManager.I.LogInfo(InfoEvent.Reward, "reward:1");
            } else if ((AppManager.I.Player.CurrentJourneyPosition.PlaySession - 1) == 2) {
                AppManager.I.Player.AnturaCurrentPreset = 2;
                tutorialIndex = 20;
                //LoggerEA4S.Log("app", "Reward", "get_reward", "2");
                LogManager.I.LogInfo(InfoEvent.Reward, "reward:2");
            } else if ((AppManager.I.Player.CurrentJourneyPosition.PlaySession - 1) > 2) {
                AppManager.I.Player.AnturaCurrentPreset = 3;
                tutorialIndex = 30;
                //LoggerEA4S.Log("app", "Reward", "get_reward", "3");
                LogManager.I.LogInfo(InfoEvent.Reward, "reward:3");
            }
            AnturaController.SetPreset(AppManager.I.Player.AnturaCurrentPreset);
            SceneTransitioner.Close();
            ShowReward();
            //ShowTutor();
        }

        //public void ShowTutor()
        //{
        //    switch (tutorialIndex) {
        //        case 10:
        //            tutorialIndex++;
        //            WidgetSubtitles.I.DisplaySentence("Reward_Intro", 2, true, ShowTutor);
        //            break;
        //        case 11:
        //            tutorialIndex++;
        //            WidgetSubtitles.I.DisplaySentence("game_rewards_intro2", 2, true, ShowReward);
        //            break;
        //        case 20:
        //            tutorialIndex++;
        //            WidgetSubtitles.I.DisplaySentence("game_reward_A1", 2, true, ShowTutor);
        //            break;
        //        case 21:
        //            tutorialIndex++;
        //            WidgetSubtitles.I.DisplaySentence("game_reward_A2", 2, true, ShowReward);
        //            break;
        //        case 30:
        //            tutorialIndex++;
        //            WidgetSubtitles.I.DisplaySentence("end_learningblock_A1", 2, true, ShowTutor);
        //            break;
        //        case 31:
        //            tutorialIndex++;
        //            WidgetSubtitles.I.DisplaySentence("end_learningblock_A2", 2, true, ShowReward);
        //            break;
        //    }
        //}

        public void ShowReward()
        {
            StartCoroutine(StartReward());
        }

        IEnumerator StartReward()
        {
            // Wait for animation to complete
            //RewardsAnimator animator = this.GetComponent<RewardsAnimator>();
            //while (!animator.IsComplete)
            //    yield return null;
            yield return new WaitForSeconds(3.5f);

            /* FIRST CONTACT FEATURE */
            if (AppManager.I.Player.IsFirstContact()) {
                AppManager.I.Modules.SceneModule.LoadSceneWithTransition("app_AnturaSpace");
            }
            /* --------------------- */
            ContinueScreen.Show(Continue, ContinueScreenMode.Button);
            yield return null;
        }


        public void Continue()
        {
            AppManager.I.Player.ResetPlaySessionMinigame();
            AppManager.I.Player.SetMaxJourneyPosition(TeacherAI.I.journeyHelper.FindNextJourneyPosition(AppManager.I.Player.CurrentJourneyPosition));
            NavigationManager.I.GoToNextScene();
        }

    }
}