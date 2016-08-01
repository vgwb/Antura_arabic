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

            // here we set the Rewards base on current progression level (playsession -1 because Rewards appear when playsession is already incremented)
            if ((AppManager.Instance.PlaySession -1) == 1) {
                AnturaController.SetPreset(1);
                LoggerEA4S.Log("app", "Reward", "get_reward", "1");
            } else if ((AppManager.Instance.PlaySession - 1) == 2) {
                AnturaController.SetPreset(2);
                LoggerEA4S.Log("app", "Reward", "get_reward", "2");
            } else if ((AppManager.Instance.PlaySession - 1) > 2) {
                AnturaController.SetPreset(3);
                LoggerEA4S.Log("app", "Reward", "get_reward", "3");
            }
            LoggerEA4S.Save();


            // Wait for animation to complete
            RewardsAnimator animator = this.GetComponent<RewardsAnimator>();
            while (!animator.IsComplete)
                yield return null;

            ContinueScreen.Show(Continue, ContinueScreenMode.Button);
        }

        public void Continue()
        {
            // if we just did Assestment then go mood
            if ((AppManager.Instance.PlaySession - 1) > 2) {
                GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("app_Mood");
            } else {
                GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("app_Journey");
            }

        }

    }
}