using UnityEngine;
using System.Collections;
using Panda;

namespace EA4S {
    [RequireComponent(typeof(LetterObjectView))]
    /// <summary>
    /// Manage player drag behaviour.
    /// </summary>
    public class Hangable : MonoBehaviour
    {
        [Task]
        public bool OnDrag = false;
        LetterObjectView letterView = null;

        void Start() {
            letterView = GetComponent<LetterObjectView>();
        }

        [Task]
        public bool ToBeRelease = false;

        void OnMouseDown() {
            OnDrag = true;
            if (OnLetterHangOff != null)
                OnLetterHangOff(letterView);
        }

        void OnMouseUp() {
            OnDrag = false;
            ToBeRelease = true;
            if (OnLetterHangOn != null)
                OnLetterHangOn(letterView);
        }

        void Update() {
            if (OnDrag) {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 100)) {
                    transform.position = hit.point;
                }
            }
        }

        #region events
        public delegate void HangAction(LetterObjectView _letterView);

        /// <summary>
        /// Start hang.
        /// </summary>
        public static event HangAction OnLetterHangOn;

        /// <summary>
        /// End hang.
        /// </summary>
        public static event HangAction OnLetterHangOff;
        #endregion

    }
}
