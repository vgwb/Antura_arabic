using EA4S.Antura;
using EA4S.Audio;
using EA4S.Core;
using EA4S.LivingLetters;
using EA4S.UI;
using UnityEngine;

namespace EA4S.Scenes
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
        public LetterObjectView LLAnimController;
        public GameObject DialogReservedArea;
        public GameObject ProfileSelectorUI;

        protected override void Start()
        {
            base.Start();
            GlobalUI.ShowPauseMenu(true, PauseMenuType.StartScreen);
            AudioManager.I.PlaySound(Sfx.GameTitle);

            AnturaAnimController.State = AnturaAnimation;
            LLAnimController.State = LLAnimation;

            // after 2 seconds (after the game title audio) invite palyer to create a profile
            Invoke("TutorCreateProfile", 2.3f);
        }

        void TutorCreateProfile()
        {
            if (AppManager.I.PlayerProfileManager.GetPlayersIconData().Count < 1) {
                AudioManager.I.PlayDialogue(Database.LocalizationDataId.Action_Createprofile);
            }
        }

        /// <summary>
        /// Start the game using the currently selected player.
        /// </summary>
        public void Play()
        {
            Debug.Log("Play with Player ID: " + AppManager.I.Player.Uuid);

            GlobalUI.ShowPauseMenu(true);

            // refactor: move this initialisation logic to the AppManager
            LogManager.I.InitNewSession();
            LogManager.I.LogInfo(InfoEvent.AppPlay, JsonUtility.ToJson(new DeviceInfo()));

            AppManager.I.NavigationManager.GoToNextScene();
        }

        #region Reserved Area

        private bool reservedAreaIsOpen = false;

        public void OnClickReservedAreaButton()
        {
            if (reservedAreaIsOpen) {
                OnCloseReservedArea();
            } else {
                OnOpenReservedArea();
            }
        }

        public void OnOpenReservedArea()
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

        public void OnCloseReservedArea()
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
    }
}