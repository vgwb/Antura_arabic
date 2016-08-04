using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using Panda;
using Lean;
using System;

namespace EA4S
{
    [RequireComponent(typeof(LetterObjectView))]
    /// <summary>
    /// Manage player drag behaviour.
    /// </summary>
    public class Hangable : MonoBehaviour
    {

        float holdSfxStartTime;
        bool holdSfxPlayed;

        #region tasks

        [Task]
        public bool OnDrag = false;
        LetterObjectView letterView = null;

        void Start()
        {
            letterView = GetComponent<LetterObjectView>();
        }

        [Task]
        public bool ToBeRelease = false;

        #endregion

        #region input

        public float HoldThreshold = 0.25f;
        float startMouseDown = -1;

        void OnMouseDown()
        {
            startMouseDown = Time.time;
        }

        void OnMouseUp()
        {
            startMouseDown = -1;
            if (OnDrag)
                OnLongTap();
            else
                OnShortTap();
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
            if (OnLetterHangOff != null)
                OnLetterHangOff(letterView);
        }

        #endregion

        void Update()
        {
            if (startMouseDown > 0 && Time.time - startMouseDown > HoldThreshold) {
                if (!OnDrag)
                    OnHoldStart();
                OnDrag = true;
            } else {
                OnDrag = false;
                holdSfxPlayed = false;
            }
            if (OnDrag) {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 100)) {
                    transform.position = hit.point;
                }

                if (Time.time > holdSfxStartTime && !holdSfxPlayed) {
                    holdSfxPlayed = true;
                    AudioManager.I.PlaySfx(Sfx.LetterHold);
                }
            }
        }

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
