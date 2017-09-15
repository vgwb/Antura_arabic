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
                        MoveToDot(dot.playerPosIndex);

                    }
                    else if (hit.collider.CompareTag("Pin"))
                    {
                        var pin = hit.collider.transform.gameObject.GetComponent<Pin>();
                        if (pin.isLocked) return;

                        AudioManager.I.PlaySound(Sfx.UIButtonClick);
                        MoveToDot(pin.dot.playerPosIndex);
                    }
                }
            }
        }

        private int CurrentPlayerPosIndex {
            get { return stageMap.currentPlayerPosIndex; }
        }

        #region Movement

        private int CurrentTargetPosIndex
        {
            get { return StageMapsManager.GetPosIndexFromJourneyPosition(stageMap, StageMapsManager.CurrentJourneyPosition); }
        }

        public void MoveToNextDot()
        {
            MoveToDot(CurrentTargetPosIndex + 1);
        }

        public void MoveToPreviousDot()
        {
            MoveToDot(CurrentTargetPosIndex - 1);
        }

        public void MoveToJourneyPosition(JourneyPosition journeyPosition)
        {
            MoveToDot(StageMapsManager.GetPosIndexFromJourneyPosition(stageMap, journeyPosition)); 
        }

        private void MoveToDot(int dotIndex)
        {
            //if (dotIndex == CurrentTargetPosIndex) return;

            if (CanMoveTo(dotIndex))
            {
                int lastIndex = CurrentPlayerPosIndex;
                AnimateToPlayerPosition(dotIndex);
                LookAtPin(dotIndex < lastIndex, true, stageMap.GetCurrentPlayerPosJourneyPosition());
            }
        }

        private bool CanMoveTo(int dotIndex)
        {
            return dotIndex >= 0 &&
                   (dotIndex < stageMap.mapLocations.Count) &&
                   (dotIndex <= stageMap.maxPlayerPosIndex);
        }

        public void ForceToJourneyPosition(JourneyPosition journeyPosition, bool justVisuals = false)
        {
            int posIndex = StageMapsManager.GetPosIndexFromJourneyPosition(stageMap, journeyPosition);
            ForceToPlayerPosition(posIndex, justVisuals);
            LookAtNextPin(false);
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

        private Coroutine animatePositionCO;
        void AnimateToPlayerPosition(int newIndex)
        {
            StopAnimation();
            animatePositionCO = StartCoroutine(AnimateToPlayerPositionCO(newIndex));
        }

        public void StopAnimation(bool stopWhereItIs = true)
        {
            if (animatePositionCO != null && isAnimating)
            {
                StopCoroutine(animatePositionCO);
                animatePositionCO = null;
                if (stopWhereItIs)
                {
                    UpdatePlayerJourneyPosition(stageMap.GetCurrentPlayerPosJourneyPosition());
                }
            }
        }

        IEnumerator AnimateToPlayerPositionCO(int targetIndex)
        {
            isAnimating = true;
            if (onMoveStart != null) onMoveStart();
            CheckMovementButtonsEnabling();
            int tmpCurrentIndex = stageMap.currentPlayerPosIndex;
            UpdatePlayerJourneyPosition(stageMap.mapLocations[targetIndex].JourneyPos);
            while (tmpCurrentIndex != targetIndex)
            {
                float stepDuration = Mathf.Max(0.1f, 0.5f / Mathf.Abs(targetIndex - tmpCurrentIndex));
                bool isAdvancing = targetIndex > tmpCurrentIndex;
                tmpCurrentIndex += isAdvancing ? 1 : -1;
                LookAtPin(!isAdvancing, true, stageMap.mapLocations[tmpCurrentIndex].JourneyPos);
                var nextPos = stageMap.mapLocations[tmpCurrentIndex].Position;
                yield return MoveToCO(nextPos, stepDuration);
                stageMap.currentPlayerPosIndex = tmpCurrentIndex;
            }

            CheckMovementButtonsEnabling();
            isAnimating = false;
            if (onMoveEnd != null) onMoveEnd();
        }

        void ForceToPlayerPosition(int newIndex, bool justVisuals = false)
        {
            //Debug.Log("Forcing to " + newIndex);
            stageMap.currentPlayerPosIndex = newIndex;
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