using UnityEngine;
using System.Collections;

namespace EA4S {

    public class PlaySessionResultManager : MonoBehaviour {

        void Start() {
            // Navigation manager 
            NavigationManager.I.CurrentScene = AppScene.PlaySessionResult;

            // Calculate items to unlock count
            int itemToUnlock = NavigationManager.I.CalculateUnlockItemCount();
            // Show UI result and unlock transform parent where show unlocked items
            GameObject[] objs = GameResultUI.ShowEndsessionResult(NavigationManager.I.UseEndSessionResults(),itemToUnlock);
            // retrive a random and mount model on 
            for (int i = 0; i < itemToUnlock; i++) {
                RewardPack rp = RewardSystemManager.GetNextRewardPack(RewardTypes.reward);
                ModelsManager.MountModel(
                    rp.ItemID,
                    objs[i].transform,
                    RewardSystemManager.GetMaterialPairFromRewardIdAndColorId(rp.ItemID, rp.ColorId)
                    );
            }
            
        }

    }
}