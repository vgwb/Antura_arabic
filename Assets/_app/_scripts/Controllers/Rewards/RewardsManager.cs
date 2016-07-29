using UnityEngine;
using System.Collections;
using ModularFramework.Core;

namespace EA4S
{
    public class RewardsManager : MonoBehaviour
    {


        void Start() {
            ContinueScreen.Show(Continue, ContinueScreenMode.Button);
        }

        public void Continue() {
            GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("app_Journey");
        }

    }
}