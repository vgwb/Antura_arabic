using UnityEngine;
using System.Collections;

namespace EA4S.Core
{
    public class ProgressionRateUnlock : MonoBehaviour {
        // TODO: https://trello.com/c/Mxjrb2in

        public int UnlockThreshold;
        public Material LockMaterial;
        public Material UnlockMaterial;

        void OnEnable() {
            // Remove UniRx refactoring request: any reactive interaction within this class must be called manually.
        }

        void Unlock(bool _unlock) {
            if (_unlock) {
                GetComponent<Renderer>().material = UnlockMaterial;
            } else {
                GetComponent<Renderer>().material = LockMaterial;
            }
        }

    }
}
