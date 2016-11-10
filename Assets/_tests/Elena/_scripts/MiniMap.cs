using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EA4S;
using ModularFramework.Core;
using EA4S.TestE;

namespace EA4S
{
    public class MiniMap : MonoBehaviour
    {
        [Header("Letter")]
        public GameObject letter;

        [Header("Cameras")]
        public GameObject[] cameraM;

        [Header("Pines")]
        public Transform[] posPines;
        public int numStepsBetweenPines;
        public GameObject dot;
        public GameObject[] posDots;
        public Vector3 pinLeft, pinRight;
        Quaternion rot;
        int numDot=0;
        int p;
        void Awake()
        {
            posDots = new GameObject[22];
            for(p=0; p<(posPines.Length-1);p++)
            {
                pinLeft = posPines[p].position;
                pinRight = posPines[p+1].position;

                CalculateStepsBetweenPines(pinLeft, pinRight);        
            }
            pinLeft = posPines[0].position;
            pinRight = posPines[1].position;
            // CameraGameplayController.I.MoveToPosition(cameraM[0].transform.position, cameraM[0].transform.rotation);
        }

        List<Db.PlaySessionData> myList = new List<Db.PlaySessionData>();
        List<PlaySessionState> myListPlaySessionState = new List<PlaySessionState>();
        void Start()
        {
            Debug.Log("MapManager PlaySession " + AppManager.Instance.Player.CurrentJourneyPosition.PlaySession);
            Debug.Log("Learning Block " + AppManager.Instance.Player.CurrentJourneyPosition.LearningBlock);
            //Debug.Log("LBlock " + AppManager.Instance.Teacher.GetMiniGamesForCurrentPlaySession());
            //Debug.Log("LBlock " + AppManager.Instance.GameSettin);

            myList = GetAllPlaySessionDataForStage(1);
            Debug.Log(myList.Count);
            //myListPlaySessionState = GetAllPlaySessionStateForStage(1);
            //Debug.Log(myListPlaySessionState[0].data.Description);
        }
        void Update()
        {
            Debug.Log("MapManager PlaySession " + AppManager.Instance.Player.CurrentJourneyPosition.PlaySession);
            Debug.Log("Learning Block " + AppManager.Instance.Player.CurrentJourneyPosition.LearningBlock);
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if(hit.collider.name == "pin-11")
                    {
                        pinRight = posPines[1].position;
                        pinLeft = posPines[2].position;

                        CalculateStepsBetweenPines(pinLeft, pinRight);

                        CameraGameplayController.I.MoveToPosition(cameraM[1].transform.position, cameraM[1].transform.rotation);

                        letter.GetComponent<LetterMovement>().ResetPosLetter(2, pinRight);
                    }

                }
            }
        }
        void CalculateStepsBetweenPines(Vector3 p1, Vector3 p2)
        {
           // int i = 0;
           // posDots = new Vector3[numStepsBetweenPines];
            float step = 1f / (numStepsBetweenPines + 1);
            for (float perc = step; perc < 1f; perc += step)
            {
                Vector3 v = Vector3.Lerp(p1, p2, perc);
                //posDots[numDot].transform.position = new Vector3(v.x, v.y, v.z);
                
                rot.eulerAngles = new Vector3(90, 0, 0);
                GameObject dotGo;
                dotGo = Instantiate(dot, v, rot) as GameObject;
                dotGo.GetComponent<Dot>().learningBlockActual = p+1;
                if(numDot%2==0)
                    dotGo.GetComponent<Dot>().playSessionActual = 2;
                else
                    dotGo.GetComponent<Dot>().playSessionActual = 3;
                posDots[numDot] = dotGo;
                numDot++;
            }
        }



        private class PlaySessionState
        {
            public Db.PlaySessionData data;
            public float score;

            public PlaySessionState(Db.PlaySessionData _data, float _score)
            {
                this.data = _data;
                this.score = _score;
            }
        }

        /// <summary>
        /// Returns a list of all play session data with its score (if a score exists) for the given stage
        /// </summary>
        /// <param name="_stage"></param>
        /// <returns></returns>
        private List<PlaySessionState> GetAllPlaySessionStateForStage(int _stage)
        {
            // Get all available scores for this stage
            List<Db.ScoreData> scoreData_list = AppManager.Instance.Teacher.GetCurrentScoreForPlaySessionsOfStage(_stage);

            // For each score entry, get its play session data and build a structure containing both
            List<PlaySessionState> playSessionState_list = new List<PlaySessionState>();
            for (int i = 0; i < scoreData_list.Count; i++)
            {
                var data = AppManager.Instance.DB.GetPlaySessionDataById(scoreData_list[i].ElementId);
                playSessionState_list.Add(new PlaySessionState(data, scoreData_list[i].Score));
            }

            return playSessionState_list;
        }

        /// <summary>
        /// Given a stage, returns the list of all play session data corresponding to it.
        /// </summary>
        /// <param name="_stage"></param>
        /// <returns></returns>
        private List<Db.PlaySessionData> GetAllPlaySessionDataForStage(int _stage)
        {
            return AppManager.Instance.DB.FindPlaySessionData(x => x.Stage == _stage);
        }

        public void Play()
        {
            if (AppManager.Instance.IsAssessmentTime)
                GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("app_Assessment");
            else
                GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("app_Wheel");
        }
    }
}
