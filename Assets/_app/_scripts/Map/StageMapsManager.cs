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
        [Header("Debug")] public bool SimulateFirstContact;

        [Header("Settings")]
        // public Color[] colorMaps;
        public StageMap[] stageMaps;
       // public Transform[] cameraPivots;

        public GameObject letter;

        [Header("References")]
        public MapStageIndicator mapStageIndicator;

        public Camera UICamera;

        [Header("LockUI")]
        public GameObject lockUI;

        [Header("UIButtons")]
        public GameObject leftStageButton;
        public GameObject rightStageButton;
        public GameObject uiButtonMovementPlaySession;
        public GameObject nextPlaySessionButton;
        public GameObject beforePlaySessionButton;
        public GameObject playButton;
        public GameObject bookButton;
        public GameObject anturaButton;

        //public int currentStage;
        //public int maxNumberOfStages;
        //private int maxUnlockedStage;
        //private int i;
        private int previousStage;
        private bool inTransition;
        private static int firstContactSimulationStep;
        private GameObject tutorial;

        #region Properties

        private int CurrentStage
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
            get { return CurrentStage == 1; }
        }

        private bool IsAtMaxUnlockedStage
        {
            get { return CurrentStage == MaxUnlockedStage; }
        }

        public bool IsAtFinalStage
        {
            get { return CurrentStage == FinalStage; }
        }

        private bool IsCurrentStageAvailableForPlay
        {
            get { return CurrentStage >= MaxUnlockedStage; }
        }

        #endregion

        private void Awake()
        {
            if (!Application.isEditor)
                SimulateFirstContact = false; // Force debug options to FALSE if we're not in the editor

            //maxNumberOfStages = AppConstants.MaximumStage;
           // currentStage = AppManager.I.Player.CurrentJourneyPosition.Stage;
            //maxUnlockedStage = AppManager.I.Player.MaxJourneyPosition.Stage;

            //int nUnlockedStages;
            /*if (MaxUnlockedStage == FinalStage)
            {
                nUnlockedStages = MaxUnlockedStage;
            }*/
            //else nUnlockedStages = MaxUnlockedStage - 1;
            for (int i = 0; i < MaxUnlockedStage; i++)
            {
                stageMaps[i].gameObject.SetActive(false);
                stageMaps[i].isAvailableTheWholeMap = true;
                stageMaps[i].CalculateStepsStage();
            }
            if (MaxUnlockedStage < FinalStage)
            {
                stageMaps[MaxUnlockedStage].CalculateStepsStage();
            }

            StageMap(CurrentStage).gameObject.SetActive(true);
            Camera.main.backgroundColor = StageColor(CurrentStage);
            Camera.main.GetComponent<CameraFog>().color = StageColor(CurrentStage);
            letter.GetComponent<LetterMovement>().stageScript = StageMap(CurrentStage);

            StartCoroutine("ResetPosLetterCO");

            UpdateStageIndicator();
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

            UpdateStageButtons();

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

        private void UpdateStageIndicator()
        {
            // Debug.Log("UpdateStageIndicator " + currentStageNumber + "/" + maxNumberOfStages);
            mapStageIndicator.Init(CurrentStage - 1, FinalStage);
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
            tutorial = GameObject.Find("[TutorialUI]");
            if (tutorial != null) tutorial.transform.localScale = new Vector3(0, 0, 0);
        }

        private void ShowTutorial()
        {
            if (tutorial != null) tutorial.transform.localScale = new Vector3(1, 1, 1);
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

            previousStage = CurrentStage;
            AppManager.I.Player.AdvanceCurrentStage();

            InitialiseStageSwitch();

            if (CurrentStage <= MaxUnlockedStage &&
                previousStage != CurrentStage)
            {
                // TODO: did we advance the stage?
                //AppManager.I.Player.CurrentJourneyPosition.Stage++;
                ChangeStage();
            }
            else
            {
                // TODO: stage was not available!
                StageNotAvailable();
            }
            UpdateStageIndicator();

            StartCoroutine("DeactivatePreviousMapCO");
            HideTutorial();
        }

        /// <summary>
        ///     Move to the previous Stage map
        /// </summary>
        public void MoveToPreviousStageMap()
        {
            if (inTransition) return;
            if (IsAtFirstStage) return;

            previousStage = CurrentStage;
            // TODO: CurrentStage--;
            InitialiseStageSwitch();

            if (CurrentStage <= MaxUnlockedStage &&
                AppManager.I.Player.CurrentJourneyPosition.Stage != CurrentStage)
            {
                AppManager.I.Player.RetractCurrentStage();
                ChangeStage();
            }
            else if (AppManager.I.Player.CurrentJourneyPosition.Stage == CurrentStage)
            {
                //lockUI.SetActive(false);
                letter.GetComponent<LetterMovement>().AmIFirstorLastPos();
                //isStageAvailable = false;
            }
            else
            {
                StageNotAvailable();
            }
            UpdateStageIndicator();
            StartCoroutine("DeactivatePreviousMapCO");

            /*
            TODO: how to do this?
            if (IsAtFirstStage)
            {
                ShowTutorial();
            }*/
        }

        private void InitialiseStageSwitch()
        {
            //DeactiveUIButtonsDuringTransition();

            inTransition = true;

            StageMap(CurrentStage).gameObject.SetActive(true);

            SwitchToCamera(CurrentStage);

            UpdateStageButtons();

            //lockUI.SetActive(true);
        }

        private void ChangeStage()
        {
            // We just switched the pos pin
            //isStageAvailable = false;
            letter.GetComponent<LetterMovement>().stageScript = StageMap(CurrentStage);
            letter.GetComponent<LetterMovement>().ResetPosLetterAfterChangeStage();
            //lockUI.SetActive(false);
            letter.GetComponent<LetterMovement>().AmIFirstorLastPos();
        }

        private void StageNotAvailable()
        {
            //isStageAvailable = true;

            playButton.SetActive(false);
            nextPlaySessionButton.SetActive(false);
            beforePlaySessionButton.SetActive(false);
        }

        private IEnumerator ResetPosLetterCO()
        {
            yield return new WaitForSeconds(0.2f);
            // TODO: letter.GetComponent<LetterMovement>().ResetPosLetter();
            letter.SetActive(true);
            CameraGameplayController.I.transform.position = StageCameraPivot(CurrentStage).position;
        }


        private IEnumerator DeactivatePreviousMapCO()
        {
            yield return new WaitForSeconds(0.005f);
            DeactiveUIButtonsDuringTransition();

            yield return new WaitForSeconds(0.8f);

            playButton.SetActive(IsCurrentStageAvailableForPlay);

            DeactiveUIButtonsDuringTransition();
            //yield return new WaitForSeconds(0.3f);

            StageMap(previousStage).gameObject.SetActive(false);

            inTransition = false;
        }

        #endregion

        #region Camera

        private void SwitchToCamera(int stage)
        {
            var pivot = StageCameraPivot(stage);
            CameraGameplayController.I.MoveToPosition(pivot.position, pivot.rotation, 0.6f);
            Camera.main.DOColor(StageColor(CurrentStage), 1);
            Camera.main.GetComponent<CameraFog>().color = StageColor(CurrentStage);
        }

        #endregion

        #region UI Activation

        private void UpdateStageButtons()
        {
            if (IsAtFirstStage)
            {
                StartCoroutine("DeactivateButtonWithDelay", rightStageButton);
            }
            else if (IsAtFinalStage)
            {
                StartCoroutine("DeactivateButtonWithDelay", leftStageButton);
            }
            else
            {
                rightStageButton.SetActive(true);
                leftStageButton.SetActive(true);
            }
        }

        private void DeactiveUIButtonsDuringTransition()
        {
            uiButtonMovementPlaySession.SetActive(!uiButtonMovementPlaySession.activeSelf);
        }

        private IEnumerator DeactivateButtonWithDelay(GameObject button)
        {
            yield return new WaitForSeconds(0.1f);
            button.SetActive(false);
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