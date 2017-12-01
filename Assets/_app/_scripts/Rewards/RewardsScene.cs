using Antura.Dog;
using Antura.Core;
using Antura.Database;
using Antura.Keeper;
using Antura.Profile;
using Antura.Tutorial;
using Antura.UI;
using DG.Tweening;
using System.Collections;
using UnityEngine;
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

        private TutorialManager tutorialManager;
        Tween btAnturaTween;

        protected override void Start()
        {
            base.Start();
            GlobalUI.ShowPauseMenu(false);
            Debug.Log("RewardsManager playsession: " + AppManager.I.Player.CurrentJourneyPosition.PlaySession);

            AnturaAnimController.State = AnturaAnimation;
            //AnturaSpaceBtton.gameObject.SetActive(false);
            ShowReward();

            AnturaSpaceBtton.onClick.AddListener(() => AppManager.I.NavigationManager.GoToAnturaSpace());

            var tutorialManager = gameObject.GetComponentInChildren<RewardsTutorialManager>();
            tutorialManager.HandleStart();
        }

        void OnDestroy()
        {
            btAnturaTween.Kill();
        }

        public void ShowReward()
        {
            StartCoroutine(StartReward());
        }

        IEnumerator StartReward()
        {
            if (FirstContactManager.I.IsFinished()) {
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
            if (FirstContactManager.I.IsFinished()) {
                AnturaSpaceBtton.gameObject.SetActive(true);
                btAnturaTween = AnturaSpaceBtton.transform.DOScale(0.1f, 0.4f).From().SetEase(Ease.OutBack);
            }
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
            if (FirstContactManager.I.IsInPhase(FirstContactPhase.Reward_FirstBig)) {
                return AppManager.I.Player.RewardsUnlocked.Find(r => r.Type == RewardTypes.reward);
            } else {
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
            if (FirstContactManager.I.IsInPhase(FirstContactPhase.Reward_FirstBig))
                FirstContactManager.I.CompleteCurrentPhase();

            AppManager.I.NavigationManager.GoToNextScene();
        }
    }
}