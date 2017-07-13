using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using Antura.Core;
using DG.Tweening;

namespace Antura.Map
{
    public class PlayerPin : MonoBehaviour
    {
        [Header("Stage")]
        public StageMap stageScript;

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
        MapRope ropeSelected;
        Collider colliderRaycast;
        Tween moveTween, rotateTween;

        void Start()
        {
            Floating();

            if (!AppManager.I.Player.IsFirstContact()) {
            // TODO:    AmIFirstorLastPos();
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

            if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject() && (!swipeScript.swipe)) {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                RaycastHit hit;
                int layerMask = 1 << 15;
                if (Physics.Raycast(ray, out hit, 500, layerMask))
                {
                    if (hit.collider.CompareTag("Rope"))
                    {
                        playerOverDotPin = true;
                        ropeSelected = hit.transform.parent.gameObject.GetComponent<MapRope>();
                        int nRopeActiveDots = 0;
                        foreach (MapDot dot in ropeSelected.dots)
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
                        MoveToDot();
                    }
                    else if (hit.collider.CompareTag("Pin"))
                    {
                        playerOverDotPin = true;
                        colliderRaycast = hit.collider;
                        MoveToPin();
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
                StartCoroutine("PlayerOverDotPinToFalse");
            }
        }

        IEnumerator PlayerOverDotPinToFalse()
        {
            yield return new WaitForSeconds(0.3f);
            playerOverDotPin = false;
        }

        void LateUpdate()
        {
            if (Input.GetMouseButtonUp(0) && (!EventSystem.current.IsPointerOverGameObject()) && (colliderRaycast != null))
            {
                if (colliderRaycast.CompareTag("Rope"))
                {
                    MoveToDot();
                    UpdateCurrentJourneyPosition();
                }
                else if (colliderRaycast.CompareTag("Pin"))
                {
                    MoveToPin();
                    UpdateCurrentJourneyPosition();
                }
            }
        }

        void MoveToDot()
        {
            stageScript.currentPlayerPositionIndex = ropeSelected.dots[dotCloser].posIndex;
            MoveTo(stageScript.playerPinTargetPositions[stageScript.currentPlayerPositionIndex]);

            // Update JourneyPos
            AppManager.I.Player.CurrentJourneyPosition.PlaySession = ropeSelected.dots[dotCloser].playSessionActual;
            AppManager.I.Player.CurrentJourneyPosition.LearningBlock = ropeSelected.dots[dotCloser].learningBlockActual;

            AmIFirstorLastPos();
            transform.LookAt(stageScript.PinForLB(ropeSelected.assignedLearningBlock).transform);
        }

        void MoveToPin()
        {
            MoveTo(colliderRaycast.transform.position);

            var pin = colliderRaycast.transform.gameObject.GetComponent<MapPin>();

            stageScript.currentPlayerPositionIndex = pin.pos;

            // Update JourneyPos
            AppManager.I.Player.CurrentJourneyPosition.PlaySession = 100;
            AppManager.I.Player.CurrentJourneyPosition.LearningBlock = pin.learningBlock;

            var current_lb = AppManager.I.Player.CurrentJourneyPosition.LearningBlock;
            //var current_ps = AppManager.I.Player.CurrentJourneyPosition.LearningBlock;

            if (current_lb < stageScript.nLearningBlocks)
            {
                transform.LookAt(stageScript.PinForLB(current_lb + 1).transform);
            }
            else
            {
                transform.LookAt(stageScript.PinForLB(current_lb - 1).transform);
                transform.rotation = Quaternion.Euler(
                    new Vector3(transform.rotation.eulerAngles.x,
                        transform.rotation.eulerAngles.y + 180,
                        transform.rotation.eulerAngles.z)
                );
            }
            AmIFirstorLastPos();
        }

        public void MoveToTheRightDot()
        {
            if ((stageScript.currentPlayerPositionIndex < (stageScript.playerPinTargetPositions.Count - 1)) &&
                (stageScript.currentPlayerPositionIndex != stageScript.maxPlayerPositionIndex))
            {
                stageScript.currentPlayerPositionIndex++;
                MoveTo(stageScript.playerPinTargetPositions[stageScript.currentPlayerPositionIndex], true);

                SetJourneyPosition();
                LookAtRightPin();
            }
            AmIFirstorLastPos();
        }

        public void MoveToTheLeftDot()
        {
            if (stageScript.currentPlayerPositionIndex > 1)
            {
                stageScript.currentPlayerPositionIndex--;
                MoveTo(stageScript.playerPinTargetPositions[stageScript.currentPlayerPositionIndex], true);

                SetJourneyPosition();
                LookAtLeftPin();
            }
            AmIFirstorLastPos();
        }

        public void ResetPosLetter()
        {
            if (AppManager.I.Player.IsAssessmentTime()) 
            {
                // Player is on a pin
                var current_lb = AppManager.I.Player.CurrentJourneyPosition.LearningBlock;
                var pin = stageScript.PinForLB(current_lb);

                MoveTo(pin.transform.position);

                stageScript.currentPlayerPositionIndex = pin.pos;

                if (AppManager.I.Player.CurrentJourneyPosition.LearningBlock < stageScript.nLearningBlocks)
                {
                    transform.LookAt(stageScript.PinForLB(AppManager.I.Player.CurrentJourneyPosition.LearningBlock + 1).transform);
                }
                else
                {
                    transform.LookAt(stageScript.PinForLB(AppManager.I.Player.CurrentJourneyPosition.LearningBlock - 1).transform);
                    transform.rotation = Quaternion.Euler(
                        new Vector3(transform.rotation.eulerAngles.x,
                            transform.rotation.eulerAngles.y + 180,
                            transform.rotation.eulerAngles.z)
                    );
                }
            }
            else
            {
                var current_lb = AppManager.I.Player.CurrentJourneyPosition.LearningBlock;
                var current_ps = AppManager.I.Player.CurrentJourneyPosition.LearningBlock;
                //var rope = stageScript.RopeForLB(current_lb);

                // Player is on a dot
                var dot = stageScript.RopeForLB(current_lb - 1).DotForPS(current_ps - 1);
                MoveTo(dot.transform.position);
                stageScript.currentPlayerPositionIndex = dot.posIndex;

                transform.LookAt(stageScript.PinForLB(current_lb).transform);
            }
            //AmIFirstorLastPos();
        }

        public void ResetPosLetterAfterChangeStage()
        {
            stageScript.currentPlayerPositionIndex = 1;
            MoveTo(stageScript.playerPinTargetPositions[1]);
            AppManager.I.Player.CurrentJourneyPosition.LearningBlock = 1;
            AppManager.I.Player.CurrentJourneyPosition.PlaySession = 1;
            LookAtRightPin();
            UpdateCurrentJourneyPosition();
        }

        void SetJourneyPosition()
        {
            /* TODO:
            if (stageScript.playerPinTargetPositions[stageScript.currentPlayerPositionIndex].GetComponent<MapDot>() != null)
            {
                AppManager.I.Player.CurrentJourneyPosition.PlaySession =
                    stageScript.playerPinTargetPositions[stageScript.currentPlayerPositionIndex].GetComponent<MapDot>().playSessionActual;
                AppManager.I.Player.CurrentJourneyPosition.LearningBlock =
                    stageScript.playerPinTargetPositions[stageScript.currentPlayerPositionIndex].GetComponent<MapDot>().learningBlockActual;
            }
            else
            {
                AppManager.I.Player.CurrentJourneyPosition.PlaySession =
                    stageScript.playerPinTargetPositions[stageScript.currentPlayerPositionIndex].GetComponent<MapPin>().playSessionPin;
                AppManager.I.Player.CurrentJourneyPosition.LearningBlock =
                    stageScript.playerPinTargetPositions[stageScript.currentPlayerPositionIndex].GetComponent<MapPin>().learningBlockPin;
            }*/
            UpdateCurrentJourneyPosition();
        }

        void UpdateCurrentJourneyPosition()
        {
            AppManager.I.Player.SetCurrentJourneyPosition(
                new JourneyPosition(AppManager.I.Player.CurrentJourneyPosition.Stage,
                    AppManager.I.Player.CurrentJourneyPosition.LearningBlock,
                    AppManager.I.Player.CurrentJourneyPosition.PlaySession),
                true
            );
        }

        #region LookAt

        void LookAtRightPin()
        {
            LookAt(false);
        }

        void LookAtLeftPin()
        {
            LookAt(true);
        }

        void LookAt(bool leftPin)
        {
            rotateTween.Kill();
            Quaternion currRotation = transform.rotation;
            transform.LookAt(leftPin
                ? stageScript.PinForLB(AppManager.I.Player.CurrentJourneyPosition.LearningBlock-1).transform.position
                : stageScript.PinForLB(AppManager.I.Player.CurrentJourneyPosition.LearningBlock).transform.position
            );
            Quaternion toRotation = transform.rotation;
            transform.rotation = currRotation;
            rotateTween = transform.DORotate(toRotation.eulerAngles, 0.3f).SetEase(Ease.InOutQuad);
        }

        #endregion

        // If animate is TRUE, animates the movement, otherwise applies the movement immediately
        void MoveTo(Vector3 position, bool animate = false)
        {
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

        private void AmIFirstorLastPos()
        {
            if (stageScript.currentPlayerPositionIndex == 1)
            {
                if (stageScript.maxPlayerPositionIndex == 1)
                {
                    moveRightButton.SetActive(false);
                    moveLeftButton.SetActive(false);
                }
                else
                {
                    StartCoroutine("DesactivateButtonWithDelay", moveRightButton);
                    moveLeftButton.SetActive(true);
                }
            }
            else if (stageScript.currentPlayerPositionIndex == stageScript.maxPlayerPositionIndex)
            {
                moveRightButton.SetActive(true);
                StartCoroutine("DesactivateButtonWithDelay", moveLeftButton);
            }
            else
            {
                moveRightButton.SetActive(true);
                moveLeftButton.SetActive(true);
            }
        }

        private IEnumerator DesactivateButtonWithDelay(GameObject button)
        {
            yield return new WaitForSeconds(0.1f);
            button.SetActive(false);
        }
    }
}