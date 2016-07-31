using UnityEngine;
using System.Collections;
using ModularFramework.Core;

namespace EA4S
{
    [RequireComponent(typeof(RewardsAnimator))]
    public class RewardsManager : MonoBehaviour
    {
        public Antura AnturaController;

        IEnumerator Start()
        {
            AudioManager.I.PlayMusic(Music.Theme4);

            // here we set the Rewards base on current progression level
            if (AppManager.Instance.PlaySession == 1) {
                AnturaController.SetPreset(1);
            } else if (AppManager.Instance.PlaySession == 2) {
                AnturaController.SetPreset(2);
            } else if (AppManager.Instance.PlaySession > 2) {
                AnturaController.SetPreset(3);
            }


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