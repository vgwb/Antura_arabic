using UnityEngine;
using System.Collections;

namespace EA4S {

    public class AnturaSpaceManager : MonoBehaviour {
        
        public void Exit() {
            AppManager.Instance.Modules.SceneModule.LoadSceneWithTransition("app_Rewards");
        }
    }

}