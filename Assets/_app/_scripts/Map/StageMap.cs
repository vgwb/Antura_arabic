using System.Collections.Generic;
using System.Linq;
using Antura.Core;
using Antura.Database;
using Antura.Helpers;
using DG.DeExtensions;
using UnityEngine;

namespace Antura.Map
{
    /// <summary>
    /// A single stage map. Contains all dots and pins for the current stage.
    /// </summary>
    public class StageMap : MonoBehaviour
    {
        [Header("Stage")]
        public int stageNumber;

        [Header("Settings")]
        public Color color;
        public Transform cameraPivot;

        [HideInInspector]
        // Position of the PlayerPin in the map
        public int currentPlayerPosIndex; 
        [HideInInspector]
        // Max position index PlayerPin can take
        public int maxPlayerPosIndex;

        // Pins: number of learning blocks
        private List<Pin> pins;

        public Vector3 GetCurrentPlayerPosition()
        {
            return mapLocations[currentPlayerPosIndex].Position;
        }

        public JourneyPosition GetCurrentPlayerJourneyPosition()
        {
            return mapLocations[currentPlayerPosIndex].JourneyPos;
        }

        // Dots: number of play sessions (N per learning block + assessment under the Pin)
        //private List<Dot> dots;

        // Ropes: number of pins - 1
        //private List<Rope> ropes;

        // Data
        //private int nLearningBlocks;
        private int[] nPlaySessionsPerLb;

        #region Properties

        public Pin PinForLB(int lb)
        {
            return pins[lb];    // @note: we have pin 0 as fake
        }

        public Pin LastLBPin()
        {
            return pins.Last();
        }
        #endregion

        [Header("References")]
        public Transform dotsPivot;
        public GameObject dotPrefab;

        [HideInInspector]
        public bool wholeStageUnlocked;

        [Header("PlayerPin")]
        public List<IMapLocation> mapLocations = new List<IMapLocation>();
        //public List<Vector3> playerPinTargetPositions = new List<Vector3>();
           
        #region Setup

        public void Initialise()
        {
            CountPlaySessionsPerLearningBlock();

            // Find all pins and ropes and connect them
            pins = new List<Pin>(gameObject.GetComponentsInChildren<Pin>());
            pins.Sort((x, y) => x.transform.GetSiblingIndex() - y.transform.GetSiblingIndex());
            //Debug.Log(pins.ToDebugString());

            var ropes = new List<Rope>(gameObject.GetComponentsInChildren<Rope>());
            ropes.Sort((x, y) => x.transform.GetSiblingIndex() - y.transform.GetSiblingIndex());
            // Debug.Log(ropes.ToDebugString());

            //Debug.Log(pins.Count + " " + ropes.Count);
            for (int i = 1; i < pins.Count; i++)
            {
                pins[i].rope = ropes[i - 1];
                ropes[i - 1].pin = pins[i];
            }

            // Setup the first pin (it is not a LB, just for visual purposes)
            pins[0].SetLocked();
            pins[0].Initialise(stageNumber, 0);

            // Set the correct data (also creating the dots)
            int _playerPosIndexCount = 0; 
            for (var lb_i = 1; lb_i < pins.Count; lb_i++)
            {
                var pin = pins[lb_i];
                pin.Initialise(stageNumber, lb_i);

                CreateDotsBetweenPins(lb_i, pins[lb_i], pins[lb_i-1]);

                for (var ps_i = 1; ps_i <= pin.rope.dots.Count; ps_i++)
                {
                    var dot = pin.rope.DotForPS(ps_i);
                    mapLocations.Add(dot);
                    dot.playerPosIndex = _playerPosIndexCount++;
                    dot.Initialise(stageNumber, lb_i, ps_i);
                }

                mapLocations.Add(pin.dot);
                pin.dot.playerPosIndex = _playerPosIndexCount++;
            }

            /*dots = new List<Dot>(gameObject.GetComponentsInChildren<Dot>());
            dots.Sort((x, y) => x.transform.GetSiblingIndex() - y.transform.GetSiblingIndex());
            Debug.Log(dots.ToDebugString());*/

            // TODO: LAST?
            //playerPinTargetPositions.Add(pins[lb_i].transform.position);
            //pins[lb_i].pos = nPos;


            CalculateUnlockedPlaySessions();
            //CalculateUnlockedRopes();
        }

        private void CreateDotsBetweenPins(int lb_i, Pin pinFront, Pin pinBack)
        {
            int nPlaySessions = nPlaySessionsPerLb[lb_i-1];

            Vector3 pFront = pinFront.transform.position;
            Vector3 pBack = pinBack.transform.position;
            int ps_i;
            Vector3 v;

            int z = 1;
            var d = Vector3.Distance(pFront, pBack);
            var x = (d - 5 - 2 * 0.5f * nPlaySessions) / (nPlaySessions + 1);

            // Stretch the ROPE
            var rope = pinFront.rope;
            rope.transform.position = pinBack.transform.position;
            rope.transform.position += pinBack.currentPinMesh.transform.up * 4;
            rope.transform.LookAt(pinFront.transform.position + pinFront.currentPinMesh.transform.up * 4);
            rope.transform.Rotate(Vector3.forward,-90f);
            rope.transform.Rotate(0, 4, 0);
            rope.transform.SetLocalScaleZ((d / 20f) * 1.1f);

            for (ps_i = 1; ps_i <= nPlaySessions; ps_i++)
            {
                v = (ps_i * x + 0.5f * z + 2.5f) * Vector3.Normalize(pFront - pBack) + pBack;
                z += 2;

                Quaternion rot = Quaternion.identity;
                rot.eulerAngles = new Vector3(90, 0, 0);

                // Create a new dot
                GameObject dotGo = Instantiate(dotPrefab, v, rot);
                dotGo.transform.parent = dotsPivot;
                var mapDot = dotGo.GetComponent<Dot>();
                mapDot.SetLocked();
                //mapDot.learningBlock = lb_i + 1;
                //mapDot.playSession = ps_i;
                //mapDot.playerPosIndex = nPos;

                // Add it to the dot's rope
                //if (lb_i < pins.Count - 1)
                {
                    pinFront.rope.dots.Add(mapDot);
                    //ropes[lb_i].assignedLearningBlock = lb_i + 1;
                }

                //playerPinTargetPositions.Add(dotGo.transform.position);


                // TODO: move later
                if (!wholeStageUnlocked)
                {
                    dotGo.SetActive(false);
                }
                //nPos++;

                //if first playsession of the map
                if (lb_i == 0 && ps_i == 1)
                {
                    var introDialogues = dotGo.AddComponent<IntroDialogues>();
                    introDialogues.numberStage = stageNumber;
                }
            }
        }

        private void CalculateUnlockedPlaySessions()
        {
            if (wholeStageUnlocked)
            {
                for (var i = 1; i < pins.Count; i++)
                {
                    var pin = pins[i];
                    pin.SetUnlocked();
                    foreach (var dot in pin.rope.dots)
                    {
                        dot.SetUnlocked();
                    }
                }

                maxPlayerPosIndex = pins.Last().dot.playerPosIndex;
            }
            else
            {
                var max_lb = AppManager.I.Player.MaxJourneyPosition.LearningBlock;
                var max_ps = AppManager.I.Player.MaxJourneyPosition.PlaySession;
                for (var lb = 1; lb <= max_lb; lb++)
                {
                    var pin = PinForLB(lb);

                    if (lb < max_lb)
                    {
                        // Completed LB: enable all dots of that lb
                        pin.SetUnlocked();
                        for (var ps = 1; ps <= pin.rope.dots.Count; ps++)
                        {
                            var dot = pin.rope.DotForPS(ps);
                            dot.SetUnlocked();
                        }
                    }
                    else
                    { 
                        // Maximum reached LB: we check the max PS we reached
                        if (max_ps == AppManager.I.JourneyHelper.AssessmentPlaySessionIndex)
                        {
                            max_ps = pin.rope.dots.Count;   // fake 100 -> N
                            pin.SetUnlocked();
                        }

                        for (var ps = 1; ps <= max_ps; ps++)
                        {
                            var dot = pin.rope.DotForPS(ps);
                            dot.SetUnlocked();
                        }

                        maxPlayerPosIndex = pin.rope.DotForPS(max_ps).playerPosIndex;
                    }
                }
            }
        }

        /*private void CalculateUnlockedRopes()
        {
            if (wholeStageUnlocked)
            {
                //maxPlayerPosIndex = playerPinTargetPositions.Count - 1;
                for (var lb = 1; lb < pins.Count - 1; lb++)
                {
                    // TODO: assign TAGS before?
                    //var pin = PinForLB(lb);
                    //pin.gameObject.tag = "Pin";
                    //pin.unlocked = true;

                    //RopeForLB(lb).transform.GetChild(0).tag = "Rope";
                }
                //pins.Last().gameObject.tag = "Pin";
                //pins.Last().unlocked = true;
            }
            else
            {
                var max_lb = AppManager.I.Player.MaxJourneyPosition.LearningBlock;
                var max_ps = AppManager.I.Player.MaxJourneyPosition.PlaySession;
                for (var lb = 1; lb <= max_lb; lb++)
                {
                    var pin = PinForLB(lb);
                    //pin.gameObject.tag = "Pin";
                    pin.SetUnlocked();
                    //pin.unlocked = true;

                    RopeForLB(lb).transform.GetChild(0).tag = "Rope";
                }

                if (max_ps == 100)
                {
                    //var pin = PinForLB(max_lb);
                    maxPlayerPosIndex = pins[max_lb].playerPosIndex;
                    pins[max_lb].gameObject.tag = "Pin";
                    pins[max_lb].unlocked = true;
                }
            }
        }*/

        // Count the number of steps (PlaySessions) per each learning block
        private void CountPlaySessionsPerLearningBlock()
        {
            var psDataList = GetAllPlaySessionDataForStage(stageNumber);
            var lbDataList = GetAllLearningBlockDataForStage(stageNumber);
            //nLearningBlocks = lbDataList.Count;
            nPlaySessionsPerLb = new int[lbDataList.Count];
            foreach (PlaySessionData psData in psDataList)
            {
                if (!psData.GetJourneyPosition().IsAssessment())
                {
                    nPlaySessionsPerLb[psData.LearningBlock-1]++;
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
        private List<PlaySessionState> GetAllPlaySessionStatesForStage(int _stage)
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
        private List<PlaySessionData> GetAllPlaySessionDataForStage(int _stage)
        {
            return AppManager.I.DB.FindPlaySessionData(x => x.Stage == _stage);
        }

        /// <summary>
        /// Given a stage, returns the list of all learning block data corresponding to it.
        /// </summary>
        private List<LearningBlockData> GetAllLearningBlockDataForStage(int _stage)
        {
            return AppManager.I.DB.FindLearningBlockData(x => x.Stage == _stage);
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