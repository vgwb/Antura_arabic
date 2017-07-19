using System.Collections;
using Antura.CameraControl;
using Antura.Core;
using Antura.Database;
using Antura.MinigamesCommon;
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
        public GameObject bookButton;
        public GameObject anturaButton;

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

        private int MaxUnlockedStage
        {
            get { return AppManager.I.Player.MaxJourneyPosition.Stage; }
        }

        private int FinalStage
        {
            get {  return AppManager.I.JourneyHelper.GetFinalJourneyPosition().Stage; }
        }

        private StageMap StageMap(int Stage)
        {
            return stageMaps[Stage - 1];
        }

        // TODO: move to the StageMape class
        private Color StageColor(int Stage)
        {
            return StageMap(Stage).color;
        }

        // TODO: move to the StageMape class
        private Transform StageCameraPivot(int Stage)
        {
            return StageMap(Stage).cameraPivot;
        }

        public bool IsAtFirstStage
        {
            get { return shownStage == 1; }
        }

        private bool IsAtMaxUnlockedStage
        {
            get { return shownStage == MaxUnlockedStage; }
        }

        public bool IsAtFinalStage
        {
            get { return shownStage == FinalStage; }
        }

        private bool IsShownStagePlayable
        {
            get { return shownStage <= MaxUnlockedStage; }
        }

        private bool IsStagePlayable(int stage)
        {
           return stage <= MaxUnlockedStage;
        }

        #endregion

        private void Awake()
        {
            if (!Application.isEditor)
            {
                SimulateFirstContact = false; // Force debug options to FALSE if we're not in the editor
            }

            shownStage = CurrentPlayerStage;

            // Setup stage availability
            for (int stage_i = 1; stage_i < MaxUnlockedStage; stage_i++)
            {
                StageMap(stage_i).Hide();
                StageMap(stage_i).Initialise(true);
            }
            if (MaxUnlockedStage <= FinalStage)
            {
                StageMap(MaxUnlockedStage).Initialise(false);
            }

            // Show the current stage
            TeleportToShownStage(shownStage);
            UpdateStageIndicatorUI(shownStage);
            UpdateButtonsForStage(shownStage);

            playerPin.ResetPlayerPosition();
            playerPin.gameObject.SetActive(true);

        }

        private void Start()
        {
            /* FIRST CONTACT FEATURE */
            if (AppManager.I.Player.IsFirstContact() || SimulateFirstContact)
            {
                FirstContactBehaviour();
                mapStageIndicator.gameObject.SetActive(false);
            }
            /* --------------------- */

            UpdateStageButtonsUI();

            var isGameCompleted = AppManager.I.Player.HasFinalBeenShown();
            if (!isGameCompleted && WillPlayAssessmentNext())
            {
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

            if (isFirstStep)
            {
                DeactivateUI();

                KeeperManager.I.PlayDialog(LocalizationDataId.Map_Intro, true, true, () =>
                {
                    KeeperManager.I.PlayDialog(LocalizationDataId.Map_Intro_AnturaSpace, true, true, ActivateAnturaButton);
                });

                AppManager.I.Player.FirstContactPassed();
                Debug.Log("First Contact Step1 finished! Go to Antura Space!");
            }
            else if (isSecondStep)
            {
                ActivateUI();
                AppManager.I.Player.FirstContactPassed(2);

                KeeperManager.I.PlayDialog(LocalizationDataId.Map_First, true, true, () =>
                {
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
            anturaButton.SetActive(true);
            StartCoroutine(CO_Tutorial());
        }

        private IEnumerator CO_Tutorial()
        {
            TutorialUI.SetCamera(UICamera);
            var anturaBtPos = anturaButton.transform.position;
            anturaBtPos.z -= 1;
            while (true)
            {
                TutorialUI.Click(anturaButton.transform.position);
                yield return new WaitForSeconds(0.85f);
            }
        }

        private IEnumerator CO_Tutorial_PlayButton()
        {
            TutorialUI.SetCamera(UICamera);
            var pos = playButton.transform.position;
            pos.y += 2;
            while (true)
            {
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

            if (IsAtFirstStage)
            {
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
            Debug.Log("Switch from " + fromStage + " to " + toStage);

            HidePlaySessionMovementButtons();

            // Animate the switch
            AnimateToShownStage(toStage);
            yield return new WaitForSeconds(0.8f);

            // DEPRECATED: player stage updates only after clicking on a new stage dot
            // Update Player Stage too if needed
            /*if (IsStagePlayable(shownStage))
            {
                AppManager.I.Player.CurrentJourneyPosition.Stage = shownStage;
                playerPin.ResetPlayerPositionAfterStageChange();
            }*/

            // Show the new stage
            UpdateStageIndicatorUI(shownStage);
            UpdateButtonsForStage(shownStage);
            ShowPlaySessionMovementButtons();

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
            // SwitchToStage(CurrentPlayerStage);
            StageMap(stage).Show();
            var pivot = StageCameraPivot(stage);
            CameraGameplayController.I.transform.position = pivot.position;
            CameraGameplayController.I.transform.rotation = pivot.rotation;
            Camera.main.backgroundColor = StageColor(stage);
            Camera.main.GetComponent<CameraFog>().color = StageColor(stage);
            playerPin.stageScript = StageMap(stage);
        }

        private void AnimateToShownStage(int stage)
        {
            Debug.Log("Animating to stage " + stage);
            StageMap(stage).Show();
            var pivot = StageCameraPivot(stage);
            CameraGameplayController.I.MoveToPosition(pivot.position, pivot.rotation, 0.6f);
            Camera.main.DOColor(StageColor(stage), 1);
            Camera.main.GetComponent<CameraFog>().color = StageColor(stage);
            playerPin.stageScript = StageMap(stage);
        }

        #endregion

        #region UI Activation

        private void UpdateStageIndicatorUI(int stage)
        {
            // Debug.Log("UpdateStageIndicatorUI " + currentStageNumber + "/" + maxNumberOfStages);
            mapStageIndicator.Init(stage - 1, FinalStage);
        }

        private void UpdateStageButtonsUI()
        {
            if (IsAtFirstStage)
            {
                rightStageButton.SetActive(false);
                //StartCoroutine("DeactivateButtonWithDelay", rightStageButton);
            }
            else if (IsAtFinalStage)
            {
                leftStageButton.SetActive(false);
                //StartCoroutine("DeactivateButtonWithDelay", leftStageButton);
            }
            else
            {
                rightStageButton.SetActive(true);
                leftStageButton.SetActive(true);
            }
        }

        private void ShowPlaySessionMovementButtons()
        {
            uiButtonMovementPlaySession.SetActive(true);
        }

        private void HidePlaySessionMovementButtons()
        {
            uiButtonMovementPlaySession.SetActive(!false);
        }

        private void DeactivateUI()
        {
            uiButtonMovementPlaySession.SetActive(false);
            bookButton.SetActive(false);
            anturaButton.SetActive(false);
            GlobalUI.ShowPauseMenu(false);
        }

        private void ActivateUI()
        {
            uiButtonMovementPlaySession.SetActive(true);
            bookButton.SetActive(true);
            anturaButton.SetActive(true);
            GlobalUI.ShowPauseMenu(true);
        }

        #endregion
    }
}