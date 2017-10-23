using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Antura.Core;
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

        [Header("References")]
        public Transform dotsPivot;
        public GameObject dotPrefab;
        public GameObject ropePrefab;

        // Configuration
        private static float dotsSpan = 5.0f;
        private static float startAppearDuration = 0.4f;
        private static float appearSpeedupMultiplier = 0.9f;
        private static float minAppearDuration = 0.01f;

        #region Properties

        // Current position of the PlayerPin in this stage map
        public int CurrentPinIndex { get; private set; }

        // Max position index PlayerPin can take in this stage map
        public int MaxUnlockedPinIndex { get; private set; }

        public void ForceCurrentPinIndex(int newIndex)
        {
            CurrentPinIndex = newIndex;
        }

        public Vector3 CurrentPlayerPosVector
        {
            get { return mapLocations[CurrentPinIndex].Position; }
        }

        public JourneyPosition CurrentPlayerPosJourneyPosition
        {
            get { return mapLocations[CurrentPinIndex].JourneyPos; }
        }

        public Pin FirstPin
        {
            get { return playPins[0]; } 
        }

        public List<Pin> Pins
        {
            get
            {
                return playPins;
            }
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

        #region State

        // Pins: one per Play Session
        private List<Pin> playPins;

        [HideInInspector]
        private bool stageUnlocked;   // at least one PS for this stage is unlocked
        [HideInInspector]
        private bool wholeStageUnlocked;    // all PS and LB of this stage are unlocked

        #endregion

        public List<IMapLocation> mapLocations = new List<IMapLocation>();
           
        #region Editor Setup

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

        #endregion

        #region Initialisation

        public void Initialise(bool _stageUnlocked, bool _wholeStageUnlocked)
        {
            stageUnlocked = _stageUnlocked;
            wholeStageUnlocked = _wholeStageUnlocked;

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
            /*
            else if (allPlaySessionStates.Count < playPins.Count)
            {
                Debug.LogError("Stage " + stageNumber + " has " + playPins.Count + " pins but needs only " + allPlaySessionStates.Count);
                return;
            }*/

            int playerPosIndexCount = 0; 
            JourneyPosition assignedJourneyPosition = new JourneyPosition(stageNumber,1,1);

            for (int jp_i = 0; jp_i < playPins.Count; jp_i++)
            {
                var pin = playPins[jp_i];
                //var psState = allPlaySessionStates[jp_i];
                //var journeyPos = psState.data.GetJourneyPosition();
                pin.Initialise(playerPosIndexCount++, assignedJourneyPosition);
                mapLocations.Add(pin);

                pin.SetLocked();
                //Debug.Log(assignedJourneyPosition);
                var psState = allPlaySessionStates.Find(x =>  x.psData.GetJourneyPosition().Equals(assignedJourneyPosition));
                if (psState != null)
                {
                    pin.SetPlaySessionState(psState);
                }

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

            //Debug.Log("Animating from " + fromPos + " to " + toPos);

            // First, let all the available dots appear, up to FROM
            FlushAppear(fromPos);

            // Then, let the remaining ones appear in order, up to TO
            int upToPosIndex = StageMapsManager.GetPosIndexFromJourneyPosition(this, toPos);
            float duration = startAppearDuration;
            foreach (var pin in playPins)
            {
                // Then the pins
                if (!pin.Appeared && pin.pinIndex <= upToPosIndex)
                {
                    // First the dots that connect the pins
                    foreach (var dot in pin.dots)
                    {
                        if (!dot.Appeared) 
                        {
                            dot.Appear(0.0f, duration);
                            yield return new WaitForSeconds(duration);
                            duration *= appearSpeedupMultiplier;
                            if (duration <= minAppearDuration) duration = minAppearDuration;
                        }
                    }

                    pin.Appear(duration);
                    yield return new WaitForSeconds(duration);
                    duration *= appearSpeedupMultiplier;
                    if (duration <= minAppearDuration) duration = minAppearDuration;
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
                foreach (var dot in pin.dots)
                {
                    dot.FlushAppear();
                }

                // Then the pin itself
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

            // Create and stretch the rope between pins of the same Learning Block
            if (!pinBack.journeyPosition.IsAssessment())
            {
                GameObject ropeGo = Instantiate(ropePrefab);
                ropeGo.transform.SetParent(dotsPivot);
                var rope = ropeGo.GetComponent<Rope>();
                rope.name = "MapRope_" + pinFront.pinIndex;
                pinFront.rope = rope;
                rope.transform.position = pinBack.transform.position;
                rope.transform.position += pinBack.currentPinMesh.transform.up * 4;
                rope.transform.LookAt(pinFront.transform.position + pinFront.currentPinMesh.transform.up * 4);
                rope.transform.Rotate(Vector3.forward, -90f);
                rope.transform.Rotate(0, 4, 0);
                rope.transform.SetLocalScaleZ((distance / 20f) * 1.1f);
            }

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
                MaxUnlockedPinIndex = 0;
            }
            else if (wholeStageUnlocked)
            {
                // All is unlocked
                playPins.ForEach(pin => pin.SetUnlocked());
                MaxUnlockedPinIndex = playPins.Last().pinIndex;
            }
            else
            {
                // Part of the stage is locked
                var maxJp = AppManager.I.Player.MaxJourneyPosition;
                playPins.ForEach(pin =>
                {
                    if (pin.JourneyPos.IsMinorOrEqual(maxJp))
                    {
                        pin.SetUnlocked();
                        MaxUnlockedPinIndex = pin.pinIndex;
                    }
                });
            }
        }

        public void ResetStageOnShow(bool playerIsHere)
        {
            //Debug.Log("Stage " + name + " player here? " + playerIsHere);
            foreach (var pin in playPins)
            {
                pin.Highlight(playerIsHere && Equals(pin.JourneyPos, CurrentPlayerPosJourneyPosition));
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
            // Get all PS for this stage
            var allPlaySessionData = AppManager.I.DB.GetAllPlaySessionData().Where(ps => ps.Stage == _stage).ToList();

            // Get all available scores for this stage
            var allScoreData = AppManager.I.ScoreHelper.GetCurrentScoreForPlaySessionsOfStage(_stage);

            // Build a structure containing both
            var playSessionStateList = new List<PlaySessionState>();
            for (var i = 0; i < allPlaySessionData.Count; i++)
            {
                //var data = AppManager.I.DB.GetPlaySessionDataById(scoreData_list[i].ElementId);
                var scoreData = allScoreData.FirstOrDefault(sc => sc.ElementId == allPlaySessionData[i].Id);
                playSessionStateList.Add(new PlaySessionState(allPlaySessionData[i], scoreData));
                //Debug.Log(scoreData_list[i].ElementId + " SCORE " + scoreData_list[i].Stars);
            }
            return playSessionStateList;
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