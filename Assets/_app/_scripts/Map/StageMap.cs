using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using Antura.Core;
using Antura.Database;
using Antura.Teacher;
using DG.DeExtensions;
using UnityEngine;
using DG.DeInspektor;
using DG.DeInspektor.Attributes;
using UnityEditor;

namespace Antura.Map
{
    /// <summary>
    /// A single stage map.
    /// Contains all pins for the current stage.
    /// </summary>
    public class StageMap : MonoBehaviour
    {
        [Header("Stage")]
        // Stage number assigned to this map
        public int stageNumber;

        [Header("Settings")]
        public Color color;
        public Transform cameraPivot;

        [HideInInspector]
        // Current position of the PlayerPin in this stage map
        public int currentPinIndex; 
        [HideInInspector]
        // Max position index PlayerPin can take in this stage map
        public int maxPinIndex;

        // Pins: one per Play Session
        // (TODO: remove: plus a fake one at the start ?)
        private List<Pin> pins;

        // Data
        private int[] nPlaySessionsPerLb;

        #region Properties

        public Vector3 GetCurrentPlayerPosVector()
        {
            return mapLocations[currentPinIndex].Position;
        }

        public JourneyPosition GetCurrentPlayerPosJourneyPosition()
        {
            return mapLocations[currentPinIndex].JourneyPos;
        }

        public Pin PinForLB(int lb)
        {
            if (lb >= pins.Count) return null;
            return pins[lb];    // @note: we have pin 0 as fake
        }


        public Pin PinForJourneyPosition(JourneyPosition jp)
        {
            return pins.FirstOrDefault(p => p.JourneyPos.Equals(jp));
        }

        #endregion

        [Header("References")]
        public Transform dotsPivot;
        public GameObject dotPrefab;

        [HideInInspector]
        private bool stageUnlocked;   // at least one PS for this stage is unlocked
        [HideInInspector]
        private bool wholeStageUnlocked;    // all PS and LB of this stage are unlocked

        [Header("PlayerPin")]
        public List<IMapLocation> mapLocations = new List<IMapLocation>();
           
        #region Setup

        [DeMethodButton("Rename Pins")]
        public void RenamePins()
        {
            var pins = new List<Pin>(gameObject.GetComponentsInChildren<Pin>());
            for (var index = 0; index < pins.Count; index++)
            {
                var pin = pins[index];
                pin.gameObject.name = "Pin_" + (index+1);
                EditorUtility.SetDirty(pin.gameObject);
            }
        }

        [DeMethodButton("Randomize Pins")]
        public void RandomizePins()
        {
            // Randomize the position of the pins
            var pins = new List<Pin>(gameObject.GetComponentsInChildren<Pin>());
            for (var index = 0; index < pins.Count; index++)
            {
                var pin = pins[index];
                pin.transform.localPosition = new Vector3(index * (-30), 0, Random.Range(-30, 30));
                EditorUtility.SetDirty(pin.gameObject);
            }
        }


        public void Initialise(bool _stageUnlocked, bool _wholeStageUnlocked)
        {
            stageUnlocked = _stageUnlocked;
            wholeStageUnlocked = _wholeStageUnlocked;

            CountPlaySessionsPerLearningBlock();

            // Find all pins and ropes and connect them
            pins = new List<Pin>(gameObject.GetComponentsInChildren<Pin>());
            pins.Sort((x, y) => x.transform.GetSiblingIndex() - y.transform.GetSiblingIndex());

            var ropes = new List<Rope>(gameObject.GetComponentsInChildren<Rope>());
            ropes.Sort((x, y) => x.transform.GetSiblingIndex() - y.transform.GetSiblingIndex());

            for (int i = 1; i < pins.Count; i++)
            {
                //pins[i].rope = ropes[i - 1];  
                pins[i].rope = ropes[Mathf.Min(i-1,ropes.Count-1)];      // TEMPORARY

                pins[i].rope.name = "MapRope " + i;
            }

            // Setup the first pin (it is not a LB, just for visual purposes)
            pins[0].SetLocked();
            pins[0].Initialise(-1, new JourneyPosition(0,0,0));

            // Set the correct data to all pins (also creating the dots)
            var allPlaySessionStates = GetAllPlaySessionStatesForStage(stageNumber);

            if (allPlaySessionStates.Count > pins.Count)
            {
                Debug.LogError("Stage " + stageNumber + " has only " + pins.Count + " pins but needs " + allPlaySessionStates.Count);
                return;
            }
            else if (allPlaySessionStates.Count < pins.Count)
            {
                Debug.LogError("Stage " + stageNumber + " has " + pins.Count + " pins but needs only " + allPlaySessionStates.Count);
                return;
            }

            int playerPosIndexCount = 0; 
            JourneyPosition assignedJourneyPosition = new JourneyPosition(stageNumber,1,1);

            for (int jp_i = 0; jp_i < allPlaySessionStates.Count; jp_i++)
            {
                var pin = pins[jp_i];
                var psState = allPlaySessionStates[jp_i];
                var journeyPos = psState.data.GetJourneyPosition();
                pin.Initialise(playerPosIndexCount++, journeyPos);
                mapLocations.Add(pin);

                // Advance to the next one
                //assignedJourneyPosition = AppManager.I.JourneyHelper.FindNextJourneyPosition(assignedJourneyPosition);
                // TODO: advance assignedJourneyPosition

                pin.SetLocked();
                pin.SetPlaySessionState(allPlaySessionStates.Find(x =>
                    x.data.GetJourneyPosition().Equals(assignedJourneyPosition)
                ));

                // TODO: dots are now handled DIFFERENTLY, they are all pins and the dots are just small visual dots
                /*
                CreateDotsBetweenPins(lb_i, pins[lb_i], pins[lb_i-1]);
                for (var ps_i = 1; ps_i <= pin.rope.dots.Count; ps_i++)
                {
                    var dot = pin.rope.DotForPS(ps_i);
                    mapLocations.Add(dot);
                    dot.pinIndex = playerPosIndexCount++;
                    dot.Initialise(stageNumber, lb_i, ps_i);
                    dot.SetPlaySessionState(allPlaySessionStates.Find(x =>
                            x.data.LearningBlock == lb_i && x.data.PlaySession == ps_i
                    ));
                }*/
            }

            UnlockPlaySessions();

            Disappear();
        }

        #region Appear Animation
        private bool hasAppeared = false;

        void Disappear()
        {
            foreach (var pin in pins)
            {
                pin.Disappear();
            }
        }

        public void Appear(JourneyPosition fromPos, JourneyPosition toPos)
        {
            StartCoroutine(AppearCO(fromPos,toPos));
        }

        private IEnumerator AppearCO(JourneyPosition fromPos, JourneyPosition toPos)
        {
            if (hasAppeared)
            {
                yield break;
            }

            hasAppeared = true;

            Debug.Log("Animating from " + fromPos + " to " + toPos);

            // First, let all the available dots appear, up to FROM
            FlushAppear(fromPos);

            // Then, let the remaining ones appear in order, up to TO
            int upToPosIndex = StageMapsManager.GetPosIndexFromJourneyPosition(this, toPos);
            float duration = 0.2f;
            foreach (var pin in pins)
            {
                // First the dots
                // TODO: DEPRECATED!
                /*if (pin.rope != null)
                {
                    foreach (var ropeDot in pin.rope.dots)
                    {
                        //if (!ropeDot.isLocked)
                        if (!ropeDot.Appeared && ropeDot.playerPosIndex <= upToPosIndex)
                        {
                            ropeDot.Appear(0.0f, duration);
                            yield return new WaitForSeconds(duration);
                            duration *= 0.9f;
                            if (duration <= 0.01f) duration = 0.02f;
                        }
                    }
                }*/

                // Then the pins
                //if (!pin.isLocked)
                if (!pin.Appeared && pin.pinIndex <= upToPosIndex)
                {
                    pin.Appear(duration);
                    yield return new WaitForSeconds(duration);
                    duration *= 0.9f;
                    if (duration <= 0.01f) duration = 0.02f;
                }
            }

        }

        public void FlushAppear(JourneyPosition upToJourneyPos)
        {
            //Debug.Log("FLUSH TO " + upToJourneyPos);
            int upToPosIndex = StageMapsManager.GetPosIndexFromJourneyPosition(this, upToJourneyPos);
            foreach (var pin in pins)
            {
                // First the dots
                // TODO: DEPRECATED!
                /*if (pin.rope != null)
                {
                    foreach (var ropeDot in pin.rope.dots)
                    {
                        if (ropeDot.playerPosIndex <= upToPosIndex) ropeDot.FlushAppear();
                    }
                }*/

                // Then the pins
                if (pin.pinIndex <= upToPosIndex) pin.FlushAppear();
            }
        }

        #endregion

        // TODO: this should create dots just for a visual purpose
        private void CreateDotsBetweenPins(int lb_i, Pin pinFront, Pin pinBack)
        {
            int nPlaySessions = nPlaySessionsPerLb[lb_i-1];

            Vector3 pFront = pinFront.transform.position;
            Vector3 pBack = pinBack.transform.position;

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

            for (int ps_i = 1; ps_i <= nPlaySessions; ps_i++)
            {
                var v = (ps_i * x + 0.5f * z + 2.5f) * Vector3.Normalize(pFront - pBack) + pBack;
                z += 2;

                Quaternion rot = Quaternion.identity;
                rot.eulerAngles = new Vector3(90, 0, 0);

                // Create a new dot
                GameObject dotGo = Instantiate(dotPrefab, v, rot);
                dotGo.transform.parent = dotsPivot;
                var mapDot = dotGo.GetComponent<Dot>();
                mapDot.SetLocked();
                pinFront.rope.dots.Add(mapDot);

                // Dialogues added to first playsession of the map
                if (lb_i == 1 && ps_i == 1)
                {
                    var introDialogues = dotGo.AddComponent<IntroDialogues>();
                    introDialogues.numberStage = stageNumber;
                }
            }
        }

        private void UnlockPlaySessions()
        {
            if (!stageUnlocked)
            {
                // All is locked
                maxPinIndex = 0;
            }
            else if (wholeStageUnlocked)
            {
                // All is unlocked
                for (var i = 1; i < pins.Count; i++)
                {
                    var pin = pins[i];
                    pin.SetUnlocked();
                    /*foreach (var dot in pin.rope.dots)
                    {
                        dot.SetUnlocked();
                    }*/
                }

                maxPinIndex = pins.Last().pinIndex;
            }
            else
            {
                // Part is locked
                var max_lb = AppManager.I.Player.MaxJourneyPosition.LearningBlock;
                var max_ps = AppManager.I.Player.MaxJourneyPosition.PlaySession;

                // TODO: get the journey position of each pin and check agaisnt MaxJourneyPosition
                foreach (var pin in pins)
                {
                    if (pin.JourneyPos.IsMinorOrEqual(AppManager.I.Player.MaxJourneyPosition))
                    {
                        pin.SetUnlocked();
                    }
                }
                // TODO: set the maxPinIndex too

                /*
                for (var lb = 1; lb <= max_lb; lb++)
                {
                    var pin = PinForLB(lb);

                    if (lb < max_lb)
                    {
                        // Completed LB: enable all dots of that lb
                        pin.SetUnlocked();
                        /*for (var ps = 1; ps <= pin.rope.dots.Count; ps++)
                        {
                            var dot = pin.rope.DotForPS(ps);
                            dot.SetUnlocked();
                        }*/
                   /* }
                    else
                    { 
                        // Maximum reached LB: we check the max PS we reached
                        if (max_ps == AppManager.I.JourneyHelper.AssessmentPlaySessionIndex)
                        {
                            max_ps = pin.rope.dots.Count; // fake 100 -> N
                            pin.SetUnlocked();
                            maxPinIndex = pin.dot.playerPosIndex;
                        }
                        else
                        {
                            maxPinIndex = pin.rope.DotForPS(max_ps).playerPosIndex;
                        }

                        for (var ps = 1; ps <= max_ps; ps++)
                        {
                            var dot = pin.rope.DotForPS(ps);
                            dot.SetUnlocked();
                        }
                    }
                }*/
            }
        }

        // Count the number of steps (PlaySessions) per each learning block
        private void CountPlaySessionsPerLearningBlock()
        {
            var psDataList = GetAllPlaySessionDataForStage(stageNumber);
            var lbDataList = GetAllLearningBlockDataForStage(stageNumber);
            nPlaySessionsPerLb = new int[lbDataList.Count];
            foreach (PlaySessionData psData in psDataList)
            {
                if (!psData.GetJourneyPosition().IsAssessment())
                {
                    nPlaySessionsPerLb[psData.LearningBlock-1]++;
                }
            }
        }

        public void ResetStageOnShow(bool playerIsHere)
        {
            //Debug.Log("Stage " + name + " player here? " + playerIsHere);
            foreach (var pin in pins)
            {
                pin.Highlight(playerIsHere && Equals(pin.JourneyPos, GetCurrentPlayerPosJourneyPosition()));
                /*if (pin.rope != null)
                {
                    foreach (var dot in pin.rope.dots)
                    {
                        dot.Highlight(playerIsHere && Equals(dot.JourneyPos, GetCurrentPlayerPosJourneyPosition()));
                    }
                }*/
            }
        }

        #endregion


        #region Play Session State

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
                //Debug.Log(scoreData_list[i].ElementId + " SCORE " + scoreData_list[i].Stars);
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
            if (hasAppeared) FlushAppear(AppManager.I.Player.MaxJourneyPosition);
        }

        #endregion

    }
}