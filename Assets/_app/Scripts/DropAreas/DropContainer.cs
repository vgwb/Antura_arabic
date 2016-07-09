using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

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
            for (int i = 0; i < Aree.Count; i++) {
                Aree[i].AreaState = DropSingleArea.State.disabled;
                Aree[i].transform.position = new Vector3(Aree[i].transform.position.x - (6 * i),
                                                        0.1f, 
                                                        Aree[i].transform.position.z);
                Debug.Log("");
            }
            if (actualAreaIndex < Aree.Count)
                Aree[actualAreaIndex].AreaState = DropSingleArea.State.enabled;
        }

        /// <summary>
        /// Risen on letter or world match.
        /// </summary>
        void OnItemMatch() {

            Sequence sequence = DOTween.Sequence();
            sequence.OnComplete(delegate () {
                // done callback.
            });
            sequence.Append(Aree[actualAreaIndex].transform.DOMoveX(6, 1));
            sequence.Append(Aree[actualAreaIndex-1].transform.DOMoveX(0, 1));
            
        }



    }
}