using UnityEngine;
using System.Collections;
using ModularFramework.Core;

namespace EA4S
{
    [RequireComponent(typeof(RewardsAnimator))]
    public class RewardsManager : MonoBehaviour
    {


        IEnumerator Start()
        {
            // Wait for animation to complete
            RewardsAnimator animator = this.GetComponent<RewardsAnimator>();
            while (!animator.IsComplete) yield return null;

            ContinueScreen.Show(Continue, ContinueScreenMode.ButtonFullscreen);
        }

        public void Continue() {
            GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("app_Journey");
        }

    }
}