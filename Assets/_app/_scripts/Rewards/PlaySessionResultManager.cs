using Antura.Core;
using System.Linq;

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
            var jp = AppManager.I.Player.CurrentJourneyPosition;
            var nTotalRewardPacksToUnlock = AppManager.I.NavigationManager.CalculateRewardPacksUnlockCount();
            var nEarnedStars = AppManager.I.NavigationManager.CalculateEarnedStarsCount();
            if (NavigationManager.TEST_SKIP_GAMES) { nEarnedStars = 3; }

            var rewardPacksForJourneyPosition = AppManager.I.RewardSystemManager.GetOrGenerateAllRewardPacksForJourneyPosition(jp);
            var rewardPacksUnlocked = rewardPacksForJourneyPosition.Where(x => x.IsUnlocked).ToList();

            int nRewardPacksAlreadyUnlocked = rewardPacksUnlocked.Count();
            int nNewRewardPacksToUnlock = nTotalRewardPacksToUnlock - nRewardPacksAlreadyUnlocked;

            // Unlock the new rewards
            AppManager.I.RewardSystemManager.UnlockPacks(rewardPacksForJourneyPosition, jp, nNewRewardPacksToUnlock);

            // Log various data
            LogManager.I.LogPlaySessionScore(AppManager.I.JourneyHelper.GetCurrentPlaySessionData().Id, nEarnedStars);
            AppManager.I.Teacher.logAI.UnlockVocabularyDataForJourneyPosition(AppManager.I.Player.CurrentJourneyPosition);

            // Advance journey if we earned enough stars
            if (nEarnedStars > 0)
            {
                AppManager.I.Player.AdvanceMaxJourneyPosition();
            }

            // Show UI result and unlock transform parent where show unlocked items
            var uiGameObjects = GameResultUI.ShowEndsessionResult(AppManager.I.NavigationManager.UseEndSessionResults(), nRewardPacksAlreadyUnlocked);

            // For any rewards mount them model on parent transform object (objs)
            for (int i = 0; i < rewardPacksUnlocked.Count() && i < uiGameObjects.Length; i++)
            {
                var matPair = rewardPacksUnlocked[i].GetMaterialPair();
                ModelsManager.MountModel(rewardPacksUnlocked[i].BaseId, uiGameObjects[i].transform, matPair);
            }
        }
    }
}