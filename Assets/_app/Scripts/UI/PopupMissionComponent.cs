using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace EA4S {
    public class PopupMissionComponent : MonoBehaviour {

        public void Show() {
            Time.timeScale = 0;

            Sequence sequence = DOTween.Sequence();
            sequence.timeScale = 1;
            sequence.InsertCallback(5, delegate() {

            });
        }

    }
}