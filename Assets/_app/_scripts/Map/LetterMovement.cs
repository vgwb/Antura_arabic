using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using EA4S;
using System.Collections.Generic;
using DG.Tweening;

namespace EA4S
{
    public class LetterMovement : MonoBehaviour
    {
        [Header("MiniMap")]
        public MiniMap miniMapScript;

        [Header("Materials")]
        public Material black;
        public Material red;

        [Header("UIButtons")]
        public GameObject moveRightButton;
        public GameObject moveLeftButton;

        public int pos;

        float distanceNextDotToHitPoint;
        float distanceBeforelDotToHitPoint;
        float distanceActualDotToHitPoint;
        int numDots;
        int posDotMiniMapScript, dotCloser;
        Rope ropeSelected;
        Collider colliderRaycast;
        Tween moveTween, rotateTween;

        int learningblock, learningblockMax;
        int playSession, playSessionMax;
        int stage, stageMax;

        void Start()
        {
            Floating();
            learningblock = AppManager.I.Player.CurrentJourneyPosition.LearningBlock;
            playSession = AppManager.I.Player.CurrentJourneyPosition.PlaySession;
            stage = AppManager.I.Player.CurrentJourneyPosition.Stage;

            learningblockMax = AppManager.I.Player.MaxJourneyPosition.LearningBlock;
            playSessionMax = AppManager.I.Player.MaxJourneyPosition.PlaySession;
            stageMax = AppManager.I.Player.MaxJourneyPosition.Stage;

            /* FIRST CONTACT FEATURE */
            if (!AppManager.I.Player.IsFirstContact()) {
                AmIFirstorLastPos();
            }
            /* --------------------- */
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
            Debug.Log("MaxPS"+AppManager.I.Player.MaxJourneyPosition.PlaySession);    */    

            if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject()) {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Debug.DrawRay(ray.origin, ray.direction * 500, Color.yellow);

                RaycastHit hit;
                int layerMask = 1 << 15;
                if (Physics.Raycast(ray, out hit, 500, layerMask)) {
                    if (hit.collider.tag == "Rope") {
                        if (hit.transform.parent.gameObject.GetComponent<Rope>().dots[1].activeInHierarchy)//All dots available of all ropes
                        {
                            int numDotsRope = hit.transform.parent.transform.gameObject.GetComponent<Rope>().dots.Count;
                            float distaceHitToDot = 1000;
                            float distanceHitBefore = 0;
                            dotCloser = 0;

                            for (int i = 0; i < numDotsRope; i++) {
                                distanceHitBefore = Vector3.Distance(hit.point,
                                    hit.transform.parent.transform.gameObject.GetComponent<Rope>().dots[i].transform.position);
                                if (distanceHitBefore < distaceHitToDot) {
                                    distaceHitToDot = distanceHitBefore;
                                    dotCloser = i;
                                }
                            }
                        } else {
                            dotCloser = 0;
                        }


                        posDotMiniMapScript = hit.transform.parent.transform.gameObject.GetComponent<Rope>().dots[dotCloser].GetComponent<Dot>().pos;

                        ChangeMaterialDotToBlack(miniMapScript.posDots[pos]);
                        pos = posDotMiniMapScript;
                        ChangeMaterialDotToRed(miniMapScript.posDots[pos]);

                        ropeSelected = hit.transform.parent.transform.gameObject.GetComponent<Rope>();

                        MoveTo(miniMapScript.posDots[posDotMiniMapScript].transform.position);
                        transform.LookAt(miniMapScript.posPines[hit.transform.parent.transform.gameObject.GetComponent<Rope>().learningBlockRope]);

                        colliderRaycast = hit.collider;
                        AppManager.I.Player.CurrentJourneyPosition.PlaySession = ropeSelected.dots[dotCloser].GetComponent<Dot>().playSessionActual;
                        AppManager.I.Player.CurrentJourneyPosition.LearningBlock = ropeSelected.dots[dotCloser].GetComponent<Dot>().learningBlockActual;
                        UpdateCurrenJourneyPosition();
                        AmIFirstorLastPos();
                    } else if (hit.collider.tag == "Pin") {
                        ChangeMaterialDotToBlack(miniMapScript.posDots[pos]);
                        colliderRaycast = hit.collider;
                        if (hit.transform.gameObject.GetComponent<MapPin>().Number < miniMapScript.posPines.Length - 1)
                            transform.LookAt(miniMapScript.posPines[hit.transform.gameObject.GetComponent<MapPin>().Number + 1]);
                        MoveTo(colliderRaycast.transform.position);
                        pos = colliderRaycast.transform.gameObject.GetComponent<MapPin>().posBefore;
                        AppManager.I.Player.CurrentJourneyPosition.PlaySession = 100;
                        AppManager.I.Player.CurrentJourneyPosition.LearningBlock = colliderRaycast.transform.gameObject.GetComponent<MapPin>().Number;
                        UpdateCurrenJourneyPosition();
                        AmIFirstorLastPos();
                    } else colliderRaycast = null;
                } else colliderRaycast = null;
            } /*else if (Input.GetMouseButtonUp(0) && (!EventSystem.current.IsPointerOverGameObject()) && (colliderRaycast != null)) {
                if (colliderRaycast.tag == "Rope") {
                    MoveTo(miniMapScript.posDots[posDotMiniMapScript].transform.position);

                    AppManager.I.Player.CurrentJourneyPosition.PlaySession = ropeSelected.dots[dotCloser].GetComponent<Dot>().playSessionActual;
                    AppManager.I.Player.CurrentJourneyPosition.LearningBlock = ropeSelected.dots[dotCloser].GetComponent<Dot>().learningBlockActual;
                    LookAtRightPin();
                    UpdateCurrenJourneyPosition();
                    AmIFirstorLastPos();
                }
                if (colliderRaycast.tag == "Pin") {
                    MoveTo(colliderRaycast.transform.position);
                    pos = colliderRaycast.transform.gameObject.GetComponent<MapPin>().posBefore;

                    AppManager.I.Player.CurrentJourneyPosition.PlaySession = 100;
                    AppManager.I.Player.CurrentJourneyPosition.LearningBlock = colliderRaycast.transform.gameObject.GetComponent<MapPin>().Number;
                    if (colliderRaycast.transform.gameObject.GetComponent<MapPin>().Number < miniMapScript.posPines.Length - 1)
                        transform.LookAt(miniMapScript.posPines[AppManager.I.Player.CurrentJourneyPosition.LearningBlock + 1]);
                    UpdateCurrenJourneyPosition();
                    AmIFirstorLastPos();
                }

            }*/
        }

        void LateUpdate()
        {
            if (Input.GetMouseButtonUp(0) && (!EventSystem.current.IsPointerOverGameObject()) && (colliderRaycast != null))
            {
                if (colliderRaycast.tag == "Rope")
                {
                    MoveTo(miniMapScript.posDots[posDotMiniMapScript].transform.position);

                    AppManager.I.Player.CurrentJourneyPosition.PlaySession = ropeSelected.dots[dotCloser].GetComponent<Dot>().playSessionActual;
                    AppManager.I.Player.CurrentJourneyPosition.LearningBlock = ropeSelected.dots[dotCloser].GetComponent<Dot>().learningBlockActual;
                    LookAtRightPin();
                    UpdateCurrenJourneyPosition();
                    AmIFirstorLastPos();
                }
                if (colliderRaycast.tag == "Pin")
                {
                    MoveTo(colliderRaycast.transform.position);
                    pos = colliderRaycast.transform.gameObject.GetComponent<MapPin>().posBefore;

                    AppManager.I.Player.CurrentJourneyPosition.PlaySession = 100;
                    AppManager.I.Player.CurrentJourneyPosition.LearningBlock = colliderRaycast.transform.gameObject.GetComponent<MapPin>().Number;
                    if (AppManager.I.Player.CurrentJourneyPosition.LearningBlock < miniMapScript.ropes.Length)
                        transform.LookAt(miniMapScript.posPines[AppManager.I.Player.CurrentJourneyPosition.LearningBlock + 1]);
                    else transform.LookAt(miniMapScript.posPines[AppManager.I.Player.CurrentJourneyPosition.LearningBlock - 1]);
                    UpdateCurrenJourneyPosition();
                    AmIFirstorLastPos();
                }

            }
        }
        public void MoveToTheRightDot()
        {
            if ((AppManager.I.Player.CurrentJourneyPosition.PlaySession == 2) && (miniMapScript.posPines[AppManager.I.Player.CurrentJourneyPosition.LearningBlock].tag == "Pin")) {
                ChangeMaterialDotToBlack(miniMapScript.posDots[pos]);
                MoveTo(miniMapScript.posPines[AppManager.I.Player.CurrentJourneyPosition.LearningBlock].transform.position, true);
                AppManager.I.Player.CurrentJourneyPosition.PlaySession = 100;
                UpdateCurrenJourneyPosition();
                if (AppManager.I.Player.CurrentJourneyPosition.LearningBlock != miniMapScript.posPines.Length - 1)
                    transform.LookAt(miniMapScript.posPines[AppManager.I.Player.CurrentJourneyPosition.LearningBlock + 1]);
            } else if ((AppManager.I.Player.CurrentJourneyPosition.PlaySession == 100) && (pos <= (miniMapScript.posMax - 1))) {
                if (pos % 2 != 0)
                    pos++;
                MoveTo(miniMapScript.posDots[pos].transform.position, true);
                ChangeMaterialDotToRed(miniMapScript.posDots[pos]);
                AppManager.I.Player.CurrentJourneyPosition.PlaySession = miniMapScript.posDots[pos].GetComponent<Dot>().playSessionActual;
                AppManager.I.Player.CurrentJourneyPosition.LearningBlock++;
                UpdateCurrenJourneyPosition();
                LookAtRightPin();
            } else {
                if (pos < (miniMapScript.posMax - 1)) {
                    ChangeMaterialDotToBlack(miniMapScript.posDots[pos]);
                    pos++;
                    ChangeMaterialDotToRed(miniMapScript.posDots[pos]);
                    MoveTo(miniMapScript.posDots[pos].transform.position, true);
                    AppManager.I.Player.CurrentJourneyPosition.PlaySession = miniMapScript.posDots[pos].GetComponent<Dot>().playSessionActual;
                    UpdateCurrenJourneyPosition();
                    LookAtRightPin();
                }
            }
            AmIFirstorLastPos();
        }
        public void MoveToTheLeftDot()
        {
            if (AppManager.I.Player.CurrentJourneyPosition.PlaySession == 1) {
                if (pos > 0) {
                    ChangeMaterialDotToBlack(miniMapScript.posDots[pos]);
                    MoveTo(miniMapScript.posPines[AppManager.I.Player.CurrentJourneyPosition.LearningBlock - 1].transform.position, true);
                    AppManager.I.Player.CurrentJourneyPosition.PlaySession = 100;
                    AppManager.I.Player.CurrentJourneyPosition.LearningBlock--;

                    UpdateCurrenJourneyPosition();
                    LookAtLeftPin();
                }
            } else if (AppManager.I.Player.CurrentJourneyPosition.PlaySession == 100) {
                if (pos % 2 == 0)
                    pos--;
                MoveTo(miniMapScript.posDots[pos].transform.position, true);
                ChangeMaterialDotToRed(miniMapScript.posDots[pos]);
                AppManager.I.Player.CurrentJourneyPosition.PlaySession = miniMapScript.posDots[pos].GetComponent<Dot>().playSessionActual;

                UpdateCurrenJourneyPosition();
                LookAtLeftPin();
            } else {
                if (pos > 0) {
                    ChangeMaterialDotToBlack(miniMapScript.posDots[pos]);
                    pos--;
                    MoveTo(miniMapScript.posDots[pos].transform.position, true);
                    ChangeMaterialDotToRed(miniMapScript.posDots[pos]);
                    LookAtLeftPin();
                }
                AppManager.I.Player.CurrentJourneyPosition.PlaySession = miniMapScript.posDots[pos].GetComponent<Dot>().playSessionActual;
                AppManager.I.Player.CurrentJourneyPosition.LearningBlock = miniMapScript.posDots[pos].GetComponent<Dot>().learningBlockActual;
                UpdateCurrenJourneyPosition();
            }
            AmIFirstorLastPos();
        }

        public void ResetPosLetter()
        {
            if (AppManager.I.Player.CurrentJourneyPosition.PlaySession == 100)//Letter is on a pin
            {
                MoveTo(miniMapScript.posPines[AppManager.I.Player.CurrentJourneyPosition.LearningBlock].transform.position);
                pos = miniMapScript.posPines[AppManager.I.Player.CurrentJourneyPosition.LearningBlock].GetComponent<MapPin>().posBefore;
                if (AppManager.I.Player.CurrentJourneyPosition.LearningBlock < miniMapScript.ropes.Length)
                    transform.LookAt(miniMapScript.posPines[AppManager.I.Player.CurrentJourneyPosition.LearningBlock + 1]);
                else transform.LookAt(miniMapScript.posPines[AppManager.I.Player.CurrentJourneyPosition.LearningBlock - 1]);
            } else  //Letter is on a dot
              {
                MoveTo(miniMapScript.ropes[AppManager.I.Player.CurrentJourneyPosition.LearningBlock - 1].GetComponent<Rope>().dots
                    [AppManager.I.Player.CurrentJourneyPosition.PlaySession - 1].transform.position);
                pos = miniMapScript.ropes[AppManager.I.Player.CurrentJourneyPosition.LearningBlock - 1].GetComponent<Rope>().dots
                    [AppManager.I.Player.CurrentJourneyPosition.PlaySession - 1].GetComponent<Dot>().pos;
                miniMapScript.posDots[pos].GetComponent<Renderer>().material = red;
                transform.LookAt(miniMapScript.posPines[AppManager.I.Player.CurrentJourneyPosition.LearningBlock]);
            }
        }
        public void ResetPosLetterAfterChangeStage()
        {
            pos = 0;
            MoveTo(miniMapScript.posDots[pos].transform.position);
            miniMapScript.posDots[pos].GetComponent<Renderer>().material = red;
            AppManager.I.Player.CurrentJourneyPosition.LearningBlock = 1;
            AppManager.I.Player.CurrentJourneyPosition.PlaySession = 1;
            LookAtRightPin();
            UpdateCurrenJourneyPosition();
        }
        void UpdateCurrenJourneyPosition()
        {
            AppManager.I.Player.SetCurrentJourneyPosition(new JourneyPosition(AppManager.I.Player.CurrentJourneyPosition.Stage,
             AppManager.I.Player.CurrentJourneyPosition.LearningBlock,
              AppManager.I.Player.CurrentJourneyPosition.PlaySession), true);
        }
        public void ChangeMaterialDotToBlack(GameObject dot)
        {
            dot.GetComponent<Renderer>().material = black;
        }
        void ChangeMaterialDotToRed(GameObject dot)
        {
            dot.GetComponent<Renderer>().material = red;
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
            Quaternion currRotation = this.transform.rotation;
            this.transform.LookAt(leftPin
                ? miniMapScript.posPines[AppManager.I.Player.CurrentJourneyPosition.LearningBlock - 1]
                : miniMapScript.posPines[AppManager.I.Player.CurrentJourneyPosition.LearningBlock]
            );
            Quaternion toRotation = this.transform.rotation;
            this.transform.rotation = currRotation;
            rotateTween = this.transform.DORotate(toRotation.eulerAngles, 0.3f).SetEase(Ease.InOutQuad);
        }
        // If animate is TRUE, animates the movement, otherwise applies the movement immediately
        void MoveTo(Vector3 position, bool animate = false)
        {
            if (moveTween != null) moveTween.Complete();
            if (animate) moveTween = this.transform.DOMove(position, 0.25f);
            else this.transform.position = position;
        }

        public void AmIFirstorLastPos()
        {
            CanNotMoveToRight();
            CanNotMoveToLeft();
        }
        void CanNotMoveToLeft()
        {
            learningblock = AppManager.I.Player.CurrentJourneyPosition.LearningBlock;
            playSession = AppManager.I.Player.CurrentJourneyPosition.PlaySession;
            stage = AppManager.I.Player.CurrentJourneyPosition.Stage;

            if (miniMapScript.isAvailableTheWholeMap) {
                if ((learningblock == miniMapScript.posPines.Length - 1) &&
                    (playSession == 100)) moveLeftButton.SetActive(false);
                else moveLeftButton.SetActive(true);
            } else {
                if (learningblock == learningblockMax) {
                    if (playSession == playSessionMax) moveLeftButton.SetActive(false);
                    else moveLeftButton.SetActive(true);
                } else moveLeftButton.SetActive(true);
            }
        }
        void CanNotMoveToRight()
        {
            if (pos == 0) moveRightButton.SetActive(false);
            else moveRightButton.SetActive(true);
        }
    }
}

