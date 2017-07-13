using System.Collections.Generic;
using Antura.Database;
using Antura.Helpers;
using UnityEngine;

namespace Antura.Map
{
    /// <summary>
    ///     A single stage map. Contains all dots and pins for the current stage.
    /// </summary>
    public class StageMap : MonoBehaviour
    {
        [Header("Settings")]
        public Color color;
        public Transform cameraPivot;

        [Header("Stage")]
        public int numberStage;
        public int numberLearningBlocks;
        public int[] numberStepsPerLB;
        public bool isTheBeginningNewStage;

        public int positionPin; //position of the PlayerPin in the map
        public int positionPinMax; //Max position PlayerPin can take
        private int nPos;

        public Transform stepsParent;

        // state
        private List<MapPin> pins;
        private List<MapDot> dots;

        [Header("Pines")] public GameObject[] pines;
        [Header("PlaySessions")] public GameObject dot;
        [Header("Ropes")] public GameObject[] ropes;


        [HideInInspector] public bool isAvailableTheWholeMap;

        [Header("PlayerPin")]
        public List<GameObject> positionsPlayerPin = new List<GameObject>();
           
        private Quaternion rot = Quaternion.identity;
        private int i;

        private List<PlaySessionData> psData = new List<PlaySessionData>();

        public void CalculateStepsStage()
        {
            pins = new List<MapPin>(gameObject.GetComponentsInChildren<MapPin>());
            pins.Sort((x, y) => x.transform.GetSiblingIndex() - y.transform.GetSiblingIndex());
            Debug.Log(pins.ToDebugString());

            dots = new List<MapDot>(gameObject.GetComponentsInChildren<MapDot>());
            dots.Sort((x, y) => x.transform.GetSiblingIndex() - y.transform.GetSiblingIndex());
            Debug.Log(dots.ToDebugString());

            NumberPlaySessionsPerLEB();
            Vector3 pinRight, pinLeft;
            for (i = 0; i < numberLearningBlocks; i++)
            {
                pinRight = pines[i].transform.position;
                pinLeft = pines[i + 1].transform.position;
                positionsPlayerPin.Add(pines[i]);
                pines[i].GetComponent<MapPin>().pos = nPos;
                nPos++;
                CalculateStepsBetweenPines(pinLeft, pinRight, numberStepsPerLB[i]);
            }
            positionsPlayerPin.Add(pines[i]);
            pines[i].GetComponent<MapPin>().pos = nPos;
            if (!isAvailableTheWholeMap) CalculatePlaySessionAvailables();
            CalculatePin_RopeAvailable();
        }

        private void CalculateStepsBetweenPines(Vector3 p1, Vector3 p2, int steps)
        {
            int p, z;
            Vector3 v;

            z = 1;
            var d = Vector3.Distance(p1, p2);
            var x = (d - 5 - 2 * 0.5f * steps) / (steps + 1);
            for (p = 1; p <= steps; p++)
            {
                v = (p * x + 0.5f * z + 2.5f) * Vector3.Normalize(p1 - p2) + p2;
                z += 2;

                rot.eulerAngles = new Vector3(90, 0, 0);
                GameObject dotGo;
                dotGo = Instantiate(dot, v, rot);
                dotGo.GetComponent<MapDot>().learningBlockActual = i + 1;
                dotGo.GetComponent<MapDot>().playSessionActual = p;
                dotGo.GetComponent<MapDot>().pos = nPos;

                if (i < pines.Length - 1)
                {
                    ropes[i].GetComponent<Rope>().dots.Add(dotGo);
                    ropes[i].GetComponent<Rope>().learningBlockRope = i + 1;
                }

                positionsPlayerPin.Add(dotGo);

                dotGo.transform.parent = stepsParent;
                if (!isAvailableTheWholeMap) dotGo.SetActive(false);
                nPos++;

                //if first playsession of the map
                if (i == 0 && p == 1)
                {
                    dotGo.AddComponent<IntroDialogues>();
                    dotGo.GetComponent<IntroDialogues>().numberStage = numberStage;
                }
            }
        }

        private void CalculatePlaySessionAvailables()
        {
            var l = AppManager.I.Player.MaxJourneyPosition.LearningBlock;
            var p = AppManager.I.Player.MaxJourneyPosition.PlaySession;
            for (var i = 0; i < l; i++)
                if (ropes[i].GetComponent<Rope>().learningBlockRope == l)
                {
                    if (p == 100) p = ropes[i].GetComponent<Rope>().dots.Count;
                    for (var j = 0; j < p; j++)
                    {
                        ropes[i].GetComponent<Rope>().dots[j].SetActive(true);
                        positionPinMax = ropes[i].GetComponent<Rope>().dots[j].GetComponent<MapDot>().pos;
                    }
                }
                else
                {
                    for (var m = 0; m < ropes[i].GetComponent<Rope>().dots.Count; m++)
                        ropes[i].GetComponent<Rope>().dots[m].SetActive(true);
                }
            CalculatePin_RopeAvailable();
        }

        private void CalculatePin_RopeAvailable()
        {
            if (isAvailableTheWholeMap)
            {
                positionPinMax = positionsPlayerPin.Count - 1;
                for (var i = 1; i < pines.Length - 1; i++)
                {
                    pines[i].tag = "Pin";
                    pines[i].GetComponent<MapPin>().unlocked = true;
                    ropes[i].transform.GetChild(0).tag = "Rope";
                }
                pines[pines.Length - 1].tag = "Pin";
                pines[pines.Length - 1].GetComponent<MapPin>().unlocked = true;
            }
            else
            {
                var l = AppManager.I.Player.MaxJourneyPosition.LearningBlock;
                var p = AppManager.I.Player.MaxJourneyPosition.PlaySession;
                for (var i = 1; i < l; i++)
                {
                    pines[i].tag = "Pin";
                    pines[i].GetComponent<MapPin>().unlocked = true;
                    ropes[i].transform.GetChild(0).tag = "Rope";
                }
                if (p == 100)
                {
                    positionPinMax = pines[l].GetComponent<MapPin>().pos;
                    pines[l].tag = "Pin";
                    pines[l].GetComponent<MapPin>().unlocked = true;
                }
            }
        }

        private void NumberPlaySessionsPerLEB()
        {
            psData = GetAllPlaySessionDataForStage(numberStage);
            for (var d = 0; d < psData.Count; d++)
            {
                if (!psData[d].GetJourneyPosition().IsAssessment())
                {
                    switch (psData[d].LearningBlock)
                    {
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

        // TODO refactor: these methods are not called at all, but are needed by the Map to show the state of a play session. The map should be improved to use these.

        /// <summary>
        ///     Returns a list of all play session data with its score (if a score exists) for the given stage
        /// </summary>
        /// <param name="_stage"></param>
        /// <returns></returns>
        private List<PlaySessionState> GetAllPlaySessionStateForStage(int _stage)
        {
            // Get all available scores for this stage
            var scoreData_list = AppManager.I.ScoreHelper.GetCurrentScoreForPlaySessionsOfStage(_stage);

            // For each score entry, get its play session data and build a structure containing both
            var playSessionState_list = new List<PlaySessionState>();
            for (var i = 0; i < scoreData_list.Count; i++)
            {
                var data = AppManager.I.DB.GetPlaySessionDataById(scoreData_list[i].ElementId);
                playSessionState_list.Add(new PlaySessionState(data, scoreData_list[i].Stars));
            }

            return playSessionState_list;
        }

        /// <summary>
        ///     Given a stage, returns the list of all play session data corresponding to it.
        /// </summary>
        /// <param name="_stage"></param>
        /// <returns></returns>
        private List<PlaySessionData> GetAllPlaySessionDataForStage(int _stage)
        {
            return AppManager.I.DB.FindPlaySessionData(x => x.Stage == _stage);
        }

        #region Show / Hide

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        #endregion
    }
}