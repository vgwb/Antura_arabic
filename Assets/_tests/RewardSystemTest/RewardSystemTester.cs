using Antura.Core;
using UnityEngine;
using DG.DeInspektor.Attributes;

namespace Antura.Test.Rewards
{ 
    public class RewardSystemTester : MonoBehaviour
    {

        [DeMethodButton("Test Unlocks")]
	    void TestUnlocks ()
        {
            AppManager.I.RewardSystemManager.UnlockRewardPacksForJourneyPosition(new JourneyPosition(1, 2, 2));
        }
	
    }
}