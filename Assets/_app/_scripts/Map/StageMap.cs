using System.Collections.Generic;
using System.Linq;
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
        [Header("Stage")]
        public int numberStage;

        [Header("Settings")]
        public Color color;
        public Transform cameraPivot;


        public int currentPlayerPositionIndex; //position of the PlayerPin in the map
        public int maxPlayerPositionIndex; //Max position PlayerPin can take


        // State
        private List<MapPin> pins;
        private List<MapDot> dots;
        private List<MapRope> ropes;
        [HideInInspector]
        public int nLearningBlocks;
        private int[] numberStepsPerLB;
        private int nPos;

        #region Properties

        public MapPin PinForLB(int lb)
        {
            return pins[lb - 1];
        }

        public MapRope RopeForLB(int lb)
        {
            return ropes[lb - 1];
        }

        #endregion

        [Header("References")]
        public Transform dotsPivot;
        public GameObject dotPrefab;

        [HideInInspector]
        public bool wholeStageUnlocked;

        [Header("PlayerPin")]
        public List<Vector3> playerPinTargetPositions = new List<Vector3>();
           
        #region Setup

        public void CalculateStepsStage()
        {
            // Find all pins 
            pins = new List<MapPin>(gameObject.GetComponentsInChildren<MapPin>());
            pins.Sort((x, y) => x.transform.GetSiblingIndex() - y.transform.GetSiblingIndex());
            Debug.Log(pins.ToDebugString());

            // TODO: Initialise all pins (note that first is fake)

            ropes = new List<MapRope>(gameObject.GetComponentsInChildren<MapRope>());
            ropes.Sort((x, y) => x.transform.GetSiblingIndex() - y.transform.GetSiblingIndex());
            Debug.Log(ropes.ToDebugString());
            
            /*dots = new List<MapDot>(gameObject.GetComponentsInChildren<MapDot>());
            dots.Sort((x, y) => x.transform.GetSiblingIndex() - y.transform.GetSiblingIndex());
            Debug.Log(dots.ToDebugString());*/

            // Setup all LBs and PSs
            CountPlaySessionsPerLearningBlock();
            for (int lb_i = 0; lb_i < nLearningBlocks; lb_i++)
            {
                var pinRight = pins[lb_i].transform.position;
                var pinLeft = pins[lb_i + 1].transform.position;    // TODO: we have 1 more pin in respect to LBs!
                playerPinTargetPositions.Add(pins[lb_i].transform.position);
                pins[lb_i].pos = nPos;
                nPos++;
                CreateDotsBetweenPins(lb_i, pinLeft, pinRight, numberStepsPerLB[lb_i]);
            }

            // TODO: LAST?
            //playerPinTargetPositions.Add(pins[lb_i].transform.position);
            //pins[lb_i].pos = nPos;

            if (!wholeStageUnlocked)
            {
                CalculateAvailablePlaySessions();
            }
            CalculatePin_RopeAvailable();
        }

        private void CreateDotsBetweenPins(int lb_i, Vector3 p1, Vector3 p2, int nPlaySessions)
        {
            int ps_i, z;
            Vector3 v;

            z = 1;
            var d = Vector3.Distance(p1, p2);
            var x = (d - 5 - 2 * 0.5f * nPlaySessions) / (nPlaySessions + 1);
            for (ps_i = 1; ps_i <= nPlaySessions; ps_i++)
            {
                v = (ps_i * x + 0.5f * z + 2.5f) * Vector3.Normalize(p1 - p2) + p2;
                z += 2;

                Quaternion rot = Quaternion.identity;
                rot.eulerAngles = new Vector3(90, 0, 0);

                // Create the new dot
                GameObject dotGo;
                dotGo = Instantiate(dotPrefab, v, rot);
                var mapDot = dotGo.GetComponent<MapDot>();
                mapDot.learningBlockActual = lb_i + 1;
                mapDot.playSessionActual = ps_i;
                mapDot.posIndex = nPos;

                if (lb_i < pins.Count - 1)
                {
                    ropes[lb_i].dots.Add(mapDot);
                    ropes[lb_i].assignedLearningBlock = lb_i + 1;
                }

                playerPinTargetPositions.Add(dotGo.transform.position);

                dotGo.transform.parent = dotsPivot;
                if (!wholeStageUnlocked)
                {
                    dotGo.SetActive(false);
                }
                nPos++;

                //if first playsession of the map
                if (lb_i == 0 && ps_i == 1)
                {
                    var introDialogues = dotGo.AddComponent<IntroDialogues>();
                    introDialogues.numberStage = numberStage;
                }
            }
        }

        private void CalculateAvailablePlaySessions()
        {
            var max_lb = AppManager.I.Player.MaxJourneyPosition.LearningBlock;
            var max_ps = AppManager.I.Player.MaxJourneyPosition.PlaySession;
            for (var lb = 1; lb <= max_lb; lb++)
            {
                var rope = RopeForLB(lb);

                if (lb == max_lb)
                {
                    // Maximum LB: check the max PS
                    if (max_ps == 100)
                    {
                        max_ps = rope.dots.Count;
                    }

                    for (var ps = 1; ps <= max_ps; ps++)
                    {
                        var dot = rope.DotForPS(ps);
                        dot.gameObject.SetActive(true);
                        maxPlayerPositionIndex = dot.posIndex;
                    }
                }
                else
                {
                    // Completed LB: enable all dots
                    for (var ps = 1; ps <= rope.dots.Count; ps++)
                    {
                        var dot = rope.DotForPS(ps);
                        dot.gameObject.SetActive(true);
                    }
                }
            }
            CalculatePin_RopeAvailable();
        }

        private void CalculatePin_RopeAvailable()
        {
            if (wholeStageUnlocked)
            {
                maxPlayerPositionIndex = playerPinTargetPositions.Count - 1;
                for (var lb = 1; lb < pins.Count - 1; lb++)
                {
                    // TODO: assign TAGS before?
                    var pin = PinForLB(lb);
                    pin.gameObject.tag = "Pin";
                    pin.unlocked = true;

                    RopeForLB(lb).transform.GetChild(0).tag = "Rope";
                }
                pins.Last().gameObject.tag = "Pin";
                pins.Last().unlocked = true;
            }
            else
            {
                var max_lb = AppManager.I.Player.MaxJourneyPosition.LearningBlock;
                var max_ps = AppManager.I.Player.MaxJourneyPosition.PlaySession;
                for (var lb = 1; lb <= max_lb; lb++)
                {
                    var pin = PinForLB(lb);
                    pin.gameObject.tag = "Pin";
                    pin.unlocked = true;

                    RopeForLB(lb).transform.GetChild(0).tag = "Rope";
                }

                if (max_ps == 100)
                {
                    //var pin = PinForLB(max_lb);
                    maxPlayerPositionIndex = pins[max_lb].pos;
                    pins[max_lb].gameObject.tag = "Pin";
                    pins[max_lb].unlocked = true;
                }
            }
        }

        // Count the number of steps (PlaySessions) per each learning block
        [HideInInspector]
        private void CountPlaySessionsPerLearningBlock()
        {
            nLearningBlocks = 0;
            var psDataList = GetAllPlaySessionDataForStage(numberStage);
            foreach (PlaySessionData psData in psDataList)
            {
                if (!psData.GetJourneyPosition().IsAssessment())
                {
                    numberStepsPerLB[psData.LearningBlock-1]++;
                    nLearningBlocks = Mathf.Max(nLearningBlocks, psData.LearningBlock);
                }
            }
        }

        #endregion


        #region Play Session State

        // TODO refactor: this is not called at all, but needed by the Map to show the state of a play session. The map should be improved to use these.

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
        /// Given a stage, returns the list of all play session data corresponding to it.
        /// </summary>
        /// <param name="_stage"></param>
        /// <returns></returns>
        private List<PlaySessionData> GetAllPlaySessionDataForStage(int _stage)
        {
            return AppManager.I.DB.FindPlaySessionData(x => x.Stage == _stage);
        }

        #endregion

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