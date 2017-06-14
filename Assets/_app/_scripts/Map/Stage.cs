using System.Collections.Generic;
using EA4S.Core;
using UnityEngine;

namespace EA4S.Map
{
    public class Stage : MonoBehaviour
    {

        [Header("Pines")]
        public GameObject[] pines;
        public int numberLearningBlocks;

        [Header("PlaySessions")]
        public GameObject dot;
        public Transform stepsParent;
        public int[] numberStepsPerLB;

        [Header("Ropes")]
        public GameObject[] ropes;

        [Header("Stage")]
        public bool isAvailableTheWholeMap;
        public int numberStage;
        public bool isTheBeginningNewStage;

        [Header("PlayerPin")]
        public List<GameObject> positionsPlayerPin = new List<GameObject>(); //All positions Pin can take over the map: dots and pins
        public int positionPin; //position of the PlayerPin in the map
        public int positionPinMax;//Max position PlayerPin can take
        int nPos = 0;

        Quaternion rot = Quaternion.identity;
        int i;

        List<Database.PlaySessionData> psData = new List<Database.PlaySessionData>();

        public void CalculateStepsStage()
        {
            NumberPlaySessionsPerLEB();
            Vector3 pinRight, pinLeft;
            for (i = 0; i < numberLearningBlocks; i++) {
                pinRight = pines[i].transform.position;
                pinLeft = pines[i + 1].transform.position;
                positionsPlayerPin.Add(pines[i]);
                pines[i].GetComponent<MapPin>().pos = nPos;
                nPos++;
                CalculateStepsBetweenPines(pinLeft, pinRight, numberStepsPerLB[i]);
            }
            positionsPlayerPin.Add(pines[i]);
            pines[i].GetComponent<MapPin>().pos = nPos;
            if (!isAvailableTheWholeMap) {
                CalculatePlaySessionAvailables();
            }
            CalculatePin_RopeAvailable();
        }

        void CalculateStepsBetweenPines(Vector3 p1, Vector3 p2, int steps)
        {
            int p, z;
            Vector3 v;

            z = 1;
            float d = Vector3.Distance(p1, p2);
            float x = (((d - 5) - (2 * 0.5f * steps))) / (steps + 1);
            for (p = 1; p <= steps; p++) {
                v = (p * x + (0.5f * z) + 2.5f) * Vector3.Normalize(p1 - p2) + p2;
                z += 2;

                rot.eulerAngles = new Vector3(90, 0, 0);
                GameObject dotGo;
                dotGo = Instantiate(dot, v, rot) as GameObject;
                dotGo.GetComponent<Dot>().learningBlockActual = i + 1;
                dotGo.GetComponent<Dot>().playSessionActual = p;
                dotGo.GetComponent<Dot>().pos = nPos;

                if (i < pines.Length - 1) {
                    ropes[i].GetComponent<Rope>().dots.Add(dotGo);
                    ropes[i].GetComponent<Rope>().learningBlockRope = i + 1;
                }

                positionsPlayerPin.Add(dotGo);

                dotGo.transform.parent = stepsParent;
                if (!isAvailableTheWholeMap) {
                    dotGo.SetActive(false);
                }
                nPos++;

                //if first playsession of the map
                if ((i == 0) && (p == 1)) {
                    dotGo.AddComponent<Dialogues>();
                    dotGo.GetComponent<Dialogues>().numberStage = numberStage;
                }
            }
        }
        void CalculatePlaySessionAvailables()
        {
            int l = (AppManager.Instance as AppManager).Player.MaxJourneyPosition.LearningBlock;
            int p = (AppManager.Instance as AppManager).Player.MaxJourneyPosition.PlaySession;
            for (int i = 0; i < l; i++) {
                if (ropes[i].GetComponent<Rope>().learningBlockRope == l) {
                    if (p == 100) {
                        p = ropes[i].GetComponent<Rope>().dots.Count;
                    }
                    for (int j = 0; j < p; j++) {
                        ropes[i].GetComponent<Rope>().dots[j].SetActive(true);
                        positionPinMax = ropes[i].GetComponent<Rope>().dots[j].GetComponent<Dot>().pos;
                    }
                } else {
                    for (int m = 0; m < ropes[i].GetComponent<Rope>().dots.Count; m++) {
                        ropes[i].GetComponent<Rope>().dots[m].SetActive(true);
                    }
                }
            }
            CalculatePin_RopeAvailable();
        }
        void CalculatePin_RopeAvailable()
        {
            if (isAvailableTheWholeMap) {
                positionPinMax = positionsPlayerPin.Count - 1;
                for (int i = 1; i < (pines.Length - 1); i++) {
                    pines[i].tag = "Pin";
                    pines[i].GetComponent<MapPin>().unlocked = true;
                    ropes[i].transform.GetChild(0).tag = "Rope";
                }
                pines[pines.Length - 1].tag = "Pin";
                pines[pines.Length - 1].GetComponent<MapPin>().unlocked = true;
            } else {
                int l = (AppManager.Instance as AppManager).Player.MaxJourneyPosition.LearningBlock;
                int p = (AppManager.Instance as AppManager).Player.MaxJourneyPosition.PlaySession;
                for (int i = 1; i < l; i++) {
                    pines[i].tag = "Pin";
                    pines[i].GetComponent<MapPin>().unlocked = true;
                    ropes[i].transform.GetChild(0).tag = "Rope";
                }
                if (p == 100) {
                    positionPinMax = pines[l].GetComponent<MapPin>().pos;
                    pines[l].tag = "Pin";
                    pines[l].GetComponent<MapPin>().unlocked = true;
                }
            }
        }

        void NumberPlaySessionsPerLEB()
        {
            psData = GetAllPlaySessionDataForStage(numberStage);
            for (int d = 0; d < psData.Count; d++) {
                if (psData[d].PlaySession != 100) {
                    switch (psData[d].LearningBlock) {
                        case 1:
                            numberStepsPerLB[0]++;
                            break;
                        case 2:
                            numberStepsPerLB[1]++;
                            break;
                        case 3:
                            numberStepsPerLB[2]++;
                            break;
                        case 4:
                            numberStepsPerLB[3]++;
                            break;
                        case 5:
                            numberStepsPerLB[4]++;
                            break;
                        case 6:
                            numberStepsPerLB[5]++;
                            break;
                        case 7:
                            numberStepsPerLB[6]++;
                            break;
                        case 8:
                            numberStepsPerLB[7]++;
                            break;
                        case 9:
                            numberStepsPerLB[8]++;
                            break;
                        case 10:
                            numberStepsPerLB[9]++;
                            break;
                        case 11:
                            numberStepsPerLB[10]++;
                            break;
                        case 12:
                            numberStepsPerLB[11]++;
                            break;
                        case 13:
                            numberStepsPerLB[12]++;
                            break;
                        case 14:
                            numberStepsPerLB[13]++;
                            break;
                        case 15:
                            numberStepsPerLB[14]++;
                            break;
                        default:
                            break;

                    }
                }
            }
        }

        private class PlaySessionState
        {
            public Database.PlaySessionData data;
            public float score;

            public PlaySessionState(Database.PlaySessionData _data, float _score)
            {
                this.data = _data;
                this.score = _score;
            }
        }

        // refactor: these methods are not called at all, but are needed by the Map to show the state of a play session. The map should be improved to use these.

        /// <summary>
        /// Returns a list of all play session data with its score (if a score exists) for the given stage
        /// </summary>
        /// <param name="_stage"></param>
        /// <returns></returns>
        private List<PlaySessionState> GetAllPlaySessionStateForStage(int _stage)
        {
            // Get all available scores for this stage
            List<Database.JourneyScoreData> scoreData_list = (AppManager.Instance as AppManager).ScoreHelper.GetCurrentScoreForPlaySessionsOfStage(_stage);

            // For each score entry, get its play session data and build a structure containing both
            List<PlaySessionState> playSessionState_list = new List<PlaySessionState>();
            for (int i = 0; i < scoreData_list.Count; i++) {
                var data = (AppManager.Instance as AppManager).DB.GetPlaySessionDataById(scoreData_list[i].ElementId);
                playSessionState_list.Add(new PlaySessionState(data, scoreData_list[i].Stars));
            }

            return playSessionState_list;
        }

        /// <summary>
        /// Given a stage, returns the list of all play session data corresponding to it.
        /// </summary>
        /// <param name="_stage"></param>
        /// <returns></returns>
        private List<Database.PlaySessionData> GetAllPlaySessionDataForStage(int _stage)
        {
            return (AppManager.Instance as AppManager).DB.FindPlaySessionData(x => x.Stage == _stage);
        }

    }
}
