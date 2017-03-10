using UnityEngine;
using System.Collections;
using EA4S.Antura;
using EA4S.Audio;
using EA4S.Core;
using EA4S.Database;
using EA4S.UI;

namespace EA4S.Rewards
{
    /// <summary>
    /// Manages the Rewards scene.
    /// Accessed after a learning block is completed.
    /// </summary>
    [RequireComponent(typeof(RewardsAnimator))]
    public class RewardsManager : MonoBehaviour
    {
        public AnturaAnimationStates AnturaAnimation = AnturaAnimationStates.sitting;
        [Header("References")]
        public AnturaAnimationController AnturaAnimController;

        void Start()
        {
            if (AppManager.I.Player.IsFirstContact())
                GlobalUI.ShowPauseMenu(false);

            AudioManager.I.PlayMusic(Music.Theme10);
            Debug.Log("RewardsManager playsession: " + AppManager.I.Player.CurrentJourneyPosition.PlaySession);

            // here we set the Rewards base on current progression level (playsession -1 because Rewards appear when playsession is already incremented)
            //if ((AppManager.I.Player.CurrentJourneyPosition.PlaySession - 1) == 1) {
            //    AppManager.I.Player.AnturaCurrentPreset = 1;

            //    tutorialIndex = 10;
            //    //LoggerEA4S.Log("app", "Reward", "get_reward", "1");
            //    LogManager.I.LogInfo(InfoEvent.Reward, "reward:1");
            //} else if ((AppManager.I.Player.CurrentJourneyPosition.PlaySession - 1) == 2) {
            //    AppManager.I.Player.AnturaCurrentPreset = 2;
            //    tutorialIndex = 20;
            //    //LoggerEA4S.Log("app", "Reward", "get_reward", "2");
            //    LogManager.I.LogInfo(InfoEvent.Reward, "reward:2");
            //} else if ((AppManager.I.Player.CurrentJourneyPosition.PlaySession - 1) > 2) {
            //    AppManager.I.Player.AnturaCurrentPreset = 3;
            //    tutorialIndex = 30;
            //    //LoggerEA4S.Log("app", "Reward", "get_reward", "3");
            //    LogManager.I.LogInfo(InfoEvent.Reward, "reward:3");
            //}

            AnturaAnimController.State = AnturaAnimation;
            SceneTransitioner.Close();
            ShowReward();
        }

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
            //yield return new WaitForSeconds(3.5f);

            /* FIRST CONTACT FEATURE */
            if (AppManager.I.Player.IsFirstContact()) {
                KeeperManager.I.PlayDialog(Database.LocalizationDataId.Reward_Intro);

            } else {
                int rnd = Random.Range(1, 3);
                switch (rnd) {
                    case 1:
                        KeeperManager.I.PlayDialog(Database.LocalizationDataId.Reward_Big_1);
                        break;
                    case 3:
                        KeeperManager.I.PlayDialog(Database.LocalizationDataId.Reward_Big_2);
                        break;
                    default:
                        KeeperManager.I.PlayDialog(Database.LocalizationDataId.Reward_Big_3);
                        break;
                }
            }
            /* --------------------- */
            // Wait animation ending before show continue button
            yield return new WaitForSeconds(4.4f);
            ContinueScreen.Show(Continue, ContinueScreenMode.Button, true);
            yield return null;
        }

        #region API for animation driven

        public void ClearLoadedRewardsOnAntura() {
            // Clean and Charge antura reward.
            AnturaModelManager.Instance.ClearLoadedRewards();
        }

        /// <summary>
        /// Gets the reward to instantiate.
        /// </summary>
        /// <returns></returns>
        public RewardPackUnlockData GetRewardToInstantiate() {
            if (AppManager.I.Player.IsFirstContact()) {
                return AppManager.I.Player.RewardsUnlocked.Find(r => r.Type == RewardTypes.reward);
            } else {
                RewardPackUnlockData newRewardToInstantiate = RewardSystemManager.GetNextRewardPack(true)[0];
                AppManager.I.Player.AddRewardUnlocked(newRewardToInstantiate);
                return newRewardToInstantiate;
            }
        }

        /// <summary>
        /// Instantiates the reward, mount on antura and return gameobject.
        /// </summary>
        /// <param name="_rewardToInstantiate">The reward to instantiate.</param>
        /// <returns></returns>
        public GameObject InstantiateReward(RewardPackUnlockData _rewardToInstantiate) {
            return AnturaModelManager.Instance.LoadRewardPackOnAntura(_rewardToInstantiate);
        }

        #endregion

        public void Continue()
        {
            AppManager.I.NavigationManager.GoToNextScene();
        }

    }
}