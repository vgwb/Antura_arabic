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
    public class HomeScene : MonoBehaviour
    {
        // refactor: Remove the static access. The ProfileSelectorUI can directly access the HomeManager. Better yet, remove the Play() method from here and place something similar in AppManager.
        public static HomeScene I;

        [Header("Scene Setup")]
        public Music SceneMusic;
        public AnturaAnimationStates AnturaAnimation = AnturaAnimationStates.sitting;
        public LLAnimationStates LLAnimation = LLAnimationStates.LL_dancing;

        [Header("References")]
        public AnturaAnimationController AnturaAnimController;
        public LetterObjectView LLAnimController;
        public GameObject DialogReservedArea;
        public GameObject ProfileSelectorUI;

        void Awake()
        {
            I = this;
        }

        void Start()
        {
            GlobalUI.ShowPauseMenu(true, PauseMenuType.StartScreen);
            AudioManager.I.PlayMusic(SceneMusic);
            AudioManager.I.PlaySound(Sfx.GameTitle);

            AnturaAnimController.State = AnturaAnimation;
            LLAnimController.State = LLAnimation;

            AppManager.I.NavigationManager.SetStartingScene(AppScene.Home);
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
            LogManager.I.LogInfo(InfoEvent.AppPlay, JsonUtility.ToJson(new AppInfoParameters()));

            AppManager.I.NavigationManager.GoToNextScene();
        }

        private bool reservedAreaIsOpen = false;
        public void OnClickReservedAreaButton()
        {
            if (reservedAreaIsOpen)
            {
                OnCloseReservedArea();
            }
            else
            {
                OnOpenReservedArea();
            }
        }

        public void OnOpenReservedArea()
        {
            // HACK: hide LL since it covers the Arabic TMpro (incredible but true!)
            LLAnimController.gameObject.SetActive(false);
            DialogReservedArea.SetActive(true);
            ProfileSelectorUI.SetActive(false);
            GlobalUI.ShowPauseMenu(false);
            reservedAreaIsOpen = true;
        }

        public void OnCloseReservedArea()
        {
            // HACK: show LL since it covers the Arabic TMpro (incredible but true!)
            LLAnimController.gameObject.SetActive(true);
            DialogReservedArea.SetActive(false);
            ProfileSelectorUI.SetActive(true);
            GlobalUI.ShowPauseMenu(true, PauseMenuType.StartScreen);
            reservedAreaIsOpen = false;
        }
    }
}