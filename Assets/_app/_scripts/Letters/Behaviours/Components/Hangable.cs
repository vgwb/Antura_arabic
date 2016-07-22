using UnityEngine;
using System.Collections;
using Panda;
using Lean;

namespace EA4S {
    [RequireComponent(typeof(LetterObjectView))]
    /// <summary>
    /// Manage player drag behaviour.
    /// </summary>
    public class Hangable : MonoBehaviour
    {
        #region tasks

        [Task]
        public bool OnDrag = false;
        LetterObjectView letterView = null;

        void Start() {
            letterView = GetComponent<LetterObjectView>();
        }

        [Task]
        public bool ToBeRelease = false;

        #endregion

        #region input

        void OnMouseDown() {
            OnDrag = true;
            if (OnLetterHangOn != null)
                OnLetterHangOn(letterView);
        }

        void OnMouseUp() {
            OnDrag = false;
            ToBeRelease = true;
            if (OnLetterHangOff != null)
                OnLetterHangOff(letterView);
        }

        #endregion

        void Update() {
            if (OnDrag) {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 100)) {
                    transform.position = hit.point;
                }
            }
        }

        #region event subscriptions

        void OnEnable() {
            // Hook events
            //Lean.LeanTouch.OnFingerDown += OnFingerDown;
            //Lean.LeanTouch.OnFingerSet += OnFingerSet;
            //Lean.LeanTouch.OnFingerUp += OnFingerUp;
            //Lean.LeanTouch.OnFingerDrag += OnFingerDrag;
            //Lean.LeanTouch.OnFingerTap += OnFingerTap;
            //Lean.LeanTouch.OnFingerSwipe += OnFingerSwipe;
            //Lean.LeanTouch.OnFingerHeldDown += OnFingerHeldDown;
            //Lean.LeanTouch.OnFingerHeldSet += OnFingerHeld;
            //Lean.LeanTouch.OnFingerHeldUp += OnFingerHeldUp;
            //Lean.LeanTouch.OnMultiTap += OnMultiTap;
            //Lean.LeanTouch.OnDrag += OnDrag;

        }

        void OnDisable() {
        }

        #endregion

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
