using UnityEngine;
using System.Collections;
using System;

namespace CGL.Antura {
    public class BaseBehaviour : MonoBehaviour, IBehaviour {

        public LetterObject Model;

        void OnEnable() {
            OnStartBehaviour();
        }

        public void StartBehaviour(LetterObject _model) {
            Model = _model;
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
