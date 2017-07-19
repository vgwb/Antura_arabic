using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Linq;

namespace Antura.Map
{
    /// <summary>
    /// The pin representing the player on the map.
    /// </summary>
    public class PlayerPin : MonoBehaviour
    {
        [Header("Stage")]
        public StageMap stageScript;

        [Header("References")]
        public FingerStage swipeScript;

        [Header("UIButtons")]
        public GameObject moveRightButton;
        public GameObject moveLeftButton;

        [Header("PinState")]
        public bool playerOverDotPin;

        // Interaction state
        float distanceNextDotToHitPoint;
        float distanceBeforelDotToHitPoint;
        float distanceActualDotToHitPoint;
        int closerDotIndex;
        Rope selectedRope;

        // Animation
        Tween moveTween, rotateTween;

        #region Initialisation

        void Start()
        {
            StartFloatingAnimation();

            if (!AppManager.I.Player.IsFirstContact()) {
                CheckButtonsEnabling();
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

            if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject() && !swipeScript.isSwiping)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                RaycastHit hit;
                int layerMask = 1 << 15;
                if (Physics.Raycast(ray, out hit, 500, layerMask))
                {
                    if (hit.collider.CompareTag("Rope"))
                    {
                        //playerOverDotPin = true;

                        Rope selectedRope = hit.transform.parent.gameObject.GetComponent<Rope>();
                        int nRopeActiveDots = selectedRope.dots.Count(x => x.gameObject.activeInHierarchy);
                        closerDotIndex = 0;
                        if (nRopeActiveDots > 1)
                        {
                            float distaceHitToDot = 1000;
                            closerDotIndex = 0;

                            for (int i = 0; i < nRopeActiveDots; i++)
                            {
                                var distanceHitBefore = Vector3.Distance(hit.point,
                                    selectedRope.dots[i].transform.position);
                                if (distanceHitBefore < distaceHitToDot)
                                {
                                    distaceHitToDot = distanceHitBefore;
                                    closerDotIndex = i;
                                }
                            }
                        }

                        var dot = selectedRope.dots[closerDotIndex];
                        TeleportToDot(dot);
                    }
                    else if (hit.collider.CompareTag("Pin"))
                    {
                        //Debug.Log("Hitting PIN");
                        var pin = hit.collider.transform.gameObject.GetComponent<Pin>();
                        TeleportToPin(pin);
                    }
                }
            }
            else
            {
                // TODO?
            //    StartCoroutine("PlayerOverDotPinToFalse");
            }
        }

        /*IEnumerator PlayerOverDotPinToFalse()
        {
            yield return new WaitForSeconds(0.3f);
            playerOverDotPin = false;
        }*/


        private int CurrentPlayerPosIndex
        {
            get { return stageScript.currentPlayerPosIndex; }
        }

        #region Movement

        private void TeleportToDot(Dot dot)
        {
            ForceToPlayerPosition(dot.playerPosIndex);
            LookAtNextPin();
        }

        private void TeleportToPin(Pin pin)
        {
            ForceToPlayerPosition(pin.dot.playerPosIndex);
            LookAtNextPin();
        }

        public void MoveToNextDot()
        {
            if ((CurrentPlayerPosIndex < (stageScript.mapLocations.Count - 1)) &&
                (CurrentPlayerPosIndex != stageScript.maxPlayerPosIndex))
            {
                // We can advance
                AnimateToPlayerPosition(CurrentPlayerPosIndex + 1);
                LookAtNextPin();
            }
        }

        public void MoveToPreviousDot()
        {
            if (CurrentPlayerPosIndex > 0)
            {
                // We can retract
                AnimateToPlayerPosition(CurrentPlayerPosIndex - 1);
                LookAtPreviousPin();
            }
        }

        public void ResetPlayerPosition()
        {
            var current_lb = AppManager.I.Player.CurrentJourneyPosition.LearningBlock;
            var current_ps = AppManager.I.Player.CurrentJourneyPosition.LearningBlock;

            if (AppManager.I.Player.IsAssessmentTime()) 
            {
                // Player is on a pin
                var pin = stageScript.PinForLB(current_lb);

                ForceToPlayerPosition(pin.dot.playerPosIndex);
                LookAtNextPin();
            }
            else
            {
                // Player is on a dot
                var dot = stageScript.PinForLB(current_lb).rope.DotForPS(current_ps);
                ForceToPlayerPosition(dot.playerPosIndex);
                LookAtNextPin();
            }
        }

        public void ResetPositionAfterStageChange()
        {
            ForceToPlayerPosition(0);
        }

        void AnimateToPlayerPosition(int newIndex)
        {
            Debug.LogError("Animating to " + newIndex);
            stageScript.currentPlayerPosIndex = newIndex;
            MoveTo(stageScript.GetCurrentPlayerPosition(), true);

            CheckButtonsEnabling();
            AppManager.I.Player.SetCurrentJourneyPosition(stageScript.GetCurrentPlayerJourneyPosition());
        }

        void ForceToPlayerPosition(int newIndex)
        {
            Debug.LogError("Forcing to " + newIndex);
            stageScript.currentPlayerPosIndex = newIndex;
            MoveTo(stageScript.GetCurrentPlayerPosition(), false);

            CheckButtonsEnabling();
            AppManager.I.Player.SetCurrentJourneyPosition(stageScript.GetCurrentPlayerJourneyPosition());
        }

        #endregion

        #region LookAt

        void LookAtNextPin()
        {
            LookAt(false);
        }

        void LookAtPreviousPin()
        {
            LookAt(true);
        }

        void LookAt(bool lookAtPrevious)
        {
            var current_lb = AppManager.I.Player.CurrentJourneyPosition.LearningBlock;
            Debug.Log("Looking " + current_lb + " prev? " + lookAtPrevious);

            if (!lookAtPrevious && current_lb == stageScript.LastLBPin().learningBlock)
            {
                transform.LookAt(stageScript.PinForLB(current_lb).transform);
                transform.rotation = Quaternion.Euler(
                    new Vector3(transform.rotation.eulerAngles.x,
                        transform.rotation.eulerAngles.y + 180,
                        transform.rotation.eulerAngles.z)
                );
                return;
            }

            rotateTween.Kill();
            Quaternion currRotation = transform.rotation;
            transform.LookAt(lookAtPrevious
                ? stageScript.PinForLB(current_lb - 1).transform.position
                : stageScript.PinForLB(current_lb).transform.position
            );
            Quaternion toRotation = transform.rotation;
            transform.rotation = currRotation;
            rotateTween = transform.DORotate(toRotation.eulerAngles, 0.3f).SetEase(Ease.InOutQuad);

            /*if (!lookAtPrevious)
            {
                if (current_lb < stageScript.nLearningBlocks)
                {
                    //transform.LookAt(stageScript.PinForLB(current_lb + 1).transform);
                }
                else
                {
                }
            }*/
        }

        #endregion

        #region Actual Movement

        // If animate is TRUE, animates the movement, otherwise applies the movement immediately
        void MoveTo(Vector3 position, bool animate)
        {
            //Debug.Log("Moving to " + position);
            if (moveTween != null)
            {
                moveTween.Complete();
            }
            if (animate)
            {
                moveTween = transform.DOMove(position, 0.25f);
            }
            else
            {
                transform.position = position;
            }
        }

        #endregion

        #region UI

        private void CheckButtonsEnabling()
        {
            if (CurrentPlayerPosIndex == 0)
            {
                if (stageScript.maxPlayerPosIndex == 0)
                {
                    moveRightButton.SetActive(false);
                    moveLeftButton.SetActive(false);
                }
                else
                {
                    // TODO: maybe no routine?
                    StartCoroutine("DeactivateButtonWithDelay", moveRightButton);
                    moveLeftButton.SetActive(true);
                }
            }
            else if (CurrentPlayerPosIndex == stageScript.maxPlayerPosIndex)
            {
                moveRightButton.SetActive(true);
                StartCoroutine("DeactivateButtonWithDelay", moveLeftButton);
            }
            else
            {
                moveRightButton.SetActive(true);
                moveLeftButton.SetActive(true);
            }
        }

        private IEnumerator DeactivateButtonWithDelay(GameObject button)
        {
            yield return new WaitForSeconds(0.1f);
            button.SetActive(false);
        }

        #endregion
    }
}