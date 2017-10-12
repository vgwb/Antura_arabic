using System.Collections;
using Antura.Audio;
using Antura.Core;
using DG.Tweening;
using System.Linq;
using Antura.Teacher;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Antura.Map
{
    /// <summary>
    /// The pin representing the player on the map.
    /// The player pin will move from one Pin to the next
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
        private Tween moveTween, rotateTween;
        private bool isAnimating = false;

        public bool IsAnimating { get { return isAnimating; } }

        public System.Action onMoveStart, onMoveEnd;

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
                if (Physics.Raycast(ray, out hit, 500, layerMask))
                {
                    // TODO: no more rope clicking!
                    /*
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
                        MoveToPin(dot.pinIndex);

                    }
                    else */
                    if (hit.collider.CompareTag("Pin"))
                    {
                        var pin = hit.collider.transform.gameObject.GetComponent<Pin>();
                        if (pin.isLocked) return;

                        AudioManager.I.PlaySound(Sfx.UIButtonClick);
                        MoveToPin(pin.pinIndex);
                    }
                }
            }
        }

        private int CurrentPinIndex {
            get { return stageMap.currentPinIndex; }
        }

        #region Movement

        private int CurrentTargetPosIndex
        {
            get { return StageMapsManager.GetPosIndexFromJourneyPosition(stageMap, StageMapsManager.CurrentJourneyPosition); }
        }

        public void MoveToNextDot()
        {
            MoveToPin(CurrentTargetPosIndex + 1);
        }

        public void MoveToPreviousDot()
        {
            MoveToPin(CurrentTargetPosIndex - 1);
        }

        public void MoveToJourneyPosition(JourneyPosition journeyPosition)
        {
            MoveToPin(StageMapsManager.GetPosIndexFromJourneyPosition(stageMap, journeyPosition)); 
        }

        private void MoveToPin(int pinIndex)
        {
            //if (pinIndex == CurrentTargetPosIndex) return;

            if (CanMoveTo(pinIndex))
            {
                int lastIndex = CurrentPinIndex;
                AnimateToPin(pinIndex);
                LookAtPin(pinIndex < lastIndex, true, stageMap.GetCurrentPlayerPosJourneyPosition());
            }
        }

        private bool CanMoveTo(int pinIndex)
        {
            return pinIndex >= 0 &&
                   (pinIndex < stageMap.mapLocations.Count) &&
                   (pinIndex <= stageMap.maxPinIndex);
        }

        public void ForceToJourneyPosition(JourneyPosition journeyPosition, bool justVisuals = false)
        {
            int posIndex = StageMapsManager.GetPosIndexFromJourneyPosition(stageMap, journeyPosition);
            ForceToPin(posIndex, justVisuals);
            LookAtNextPin(false);
        }

        public void ResetPlayerPositionAfterStageChange(bool comingFromHigherStage)
        {
            if (comingFromHigherStage) {
                ForceToPin(stageMap.maxPinIndex);
                LookAtPreviousPin(false);
            } else {
                ForceToPin(0);
                LookAtNextPin(false);
            }
        }

        private Coroutine animateToPinCO;
        void AnimateToPin(int newIndex)
        {
            StopAnimation();
            animateToPinCO = StartCoroutine(AnimateToPinCO(newIndex));
        }

        public void StopAnimation(bool stopWhereItIs = true)
        {
            if (animateToPinCO != null && isAnimating)
            {
                StopCoroutine(animateToPinCO);
                animateToPinCO = null;
                if (stopWhereItIs)
                {
                    UpdatePlayerJourneyPosition(stageMap.GetCurrentPlayerPosJourneyPosition());
                }
            }
        }

        IEnumerator AnimateToPinCO(int targetIndex)
        {
            isAnimating = true;
            if (onMoveStart != null) onMoveStart();
            CheckMovementButtonsEnabling();
            int tmpCurrentIndex = stageMap.currentPinIndex;
            UpdatePlayerJourneyPosition(stageMap.mapLocations[targetIndex].JourneyPos);
            while (tmpCurrentIndex != targetIndex)
            {
                float stepDuration = Mathf.Max(0.1f, 0.5f / Mathf.Abs(targetIndex - tmpCurrentIndex));
                bool isAdvancing = targetIndex > tmpCurrentIndex;
                tmpCurrentIndex += isAdvancing ? 1 : -1;
                LookAtPin(!isAdvancing, true, stageMap.mapLocations[tmpCurrentIndex].JourneyPos);
                var nextPos = stageMap.mapLocations[tmpCurrentIndex].Position;
                yield return MoveToCO(nextPos, stepDuration);
                stageMap.currentPinIndex = tmpCurrentIndex;
            }

            CheckMovementButtonsEnabling();
            isAnimating = false;
            if (onMoveEnd != null) onMoveEnd();
        }

        void ForceToPin(int newIndex, bool justVisuals = false)
        {
            //Debug.Log("Forcing to " + newIndex);
            stageMap.currentPinIndex = newIndex;
            ForceToCO(stageMap.GetCurrentPlayerPosVector());

            if (!justVisuals) UpdatePlayerJourneyPosition(stageMap.GetCurrentPlayerPosJourneyPosition());
            CheckMovementButtonsEnabling();
        }

        private void UpdatePlayerJourneyPosition(JourneyPosition journeyPos)
        {
            AppManager.I.Player.SetCurrentJourneyPosition(journeyPos, false);
            //Debug.LogWarning("Setting journey pos current: " + AppManager.I.Player.CurrentJourneyPosition);
        }

        #endregion

        #region LookAt

        void LookAtNextPin(bool animated)
        {
            LookAtPin(false, animated, AppManager.I.Player.CurrentJourneyPosition);
        }

        void LookAtPreviousPin(bool animated)
        {
            LookAtPin(true, animated, AppManager.I.Player.CurrentJourneyPosition);
        }

        void LookAtPin(bool lookAtPrevious, bool animated, JourneyPosition fromJourneyPosition)
        {
            var current_lb = fromJourneyPosition.LearningBlock;
            //Debug.Log("Looking " + current_lb + " prev? " + lookAtPrevious);

            rotateTween.Kill();

            Quaternion currRotation = transform.rotation;

            // Target rotation 
            // TODO: use pinIndex instead
            int fromPinIndex = lookAtPrevious ? CurrentPinIndex : CurrentPinIndex - 1;
            int toPinIndex = lookAtPrevious ? CurrentPinIndex -1 : CurrentPinIndex;

            var lookingFromTr = stageMap.PinForIndex(fromPinIndex).transform;
            var toPin = stageMap.PinForIndex(toPinIndex);
            var lookingToTr = toPin != null ? toPin.transform : lookingFromTr;
            Quaternion toRotation = Quaternion.LookRotation(lookingToTr.transform.position - lookingFromTr.transform.position, Vector3.up);
            // Debug.Log("Current " + currRotation + " To " + toRotation);

            if (animated) {
                transform.rotation = currRotation;
                rotateTween = transform.DORotate(toRotation.eulerAngles, 0.15f).SetEase(Ease.InOutQuad);
            } else {
                transform.rotation = toRotation;
            }
        }

        #endregion

        #region Actual Movement

        // If animate is TRUE, animates the movement, otherwise applies the movement immediately
        private IEnumerator MoveToCO(Vector3 position, float stepDuration)
        {
            //Debug.Log("Moving to " + position);
            if (moveTween != null) {
                moveTween.Kill();
            }
            moveTween = transform.DOMove(position, stepDuration).SetEase(Ease.Linear);
            yield return moveTween.WaitForCompletion();
        }

        private void ForceToCO(Vector3 position)
        {
            if (moveTween != null)
            {
                moveTween.Kill();
            }
            transform.position = position;
        }

    #endregion

    #region UI

    public void CheckMovementButtonsEnabling()
        {
            //Debug.Log("Enabling buttons for " + CurrentPinIndex);
            if (CurrentPinIndex == 0) {
                if (stageMap.maxPinIndex == 0) {
                    moveRightButton.SetActive(false);
                    moveLeftButton.SetActive(false);
                } else {
                    moveRightButton.SetActive(false);
                    moveLeftButton.SetActive(true);
                }
            } else if (CurrentPinIndex == stageMap.maxPinIndex) {
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