using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Antura.Core;
using Antura.Database;
using DG.DeExtensions;
using UnityEngine;
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
        public Transform cameraPivotStart;
        public Transform cameraPivotEnd;

        [HideInInspector]
        // Current position of the PlayerPin in this stage map
        public int currentPinIndex; 
        [HideInInspector]
        // Max position index PlayerPin can take in this stage map
        public int maxUnlockedPinIndex;

        // Pins: one per Play Session
        private List<Pin> playPins;

        // Data
        private int[] nPlaySessionsPerLb;

        // Configuration
        private static float dotsSpan = 5.0f;

        #region Properties

        public Vector3 GetCurrentPlayerPosVector()
        {
            return mapLocations[currentPinIndex].Position;
        }

        public JourneyPosition GetCurrentPlayerPosJourneyPosition()
        {
            return mapLocations[currentPinIndex].JourneyPos;
        }

        public Pin FirstPin
        {
            get { return playPins[0]; } 
        }

        public Pin PinForJourneyPosition(JourneyPosition jp)
        {
            return playPins.FirstOrDefault(p => p.JourneyPos.Equals(jp));
        }

        public Pin PinForIndex(int index)
        {
            return playPins.FirstOrDefault(p => p.pinIndex == index);
        }
        #endregion

        [Header("References")]
        public Transform dotsPivot;
        public GameObject dotPrefab;
        public GameObject ropePrefab;

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
            var allPins = new List<Pin>(gameObject.GetComponentsInChildren<Pin>());
            for (var index = 0; index < allPins.Count; index++)
            {
                var pin = allPins[index];
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

            // Find all pins 
            playPins = new List<Pin>(gameObject.GetComponentsInChildren<Pin>());
            playPins.Sort((x, y) => x.transform.GetSiblingIndex() - y.transform.GetSiblingIndex());

            // Set the correct data to all pins (also creating the dots)
            var allPlaySessionStates = GetAllPlaySessionStatesForStage(stageNumber);

            if (allPlaySessionStates.Count > playPins.Count)
            {
                Debug.LogError("Stage " + stageNumber + " has only " + playPins.Count + " pins but needs " + allPlaySessionStates.Count);
                return;
            }
            else if (allPlaySessionStates.Count < playPins.Count)
            {
                Debug.LogError("Stage " + stageNumber + " has " + playPins.Count + " pins but needs only " + allPlaySessionStates.Count);
                return;
            }

            int playerPosIndexCount = 0; 
            JourneyPosition assignedJourneyPosition = new JourneyPosition(stageNumber,1,1);

            for (int jp_i = 0; jp_i < allPlaySessionStates.Count; jp_i++)
            {
                var pin = playPins[jp_i];
                var psState = allPlaySessionStates[jp_i];
                var journeyPos = psState.data.GetJourneyPosition();
                pin.Initialise(playerPosIndexCount++, journeyPos);
                mapLocations.Add(pin);

                pin.SetLocked();
                //Debug.Log(assignedJourneyPosition);
                pin.SetPlaySessionState(allPlaySessionStates.Find(x =>
                    x.data.GetJourneyPosition().Equals(assignedJourneyPosition)
                ));

                // Create visual dots and a rope
                if (jp_i > 0)
                {
                    CreateVisualsBetweenPins(playPins[jp_i], playPins[jp_i - 1]);
                }

                // Dialogues added to first JP of the stage
                if (jp_i == 0)
                {
                    var introDialogues = playPins[jp_i].gameObject.AddComponent<IntroDialogues>();
                    introDialogues.stageNumber = stageNumber;
                }

                // Advance to the next journey pos
                assignedJourneyPosition = AppManager.I.JourneyHelper.FindNextJourneyPosition(assignedJourneyPosition);
            }


            UnlockPlaySessions();

            Disappear();
        }

        #region Appear Animation
        private bool hasAppeared = false;

        void Disappear()
        {
            foreach (var pin in playPins)
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
            foreach (var pin in playPins)
            {
                // First the dots that connect the pins
                //if (pin.rope != null)
                {
                    foreach (var ropeDot in pin.dots)
                    {
                        //if (!ropeDot.isLocked)
                        if (!ropeDot.Appeared) // && ropeDot.playerPosIndex <= upToPosIndex)
                        {
                            ropeDot.Appear(0.0f, duration);
                            yield return new WaitForSeconds(duration);
                            duration *= 0.9f;
                            if (duration <= 0.01f) duration = 0.02f;
                        }
                    }
                }

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
            foreach (var pin in playPins)
            {
                if (pin.pinIndex > upToPosIndex)
                {
                    break;
                }

                // First the dots
                //if (pin.rope != null)
                {
                    foreach (var ropeDot in pin.dots)
                    {
                        //if (ropeDot.playerPosIndex <= upToPosIndex)
                        ropeDot.FlushAppear();
                    }
                }

                // Then the pin
                //if (pin.pinIndex <= upToPosIndex)
                pin.FlushAppear();
            }
        }

        #endregion

        private void CreateVisualsBetweenPins(Pin pinFront, Pin pinBack)
        {
            // @note: rope and dots always belong to the FRONT dot
            Vector3 pFront = pinFront.transform.position;
            Vector3 pBack = pinBack.transform.position;

            var distance = Vector3.Distance(pFront, pBack);
            int nDots = Mathf.FloorToInt(distance / dotsSpan) - 1;  // -1 as we have the two pins as start and end
            var dir = Vector3.Normalize(pFront - pBack);

            // Create and stretch the rope
            GameObject ropeGo = Instantiate(ropePrefab);
            var rope = ropeGo.GetComponent<Rope>();
            rope.name = "MapRope_" + pinFront.pinIndex;
            pinFront.rope = rope;
            rope.transform.position = pinBack.transform.position;
            rope.transform.position += pinBack.currentPinMesh.transform.up * 4;
            rope.transform.LookAt(pinFront.transform.position + pinFront.currentPinMesh.transform.up * 4);
            rope.transform.Rotate(Vector3.forward,-90f);
            rope.transform.Rotate(0, 4, 0);
            rope.transform.SetLocalScaleZ((distance / 20f) * 1.1f);

            // Create the dots
            for (int dot_i = 1; dot_i <= nDots; dot_i++)
            {
                // Create a new dot
                var dotPos = pBack + dir * dot_i * dotsSpan;
                var dotRot = Quaternion.Euler(90, 0, 0);
                GameObject dotGo = Instantiate(dotPrefab, dotPos, dotRot);
                dotGo.transform.SetParent(dotsPivot);
                var dot = dotGo.GetComponent<Dot>();
                pinFront.dots.Add(dot);

            }
        }

        private void UnlockPlaySessions()
        {
            if (!stageUnlocked)
            {
                // All is locked
                maxUnlockedPinIndex = 0;
            }
            else if (wholeStageUnlocked)
            {
                // All is unlocked
                for (var i = 0; i < playPins.Count; i++)
                {
                    var pin = playPins[i];
                    pin.SetUnlocked();
                    /*foreach (var dot in pin.rope.dots)
                    {
                        dot.SetUnlocked();
                    }*/
                }

                maxUnlockedPinIndex = playPins.Last().pinIndex;
            }
            else
            {
                // Part is locked
                var max_lb = AppManager.I.Player.MaxJourneyPosition.LearningBlock;
                var max_ps = AppManager.I.Player.MaxJourneyPosition.PlaySession;

                // TODO: get the journey position of each pin and check agaisnt MaxJourneyPosition
                foreach (var pin in playPins)
                {
                    if (pin.JourneyPos.IsMinorOrEqual(AppManager.I.Player.MaxJourneyPosition))
                    {
                        pin.SetUnlocked();
                    }
                }
                // TODO: set the maxUnlockedPinIndex too

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
                            maxUnlockedPinIndex = pin.dot.playerPosIndex;
                        }
                        else
                        {
                            maxUnlockedPinIndex = pin.rope.DotForPS(max_ps).playerPosIndex;
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
            foreach (var pin in playPins)
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