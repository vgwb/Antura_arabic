using UnityEngine;
using System.Collections;
using EA4S;
using EA4S.TestE;

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
                        Debug.Log(hit.point+"HIT");
                        Debug.Log(transform.position + "LETTER");
                        numDots = miniMapScript.posDots.Length;
                        Debug.Log(numDots);
                        if(pos == 0)
                        {
                            Debug.Log("POS0");
                            //Move to the next dot?
                            distanceActualDotToHitPoint = Vector3.Distance(hit.point, miniMapScript.posDots[0].transform.position);
                            distanceNextDotToHitPoint = Vector3.Distance(hit.point, miniMapScript.posDots[1].transform.position);
                            if (distanceNextDotToHitPoint < distanceActualDotToHitPoint)
                                MoveToTheRightDot();
                        }
                        else if (pos == (numDots-1))
                        {
                            Debug.Log("POS2");
                            //Move to the before dot?                                     
                            distanceBeforelDotToHitPoint = Vector3.Distance(hit.point, miniMapScript.posDots[pos - 1].transform.position);
                            distanceActualDotToHitPoint = Vector3.Distance(hit.point, miniMapScript.posDots[pos].transform.position);
                            if (distanceBeforelDotToHitPoint < distanceActualDotToHitPoint)
                                MoveToTheLeftDot();
                        }                           
                        else
                        {
                            Debug.Log("POSI");
                            distanceBeforelDotToHitPoint = Vector3.Distance(hit.point, miniMapScript.posDots[pos-1].transform.position);
                            distanceNextDotToHitPoint = Vector3.Distance(hit.point, miniMapScript.posDots[pos+1].transform.position);
                            if (distanceNextDotToHitPoint < distanceBeforelDotToHitPoint)
                                MoveToTheRightDot();
                            else
                                MoveToTheLeftDot();
                        }
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
                    posDot = miniMapScript.posDots[pos].transform.position;
                    pos++;
                }        
                AppManager.Instance.Player.CurrentJourneyPosition.PlaySession = miniMapScript.posDots[pos-1].GetComponent<Dot>().playSessionActual;
            }

            Vector3 pinPos = miniMapScript.pinRight;
            transform.LookAt(new Vector3(pinPos.x, pinPos.y + 3, pinPos.z));                              
        }
       public void MoveToTheLeftDot()
       {
            if (AppManager.Instance.Player.CurrentJourneyPosition.PlaySession == 2)
            {
                posDot = miniMapScript.posPines[AppManager.Instance.Player.CurrentJourneyPosition.LearningBlock-1].transform.position;
                AppManager.Instance.Player.CurrentJourneyPosition.PlaySession = 1;
                //AppManager.Instance.Player.CurrentJourneyPosition.LearningBlock++;
            }
            else if(AppManager.Instance.Player.CurrentJourneyPosition.PlaySession == 1)
            {
                pos--;
                posDot = miniMapScript.posDots[pos-1].transform.position;
                AppManager.Instance.Player.CurrentJourneyPosition.PlaySession = miniMapScript.posDots[pos-1].GetComponent<Dot>().playSessionActual;
                AppManager.Instance.Player.CurrentJourneyPosition.LearningBlock--;
            }
            else
            {
                if (pos > 0)
                {
                    Debug.Log(pos);
                    pos--;
                    posDot = miniMapScript.posDots[pos-1].transform.position;
     
                    Debug.Log(pos);
                }
                AppManager.Instance.Player.CurrentJourneyPosition.PlaySession = miniMapScript.posDots[pos-1].GetComponent<Dot>().playSessionActual;
                AppManager.Instance.Player.CurrentJourneyPosition.LearningBlock = miniMapScript.posDots[pos].GetComponent<Dot>().learningBlockActual;
            }
          
            Vector3 pinPos = miniMapScript.pinLeft;
            transform.LookAt(new Vector3(pinPos.x, pinPos.y + 3, pinPos.z));
        }

        public void ResetPosLetter(int nPin, Vector3 pin)
        {
            pos = 0;
            // posDot = miniMapScript.posDots[0].transform.position;
            posDot = miniMapScript.posPines[0].transform.position;
            transform.LookAt(pin);
            AppManager.Instance.Player.CurrentJourneyPosition.PlaySession = 1;
            AppManager.Instance.Player.CurrentJourneyPosition.LearningBlock = 1;
        }
    }
}


