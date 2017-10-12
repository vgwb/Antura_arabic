using System.Collections;
using Antura.CameraControl;
using Antura.Core;
using Antura.Database;
using Antura.Keeper;
using Antura.Minigames;
using Antura.Tutorial;
using Antura.UI;
using DG.Tweening;
using UnityEngine;

namespace Antura.Map
{
    /// <summary>
    ///     General manager for the Map scene.
    ///     Handles the different maps for all Stages of the game.
    ///     Allows navigation from one map to the next (between stages).
    /// </summary>
    public class StageMapsManager : MonoBehaviour
    {
        [Header("Options")]
        public bool MovePlayerWithStageChange = true;

        [Header("Debug")]
        public bool SimulateFirstContact;

        [Header("References")]
        public StageMap[] stageMaps;
        public PlayerPin playerPin;
        public MapCameraController mapCamera;

        [Header("UI")]
        public MapStageIndicator mapStageIndicator;
        public Camera UICamera;
        public GameObject lockUI;
        public GameObject leftStageButton;
        public GameObject rightStageButton;
        public GameObject uiButtonMovementPlaySession;
        public GameObject nextPlaySessionButton;
        public GameObject beforePlaySessionButton;
        public GameObject playButton;

        // Additional UI for navigation
        public GameObject navigationIconsPanel;
        public GameObject learningBookButton;
        public GameObject minigamesBookButton;
        public GameObject profileBookButton;
        public GameObject anturaSpaceButton;

        #region State

        // The stage that is currently shown to the player
        private int shownStage;
        private bool inTransition;

        #endregion

        #region Tutorial

        private static int firstContactSimulationStep;
        private GameObject tutorialUiGo;

        #endregion

        #region Properties

        private int PreviousPlayerStage   
        {
            get { return PreviousJourneyPosition.Stage; }
        }

        private int CurrentPlayerStage    // @note: this may be different than shownStage as you can preview the next stages
        {
            get { return CurrentJourneyPosition.Stage; }
        }

        public static JourneyPosition PreviousJourneyPosition
        {
            get
            {
                return AppManager.I.Player.PreviousJourneyPosition;
            }
        }
        private JourneyPosition targetCurrentJourneyPosition;

        public static JourneyPosition CurrentJourneyPosition
        {
            get
            {
                return AppManager.I.Player.CurrentJourneyPosition;
            }
        }

        private int MaxUnlockedStage {
            get
            {
                return AppManager.I.Player.MaxJourneyPosition.Stage;
            }
        }

        private int FinalStage {
            get { return AppManager.I.JourneyHelper.GetFinalJourneyPosition().Stage; }
        }

        private StageMap StageMap(int Stage)
        {
            return stageMaps[Stage - 1];
        }

        public bool IsAtFirstStage {
            get { return shownStage == 1; }
        }

        private bool IsAtMaxUnlockedStage {
            get { return shownStage == MaxUnlockedStage; }
        }

        public bool IsAtFinalStage {
            get { return shownStage == FinalStage; }
        }

        public StageMap CurrentShownStageMap
        {
            get { return StageMap(shownStage); }
        }

        private bool IsStagePlayable(int stage)
        {
            return stage <= MaxUnlockedStage;
        }

        #endregion

        private static bool TEST_JOURNEY_POS = true;

        private void Awake()
        {
            if (!Application.isEditor) {
                SimulateFirstContact = false; // Force debug options to FALSE if we're not in the editor
            }

            // DEBUG
            if (TEST_JOURNEY_POS)
            {
                TEST_JOURNEY_POS = false;
                AppManager.I.Player.SetMaxJourneyPosition(new JourneyPosition(1, 1, 2));
                AppManager.I.Player.SetCurrentJourneyPosition(new JourneyPosition(1, 1, 2));
                AppManager.I.Player.ForcePreviousJourneyPosition(new JourneyPosition(1, 1, 1));
                Debug.Log("FORCED TEST_JOURNEY_POS");
            }

            shownStage = PreviousPlayerStage;
            targetCurrentJourneyPosition = CurrentJourneyPosition;

            // Setup stage availability
            for (int stage_i = 1; stage_i <= stageMaps.Length; stage_i++) {
                bool isStageUnlocked = stage_i <= MaxUnlockedStage;
                bool isWholeStageUnlocked = stage_i < MaxUnlockedStage;
                StageMap(stage_i).Initialise(isStageUnlocked, isWholeStageUnlocked);
                StageMap(stage_i).Hide();
            }
        }

        private void Start()
        {
            // Show the current stage
            TeleportCameraToShownStage(shownStage);
            UpdateStageIndicatorUI(shownStage);
            UpdateButtonsForStage(shownStage);

            // Position the player
            playerPin.gameObject.SetActive(true);
            //playerPin.onMoveStart += HidePlaySessionMovementButtons;
            playerPin.onMoveStart += CheckCurrentStageForMovement;
            //playerPin.onMoveEnd += ShowPlaySessionMovementButtons;
            playerPin.ForceToJourneyPosition(PreviousJourneyPosition, justVisuals:true);

            /* FIRST CONTACT FEATURE */
            if (AppManager.I.Player.IsFirstContact() || SimulateFirstContact) {
                FirstContactBehaviour();
                mapStageIndicator.gameObject.SetActive(false);
            }
            /* --------------------- */

            UpdateStageButtonsUI();

            var isGameCompleted = AppManager.I.Player.HasFinalBeenShown();
            if (!isGameCompleted && WillPlayAssessmentNext()) {
                PlayRandomAssessmentDialog();
            }

            // Coming from the other stage
            StartCoroutine(InitialMovementCO());
        }

        private IEnumerator InitialMovementCO()
        {
            HidePlaySessionMovementButtons();
            StageMap(shownStage).FlushAppear(PreviousJourneyPosition);

            bool needsAnimation = !Equals(targetCurrentJourneyPosition, PreviousJourneyPosition);
            //Debug.Log("TARGET CURRENT: " + targetCurrentJourneyPosition  + "\n PREV: " + PreviousJourneyPosition);
            if (!needsAnimation)
            {
                StageMap(shownStage).FlushAppear(AppManager.I.Player.MaxJourneyPosition);
            }
            else
            {
                yield return new WaitForSeconds(1.0f);
                StageMap(shownStage).Appear(PreviousJourneyPosition, AppManager.I.Player.MaxJourneyPosition);

                //Debug.Log("Shown stage: " + shownStage + " TargetJourneyPos " + targetCurrentJourneyPosition +   " PreviousJourneyPos " + PreviousJourneyPosition);
                if (shownStage != targetCurrentJourneyPosition.Stage)
                {
                    //Debug.Log("ANIMATING TO STAGE: " + targetCurrentJourneyPosition.Stage + " THEN MOVING TO " + targetCurrentJourneyPosition);
                    yield return StartCoroutine(SwitchFromToStageCO(shownStage, targetCurrentJourneyPosition.Stage));
                    mapCamera.SetAutoFollowTransformCurrentMap(playerPin.transform);
                    playerPin.MoveToJourneyPosition(targetCurrentJourneyPosition);
                }
                else
                {
                    //Debug.Log("JUST MOVING TO " + targetCurrentJourneyPosition);
                    yield return new WaitForSeconds(1.0f);
                    mapCamera.SetAutoFollowTransformCurrentMap(playerPin.transform);
                    playerPin.MoveToJourneyPosition(targetCurrentJourneyPosition);
                    yield return null;
                }
            }

            while (playerPin.IsAnimating)
            {
                yield return null;
            }

            mapCamera.SetManualMovementCurrentMap();
            ShowPlaySessionMovementButtons();
        }

        private bool WillPlayAssessmentNext()
        {
            return AppManager.I.JourneyHelper.IsAssessmentTime(CurrentJourneyPosition)
                && CurrentJourneyPosition.Stage == AppManager.I.Player.MaxJourneyPosition.Stage &&
                CurrentJourneyPosition.LearningBlock == AppManager.I.Player.MaxJourneyPosition.LearningBlock;
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        #region Dialogs

        private void PlayRandomAssessmentDialog()
        {
            var data = new LocalizationDataId[3];
            data[0] = LocalizationDataId.Assessment_Start_1;
            data[1] = LocalizationDataId.Assessment_Start_2;
            data[2] = LocalizationDataId.Assessment_Start_3;
            var n = Random.Range(0, data.Length);
            KeeperManager.I.PlayDialog(data[n], true, true);
        }

        #endregion

        #region First Contact Session        

        /// <summary>
        ///     Firsts the contact behaviour.
        ///     Put Here logic for first contact only situations.
        /// </summary>
        private void FirstContactBehaviour()
        {
            if (SimulateFirstContact) firstContactSimulationStep++;
            var isFirstStep = SimulateFirstContact
                ? firstContactSimulationStep == 1
                : AppManager.I.Player.IsFirstContact(1);
            var isSecondStep = SimulateFirstContact
                ? firstContactSimulationStep == 2
                : AppManager.I.Player.IsFirstContact(2);

            if (isFirstStep) {
                DeactivateUI();

                KeeperManager.I.PlayDialog(LocalizationDataId.Map_Intro, true, true, () => {
                    KeeperManager.I.PlayDialog(LocalizationDataId.Map_Intro_AnturaSpace, true, true, ActivateAnturaButton);
                });

                AppManager.I.Player.FirstContactPassed();
                Debug.Log("First Contact Step1 finished! Go to Antura Space!");
            } else if (isSecondStep) {
                ActivateUI();
                AppManager.I.Player.FirstContactPassed(2);

                KeeperManager.I.PlayDialog(LocalizationDataId.Map_First, true, true, () => {
                    KeeperManager.I.PlayDialog(LocalizationDataId.Map_Intro_Map1);
                });


                Debug.Log("First Contact Step2 finished! Good Luck!");
                //tuto anim on the play button
                StartCoroutine(CO_Tutorial_PlayButton());
            }
        }

        // TODO: check if something called this
        /*private void PlayDialogStages(LocalizationDataId data)
        {
            KeeperManager.I.PlayDialog(data);
        }*/

        private void ActivateAnturaButton()
        {
            anturaSpaceButton.SetActive(true);
            StartCoroutine(CO_Tutorial());
        }

        private IEnumerator CO_Tutorial()
        {
            TutorialUI.SetCamera(UICamera);
            var anturaBtPos = anturaSpaceButton.transform.position;
            anturaBtPos.z -= 1;
            while (true) {
                TutorialUI.Click(anturaSpaceButton.transform.position);
                yield return new WaitForSeconds(0.85f);
            }
        }

        private IEnumerator CO_Tutorial_PlayButton()
        {
            TutorialUI.SetCamera(UICamera);
            var pos = playButton.transform.position;
            pos.y += 2;
            while (true) {
                TutorialUI.Click(pos);
                yield return new WaitForSeconds(0.85f);
            }
        }

        private void HideTutorial()
        {
            tutorialUiGo = GameObject.Find("[TutorialUI]");
            if (tutorialUiGo != null) tutorialUiGo.transform.localScale = new Vector3(0, 0, 0);
        }

        private void ShowTutorial()
        {
            if (tutorialUiGo != null) tutorialUiGo.transform.localScale = new Vector3(1, 1, 1);
        }

        #endregion

        #region Stage Navigation

        /// <summary>
        ///     Move to the next Stage map
        /// </summary>
        public void MoveToNextStageMap()
        {
            if (inTransition) return;
            if (IsAtFinalStage) return;

            int fromStage = shownStage;
            int toStage = shownStage + 1;

            SwitchFromToStage(fromStage, toStage);

            HideTutorial();
        }

        /// <summary>
        ///     Move to the previous Stage map
        /// </summary>
        public void MoveToPreviousStageMap()
        {
            if (inTransition) return;
            if (IsAtFirstStage) return;

            int fromStage = shownStage;
            int toStage = shownStage - 1;

            SwitchFromToStage(fromStage, toStage);

            if (IsAtFirstStage) {
                ShowTutorial();
            }
        }

        public void MoveToStageMap(int toStage)
        {
            if (inTransition) return;

            int fromStage = shownStage;
            if (toStage == fromStage) return;

            SwitchFromToStage(fromStage, toStage);
        }

        private void UpdateButtonsForStage(int stage)
        {
            UpdateStageButtonsUI();
            bool playable = IsStagePlayable(stage);
            playButton.SetActive(playable);
            nextPlaySessionButton.SetActive(playable);
            beforePlaySessionButton.SetActive(playable);
            lockUI.SetActive(!playable);
        }

        private void CheckCurrentStageForMovement()
        {
            //Debug.Log("ShownStage: " + shownStage + " Current: " + CurrentPlayerStage);
            if (shownStage != CurrentPlayerStage)
            {
                bool comingFromHigherStage = CurrentPlayerStage > shownStage;
                playerPin.ResetPlayerPositionAfterStageChange(comingFromHigherStage);
            }
        }

        private void SwitchFromToStage(int fromStage, int toStage)
        {
            StartCoroutine(SwitchFromToStageCO(fromStage, toStage));
        }

        private IEnumerator SwitchFromToStageCO(int fromStage, int toStage)
        {
            shownStage = toStage;

            inTransition = true;
            //Debug.Log("Switch from " + fromStage + " to " + toStage);

            HidePlaySessionMovementButtons();

            // Change stage reference
            StageMap(toStage).FlushAppear(AppManager.I.Player.MaxJourneyPosition);
            SwitchStageMapForPlayer(StageMap(toStage));

            // Update Player Stage too, if needed
            if (MovePlayerWithStageChange) {
                if (IsStagePlayable(toStage) && toStage != shownStage) {
                    bool comingFromHigherStage = fromStage > toStage;
                    playerPin.ResetPlayerPositionAfterStageChange(comingFromHigherStage);
                }
            }

            // Animate the switch
            AnimateCameraToShownStage(toStage);
            yield return new WaitForSeconds(0.8f);

            // Show the new stage
            UpdateStageIndicatorUI(toStage);
            UpdateButtonsForStage(toStage);

            if (MovePlayerWithStageChange) {
                ShowPlaySessionMovementButtons();
            } else {
                if (toStage == CurrentPlayerStage) {
                    ShowPlaySessionMovementButtons();
                }
            }

            // Hide the last stage
            StageMap(fromStage).Hide();

            // End transition
            inTransition = false;

            //Debug.Log("We are at stage " + shownStage + ". Player current is " + CurrentPlayerStage);
        }

        #endregion

        #region Camera

        private void TeleportCameraToShownStage(int stage)
        {
            var stageMap = StageMap(stage);
            stageMap.Show();

            var pivot = stageMap.cameraPivotStart;
            CameraGameplayController.I.transform.position = pivot.position;
            CameraGameplayController.I.transform.rotation = pivot.rotation;
            Camera.main.backgroundColor = stageMap.color;
            Camera.main.GetComponent<CameraFog>().color = stageMap.color;
            SwitchStageMapForPlayer(stageMap, true);
        }

        private void AnimateCameraToShownStage(int stage)
        {
            //Debug.Log("Animating to stage " + stage);
            var stageMap = StageMap(stage);
            stageMap.Show();
            stageMap.ResetStageOnShow(CurrentPlayerStage == stage);

            var pivot = stageMap.cameraPivotStart;
            mapCamera.SetAutoMoveToTransformFree(pivot, 0.6f);
            Camera.main.DOColor(stageMap.color, 1);
            Camera.main.GetComponent<CameraFog>().color = stageMap.color;
        }

        private void SwitchStageMapForPlayer(StageMap newStageMap, bool init = false)
        {
            if (playerPin.IsAnimating) playerPin.StopAnimation(stopWhereItIs: false);
            playerPin.currentStageMap = newStageMap;

            // Move the player too, if the stage is unlocked
            if (!init && !newStageMap.FirstPin.isLocked)
            {
                playerPin.ForceToJourneyPosition(CurrentJourneyPosition);
            }
        }

        #endregion

        #region UI Activation

        private void UpdateStageIndicatorUI(int stage)
        {
            mapStageIndicator.Init(stage - 1, FinalStage);
        }

        private void UpdateStageButtonsUI()
        {
            if (IsAtFirstStage) {
                rightStageButton.SetActive(false);
            } else if (IsAtFinalStage) {
                leftStageButton.SetActive(false);
            } else {
                rightStageButton.SetActive(true);
                leftStageButton.SetActive(true);
            }
        }

        private void ShowPlayButton()
        {
            playButton.SetActive(true);
        }

        private void HidePlayButton()
        {
            playButton.SetActive(false);
        }

        private void ShowPlaySessionMovementButtons()
        {
            uiButtonMovementPlaySession.SetActive(true);
            playerPin.CheckMovementButtonsEnabling();
        }

        private void HidePlaySessionMovementButtons()
        {
            uiButtonMovementPlaySession.SetActive(false);
        }

        private void DeactivateUI()
        {
            uiButtonMovementPlaySession.SetActive(false);
            learningBookButton.SetActive(false);
            minigamesBookButton.SetActive(false);
            profileBookButton.SetActive(false);
            anturaSpaceButton.SetActive(false);
            GlobalUI.ShowPauseMenu(false);
        }

        private void ActivateUI()
        {
            uiButtonMovementPlaySession.SetActive(true);
            navigationIconsPanel.SetActive(true);
            learningBookButton.SetActive(true);
            minigamesBookButton.SetActive(true);
            profileBookButton.SetActive(true);
            anturaSpaceButton.SetActive(true);
            GlobalUI.ShowPauseMenu(true);
        }

        #endregion

        #region Static Utilities

        public static int GetPosIndexFromJourneyPosition(StageMap stageMap, JourneyPosition journeyPos)
        {
            var st = journeyPos.Stage;

            if (stageMap.stageNumber > st)
                return 0;

            if (stageMap.stageNumber < st)
                return stageMap.MaxUnlockedPinIndex;

            var pin = stageMap.PinForJourneyPosition(journeyPos);
            return pin.pinIndex;
        }

        #endregion

        public void UpdateDotHighlights()
        {
            foreach (var stageMap in stageMaps)
            {
                foreach (var pin in stageMap.Pins)
                {
                    pin.Highlight(false);
                }

                var correctPin = stageMap.PinForJourneyPosition(CurrentJourneyPosition);
                if (correctPin != null) correctPin.Highlight(true);
            }
        }
    }
}