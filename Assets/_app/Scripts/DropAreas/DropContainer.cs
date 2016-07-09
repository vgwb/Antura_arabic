using UnityEngine;
using System.Collections.Generic;

namespace EA4S {

    public class DropContainer : MonoBehaviour {

        public List<DropSingleArea> Aree;
        int actualAreaIndex = 0;

        /// <summary>
        /// Setup done. Set first as active.
        /// </summary>
        public void SetupDone() {
            activateActualArea();
        }

        public void NextArea() {
            if (actualAreaIndex < Aree.Count -1)
                actualAreaIndex++;
            else { 
                // TODO: quick and dirty -> change soon as possible.
                AppManager.Instance.Modules.SceneModule.LoadSceneWithTransition("game_FastCrowd");
                Debug.Log("Win");
            }
            activateActualArea();
        }

        void activateActualArea() {
            foreach (var item in Aree) {
                item.AreaState = DropSingleArea.State.disabled;
            }
            if (actualAreaIndex < Aree.Count)
                Aree[actualAreaIndex].AreaState = DropSingleArea.State.enabled;
        }
        
    }
}