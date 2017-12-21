using Antura.Audio;
using Antura.Core;
using Antura.Database;
using Antura.Dog;
using Antura.LivingLetters;
using Antura.UI;
using UnityEngine;

namespace Antura.Scenes
{
    /// <summary>
    /// Controls the _Start scene, providing an entry point for all users prior to having selected a player profile. 
    /// </summary>
    public class HomeScene : SceneBase
    {
        [Header("Setup")]
        public AnturaAnimationStates AnturaAnimation = AnturaAnimationStates.sitting;

        public LLAnimationStates LLAnimation = LLAnimationStates.LL_dancing;

        [Header("References")]
        public AnturaAnimationController AnturaAnimController;

        public LivingLetterController LLAnimController;
        public GameObject DialogReservedArea;
        public GameObject ProfileSelectorUI;

        public GameObject PanelFirstCheck;

        protected override void Start()
        {
            base.Start();
            GlobalUI.ShowPauseMenu(true, PauseMenuType.StartScreen);
            AudioManager.I.PlaySound(Sfx.GameTitle);

            AnturaAnimController.State = AnturaAnimation;
            LLAnimController.State = LLAnimation;

            // after 2 seconds (after the game title audio) invite palyer to create a profile
            Invoke("TutorCreateProfile", 2.3f);

            if (AppManager.I.AppSettingsManager.IsAppJustUpdated) {
                AppManager.I.AppSettingsManager.AppFirstCheckDone();
                OpenFirstCheckPanel();
            }
        }

        void TutorCreateProfile()
        {
            if (AppManager.I.PlayerProfileManager.GetPlayersIconData().Count < 1) {
                AudioManager.I.PlayDialogue(LocalizationDataId.Action_Createprofile);
            }
        }

        /// <summary>
        /// Start the game using the currently selected player.
        /// </summary>
        public void Play()
        {
            Debug.Log("Play with Player: " + AppManager.I.Player);

            GlobalUI.ShowPauseMenu(true);

            // TODO refactor: move this initialisation logic to the AppManager
            LogManager.I.InitNewSession();
            LogManager.I.LogInfo(InfoEvent.AppPlay, JsonUtility.ToJson(new DeviceInfo()));

            AppManager.I.NavigationManager.GoToNextScene();
        }

        #region Reserved Area

        private bool reservedAreaIsOpen = false;

        public void OnBtnReservedArea()
        {
            if (reservedAreaIsOpen) {
                CloseReservedAreaPanel();
            } else {
                OpenReservedAreaPanel();
            }
        }

        public void OnBtnQuit()
        {
            AppManager.I.QuitApplication();
        }

        public void OpenReservedAreaPanel()
        {
            AudioManager.I.PlaySound(Sfx.UIButtonClick);
            // HACK: hide LL And Antura since they cover the Arabic TMpro (incredible but true!)
            LLAnimController.gameObject.SetActive(false);
            AnturaAnimController.gameObject.SetActive(false);

            DialogReservedArea.SetActive(true);
            ProfileSelectorUI.SetActive(false);
            GlobalUI.ShowPauseMenu(false);
            reservedAreaIsOpen = true;
        }

        public void CloseReservedAreaPanel()
        {
            AudioManager.I.PlaySound(Sfx.UIButtonClick);
            // HACK: show LL And Antura since they cover the Arabic TMpro (incredible but true!)
            LLAnimController.gameObject.SetActive(true);
            AnturaAnimController.gameObject.SetActive(true);

            DialogReservedArea.SetActive(false);
            ProfileSelectorUI.SetActive(true);
            GlobalUI.ShowPauseMenu(true, PauseMenuType.StartScreen);
            reservedAreaIsOpen = false;
        }

        #endregion
        #region FIrstCHeckPanel
        public void OpenFirstCheckPanel()
        {
            AudioManager.I.PlaySound(Sfx.UIButtonClick);
            // HACK: hide LL And Antura since they cover the Arabic TMpro (incredible but true!)
            LLAnimController.gameObject.SetActive(false);
            AnturaAnimController.gameObject.SetActive(false);
            ProfileSelectorUI.SetActive(false);
            GlobalUI.ShowPauseMenu(false);

            PanelFirstCheck.SetActive(true);
        }

        public void CloseFirstCheckPanel()
        {
            AudioManager.I.PlaySound(Sfx.UIButtonClick);
            // HACK: show LL And Antura since they cover the Arabic TMpro (incredible but true!)
            LLAnimController.gameObject.SetActive(true);
            AnturaAnimController.gameObject.SetActive(true);
            ProfileSelectorUI.SetActive(true);
            GlobalUI.ShowPauseMenu(true, PauseMenuType.StartScreen);

            PanelFirstCheck.SetActive(false);
        }
        #endregion
    }
}