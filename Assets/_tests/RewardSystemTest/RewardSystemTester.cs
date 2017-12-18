using Antura.Core;
using UnityEngine;
using DG.DeInspektor.Attributes;

namespace Antura.Test.Rewards
{ 
    public class RewardSystemTester : MonoBehaviour
    {

        [DeMethodButton("Print Unlocks")]
        void PrintCurrentUnlocks()
        {
            string s = ("We unlocked " + AppManager.I.RewardSystemManager.GetUnlockedRewardsCount());
            foreach (var unlockedRewardPack in AppManager.I.RewardSystemManager.GetUnlockedRewardPacks())
            {
                s += ("\n- " + unlockedRewardPack);
            }
            Debug.Log(s);
        }

        [DeMethodButton("Reset Unlocks")]
        void ResetUnlocks()
        {
            AppManager.I.RewardSystemManager.ResetRewardsUnlockData();
            Debug.Log("Unlocks reset!");
        }

        [DeMethodButton("Load Unlocks")]
        void LoadUnlocks()
        {
            AppManager.I.Player.LoadRewardPackUnlockDataList();
            Debug.Log("Unlocks loaded!");
        }



        [DeMethodButton("Unlock First Set")]
        void UnlockFirstSet()
        {
            AppManager.I.RewardSystemManager.UnlockFirstSetOfRewards();
        }

        [DeMethodButton("Unlock PS 1.1.1", 0, 1, 1, 1)]
        [DeMethodButton("Unlock PS 1.1.2", 1, 1, 1, 2)]
        [DeMethodButton("Unlock PS 1.1.100", 2, 1, 1, 100)]
        [DeMethodButton("Unlock PS 1.2.1", 3, 1, 2, 1)]
        void UnlockJP (int stage, int lb, int ps)
        {
            AppManager.I.RewardSystemManager.UnlockRewardPacksForJourneyPosition(new JourneyPosition(stage, lb, ps));
        }

        [DeMethodButton("Unlock Everything")]
        void UnlockEverything()
        {
            AppManager.I.RewardSystemManager.UnlockAllRewardPacks();
        }

        [DeMethodButton("Unlock Missing")]
        void UnlockMissing()
        {
            AppManager.I.RewardSystemManager.UnlockAllMissingRewardPacks();
        }

    }
}