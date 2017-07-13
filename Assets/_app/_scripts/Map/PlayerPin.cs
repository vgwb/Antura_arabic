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
        Rope ropeSelected;
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
                if (Physics.Raycast(ray, out hit, 500, layerMask)) {
                    if (hit.collider.tag == "Rope") {
                        playerOverDotPin = true;
                        ropeSelected = hit.transform.parent.gameObject.GetComponent<Rope>();
                        int numDotsRope = 0;
                        for (int r = 0; r < ropeSelected.dots.Count; r++) {
                            if (ropeSelected.dots[r].activeInHierarchy) numDotsRope++;
                        }
                        if (numDotsRope > 1) {
                            float distaceHitToDot = 1000;
                            float distanceHitBefore = 0;
                            dotCloser = 0;

                            for (int i = 0; i < numDotsRope; i++) {
                                distanceHitBefore = Vector3.Distance(hit.point,
                                    ropeSelected.dots[i].transform.position);
                                if (distanceHitBefore < distaceHitToDot) {
                                    distaceHitToDot = distanceHitBefore;
                                    dotCloser = i;
                                }
                            }
                        } else {
                            dotCloser = 0;
                        }
                        colliderRaycast = hit.collider;
                        MoveToDot();
                    } else if (hit.collider.tag == "Pin") {
                        playerOverDotPin = true;
                        colliderRaycast = hit.collider;
                        MoveToPin();
                    } else {
                        colliderRaycast = null;
                    }
                } else {
                    colliderRaycast = null;
                }
            } else {
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
            if (Input.GetMouseButtonUp(0) && (!EventSystem.current.IsPointerOverGameObject()) && (colliderRaycast != null)) {
                if (colliderRaycast.tag == "Rope") {
                    MoveToDot();
                    UpdateCurrentJourneyPosition();
                } else if (colliderRaycast.tag == "Pin") {
                    MoveToPin();
                    UpdateCurrentJourneyPosition();
                }
            }
        }

        void MoveToDot()
        {
            stageScript.positionPin = ropeSelected.dots[dotCloser].GetComponent<MapDot>().pos;
            MoveTo(stageScript.positionsPlayerPin[stageScript.positionPin].transform.position);
            AppManager.I.Player.CurrentJourneyPosition.PlaySession = ropeSelected.dots[dotCloser].GetComponent<MapDot>().playSessionActual;
            AppManager.I.Player.CurrentJourneyPosition.LearningBlock = ropeSelected.dots[dotCloser].GetComponent<MapDot>().learningBlockActual;
            AmIFirstorLastPos();
            transform.LookAt(stageScript.pines[ropeSelected.learningBlockRope].transform);
        }

        void MoveToPin()
        {
            MoveTo(colliderRaycast.transform.position);
            stageScript.positionPin = colliderRaycast.transform.gameObject.GetComponent<MapPin>().pos;
            AppManager.I.Player.CurrentJourneyPosition.PlaySession = 100;
            AppManager.I.Player.CurrentJourneyPosition.LearningBlock =
                colliderRaycast.transform.gameObject.GetComponent<MapPin>().learningBlockPin;
            if (AppManager.I.Player.CurrentJourneyPosition.LearningBlock < stageScript.numberLearningBlocks) {
                transform.LookAt(stageScript.pines[AppManager.I.Player.CurrentJourneyPosition.LearningBlock + 1].transform);
            } else {
                transform.LookAt(stageScript.pines[AppManager.I.Player.CurrentJourneyPosition.LearningBlock - 1].transform);
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
            if ((stageScript.positionPin < (stageScript.positionsPlayerPin.Count - 1)) &&
                (stageScript.positionPin != stageScript.positionPinMax)) {
                stageScript.positionPin++;
                MoveTo(stageScript.positionsPlayerPin[stageScript.positionPin].transform.position, true);

                SetJourneyPosition();
                LookAtRightPin();
            }

            AmIFirstorLastPos();
        }

        public void MoveToTheLeftDot()
        {
            if (stageScript.positionPin > 1) {
                stageScript.positionPin--;
                MoveTo(stageScript.positionsPlayerPin[stageScript.positionPin].transform.position, true);

                SetJourneyPosition();
                LookAtLeftPin();
            }
            AmIFirstorLastPos();
        }

        public void ResetPosLetter()
        {
            if (AppManager.I.Player.IsAssessmentTime()) // Letter is on a pin
            {
                MoveTo(stageScript.pines[AppManager.I.Player.CurrentJourneyPosition.LearningBlock].transform.position);
                stageScript.positionPin = stageScript.pines[AppManager.I.Player.CurrentJourneyPosition.LearningBlock].GetComponent<MapPin>()
                    .pos;
                if (AppManager.I.Player.CurrentJourneyPosition.LearningBlock < stageScript.ropes.Length) {
                    transform.LookAt(stageScript.pines[AppManager.I.Player.CurrentJourneyPosition.LearningBlock + 1].transform);
                } else {
                    transform.LookAt(stageScript.pines[AppManager.I.Player.CurrentJourneyPosition.LearningBlock - 1].transform);
                    transform.rotation = Quaternion.Euler(
                        new Vector3(transform.rotation.eulerAngles.x,
                            transform.rotation.eulerAngles.y + 180,
                            transform.rotation.eulerAngles.z)
                    );
                }
            } else {
                //Letter is on a dot
                MoveTo(stageScript.ropes[AppManager.I.Player.CurrentJourneyPosition.LearningBlock - 1].GetComponent<Rope>().dots
                    [AppManager.I.Player.CurrentJourneyPosition.PlaySession - 1].transform.position);
                stageScript.positionPin = stageScript.ropes[AppManager.I.Player.CurrentJourneyPosition.LearningBlock - 1]
                    .GetComponent<Rope>().dots
                    [AppManager.I.Player.CurrentJourneyPosition.PlaySession - 1].GetComponent<MapDot>().pos;
                transform.LookAt(stageScript.pines[AppManager.I.Player.CurrentJourneyPosition.LearningBlock].transform);
            }
            //AmIFirstorLastPos();
        }

        public void ResetPosLetterAfterChangeStage()
        {
            stageScript.positionPin = 1;
            MoveTo(stageScript.positionsPlayerPin[1].transform.position);
            AppManager.I.Player.CurrentJourneyPosition.LearningBlock = 1;
            AppManager.I.Player.CurrentJourneyPosition.PlaySession = 1;
            LookAtRightPin();
            UpdateCurrentJourneyPosition();
        }

        void SetJourneyPosition()
        {
            if (stageScript.positionsPlayerPin[stageScript.positionPin].GetComponent<MapDot>() != null) {
                AppManager.I.Player.CurrentJourneyPosition.PlaySession =
                    stageScript.positionsPlayerPin[stageScript.positionPin].GetComponent<MapDot>().playSessionActual;
                AppManager.I.Player.CurrentJourneyPosition.LearningBlock =
                    stageScript.positionsPlayerPin[stageScript.positionPin].GetComponent<MapDot>().learningBlockActual;
            } else {
                AppManager.I.Player.CurrentJourneyPosition.PlaySession =
                    stageScript.positionsPlayerPin[stageScript.positionPin].GetComponent<MapPin>().playSessionPin;
                AppManager.I.Player.CurrentJourneyPosition.LearningBlock =
                    stageScript.positionsPlayerPin[stageScript.positionPin].GetComponent<MapPin>().learningBlockPin;
            }
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
                ? stageScript.pines[AppManager.I.Player.CurrentJourneyPosition.LearningBlock - 1].transform.position
                : stageScript.pines[AppManager.I.Player.CurrentJourneyPosition.LearningBlock].transform.position
            );
            Quaternion toRotation = transform.rotation;
            transform.rotation = currRotation;
            rotateTween = transform.DORotate(toRotation.eulerAngles, 0.3f).SetEase(Ease.InOutQuad);
        }

        // If animate is TRUE, animates the movement, otherwise applies the movement immediately
        void MoveTo(Vector3 position, bool animate = false)
        {
            if (moveTween != null) {
                moveTween.Complete();
            }
            if (animate) {
                moveTween = transform.DOMove(position, 0.25f);
            } else {
                transform.position = position;
            }
        }

        public void AmIFirstorLastPos()
        {
            if (stageScript.positionPin == 1) {
                if (stageScript.positionPinMax == 1) {
                    moveRightButton.SetActive(false);
                    moveLeftButton.SetActive(false);
                } else {
                    StartCoroutine("DesactivateButtonWithDelay", moveRightButton);
                    moveLeftButton.SetActive(true);
                }
            } else if (stageScript.positionPin == stageScript.positionPinMax) {
                moveRightButton.SetActive(true);
                StartCoroutine("DesactivateButtonWithDelay", moveLeftButton);
            } else {
                moveRightButton.SetActive(true);
                moveLeftButton.SetActive(true);
            }
        }

        IEnumerator DesactivateButtonWithDelay(GameObject button)
        {
            yield return new WaitForSeconds(0.1f);
            button.SetActive(false);
        }
    }
}