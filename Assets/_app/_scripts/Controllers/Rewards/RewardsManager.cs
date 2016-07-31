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
            AudioManager.I.PlayMusic(Music.Theme4);

            // Wait for animation to complete
            RewardsAnimator animator = this.GetComponent<RewardsAnimator>();
            while (!animator.IsComplete)
                yield return null;

            ContinueScreen.Show(Continue, ContinueScreenMode.Button);
        }

        public void Continue()
        {
            // if we just did Assestment then go mood
            if (AppManager.Instance.PlaySession > 2) {
                GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("app_Mood");
            } else {
                GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("app_Journey");
            }

        }

    }
}