using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using Antura.Core;
using DG.Tweening;

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

        float distanceNextDotToHitPoint;
        float distanceBeforelDotToHitPoint;
        float distanceActualDotToHitPoint;
        int dotCloser;
        Rope ropeSelected;
        Collider colliderRaycast;
        Tween moveTween, rotateTween;

        void Start()
        {
            Floating();

            if (!AppManager.I.Player.IsFirstContact()) {
            // TODO:    CheckButtonsEnabling();
            }
        }

        void Floating()
        {
            transform.DOBlendableMoveBy(new Vector3(0, 1, 0), 1).SetLoops(-1, LoopType.Yoyo);
        }

        void OnDestroy()
        {
            moveTween.Kill();
            rotateTween.Kill();
        }

        void FixedUpdate()
        {
            /* Debug.Log(AppManager.I.Player.CurrentJourneyPosition.Stage);
             Debug.Log(AppManager.I.Player.CurrentJourneyPosition.LearningBlock);
             Debug.Log(AppManager.I.Player.CurrentJourneyPosition.PlaySession);

             Debug.Log("Max"+AppManager.I.Player.MaxJourneyPosition.Stage);
             Debug.Log("MaxLB"+AppManager.I.Player.MaxJourneyPosition.LearningBlock);
             Debug.Log("MaxPS"+AppManager.I.Player.MaxJourneyPosition.PlaySession);  */

            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject() && (!swipeScript.swipe)) {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Debug.Log("CLICKING");

                RaycastHit hit;
                int layerMask = 1 << 15;
                if (Physics.Raycast(ray, out hit, 500, layerMask))
                {
                    if (hit.collider.CompareTag("Rope"))
                    {
                        //playerOverDotPin = true;
                        ropeSelected = hit.transform.parent.gameObject.GetComponent<Rope>();
                        int nRopeActiveDots = 0;
                        foreach (Dot dot in ropeSelected.dots)
                        {
                            if (dot.gameObject.activeInHierarchy)
                            {
                                nRopeActiveDots++;
                            }
                        }

                        if (nRopeActiveDots > 1)
                        {
                            float distaceHitToDot = 1000;
                            dotCloser = 0;

                            for (int i = 0; i < nRopeActiveDots; i++)
                            {
                                var distanceHitBefore = Vector3.Distance(hit.point,
                                    ropeSelected.dots[i].transform.position);
                                if (distanceHitBefore < distaceHitToDot)
                                {
                                    distaceHitToDot = distanceHitBefore;
                                    dotCloser = i;
                                }
                            }
                        } else
                        {
                            dotCloser = 0;
                        }

                        colliderRaycast = hit.collider;
                        TeleportToDot();
                    }
                    else if (hit.collider.CompareTag("Pin"))
                    {
                        //playerOverDotPin = true;
                        colliderRaycast = hit.collider;
                        TeleportToPin();
                    }
                    else
                    {
                        colliderRaycast = null;
                    }
                }
                else
                {
                    colliderRaycast = null;
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

        /*void LateUpdate()
        {
            if (Input.GetMouseButtonUp(0) && (!EventSystem.current.IsPointerOverGameObject()) && (colliderRaycast != null))
            {
                if (colliderRaycast.CompareTag("Rope"))
                {
                    TeleportToDot();
                    UpdateCurrentJourneyPosition();
                }
                else if (colliderRaycast.CompareTag("Pin"))
                {
                    TeleportToPin();
                    UpdateCurrentJourneyPosition();
                }
            }
        }*/

        void TeleportToDot()
        {
            ForceToPlayerPosition(ropeSelected.dots[dotCloser].playerPosIndex);
            LookAtNextPin();
            // Update JourneyPos
            //AppManager.I.Player.CurrentJourneyPosition.PlaySession = ropeSelected.dots[dotCloser].playSession;
            //AppManager.I.Player.CurrentJourneyPosition.LearningBlock = ropeSelected.dots[dotCloser].learningBlock;
            //
            //CheckButtonsEnabling();
           
            //transform.LookAt(ropeSelected.pin.transform);
        }

        void TeleportToPin()
        {
            var pin = colliderRaycast.transform.gameObject.GetComponent<Pin>();
            ForceToPlayerPosition(pin.dot.playerPosIndex);
            LookAtNextPin();

            //MoveTo(colliderRaycast.transform.position);

            // Update position index
            //stageScript.currentPlayerPosIndex = pin.dot.playerPosIndex;

            //var current_lb = AppManager.I.Player.CurrentJourneyPosition.LearningBlock;
            //var current_ps = AppManager.I.Player.CurrentJourneyPosition.LearningBlock;

            // TODO: standardize LOOKATS

            //CheckButtonsEnabling();
        }

        private int CurrentPlayerPosIndex
        {
            get { return stageScript.currentPlayerPosIndex; }
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

                //MoveTo(pin.transform.position);
                //stageScript.currentPlayerPosIndex = pin.dot.playerPosIndex;

                // TODO: standardize LOOKAT
                /*
                if (current_lb < stageScript.nLearningBlocks)
                {
                    // Look at the next pin
                    transform.LookAt(stageScript.PinForLB(current_lb + 1).transform);
                }
                else
                {
                    // Last pin: look forward
                    transform.LookAt(stageScript.PinForLB(current_lb).transform);
                    transform.rotation = Quaternion.Euler(
                        new Vector3(transform.rotation.eulerAngles.x,
                            transform.rotation.eulerAngles.y + 180,
                            transform.rotation.eulerAngles.z)
                    );
                }*/
            }
            else
            {
                // Player is on a dot
                var dot = stageScript.PinForLB(current_lb).rope.DotForPS(current_ps);

                ForceToPlayerPosition(dot.playerPosIndex);

                //MoveTo(dot.transform.position);
                //stageScript.currentPlayerPosIndex = dot.playerPosIndex;

                // TODO: standardize LOOKAT
                LookAtNextPin();
                //transform.LookAt(stageScript.PinForLB(current_lb).transform);
            }
            //CheckButtonsEnabling();
        }

        public void ResetPositionAfterStageChange()
        {
            ForceToPlayerPosition(0);

            //AppManager.I.Player.CurrentJourneyPosition.LearningBlock = 1;
            // AppManager.I.Player.CurrentJourneyPosition.PlaySession = 1;
            //LookAtNextPin();
            // UpdateCurrentJourneyPosition();
            // CheckButtonsEnabling();
        }

        void AnimateToPlayerPosition(int newIndex)
        {
            Debug.LogError("Animating to " + newIndex);
            stageScript.currentPlayerPosIndex = newIndex;
            MoveTo(stageScript.GetCurrentPlayerPosition(), true);

            CheckButtonsEnabling();
            AppManager.I.Player.SetCurrentJourneyPosition(stageScript.GetCurrentPlayerJourneyPosition());
            //UpdateCurrentJourneyPosition();
        }

        void ForceToPlayerPosition(int newIndex)
        {
            Debug.LogError("Forcing to " + newIndex);
            stageScript.currentPlayerPosIndex = newIndex;
            MoveTo(stageScript.GetCurrentPlayerPosition(), false);

            CheckButtonsEnabling();
            AppManager.I.Player.SetCurrentJourneyPosition(stageScript.GetCurrentPlayerJourneyPosition());
        }

        /*void SetJourneyPosition()
        {
            /* TODO:
            if (stageScript.playerPinTargetPositions[stageScript.currentPlayerPosIndex].GetComponent<Dot>() != null)
            {
                AppManager.I.Player.CurrentJourneyPosition.PlaySession =
                    stageScript.playerPinTargetPositions[stageScript.currentPlayerPosIndex].GetComponent<Dot>().playSession;
                AppManager.I.Player.CurrentJourneyPosition.LearningBlock =
                    stageScript.playerPinTargetPositions[stageScript.currentPlayerPosIndex].GetComponent<Dot>().LearningBlock;
            }
            else
            {
                AppManager.I.Player.CurrentJourneyPosition.PlaySession =
                    stageScript.playerPinTargetPositions[stageScript.currentPlayerPosIndex].GetComponent<Pin>().playSessionPin;
                AppManager.I.Player.CurrentJourneyPosition.LearningBlock =
                    stageScript.playerPinTargetPositions[stageScript.currentPlayerPosIndex].GetComponent<Pin>().learningBlockPin;
            }*/
        //    UpdateCurrentJourneyPosition();
        //}

        /*void UpdateCurrentJourneyPosition()
        {
            // TODO: this looks USELESS
            AppManager.I.Player.SetCurrentJourneyPosition(
                new JourneyPosition(AppManager.I.Player.CurrentJourneyPosition.Stage,
                    AppManager.I.Player.CurrentJourneyPosition.LearningBlock,
                    AppManager.I.Player.CurrentJourneyPosition.PlaySession),
                true
            );
        }*/

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

            if (!lookAtPrevious && current_lb == stageScript.nLearningBlocks)
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