using Antura.Audio;
using Antura.Core;
using DG.Tweening;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Antura.Map
{
    /// <summary>
    /// The pin representing the player on the map.
    /// </summary>
    public class PlayerPin : MonoBehaviour
    {
        [Header("References")]
        public StageMap stageMap;
        public FingerStage swipeScript;

        [Header("UIButtons")]
        public GameObject moveRightButton;
        public GameObject moveLeftButton;

        // Animation
        Tween moveTween, rotateTween;

        #region Initialisation

        void Start()
        {
            StartFloatingAnimation();

            if (!AppManager.I.Player.IsFirstContact()) {
                CheckMovementButtonsEnabling();
            }
        }

        void OnDestroy()
        {
            moveTween.Kill();
            rotateTween.Kill();
        }

        #endregion

        #region Animation

        void StartFloatingAnimation()
        {
            transform.DOBlendableMoveBy(new Vector3(0, 1, 0), 1).SetLoops(-1, LoopType.Yoyo);
        }

        #endregion

        void LateUpdate()
        {
            // @note: using late update so this interaction happens after FingerStage (so that touch swipe takes precedence)
            if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject() && !swipeScript.isSwiping) {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                RaycastHit hit;
                int layerMask = 1 << 15;
                if (Physics.Raycast(ray, out hit, 500, layerMask)) {
                    if (hit.collider.CompareTag("Rope")) {
                        Rope selectedRope = hit.transform.parent.gameObject.GetComponent<Rope>();
                        int nUnlockedDots = selectedRope.dots.Count(x => !x.isLocked);
                        if (nUnlockedDots == 0) {
                            return;
                        }

                        int closerDotIndex = 0;
                        if (nUnlockedDots > 1) {
                            float distaceHitToDot = 1000;
                            closerDotIndex = 0;

                            for (int i = 0; i < nUnlockedDots; i++) {
                                var distanceHitBefore = Vector3.Distance(hit.point,
                                    selectedRope.dots[i].transform.position);
                                if (distanceHitBefore < distaceHitToDot) {
                                    distaceHitToDot = distanceHitBefore;
                                    closerDotIndex = i;
                                }
                            }
                        }

                        AudioManager.I.PlaySound(Sfx.UIButtonClick);
                        var dot = selectedRope.dots[closerDotIndex];
                        TeleportToDot(dot);
                    } else if (hit.collider.CompareTag("Pin")) {
                        var pin = hit.collider.transform.gameObject.GetComponent<Pin>();
                        if (pin.isLocked) return;

                        AudioManager.I.PlaySound(Sfx.UIButtonClick);
                        TeleportToPin(pin);
                    }
                }
            }
        }

        private int CurrentPlayerPosIndex {
            get { return stageMap.currentPlayerPosIndex; }
        }

        #region Movement

        private void TeleportToDot(Dot dot)
        {
            ForceToPlayerPosition(dot.playerPosIndex);
            LookAtNextPin(false);
        }

        private void TeleportToPin(Pin pin)
        {
            ForceToPlayerPosition(pin.dot.playerPosIndex);
            LookAtNextPin(false);
        }

        public void MoveToNextDot()
        {
            if ((CurrentPlayerPosIndex < (stageMap.mapLocations.Count - 1)) &&
                (CurrentPlayerPosIndex != stageMap.maxPlayerPosIndex)) {
                // We can advance
                AnimateToPlayerPosition(CurrentPlayerPosIndex + 1);
                LookAtNextPin(true);
            }
        }

        public void MoveToPreviousDot()
        {
            if (CurrentPlayerPosIndex > 0) {
                // We can retract
                AnimateToPlayerPosition(CurrentPlayerPosIndex - 1);
                LookAtPreviousPin(true);
            }
        }

        public void ResetPlayerPosition()
        {
            var current_lb = AppManager.I.Player.CurrentJourneyPosition.LearningBlock;
            var current_ps = AppManager.I.Player.CurrentJourneyPosition.PlaySession;

            if (AppManager.I.Player.IsAssessmentTime()) {
                // Player is on a pin
                var pin = stageMap.PinForLB(current_lb);
                TeleportToPin(pin);
            } else {
                // Player is on a dot
                var dot = stageMap.PinForLB(current_lb).rope.DotForPS(current_ps);
                TeleportToDot(dot);
            }
        }

        public void ResetPlayerPositionAfterStageChange(bool comingFromHigherStage)
        {
            if (comingFromHigherStage) {
                ForceToPlayerPosition(stageMap.maxPlayerPosIndex);
                LookAtPreviousPin(false);
            } else {
                ForceToPlayerPosition(0);
                LookAtNextPin(false);
            }
        }

        void AnimateToPlayerPosition(int newIndex)
        {
            //Debug.Log("Animating to " + newIndex);
            stageMap.currentPlayerPosIndex = newIndex;
            MoveTo(stageMap.GetCurrentPlayerPosition(), true);

            AppManager.I.Player.SetCurrentJourneyPosition(stageMap.GetCurrentPlayerJourneyPosition());
            CheckMovementButtonsEnabling();
        }

        void ForceToPlayerPosition(int newIndex)
        {
            //Debug.Log("Forcing to " + newIndex);
            stageMap.currentPlayerPosIndex = newIndex;
            MoveTo(stageMap.GetCurrentPlayerPosition(), false);

            AppManager.I.Player.SetCurrentJourneyPosition(stageMap.GetCurrentPlayerJourneyPosition());
            CheckMovementButtonsEnabling();
        }

        #endregion

        #region LookAt

        void LookAtNextPin(bool animated)
        {
            LookAt(false, animated);
        }

        void LookAtPreviousPin(bool animated)
        {
            LookAt(true, animated);
        }

        void LookAt(bool lookAtPrevious, bool animated)
        {
            var current_lb = AppManager.I.Player.CurrentJourneyPosition.LearningBlock;
            //Debug.Log("Looking " + current_lb + " prev? " + lookAtPrevious);

            rotateTween.Kill();

            Quaternion currRotation = transform.rotation;

            // Target rotation 
            int fromLb = current_lb - 1;
            int toLb = current_lb;
            if (lookAtPrevious) {
                fromLb = current_lb;
                toLb = current_lb - 1;
            }
            var lookingFromTr = stageMap.PinForLB(fromLb).transform;
            var lookingToTr = stageMap.PinForLB(toLb).transform;
            Quaternion toRotation = Quaternion.LookRotation(lookingToTr.transform.position - lookingFromTr.transform.position, Vector3.up);
            // Debug.Log("Current " + currRotation + " To " + toRotation);

            if (animated) {
                transform.rotation = currRotation;
                rotateTween = transform.DORotate(toRotation.eulerAngles, 0.3f).SetEase(Ease.InOutQuad);
            } else {
                transform.rotation = toRotation;
            }
        }

        #endregion

        #region Actual Movement

        // If animate is TRUE, animates the movement, otherwise applies the movement immediately
        void MoveTo(Vector3 position, bool animate)
        {
            //Debug.Log("Moving to " + position);
            if (moveTween != null) {
                moveTween.Complete();
            }
            if (animate) {
                moveTween = transform.DOMove(position, 0.25f);
            } else {
                transform.position = position;
            }
        }

        #endregion

        #region UI

        public void CheckMovementButtonsEnabling()
        {
            //Debug.Log("Enabling buttons for " + CurrentPlayerPosIndex);
            if (CurrentPlayerPosIndex == 0) {
                if (stageMap.maxPlayerPosIndex == 0) {
                    moveRightButton.SetActive(false);
                    moveLeftButton.SetActive(false);
                } else {
                    moveRightButton.SetActive(false);
                    moveLeftButton.SetActive(true);
                }
            } else if (CurrentPlayerPosIndex == stageMap.maxPlayerPosIndex) {
                moveRightButton.SetActive(true);
                moveLeftButton.SetActive(false);
            } else {
                moveRightButton.SetActive(true);
                moveLeftButton.SetActive(true);
            }
        }

        #endregion
    }
}