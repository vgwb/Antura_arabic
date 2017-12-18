using Antura.Core;
using System.Linq;
using Antura.Helpers;
using UnityEngine;

namespace Antura.Rewards
{
    /// <summary>
    /// Manager for the Play Session Result scene.
    /// Accessed a play session is completed.
    /// </summary>
    public class PlaySessionResultManager : SceneBase
    {
        protected override void Start()
        {
            base.Start();

            // Compute numbers we need to unlock
            var nItemsToUnlock = AppManager.I.NavigationManager.CalculateUnlockItemCount();
            var earnedStars = AppManager.I.NavigationManager.CalculateStarsCount();

            var rewardsAlreadyUnlocked = AppManager.I.RewardSystemManager.GetRewardPacksUnlockedInJourneyPosition(AppManager.I.Player.CurrentJourneyPosition).ToList();
            var nItemsAlreadyUnlocked = rewardsAlreadyUnlocked.Count();

            // TODO: fix this, is it needed?
            for (var i = 0; i < nItemsToUnlock - nItemsAlreadyUnlocked; i++)
            {
                // if necessary, add one new random reward unlocked for that JP
                var newRewardToUnlock =
                    AppManager.I.RewardSystemManager.GenerateRewardPacksForJourneyPosition(AppManager.I.Player.CurrentJourneyPosition).RandomSelectOne();
                rewardsAlreadyUnlocked.Add(newRewardToUnlock);
               // AppManager.I.Player.AddRewardUnlocked(newRewardToUnlock);
            }

            // Show UI result and unlock transform parent where show unlocked items
            var objs = GameResultUI.ShowEndsessionResult(AppManager.I.NavigationManager.UseEndSessionResults(), nItemsAlreadyUnlocked);

            // TODO: fix this, is it needed?
            for (var i = 0; i < objs.Length - rewardsAlreadyUnlocked.Count; i++)
            {
                // if necessary add one new random reward not to be unlocked!
                var newRewardToUnlock =
                  AppManager.I.RewardSystemManager.GenerateRewardPacksForJourneyPosition(AppManager.I.Player.CurrentJourneyPosition).RandomSelectOne();
                rewardsAlreadyUnlocked.Add(newRewardToUnlock);
            }

            LogManager.I.LogPlaySessionScore(AppManager.I.JourneyHelper.GetCurrentPlaySessionData().Id, earnedStars);


            if (NavigationManager.TEST_SKIP_GAMES) { earnedStars = 3; }

            AppManager.I.Teacher.logAI.UnlockVocabularyDataForJourneyPosition(AppManager.I.Player.CurrentJourneyPosition);
            // save max progression (internal check if necessary)
            if (earnedStars > 0) {
                // only if earned at least one star
                AppManager.I.Player.AdvanceMaxJourneyPosition();
            }

            // for any rewards mount them model on parent transform object (objs)
            for (int i = 0; i < rewardsAlreadyUnlocked.Count && i < objs.Length; i++) {
                var matPair = AppManager.I.RewardSystemManager.GetMaterialPairForPack(rewardsAlreadyUnlocked[i]);
                    ModelsManager.MountModel(rewardsAlreadyUnlocked[i].baseId,
                    objs[i].transform,
                    matPair
                );
            }
        }
    }
}