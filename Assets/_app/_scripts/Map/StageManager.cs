using UnityEngine;
using System.Collections;
using EA4S;
using DG.Tweening;
using ModularFramework.Components;

namespace EA4S
{
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

        [Header("LockUI")]
        public GameObject lockUI;

        [Header("UIButtons")]
        public GameObject leftStageButton;
        public GameObject rightStageButton;
        public GameObject rightMovementButton;
        public GameObject leftMovementButton;
        public GameObject playButton;
        public GameObject bookButton;
        public GameObject anturaButton;

        [Header("Other")]
        public Camera UICamera;

        int s, i, previousStage, numberStage;
        bool inTransition;
        static int firstContactSimulationStep;

        void Awake()
        {
            if (!Application.isEditor) SimulateFirstContact = false; // Force debug options to FALSE if we're not in the editor

            /*AppManager.I.Player.MaxJourneyPosition.Stage = 2;
            AppManager.I.Player.MaxJourneyPosition.LearningBlock = 3;
            AppManager.I.Player.MaxJourneyPosition.PlaySession = 1;*/
            numberStage = AppManager.I.Player.CurrentJourneyPosition.Stage;
            s = AppManager.I.Player.MaxJourneyPosition.Stage;
            for (i = 1; i <= (s - 1); i++)
            {
                stages[i].SetActive(false);
                miniMaps[i].GetComponent<MiniMap>().isAvailableTheWholeMap = true;
                miniMaps[i].GetComponent<MiniMap>().CalculateSettingsStageMap();
            }
            miniMaps[i].GetComponent<MiniMap>().CalculateSettingsStageMap();

            stages[AppManager.I.Player.CurrentJourneyPosition.Stage].SetActive(true);
            Camera.main.backgroundColor = colorMaps[AppManager.I.Player.CurrentJourneyPosition.Stage];
            Camera.main.GetComponent<CameraFog>().color = colorMaps[AppManager.I.Player.CurrentJourneyPosition.Stage];
            letter.GetComponent<LetterMovement>().miniMapScript = miniMaps[AppManager.I.Player.CurrentJourneyPosition.Stage].GetComponent<MiniMap>();

            StartCoroutine("ResetPosLetter");
        }
        void Start()
        {
            /* FIRST CONTACT FEATURE */
            if (AppManager.I.Player.IsFirstContact() || SimulateFirstContact)
            {
                FirstContactBehaviour();
            }
            /* --------------------- */
            FirstOrLastMap();
        }

        void OnDestroy()
        {
            this.StopAllCoroutines();
        }

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

            if (isFirstStep)
            {
                // First contact step 1:

                // ..and set first contact done.
                DesactivateUI();
                KeeperManager.I.PlayDialog(Db.LocalizationDataId.Map_Intro, true, true, AnturaText);
                AppManager.I.Player.FirstContactPassed();
                Debug.Log("First Contact Step1 finished! Go to Antura Space!");
            }
            else if (isSecondStep)
            {
                // First contact step 2:

                // ..and set first contact done.             
                ActivateUI();
                AppManager.I.Player.FirstContactPassed(2);
                KeeperManager.I.PlayDialog(Db.LocalizationDataId.Map_First);
                Debug.Log("First Contact Step2 finished! Good Luck!");
                anturaButton.GetComponent<OnClickButtonChangeScene>().SceneNameCustom = "app_AnturaSpace";
            }

        }
        void AnturaText()
        {
            KeeperManager.I.PlayDialog(Db.LocalizationDataId.Map_Intro_AnturaSpace, true, true, ActivateAnturaButton);
        }
        void ActivateAnturaButton()
        {
            anturaButton.SetActive(true);
            anturaButton.GetComponent<OnClickButtonChangeScene>().SceneNameCustom = "app_Rewards";
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

        public void StageLeft()
        {
            if ((numberStage < 6) && (!inTransition))
            {
                previousStage = numberStage;
                numberStage++;
                inTransition = true;
                stages[numberStage].SetActive(true);
                ChangeCamera(cameras[numberStage]);
                ChangeCameraFogColor(numberStage);
                FirstOrLastMap();
                lockUI.SetActive(true);
                DesactivateMovementPlayButtons();

                if ((numberStage <= s) && (AppManager.I.Player.CurrentJourneyPosition.Stage != numberStage))
                {
                    ChangePinDotToBlack();
                    AppManager.I.Player.CurrentJourneyPosition.Stage++;
                    letter.GetComponent<LetterMovement>().miniMapScript = miniMaps[numberStage].GetComponent<MiniMap>();
                    letter.GetComponent<LetterMovement>().ResetPosLetterAfterChangeStage();
                    lockUI.SetActive(false);
                    ActivateMovementPlayButtons();
                    letter.GetComponent<LetterMovement>().AmIFirstorLastPos();
                }
                StartCoroutine("DesactivateMap");
            }
        }
        public void StageRight()
        {
            if ((numberStage >= 1) && (!inTransition))
            {
                previousStage = numberStage;
                numberStage--;
                inTransition = true;
                stages[numberStage].SetActive(true);
                ChangeCamera(cameras[numberStage]);
                ChangeCameraFogColor(numberStage);
                FirstOrLastMap();
                lockUI.SetActive(true);
                DesactivateMovementPlayButtons();

                if ((numberStage <= s) && (AppManager.I.Player.CurrentJourneyPosition.Stage != numberStage))
                {
                    ChangePinDotToBlack();
                    AppManager.I.Player.CurrentJourneyPosition.Stage--;
                    letter.GetComponent<LetterMovement>().miniMapScript = miniMaps[numberStage].GetComponent<MiniMap>();
                    letter.GetComponent<LetterMovement>().ResetPosLetterAfterChangeStage();
                    lockUI.SetActive(false);
                    ActivateMovementPlayButtons();
                    letter.GetComponent<LetterMovement>().AmIFirstorLastPos();
                }
                else if (AppManager.I.Player.CurrentJourneyPosition.Stage == numberStage)
                {
                    lockUI.SetActive(false);
                    ActivateMovementPlayButtons();
                    letter.GetComponent<LetterMovement>().AmIFirstorLastPos();
                }


                StartCoroutine("DesactivateMap");
            }
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
        void ChangePinDotToBlack()
        {
            if (AppManager.I.Player.CurrentJourneyPosition.PlaySession == 100)//change color pin to black
            {
                letter.GetComponent<LetterMovement>().miniMapScript.posPines[AppManager.I.Player.CurrentJourneyPosition.LearningBlock].GetComponent<MapPin>().ChangeMaterialPinToBlack();
                letter.GetComponent<LetterMovement>().miniMapScript.posPines[AppManager.I.Player.CurrentJourneyPosition.LearningBlock].transform.GetChild(0).gameObject.SetActive(true); //activate pin
            }
            else
                letter.GetComponent<LetterMovement>().ChangeMaterialDotToBlack(letter.GetComponent<LetterMovement>().miniMapScript.posDots[letter.GetComponent<LetterMovement>().pos]);
        }
        void ChangeCameraFogColor(int c)
        {
            Camera.main.DOColor(colorMaps[c], 1);
            Camera.main.GetComponent<CameraFog>().color = colorMaps[c];
        }
        IEnumerator DesactivateMap()
        {
            yield return new WaitForSeconds(0.8f);
            stages[previousStage].SetActive(false);
            inTransition = false;
        }
        void FirstOrLastMap()
        {
            if (numberStage == 1)
                rightStageButton.SetActive(false);
            else if (numberStage == 6)
                leftStageButton.SetActive(false);
            else
            {
                rightStageButton.SetActive(true);
                leftStageButton.SetActive(true);
            }

        }
        void DesactivateUI()
        {
            leftStageButton.SetActive(false);
            rightStageButton.SetActive(false);
            rightMovementButton.SetActive(false);
            leftMovementButton.SetActive(false);
            playButton.SetActive(false);
            bookButton.SetActive(false);
            anturaButton.SetActive(false);
        }
        void ActivateUI()
        {
            leftStageButton.SetActive(true);
            rightStageButton.SetActive(true);
            rightMovementButton.SetActive(true);
            leftMovementButton.SetActive(true);
            playButton.SetActive(true);
            bookButton.SetActive(true);
            anturaButton.SetActive(true);
        }
        void DesactivateMovementPlayButtons()
        {
            rightMovementButton.SetActive(false);
            leftMovementButton.SetActive(false);
            playButton.SetActive(false);
        }
        void ActivateMovementPlayButtons()
        {
            rightMovementButton.SetActive(true);
            leftMovementButton.SetActive(true);
            playButton.SetActive(true);
        }
        /*void DesactivateMap()
        {
            stages[previousStage].SetActive(false);
            inTransition = false;
        }*/


    }
}