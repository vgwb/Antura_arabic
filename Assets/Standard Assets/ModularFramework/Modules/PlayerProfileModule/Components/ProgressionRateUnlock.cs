using UnityEngine;
using System.Collections;
using ModularFramework.Core;
using UniRx;

namespace ModularFramework.Modules {
    public class ProgressionRateUnlock : MonoBehaviour {
        // TODO: https://trello.com/c/Mxjrb2in

        public int UnlockThreshold;
        public Material LockMaterial;
        public Material UnlockMaterial;

        void OnEnable() {
            PlayerProfile pp = GameManager.Instance.PlayerProfile.ActivePlayer as PlayerProfile;
            pp.ObserveEveryValueChanged(p => p.ProgressionRate).Subscribe(_ => {
                if (pp.ProgressionRate >= UnlockThreshold)
                    Unlock(true);
            }).AddTo(this);
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
