using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EA4S {

    public class PlaySessionResultManager : MonoBehaviour {

        void Start() {
            // Navigation manager 
            NavigationManager.I.CurrentScene = AppScene.PlaySessionResult;

            // save max progression (internal check if necessary)
            NavigationManager.I.MaxJourneyPosistionProgress();

            // Calculate items to unlock count
            int itemsToUnlock = NavigationManager.I.CalculateUnlockItemCount();
            // counter for the previously already unlocked rewards
            int alreadyUnlockedItem = 0;
            // get rewards for this playsession result (old and/or new if is the first time unlock for this ps)
            List<RewardPack> rewards = RewardSystemManager.GetRewardPacksForPlaySession(AppManager.I.Player.CurrentJourneyPosition, itemsToUnlock, out alreadyUnlockedItem);
            // Show UI result and unlock transform parent where show unlocked items
            GameObject[] objs = new GameObject[] { };
            objs = GameResultUI.ShowEndsessionResult(NavigationManager.I.UseEndSessionResults(), alreadyUnlockedItem);
            // for any rewards mount them model on parent transform object (objs)
            for (int i = 0; i < rewards.Count && i < objs.Length; i++) {
                ModelsManager.MountModel(
                    rewards[i].ItemID,
                    objs[i].transform,
                    rewards[i].GetMaterialPair()
                    );
            }

        }

    }
}