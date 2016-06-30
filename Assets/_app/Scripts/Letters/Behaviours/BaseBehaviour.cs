using UnityEngine;
using System.Collections;
using System;

namespace CGL.Antura {
    public class BaseBehaviour : MonoBehaviour, IBehaviour {

        void OnEnable() {
            OnStartBehaviour();
        }

        void Update() {
            OnUpdateBehaviour();
        }

        void OnDisable() {
            OnEndBehaviour();
        }

        public virtual void OnStartBehaviour() { }
        public virtual void OnUpdateBehaviour() { }
        public virtual void OnEndBehaviour() { }
    }

    public interface IBehaviour {
        void OnStartBehaviour();
        void OnUpdateBehaviour();
        void OnEndBehaviour();
    }
}
