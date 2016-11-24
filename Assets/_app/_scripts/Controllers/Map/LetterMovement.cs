using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems; 
using EA4S;
using EA4S.TestE;
using System.Collections.Generic;

namespace EA4S
{
    public class LetterMovement : MonoBehaviour {
        
        public MiniMap miniMapScript;
        public float speed;
        Transform target;
        public Vector3 posDot;
        public Material black;
        public Material red;
        public Material blackPin;
        public Material redPin;
        public int pos;

        float distanceNextDotToHitPoint;
        float distanceBeforelDotToHitPoint;
        float distanceActualDotToHitPoint;
        int numDots;
        int posDotMiniMapScript, dotCloser;
        Rope ropeSelected;
        Collider colliderRaycast;

        void Update()
        {
            /*Debug.Log(AppManager.Instance.Player.CurrentJourneyPosition.Stage);
            Debug.Log(AppManager.Instance.Player.CurrentJourneyPosition.LearningBlock);
            Debug.Log(AppManager.Instance.Player.CurrentJourneyPosition.PlaySession);*/

            transform.position = Vector3.MoveTowards(transform.position, new Vector3(posDot.x, transform.position.y, posDot.z), speed * Time.deltaTime);
           /* if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.tag == "Rope")
                    {
                        if((colliderRaycast!= null)&&(colliderRaycast.tag == "Pin"))
                            colliderRaycast.gameObject.GetComponent<MapPin>().Dot.GetComponent<Renderer>().material = blackPin;

                        if(AppManager.Instance.Player.MaxJourneyPosition.PlaySession == 2)//All dots available of all ropes
                        {
                            int numDotsRope = hit.transform.parent.transform.gameObject.GetComponent<Rope>().dots.Count;
                            float distaceHitToDot = 1000;
                            float distanceHitBefore = 0;
                            dotCloser = 0;

                            for (int i = 0; i < numDotsRope; i++)
                            {
                                distanceHitBefore = Vector3.Distance(hit.point,
                                    hit.transform.parent.transform.gameObject.GetComponent<Rope>().dots[i].transform.position);
                                if (distanceHitBefore < distaceHitToDot)
                                {
                                    distaceHitToDot = distanceHitBefore;
                                    dotCloser = i;
                                }
                            }
                        }
                        else
                        {
                            dotCloser = 0;
                        }


                        posDotMiniMapScript = hit.transform.parent.transform.gameObject.GetComponent<Rope>().dots[dotCloser].GetComponent<Dot>().pos;

                        ChangeMaterialDotToBlack(miniMapScript.posDots[pos]);
                        pos = posDotMiniMapScript;
                        ChangeMaterialDotToRed(miniMapScript.posDots[pos]);

                        ropeSelected = hit.transform.parent.transform.gameObject.GetComponent<Rope>();
                  
                        colliderRaycast = hit.collider;
                    }
                    if (hit.collider.tag == "Pin")
                    {
                        miniMapScript.posPines[AppManager.Instance.Player.CurrentJourneyPosition.LearningBlock].gameObject.GetComponent<MapPin>().Dot.GetComponent<Renderer>().material = blackPin;
                        ChangeMaterialDotToBlack(miniMapScript.posDots[pos]);
                        hit.transform.gameObject.GetComponent<MapPin>().Dot.GetComponent<Renderer>().material = redPin;
                        colliderRaycast = hit.collider;
                    }
                }
            }

            if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject() && (colliderRaycast!=null))
            {
                if(colliderRaycast.tag == "Rope")
                {
                    posDot = miniMapScript.posDots[posDotMiniMapScript].transform.position;
                    if (ropeSelected.learningBlockRope != AppManager.Instance.Player.CurrentJourneyPosition.LearningBlock)
                        transform.position = miniMapScript.posDots[posDotMiniMapScript].transform.position;

                    AppManager.Instance.Player.CurrentJourneyPosition.PlaySession = ropeSelected.dots[dotCloser].GetComponent<Dot>().playSessionActual;
                    AppManager.Instance.Player.CurrentJourneyPosition.LearningBlock = ropeSelected.dots[dotCloser].GetComponent<Dot>().learningBlockActual;
                    UpdateCurrenJourneyPosition();

                }
                if (colliderRaycast.tag == "Pin")
                {
                    if((colliderRaycast.GetComponent<MapPin>().Number == AppManager.Instance.Player.CurrentJourneyPosition.LearningBlock) ||
                        (colliderRaycast.GetComponent<MapPin>().Number == AppManager.Instance.Player.CurrentJourneyPosition.LearningBlock+1))
                    {
                        posDot = colliderRaycast.transform.position;
                    }
                    else
                        transform.position = colliderRaycast.transform.position;

                    posDot = colliderRaycast.transform.position;
                    pos = colliderRaycast.transform.gameObject.GetComponent<MapPin>().posBefore;

                    AppManager.Instance.Player.CurrentJourneyPosition.PlaySession = 100;
                    AppManager.Instance.Player.CurrentJourneyPosition.LearningBlock = colliderRaycast.transform.gameObject.GetComponent<MapPin>().Number;
                    UpdateCurrenJourneyPosition();
                }

            }*/
        }

        public void MoveToTheRightDot()
        {
            if((AppManager.Instance.Player.CurrentJourneyPosition.PlaySession==2) && (miniMapScript.posPines[AppManager.Instance.Player.CurrentJourneyPosition.LearningBlock].tag == "Pin"))
            {
                ChangeMaterialDotToBlack(miniMapScript.posDots[pos]);
                posDot = miniMapScript.posPines[AppManager.Instance.Player.CurrentJourneyPosition.LearningBlock].transform.position;
                miniMapScript.posPines[AppManager.Instance.Player.CurrentJourneyPosition.LearningBlock].transform.gameObject.GetComponent<MapPin>().Dot.GetComponent<Renderer>().material = redPin;
                AppManager.Instance.Player.CurrentJourneyPosition.PlaySession = 100;              
            }
            else if ((AppManager.Instance.Player.CurrentJourneyPosition.PlaySession == 100) && (pos < (miniMapScript.posMax - 1)))
            {
                if (pos % 2 != 0) 
                    pos++;
                miniMapScript.posPines[AppManager.Instance.Player.CurrentJourneyPosition.LearningBlock].transform.gameObject.GetComponent<MapPin>().Dot.GetComponent<Renderer>().material = blackPin;
                posDot = miniMapScript.posDots[pos].transform.position;
                ChangeMaterialDotToRed(miniMapScript.posDots[pos]);
                AppManager.Instance.Player.CurrentJourneyPosition.PlaySession = miniMapScript.posDots[pos].GetComponent<Dot>().playSessionActual;
                AppManager.Instance.Player.CurrentJourneyPosition.LearningBlock++;
            }
            else
            {       
                if (pos < (miniMapScript.posMax - 1))
                {
                    ChangeMaterialDotToBlack(miniMapScript.posDots[pos]);
                    pos++;          
                    ChangeMaterialDotToRed(miniMapScript.posDots[pos]);
                    posDot = miniMapScript.posDots[pos].transform.position;                
                }        
                AppManager.Instance.Player.CurrentJourneyPosition.PlaySession = miniMapScript.posDots[pos].GetComponent<Dot>().playSessionActual;
            }

            Vector3 pinPos = miniMapScript.pinRight;
           // transform.LookAt(new Vector3(pinPos.x, pinPos.y + 3, pinPos.z));
            UpdateCurrenJourneyPosition();
        }
       public void MoveToTheLeftDot()
       {
            if (AppManager.Instance.Player.CurrentJourneyPosition.PlaySession == 1)
            {
                if(pos>0)
                {
                    ChangeMaterialDotToBlack(miniMapScript.posDots[pos]);
                    miniMapScript.posPines[AppManager.Instance.Player.CurrentJourneyPosition.LearningBlock-1].transform.gameObject.GetComponent<MapPin>().Dot.GetComponent<Renderer>().material = redPin;
                    posDot = miniMapScript.posPines[AppManager.Instance.Player.CurrentJourneyPosition.LearningBlock - 1].transform.position;
                    AppManager.Instance.Player.CurrentJourneyPosition.PlaySession = 100;
                    AppManager.Instance.Player.CurrentJourneyPosition.LearningBlock--;
                }
            }
            else if(AppManager.Instance.Player.CurrentJourneyPosition.PlaySession == 100)
            {
                if(pos%2==0)
                   pos--;
                miniMapScript.posPines[AppManager.Instance.Player.CurrentJourneyPosition.LearningBlock].transform.gameObject.GetComponent<MapPin>().Dot.GetComponent<Renderer>().material = blackPin;
                posDot = miniMapScript.posDots[pos].transform.position;
                ChangeMaterialDotToRed(miniMapScript.posDots[pos]);
                AppManager.Instance.Player.CurrentJourneyPosition.PlaySession = miniMapScript.posDots[pos].GetComponent<Dot>().playSessionActual;            
            }
            else
            {
                if (pos > 0)
                {
                    ChangeMaterialDotToBlack(miniMapScript.posDots[pos]);
                    pos--;
                    posDot = miniMapScript.posDots[pos].transform.position;
                    ChangeMaterialDotToRed(miniMapScript.posDots[pos]);
                }
                AppManager.Instance.Player.CurrentJourneyPosition.PlaySession = miniMapScript.posDots[pos].GetComponent<Dot>().playSessionActual;
                AppManager.Instance.Player.CurrentJourneyPosition.LearningBlock = miniMapScript.posDots[pos].GetComponent<Dot>().learningBlockActual;
            }
          
            Vector3 pinPos = miniMapScript.pinLeft;
           // transform.LookAt(new Vector3(pinPos.x, pinPos.y + 3, pinPos.z));
            UpdateCurrenJourneyPosition();
        }

        public void ResetPosLetter()
        {
            if (AppManager.Instance.Player.CurrentJourneyPosition.PlaySession == 100)//Letter is on a pin
            {
                posDot = miniMapScript.posPines[AppManager.Instance.Player.CurrentJourneyPosition.LearningBlock].transform.position;
                transform.position = posDot;
                miniMapScript.posPines[AppManager.Instance.Player.CurrentJourneyPosition.LearningBlock].GetComponent<MapPin>().Dot.GetComponent<Renderer>().material = redPin;
                pos = miniMapScript.posPines[AppManager.Instance.Player.CurrentJourneyPosition.LearningBlock].GetComponent<MapPin>().posBefore;
            }
            else  //Letter is on a dot
            {
                posDot = miniMapScript.ropes[AppManager.Instance.Player.CurrentJourneyPosition.LearningBlock - 1].GetComponent<Rope>().dots
                    [AppManager.Instance.Player.CurrentJourneyPosition.PlaySession - 1].transform.position;
                transform.position = posDot;
                pos = miniMapScript.ropes[AppManager.Instance.Player.CurrentJourneyPosition.LearningBlock - 1].GetComponent<Rope>().dots
                    [AppManager.Instance.Player.CurrentJourneyPosition.PlaySession - 1].GetComponent<Dot>().pos;
                miniMapScript.posDots[pos].GetComponent<Renderer>().material = red;
            }
        }
        public void ResetPosLetterAfterChangeStage()
        {
            pos = 0;
            posDot = miniMapScript.posDots[pos].transform.position;
            transform.position = posDot;
            miniMapScript.posDots[pos].GetComponent<Renderer>().material = red;
            AppManager.Instance.Player.CurrentJourneyPosition.LearningBlock = 1;
            AppManager.Instance.Player.CurrentJourneyPosition.PlaySession = 1;
            UpdateCurrenJourneyPosition();
        }
        void UpdateCurrenJourneyPosition()
        {
            AppManager.Instance.Player.SetActualJourneyPosition(new JourneyPosition(AppManager.Instance.Player.CurrentJourneyPosition.Stage,
             AppManager.Instance.Player.CurrentJourneyPosition.LearningBlock,
              AppManager.Instance.Player.CurrentJourneyPosition.PlaySession), true);
        }
        public void ChangeMaterialDotToBlack(GameObject dot)
        {
           dot.GetComponent<Renderer>().material = black;
        }
        void ChangeMaterialDotToRed(GameObject dot)
        {
            dot.GetComponent<Renderer>().material = red;
        }
        public void ChangeMaterialPinToBlack(GameObject pin)
        {
            pin.GetComponent<Renderer>().material = blackPin;
        }
    }
}


