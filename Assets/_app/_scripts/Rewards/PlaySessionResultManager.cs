using UnityEngine;
using System.Collections.Generic;

namespace EA4S.Rewards
{

    /// <summary>
    /// Manager for the Play Session Result scene.
    /// Accessed a play session is completed.
    /// </summary>
    public class PlaySessionResultManager : MonoBehaviour
    {

        void Start()
        {
            // Calculate items to unlock count
            int itemsToUnlock = AppManager.I.NavigationManager.CalculateUnlockItemCount();

            List<RewardPack> oldRewards = AppManager.I.Player.RewardsUnlocked.FindAll(ru => ru.PlaySessionId == AppManager.I.Player.CurrentJourneyPosition.ToString());
            int itemAlreadyUnlocked = oldRewards.Count;
            for (int i = 0; i < itemsToUnlock - oldRewards.Count; i++) {
                // if necessary add one new random reward unlocked
                RewardPack newRewardToUnlock = RewardSystemManager.GetNextRewardPack()[0];
                oldRewards.Add(newRewardToUnlock);
                AppManager.I.Player.AddRewardUnlocked(newRewardToUnlock);
            }

            // Show UI result and unlock transform parent where show unlocked items
            GameObject[] objs = new GameObject[] { };
            objs = GameResultUI.ShowEndsessionResult(AppManager.I.NavigationManager.UseEndSessionResults(), itemAlreadyUnlocked);

            for (int i = 0; i < objs.Length - oldRewards.Count; i++) {
                // if necessary add one new random reward not to be unlocked!
                oldRewards.Add(RewardSystemManager.GetNextRewardPack()[0]);
            }

            LogManager.I.LogPlaySessionScore(TeacherAI.I.journeyHelper.GetCurrentPlaySessionData().Id, objs.Length);
            // save max progression (internal check if necessary)
            AppManager.I.NavigationManager.MaxJourneyPositionProgress();

            // for any rewards mount them model on parent transform object (objs)
            for (int i = 0; i < oldRewards.Count && i < objs.Length; i++) {
                ModelsManager.MountModel(
                    oldRewards[i].ItemID,
                    objs[i].transform,
                    oldRewards[i].GetMaterialPair()
                    );
            }

        }

    }
}