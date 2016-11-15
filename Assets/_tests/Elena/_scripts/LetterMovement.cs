using UnityEngine;
using System.Collections;
using EA4S;
using EA4S.TestE;
using System.Collections.Generic;

namespace EA4S.TestE
{
    public class LetterMovement : MonoBehaviour {

        public MiniMap miniMapScript;
        public float speed;
        Transform target;
        public Vector3 posDot;
        public int pos;

        float distanceNextDotToHitPoint;
        float distanceBeforelDotToHitPoint;
        float distanceActualDotToHitPoint;
        int numDots;

        void Start () {
            ResetPosLetter(1, miniMapScript.pinRight);
        }

        void Update()
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(posDot.x, transform.position.y, posDot.z), speed * Time.deltaTime);
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.tag == "Rope")
                    {

                        int numDotsRope = hit.transform.gameObject.GetComponent<Rope>().dots.Count;

                        float distaceHitToDot = 1000;
                        float distanceHitBefore = 0;
                        int dotCloser = 0;

                        for (int i = 0; i < numDotsRope; i++)
                        {
                            distanceHitBefore = Vector3.Distance(hit.point,
                                hit.transform.gameObject.GetComponent<Rope>().dots[i].transform.position);
                            if (distanceHitBefore < distaceHitToDot)
                            {
                                distaceHitToDot = distanceHitBefore;
                                dotCloser = i;
                            }
                        }

                        int posDotMiniMapScript = hit.transform.gameObject.GetComponent<Rope>().dots[dotCloser].GetComponent<Dot>().pos;
                        posDot = miniMapScript.posDots[posDotMiniMapScript].transform.position;
                        pos = posDotMiniMapScript;

                        if (hit.transform.gameObject.GetComponent<Rope>().learningBlockRope != AppManager.Instance.Player.CurrentJourneyPosition.LearningBlock)
                            transform.position = miniMapScript.posDots[posDotMiniMapScript].transform.position;

                        AppManager.Instance.Player.CurrentJourneyPosition.PlaySession = hit.transform.gameObject.GetComponent<Rope>().dots[dotCloser].GetComponent<Dot>().playSessionActual;
                        AppManager.Instance.Player.CurrentJourneyPosition.LearningBlock = hit.transform.gameObject.GetComponent<Rope>().dots[dotCloser].GetComponent<Dot>().learningBlockActual;
                    }

                    if (hit.collider.tag == "Pin")
                    {
                        transform.position = hit.transform.position;
                        posDot = hit.transform.position;
                        pos = hit.transform.gameObject.GetComponent<MapPin>().posBefore;

                        AppManager.Instance.Player.CurrentJourneyPosition.PlaySession = 1;
                        AppManager.Instance.Player.CurrentJourneyPosition.LearningBlock = hit.transform.gameObject.GetComponent<MapPin>().Number;
                    }
                }
            }
        }
        public void MoveToTheRightDot()
        {
            if(AppManager.Instance.Player.CurrentJourneyPosition.PlaySession==3)
            {
                posDot = miniMapScript.posPines[AppManager.Instance.Player.CurrentJourneyPosition.LearningBlock].transform.position;
                AppManager.Instance.Player.CurrentJourneyPosition.PlaySession = 1;
                AppManager.Instance.Player.CurrentJourneyPosition.LearningBlock++;
            }
            else
            {       
                if (pos < (miniMapScript.posDots.Length - 1))
                {
                    pos++;
                    posDot = miniMapScript.posDots[pos].transform.position;                
                }        
                AppManager.Instance.Player.CurrentJourneyPosition.PlaySession = miniMapScript.posDots[pos].GetComponent<Dot>().playSessionActual;
            }

            Vector3 pinPos = miniMapScript.pinRight;
            transform.LookAt(new Vector3(pinPos.x, pinPos.y + 3, pinPos.z));                              
        }
       public void MoveToTheLeftDot()
       {
            if (AppManager.Instance.Player.CurrentJourneyPosition.PlaySession == 2)
            {
                if(pos>0)
                {
                    posDot = miniMapScript.posPines[AppManager.Instance.Player.CurrentJourneyPosition.LearningBlock - 1].transform.position;
                    AppManager.Instance.Player.CurrentJourneyPosition.PlaySession = 1;
                    //AppManager.Instance.Player.CurrentJourneyPosition.LearningBlock++;
                }
            }
            else if(AppManager.Instance.Player.CurrentJourneyPosition.PlaySession == 1)
            {
                if(pos%2==0)
                   pos--;
                posDot = miniMapScript.posDots[pos].transform.position;
                AppManager.Instance.Player.CurrentJourneyPosition.PlaySession = miniMapScript.posDots[pos].GetComponent<Dot>().playSessionActual;
                AppManager.Instance.Player.CurrentJourneyPosition.LearningBlock--;
            }
            else
            {
                if (pos > 0)
                {
                    Debug.Log(pos);
                    pos--;
                    posDot = miniMapScript.posDots[pos].transform.position;
     
                    Debug.Log(pos);
                }
                AppManager.Instance.Player.CurrentJourneyPosition.PlaySession = miniMapScript.posDots[pos].GetComponent<Dot>().playSessionActual;
                AppManager.Instance.Player.CurrentJourneyPosition.LearningBlock = miniMapScript.posDots[pos].GetComponent<Dot>().learningBlockActual;
            }
          
            Vector3 pinPos = miniMapScript.pinLeft;
            transform.LookAt(new Vector3(pinPos.x, pinPos.y + 3, pinPos.z));
        }

        public void ResetPosLetter(int nPin, Vector3 pin)
        {
            pos = 0;
            // posDot = miniMapScript.posDots[0].transform.position;
            posDot = miniMapScript.posDots[0].transform.position;
            transform.LookAt(pin);
            AppManager.Instance.Player.CurrentJourneyPosition.PlaySession = 2;
            AppManager.Instance.Player.CurrentJourneyPosition.LearningBlock = 1;
        }
    }
}


