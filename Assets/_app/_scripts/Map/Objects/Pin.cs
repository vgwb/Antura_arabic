using Antura.Core;
using Antura.UI;
using DG.DeExtensions;
using DG.Tweening;
using System.Collections.Generic;
using Antura.Animation;
using DG.Tweening.Core;
using UnityEngine;

namespace Antura.Map
{
    /// <summary>
    /// A pin on the map. 
    /// Defines a PlaySession. Either assessment or minigame.
    /// </summary>
    public class Pin : MonoBehaviour, IMapLocation
    {
        [HideInInspector]
        public JourneyPosition journeyPosition;

        [Header("References")]
        public Dot mainDot;
        [HideInInspector]
        public Rope rope;
        [HideInInspector]
        public List<Dot> dots = new List<Dot>();

        public GameObject pinV1;
        public GameObject pinV2;
        public GameObject pinAssessment;

        public GameObject playButtonGO;
        public GameObject lockedButtonGO;

        public GameObject roadSignGO;
        public TextRender roadSignTextUI;

        [HideInInspector]
        public GameObject currentPinMesh;

        public PlaySessionStateFeedback playSessionFeedback;

        [HideInInspector]
        public bool isLocked;

        private Transform shadowTr;

        [Header("Animation")]
        private Vector3 startPinPosition;
        private Vector3 startRopeScale;

        public Vector3 Position
        {
            get { return transform.position; }
        }

        public JourneyPosition JourneyPos
        {
            get { return journeyPosition; }
        }

        // Sequential index for the stage map this pin is in
        public int pinIndex;

        public void Initialise(int _pinIndex, JourneyPosition _journeyPosition)
        {
            pinIndex = _pinIndex;
            journeyPosition = _journeyPosition;

            name = "Pin_" + _journeyPosition;

            // Road sign with LB
            roadSignGO.SetActive(journeyPosition.PlaySession == 1);
            roadSignTextUI.SetText(journeyPosition.ToDisplayedString(false));

            // Choosing the correct PIN based on the journey position
            pinV1.gameObject.SetActive(false);
            pinV2.gameObject.SetActive(false);
            pinAssessment.gameObject.SetActive(false);
            if (journeyPosition.IsAssessment()) {
                currentPinMesh = pinAssessment;
            } else {
                if (journeyPosition.LearningBlock % 2 == 0) {
                    currentPinMesh = pinV1;
                } else {
                    currentPinMesh = pinV2;
                }
            }
            currentPinMesh.gameObject.SetActive(true);

            shadowTr = transform.Find("shadow");

            playButtonGO.SetActive(false);
            lockedButtonGO.SetActive(false);

            HandlePlayerClose(false);
        }

        #region Appear / Disappear

        private bool appeared = false;

        public bool Appeared { get { return appeared; } }

        public void Disappear()
        {
            appeared = false;
            startPinPosition = currentPinMesh.transform.position;
            if (rope != null) {
                startRopeScale = rope.meshRenderer.transform.localScale;
            }

            if (journeyPosition.IsAssessment())
            {
                //currentPinMesh.transform.localScale = Vector3.zero;// = startPinPosition + Vector3.down * 20;
            }
            else
            {
                currentPinMesh.transform.position = startPinPosition + Vector3.up * 60;
            }

            mainDot.transform.SetLocalScale(6f);
            if (rope != null) {
                rope.meshRenderer.transform.SetLocalScale(0);
            }
            foreach (var dot in dots) {
                dot.Disappear();
            }

            shadowTr.SetLocalScale(0);
            currentPinMesh.gameObject.SetActive(false);
            //playSessionFeedback.gameObject.SetActive(false);
        }

        public void Appear(float duration)
        {
            if (appeared) { return; }
            appeared = true;

            currentPinMesh.gameObject.SetActive(true);

            if (journeyPosition.IsAssessment())
            {
                currentPinMesh.transform.DOScale(Vector3.one, duration * 0.5f).SetEase(Ease.OutElastic);
            }
            else
            {
                currentPinMesh.transform.DOMove(startPinPosition, duration * 0.5f);
            }

            mainDot.transform.DOScale(Vector3.one * 6, duration * 0.5f).SetEase(Ease.OutElastic).SetDelay(duration * 0.5f).OnComplete(
                () =>
                playSessionFeedback.gameObject.SetActive(true)  // make the feedback appear at the end
                );
            shadowTr.DOScale(Vector3.one * 12.5f, duration * 0.5f).SetEase(Ease.OutElastic).SetDelay(duration * 0.5f);

            if (rope != null) { rope.meshRenderer.transform.DOScale(startRopeScale, duration * 0.5f).SetDelay(duration * 0.5f); }
        }

        public void FlushAppear()
        {
            if (appeared) { return; }
            appeared = true;
            currentPinMesh.gameObject.SetActive(true);
            currentPinMesh.transform.position = startPinPosition;
            currentPinMesh.transform.localScale = Vector3.one;
            mainDot.transform.localScale = Vector3.one * 6;
            shadowTr.transform.localScale = Vector3.one * 12.5f;
            playSessionFeedback.gameObject.SetActive(true);

            if (rope != null) { rope.meshRenderer.transform.localScale = startRopeScale; }
        }

        #endregion

        #region Locking

        public void SetUnlocked()
        {
            isLocked = false;
            //mainDot.gameObject.SetActive(true);

            playSessionFeedback.ShowUnhighlightedInfo();
        }

        public void SetLocked()
        {
            isLocked = true;
            //mainDot.gameObject.SetActive(false);

            playSessionFeedback.HideAllInfo();
        }

        #endregion

        #region PlayerPin Interaction

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                HandlePlayerClose(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                HandlePlayerClose(false);
            }
        }

        private Sequence playerCloseTween;

        void HandlePlayerClose(bool choice)
        {
            if (playerCloseTween == null)
            {
                float duration = 0.75f;
                playerCloseTween = DOTween.Sequence();
                playerCloseTween.Join(currentPinMesh.transform.DOScale(Vector3.zero, duration).SetEase(Ease.InOutCubic));
                playerCloseTween.Join(playSessionFeedback.surpriseGO.transform.DOMove(playSessionFeedback.surpriseGO.transform.position + Vector3.up * 15, duration).SetEase(Ease.OutElastic));
                playerCloseTween.Pause();
                playerCloseTween.SetAutoKill(false);
            }

            playSessionFeedback.surpriseGO.GetComponent<Surprise3D>().SetPulsing(choice); 

            if (choice)
            {
                playerCloseTween.PlayForward();
            }
            else
            {
                playerCloseTween.PlayBackwards();
            }
        }

        #endregion

        #region Highlight

        bool interactionEnabled = true;
        public void EnableInteraction(bool choice)
        {
            interactionEnabled = choice;
            if (!interactionEnabled) {
                mainDot.SetAsNothing();
            } else {
                mainDot.Highlight(true);    // re-highlight the selected pin
            }
        }

        public bool IsInteractionEnabled { get { return interactionEnabled; } }

        public void Select(bool choice)
        {
            if (!interactionEnabled) {
                mainDot.SetAsNothing();
                return;
            }

            // 3D buttons (DEPRECATED)
            //lockedButtonGO.SetActive(choice && isLocked);
            //playButtonGO.SetActive(choice && !isLocked);

            //Debug.Log("SELECTED " + this.name + ": " + choice);

            mainDot.Highlight(choice);
            if (choice) {
                if (isLocked) {
                    mainDot.SetAsLock();
                } else {
                    mainDot.SetAsPlay();
                }
            } else {
                mainDot.SetAsNothing();
            }

            if (!isLocked) {
                playSessionFeedback.Highlight(choice);
            }
        }

        #endregion

        #region Play Session State

        public void SetPlaySessionState(PlaySessionState playSessionState)
        {
            playSessionFeedback.Initialise(journeyPosition, playSessionState);
        }

        #endregion
    }
}