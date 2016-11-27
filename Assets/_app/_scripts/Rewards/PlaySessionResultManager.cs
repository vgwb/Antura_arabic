using UnityEngine;
using System.Collections;

namespace EA4S {

    public class PlaySessionResultManager : MonoBehaviour {

        void Start() {
            // Navigation manager 
            NavigationManager.I.CurrentScene = AppScene.PlaySessionResult;

            int itemToUnlock = NavigationManager.I.CalculateUnlockItemCount();
            GameObject[] objs = GameResultUI.ShowEndsessionResult(NavigationManager.I.UseEndSessionResults(),itemToUnlock);
            
            foreach (GameObject obj in objs) {
                ModelsManager.MountModel("reward_punk_hair", obj.transform, RewardSystemManager.GetMaterialPairFromRewardAndColor("reward_punk_hair", "red_dark", "yellow_dark"));
            }
        }

    }
}