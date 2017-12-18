using Antura.Core;
using System.Linq;
using Antura.Helpers;
using UnityEngine;

namespace Antura.Rewards
{
    /// <summary>
    /// Manager for the Play Session Result scene.
    /// Accessed when a play session is completed.
    /// </summary>
    public class PlaySessionResultManager : SceneBase
    {
        protected override void Start()
        {
            base.Start();

            // Compute numbers we need to unlock
            var nItemsToUnlock = AppManager.I.NavigationManager.CalculateUnlockItemCount();
            var nEarnedStars = AppManager.I.NavigationManager.CalculateStarsCount();

            var rewardsAlreadyUnlocked = AppManager.I.RewardSystemManager.GetRewardPacksUnlockedInJourneyPosition(AppManager.I.Player.CurrentJourneyPosition).ToList();
            var nRewardsAlreadyUnlocked = rewardsAlreadyUnlocked.Count();

            // TODO: fix this, is it needed?
            for (var i = 0; i < nItemsToUnlock - nRewardsAlreadyUnlocked; i++)
            {
                // if necessary, add one new random reward unlocked for that JP
                var newRewardToUnlock =
                    AppManager.I.RewardSystemManager.GenerateRewardPacksForJourneyPosition(AppManager.I.Player.CurrentJourneyPosition).RandomSelectOne();
                rewardsAlreadyUnlocked.Add(newRewardToUnlock);
            }

            // Show UI result and unlock transform parent where show unlocked items
            var uiGameObjects = GameResultUI.ShowEndsessionResult(AppManager.I.NavigationManager.UseEndSessionResults(), nRewardsAlreadyUnlocked);

            // TODO: fix this, is it needed?
            for (var i = 0; i < uiGameObjects.Length - rewardsAlreadyUnlocked.Count; i++)
            {
                // if necessary add one new random reward not to be unlocked!
                var newRewardToUnlock =
                  AppManager.I.RewardSystemManager.GenerateRewardPacksForJourneyPosition(AppManager.I.Player.CurrentJourneyPosition).RandomSelectOne();
                rewardsAlreadyUnlocked.Add(newRewardToUnlock);
            }

            LogManager.I.LogPlaySessionScore(AppManager.I.JourneyHelper.GetCurrentPlaySessionData().Id, nEarnedStars);


            if (NavigationManager.TEST_SKIP_GAMES) { nEarnedStars = 3; }

            AppManager.I.Teacher.logAI.UnlockVocabularyDataForJourneyPosition(AppManager.I.Player.CurrentJourneyPosition);

            // save max progression (internal check if necessary)
            if (nEarnedStars > 0) {
                // only if earned at least one star
                AppManager.I.Player.AdvanceMaxJourneyPosition();
            }

            // for any rewards mount them model on parent transform object (objs)
            for (int i = 0; i < rewardsAlreadyUnlocked.Count && i < uiGameObjects.Length; i++)
            {
                var matPair = AppManager.I.RewardSystemManager.GetMaterialPairForPack(rewardsAlreadyUnlocked[i]);
                    ModelsManager.MountModel(rewardsAlreadyUnlocked[i].BaseId,
                    uiGameObjects[i].transform,
                    matPair
                );
            }
        }
    }
}