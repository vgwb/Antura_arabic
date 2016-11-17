using UnityEngine;
using System.Collections;

namespace EA4S {

    public class AnturaSpaceManager : MonoBehaviour {

        void Start() {
            GlobalUI.ShowPauseMenu(false); 
        }

        public void Exit() {
            AppManager.Instance.Modules.SceneModule.LoadSceneWithTransition("app_Map");
        }
    }

}