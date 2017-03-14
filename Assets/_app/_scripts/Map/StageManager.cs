using UnityEngine;
using System.Collections;
using DG.Tweening;
using EA4S.CameraControl;
using EA4S.Core;
using EA4S.MinigamesCommon;
using EA4S.Tutorial;
using ModularFramework.Components;

namespace EA4S.Map
{
    /// <summary>
    /// General manager for the Map scene. Handles the different maps for all Stages of the game.
    /// Allows navigation from one map to the next (between stages).
    /// </summary>
    public class StageManager : MonoBehaviour
    {
        [Header("Debug")]
        public bool SimulateFirstContact;

        [Header("Settings")]
        public Color[] colorMaps;
        public GameObject[] stages;
        public GameObject[] cameras;
        public GameObject[] miniMaps;
        public GameObject letter;
        bool isStageAvailable;

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

        public int currentStageNumber;
        public int maxNumberOfStages;
        int maxStageUnlocked, i, previousStageNumber;
        bool inTransition;
        static int firstContactSimulationStep;

        void Awake()
        {
            if (!Application.isEditor) SimulateFirstContact = false; // Force debug options to FALSE if we're not in the editor

            maxNumberOfStages = AppConstants.maximumStage;
            currentStageNumber = AppManager.I.Player.CurrentJourneyPosition.Stage;
            maxStageUnlocked = AppManager.I.Player.MaxJourneyPosition.Stage;
            int nStage;
            if (maxStageUnlocked == maxNumberOfStages) { nStage = maxNumberOfStages; } else { nStage = maxStageUnlocked - 1; }
            for (i = 1; i <= nStage; i++) {
                stages[i].SetActive(false);
                miniMaps[i].GetComponent<Stage>().isAvailableTheWholeMap = true;
                miniMaps[i].GetComponent<Stage>().CalculateStepsStage();
            }
            if (maxStageUnlocked < maxNumberOfStages) {
                miniMaps[i].GetComponent<Stage>().CalculateStepsStage();
            }

            stages[AppManager.I.Player.CurrentJourneyPosition.Stage].SetActive(true);
            Camera.main.backgroundColor = colorMaps[AppManager.I.Player.CurrentJourneyPosition.Stage];
            Camera.main.GetComponent<CameraFog>().color = colorMaps[AppManager.I.Player.CurrentJourneyPosition.Stage];
            letter.GetComponent<LetterMovement>().stageScript = miniMaps[AppManager.I.Player.CurrentJourneyPosition.Stage].GetComponent<Stage>();

            StartCoroutine("ResetPosLetter");

            updateStageIndicator();
        }

        void Start()
        {
            /* FIRST CONTACT FEATURE */
            if (AppManager.I.Player.IsFirstContact() || SimulateFirstContact) {
                FirstContactBehaviour();
            }
            /* --------------------- */
            FirstOrLastMap();

            bool isGameCompleted = AppManager.I.Player.HasFinalBeenShown();
            if ((!isGameCompleted) && (AppManager.I.Player.CurrentJourneyPosition.Stage == AppManager.I.Player.MaxJourneyPosition.Stage) &&
                (AppManager.I.Player.CurrentJourneyPosition.LearningBlock == AppManager.I.Player.MaxJourneyPosition.LearningBlock) &&
                (AppManager.I.Player.CurrentJourneyPosition.PlaySession == 100)) {
                PlayDialogRandomly();
            }
        }

        void OnDestroy()
        {
            this.StopAllCoroutines();
        }

        void updateStageIndicator()
        {
            Debug.Log("updateStageIndicator " + currentStageNumber + "/" + maxNumberOfStages);
            mapStageIndicator.Init(currentStageNumber - 1, maxNumberOfStages);
        }

        #region Dialogs
        void PlayDialogRandomly()
        {
            Database.LocalizationDataId[] data = new Database.LocalizationDataId[3];
            data[0] = Database.LocalizationDataId.Assessment_Start_1;
            data[1] = Database.LocalizationDataId.Assessment_Start_2;
            data[2] = Database.LocalizationDataId.Assessment_Start_3;
            int n = Random.Range(0, data.Length);
            KeeperManager.I.PlayDialog(data[n], true, true);
        }
        #endregion

        #region First Contact Session        
        /// <summary>
        /// Firsts the contact behaviour.
        /// Put Here logic for first contact only situations.
        /// </summary>
        void FirstContactBehaviour()
        {
            if (SimulateFirstContact) firstContactSimulationStep++;
            bool isFirstStep = SimulateFirstContact ? firstContactSimulationStep == 1 : AppManager.I.Player.IsFirstContact(1);
            bool isSecondStep = SimulateFirstContact ? firstContactSimulationStep == 2 : AppManager.I.Player.IsFirstContact(2);

            if (isFirstStep) {
                // First contact step 1:

                // ..and set first contact done.
                DesactivateUI();
                KeeperManager.I.PlayDialog(Database.LocalizationDataId.Map_Intro, true, true, AnturaText);
                AppManager.I.Player.FirstContactPassed();
                Debug.Log("First Contact Step1 finished! Go to Antura Space!");
            } else if (isSecondStep) {
                // First contact step 2:

                // ..and set first contact done.             
                ActivateUI();
                AppManager.I.Player.FirstContactPassed(2);
                KeeperManager.I.PlayDialog(Database.LocalizationDataId.Map_First, true, true, PlayDialogFirstStage);
                Debug.Log("First Contact Step2 finished! Good Luck!");
            }
        }

        void AnturaText()
        {
            KeeperManager.I.PlayDialog(Database.LocalizationDataId.Map_Intro_AnturaSpace, true, true, ActivateAnturaButton);
        }

        void PlayDialogFirstStage()
        {
            KeeperManager.I.PlayDialog(Database.LocalizationDataId.Map_Intro_Map1);
        }

        void PlayDialogStages(Database.LocalizationDataId data)
        {
            KeeperManager.I.PlayDialog(data);
        }

        void ActivateAnturaButton()
        {
            anturaButton.SetActive(true);
            this.StartCoroutine(CO_Tutorial());
        }

        IEnumerator CO_Tutorial()
        {
            TutorialUI.SetCamera(UICamera);
            Vector3 anturaBtPos = anturaButton.transform.position;
            anturaBtPos.z -= 1;
            while (true) {
                TutorialUI.Click(anturaButton.transform.position);
                yield return new WaitForSeconds(0.85f);
            }
        }
        #endregion

        /// <summary>
        /// Move to next Stage map
        /// </summary>
        public void StageLeft()
        {
            if ((currentStageNumber < maxNumberOfStages) && (!inTransition)) {
                previousStageNumber = currentStageNumber;
                currentStageNumber++;
                CalculateSettingsStage();

                if ((currentStageNumber <= maxStageUnlocked) && (AppManager.I.Player.CurrentJourneyPosition.Stage != currentStageNumber)) {
                    AppManager.I.Player.CurrentJourneyPosition.Stage++;
                    CalculatePosPin();
                } else {
                    StageNotAvailable();
                }
                updateStageIndicator();
                StartCoroutine("DesactivateMap");
            }
        }

        /// <summary>
        /// Move to the previous Stage map
        /// </summary>
        public void StageRight()
        {
            if ((currentStageNumber >= 1) && (!inTransition)) {

                previousStageNumber = currentStageNumber;
                currentStageNumber--;
                CalculateSettingsStage();

                if ((currentStageNumber <= maxStageUnlocked) && (AppManager.I.Player.CurrentJourneyPosition.Stage != currentStageNumber)) {
                    AppManager.I.Player.CurrentJourneyPosition.Stage--;
                    CalculatePosPin();
                } else if (AppManager.I.Player.CurrentJourneyPosition.Stage == currentStageNumber) {
                    lockUI.SetActive(false);
                    letter.GetComponent<LetterMovement>().AmIFirstorLastPos();
                    isStageAvailable = false;
                } else {
                    StageNotAvailable();
                }
                updateStageIndicator();
                StartCoroutine("DesactivateMap");
            }
        }

        void CalculateSettingsStage()
        {
            //DesactiveUIButtonsDuringTransition();
            inTransition = true;
            stages[currentStageNumber].SetActive(true);
            ChangeCamera(cameras[currentStageNumber]);
            ChangeCameraFogColor(currentStageNumber);
            FirstOrLastMap();
            lockUI.SetActive(true);
        }

        void CalculatePosPin()
        {
            isStageAvailable = false;
            letter.GetComponent<LetterMovement>().stageScript = miniMaps[currentStageNumber].GetComponent<Stage>();
            letter.GetComponent<LetterMovement>().ResetPosLetterAfterChangeStage();
            lockUI.SetActive(false);
            letter.GetComponent<LetterMovement>().AmIFirstorLastPos();
        }

        void DesactiveUIButtonsDuringTransition()
        {
            uiButtonMovementPlaySession.SetActive(!uiButtonMovementPlaySession.activeSelf);
        }

        void StageNotAvailable()
        {
            isStageAvailable = true;

            playButton.SetActive(false);
            nextPlaySessionButton.SetActive(false);
            beforePlaySessionButton.SetActive(false);
        }

        public void ChangeCamera(GameObject ZoomCameraGO)
        {
            CameraGameplayController.I.MoveToPosition(ZoomCameraGO.transform.position, ZoomCameraGO.transform.rotation, 0.6f);
        }

        IEnumerator ResetPosLetter()
        {
            yield return new WaitForSeconds(0.2f);
            letter.GetComponent<LetterMovement>().ResetPosLetter();
            letter.SetActive(true);
            CameraGameplayController.I.transform.position = cameras[AppManager.I.Player.CurrentJourneyPosition.Stage].transform.position;
        }

        void ChangeCameraFogColor(int c)
        {
            Camera.main.DOColor(colorMaps[c], 1);
            Camera.main.GetComponent<CameraFog>().color = colorMaps[c];
        }

        IEnumerator DesactivateMap()
        {
            yield return new WaitForSeconds(0.005f);
            DesactiveUIButtonsDuringTransition();
            yield return new WaitForSeconds(0.8f);
            if (!isStageAvailable) {
                playButton.SetActive(true);
            }
            DesactiveUIButtonsDuringTransition();
            //yield return new WaitForSeconds(0.3f);
            stages[previousStageNumber].SetActive(false);
            inTransition = false;
        }

        void FirstOrLastMap()
        {
            if (currentStageNumber == 1) {
                StartCoroutine("DesactivateButtonWithDelay", rightStageButton);
            } else if (currentStageNumber == maxNumberOfStages) {
                StartCoroutine("DesactivateButtonWithDelay", leftStageButton);
            } else {
                rightStageButton.SetActive(true);
                leftStageButton.SetActive(true);
            }
        }

        IEnumerator DesactivateButtonWithDelay(GameObject button)
        {
            yield return new WaitForSeconds(0.1f);
            button.SetActive(false);
        }

        void DesactivateUI()
        {
            uiButtonMovementPlaySession.SetActive(false);
            bookButton.SetActive(false);
            anturaButton.SetActive(false);
        }

        void ActivateUI()
        {
            uiButtonMovementPlaySession.SetActive(true);
            bookButton.SetActive(true);
            anturaButton.SetActive(true);
        }
    }
}