using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EA4S
{

    public class PlaySessionResultManager : MonoBehaviour
    {

        void Start()
        {
            // Navigation manager 
            NavigationManager.I.CurrentScene = AppScene.PlaySessionResult;

            // Calculate items to unlock count
            int itemsToUnlock = NavigationManager.I.CalculateUnlockItemCount();

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
            objs = GameResultUI.ShowEndsessionResult(NavigationManager.I.UseEndSessionResults(), itemAlreadyUnlocked);

            for (int i = 0; i < objs.Length - oldRewards.Count; i++) {
                // if necessary add one new random reward not to be unlocked!
                oldRewards.Add(RewardSystemManager.GetNextRewardPack()[0]);
            }

            LogManager.I.LogPlaySessionScore(TeacherAI.I.journeyHelper.GetCurrentPlaySessionData().Id, objs.Length);
            // save max progression (internal check if necessary)
            NavigationManager.I.MaxJourneyPosistionProgress();

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