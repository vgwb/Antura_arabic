using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using Panda;
using DG.Tweening;
using Lean;

namespace EA4S {

    /// <summary>
    /// Manage player drag behaviour on letter.
    /// Only elements tagged with "Terrain" tag are valid for calculate drag position.
    /// </summary>
    [RequireComponent(typeof(LetterObjectView))]
    [RequireComponent(typeof(Collider))]
    public class Hangable : MonoBehaviour {

        /// <summary>
        /// Layer used to find ray collision and set position during drag behaviour.
        /// </summary>
        public LayerMask PositionLayer;

        float holdSfxStartTime;
        bool holdSfxPlayed;

        LetterObjectView view;
        Tweener tweener;

        #region tasks

        [Task]
        public bool OnDrag = false;
        LetterObjectView letterView = null;

        [Task]
        public bool ToBeRelease = false;

        #endregion

        #region input        
        /// <summary>
        /// Threshold to determine if is valid action for hold behaviour.
        /// </summary>
        public float HoldThreshold = 0.25f;

        /// <summary>
        /// Timestamp for mouse down.
        /// </summary>
        float startMouseDown = -1;

        /// <summary>
        /// Called when [mouse down].
        /// </summary>
        void OnMouseDown() {
            startMouseDown = Time.time;
        }

        /// <summary>
        /// Called when [mouse up].
        /// </summary>
        void OnMouseUp() {
            startMouseDown = -1;
            if (OnDrag)
                OnLongTap();
            else
                OnShortTap();
        }
        #endregion

        #region lifecycle

        void Start() {
            letterView = GetComponent<LetterObjectView>();
        }

        void FixedUpdate() {
            if (startMouseDown > 0 && Time.time - startMouseDown > HoldThreshold) {
                if (!OnDrag) {// not already il hold state then is starting hold state
                    OnHoldStart();
                    this.gameObject.layer = 2;
                }
                OnDrag = true;
                letterView.Model.State = LetterObjectState.Grab_State;
            } else { // not in hold state (not check if is in finish hold state, it is called in mouse up event)
                this.gameObject.layer = 9;
                OnDrag = false;
                holdSfxPlayed = false;
            }
            if (OnDrag) {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, PositionLayer)) { // to be investigated if layer filter realy work...
                    //if (hit.transform.gameObject.layer == 8) {
                        tweener.Kill();
                        tweener = transform.DOMove(hit.point, 0.3f);
                    //}
                }

                if (Time.time > holdSfxStartTime && !holdSfxPlayed) {
                    holdSfxPlayed = true;
                    AudioManager.I.PlaySfx(Sfx.LetterHold);
                }
            }
        }

        #endregion

        #region events delegates

        /// <summary>
        /// Whene start holdstate.
        /// </summary>
        void OnHoldStart()
        {
            if (OnLetterHangOn != null)
                OnLetterHangOn(letterView);
            AudioManager.I.PlayLetter(letterView.Model.Data.Key);
            AudioManager.I.PlayWord(letterView.Model.Data.Key);

            holdSfxStartTime = Time.time + 1;
        }

        /// <summary>
        /// On release of short tap action.
        /// </summary>
        void OnShortTap()
        {
            AudioManager.I.PlayLetter(letterView.Model.Data.Key);
            AudioManager.I.PlayWord(letterView.Model.Data.Key);
        }

        /// <summary>
        /// On release of long tap action.
        /// </summary>
        void OnLongTap()
        {
            ToBeRelease = true;
            letterView.Model.State = letterView.Model.OldState;
            if (OnLetterHangOff != null)
                OnLetterHangOff(letterView);
        }

        #endregion

        #region event subscriptions

        void OnEnable()
        {
        }



        void OnDisable()
        {
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
