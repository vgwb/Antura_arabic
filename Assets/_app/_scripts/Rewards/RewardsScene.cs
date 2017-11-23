using UnityEngine;
using System.Collections;
using Antura.Dog;
using Antura.Core;
using Antura.Database;
using Antura.Keeper;
using Antura.Profile;
using Antura.UI;
using UnityEngine.UI;

namespace Antura.Rewards
{
    /// <summary>
    /// Manages the Rewards scene.
    /// Accessed after a learning block is completed.
    /// </summary>
    [RequireComponent(typeof(RewardsAnimator))]
    public class RewardsScene : SceneBase
    {
        [Header("Setup")]
        public AnturaAnimationStates AnturaAnimation = AnturaAnimationStates.sitting;

        [Header("References")]
        public AnturaAnimationController AnturaAnimController;
        public Button AnturaSpaceBtton;

        protected override void Start()
        {
            base.Start();
            GlobalUI.ShowPauseMenu(false);
            Debug.Log("RewardsManager playsession: " + AppManager.I.Player.CurrentJourneyPosition.PlaySession);

            AnturaAnimController.State = AnturaAnimation;
            ShowReward();

            if (!FirstContactManager.I.IsInFirstContact())
            {
                AnturaSpaceBtton.onClick.AddListener(() => AppManager.I.NavigationManager.GoToAnturaSpace());
            }
            else
            {
                AnturaSpaceBtton.gameObject.SetActive(false);
            }
        }

        public void ShowReward()
        {
            StartCoroutine(StartReward());
        }

        IEnumerator StartReward()
        {
            if (FirstContactManager.I.IsInFirstContact())
            {
                KeeperManager.I.PlayDialog(Database.LocalizationDataId.Reward_Intro);
            }
            else
            {
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
            // Wait animation ending before show continue button
            yield return new WaitForSeconds(4.4f);
            ContinueScreen.Show(Continue, ContinueScreenMode.Button, true);
            yield return null;
        }

        #region API for animation driven

        public void ClearLoadedRewardsOnAntura()
        {
            // Clean and Charge antura reward.
            AnturaModelManager.I.ClearLoadedRewards();
        }

        /// <summary>
        /// Gets the reward to instantiate.
        /// </summary>
        /// <returns></returns>
        public RewardPackUnlockData GetRewardToInstantiate()
        {
            if (FirstContactManager.I.IsInFirstContact())
            {
                return AppManager.I.Player.RewardsUnlocked.Find(r => r.Type == RewardTypes.reward);
            }
            else
            {
                RewardPackUnlockData newRewardToInstantiate = RewardSystemManager.GetNextRewardPack(true)[0];
                AppManager.I.Player.AddRewardUnlocked(newRewardToInstantiate);
                AppManager.I.Player.AdvanceMaxJourneyPosition();
                return newRewardToInstantiate;
            }
        }

        /// <summary>
        /// Instantiates the reward, mount on antura and return gameobject.
        /// </summary>
        /// <param name="_rewardToInstantiate">The reward to instantiate.</param>
        /// <returns></returns>
        public GameObject InstantiateReward(RewardPackUnlockData _rewardToInstantiate)
        {
            return AnturaModelManager.I.LoadRewardPackOnAntura(_rewardToInstantiate);
        }

        #endregion

        public void Continue()
        {
            AppManager.I.NavigationManager.GoToNextScene();
        }
    }
}