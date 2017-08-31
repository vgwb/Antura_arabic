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

        // Current stage shown for the map. 
        private int shownStage;
        private bool inTransition;
        private static int firstContactSimulationStep;
        private GameObject tutorialUiGo;

        #endregion

        #region Properties

        private int CurrentPlayerStage    // @note: this may be different than shownStage as you can preview the next stages
        {
            get { return AppManager.I.Player.CurrentJourneyPosition.Stage; }
        }

        private int MaxUnlockedStage {
            get { return AppManager.I.Player.MaxJourneyPosition.Stage; }
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

        private bool IsStagePlayable(int stage)
        {
            return stage <= MaxUnlockedStage;
        }

        #endregion

        private void Awake()
        {
            if (!Application.isEditor) {
                SimulateFirstContact = false; // Force debug options to FALSE if we're not in the editor
            }

            shownStage = CurrentPlayerStage;

            // Setup stage availability
            for (int stage_i = 1; stage_i <= stageMaps.Length; stage_i++) {
                bool isStageUnlocked = stage_i <= MaxUnlockedStage;
                bool isWholeStageUnlocked = stage_i < MaxUnlockedStage;
                StageMap(stage_i).Initialise(isStageUnlocked, isWholeStageUnlocked);
                StageMap(stage_i).Hide();
            }
            /*if (MaxUnlockedStage <= FinalStage)
            {
                StageMap(MaxUnlockedStage).Initialise(true, false);
            }*/

            // Show the current stage
            TeleportToShownStage(shownStage);
            UpdateStageIndicatorUI(shownStage);
            UpdateButtonsForStage(shownStage);

            playerPin.gameObject.SetActive(true);
            playerPin.ResetPlayerPosition();
        }

        private void Start()
        {
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
        }

        private bool WillPlayAssessmentNext()
        {
            //AppManager.I.JourneyHelper.IsAssessmentTime()
            return AppManager.I.Player.CurrentJourneyPosition.Stage == AppManager.I.Player.MaxJourneyPosition.Stage &&
                AppManager.I.Player.CurrentJourneyPosition.LearningBlock ==
                AppManager.I.Player.MaxJourneyPosition.LearningBlock &&
                AppManager.I.Player.CurrentJourneyPosition.PlaySession == 100;
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

            shownStage = toStage;

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

            shownStage = toStage;

            SwitchFromToStage(fromStage, toStage);

            if (IsAtFirstStage) {
                ShowTutorial();
            }
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

        private void SwitchFromToStage(int fromStage, int toStage)
        {
            inTransition = true;
            StartCoroutine(SwitchFromToStageCO(fromStage, toStage));
        }

        private IEnumerator SwitchFromToStageCO(int fromStage, int toStage)
        {
            //Debug.Log("Switch from " + fromStage + " to " + toStage);

            HidePlaySessionMovementButtons();

            // Change stage reference
            playerPin.stageMap = StageMap(toStage);

            // Update Player Stage too, if needed
            if (MovePlayerWithStageChange) {
                if (IsStagePlayable(toStage) && toStage != CurrentPlayerStage) {
                    bool comingFromHigherStage = fromStage > toStage;
                    playerPin.ResetPlayerPositionAfterStageChange(comingFromHigherStage);
                }
            }

            // Animate the switch
            AnimateToShownStage(toStage);
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

            Debug.Log("We are at stage " + shownStage + ". Player current is " + CurrentPlayerStage);
        }

        #endregion

        #region Camera

        private void TeleportToShownStage(int stage)
        {
            var stageMap = StageMap(stage);
            stageMap.Show();
            var pivot = stageMap.cameraPivot;
            CameraGameplayController.I.transform.position = pivot.position;
            CameraGameplayController.I.transform.rotation = pivot.rotation;
            Camera.main.backgroundColor = stageMap.color;
            Camera.main.GetComponent<CameraFog>().color = stageMap.color;
            playerPin.stageMap = stageMap;
        }

        private void AnimateToShownStage(int stage)
        {
            //Debug.Log("Animating to stage " + stage);
            var stageMap = StageMap(stage);
            stageMap.Show();
            var pivot = stageMap.cameraPivot;
            CameraGameplayController.I.MoveToPosition(pivot.position, pivot.rotation, 0.6f);
            Camera.main.DOColor(stageMap.color, 1);
            Camera.main.GetComponent<CameraFog>().color = stageMap.color;
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
    }
}