using EA4S.Audio;
using EA4S.Core;
using EA4S.UI;
using UnityEngine;

namespace EA4S.PlayerBook
{
    public enum PlayerBookPanel
    {
        None,
        Book,
        BookLetters,
        BookWords,
        BookPhrases,
        BookLearningBlocks,
        Player,
        Parents,
        MiniGames
    }

    /// <summary>
    /// Manages the Player Book scene.
    /// - shows unlocked learning content
    /// - provides information on player progression
    /// - grants direct access to minigames
    /// - grants access to the Parents' Panel
    /// </summary>
    public class PlayerBookManager : MonoBehaviour
    {

        [Header("Scene Setup")]
        public Music SceneMusic;
        public PlayerBookPanel OpeningPanel;

        [Header("References")]
        public GameObject BookPanel;
        public GameObject PlayerPanel;
        public GameObject ParentsPanel;
        public GameObject GamesPanel;

        public UIButton BtnBook;
        public UIButton BtnPlayer;
        public UIButton BtnParents;
        public UIButton BtnGames;

        PlayerBookPanel currentPanel = PlayerBookPanel.None;

        void Start()
        {
            AppManager.I.GameSettings.CheatSuperDogMode = false;
            //Debug.Log("Setting super dog mode (by default) to: " + AppManager.I.GameSettings.CheatSuperDogMode);

            GlobalUI.ShowPauseMenu(false);
            GlobalUI.ShowBackButton(true, ExitThisScene);
            AudioManager.I.PlayMusic(SceneMusic);
            LogManager.I.LogInfo(InfoEvent.Book, "enter");

            SceneTransitioner.Close();

            AudioManager.I.PlayDialogue("Book_Intro");

            HideAllPanels();
            OpenPanel(OpeningPanel);
        }

        void OpenPanel(PlayerBookPanel newPanel)
        {
            if (newPanel != currentPanel) {
                activatePanel(currentPanel, false);
                currentPanel = newPanel;
                activatePanel(currentPanel, true);
                ResetMenuButtons();
            }
        }

        void activatePanel(PlayerBookPanel panel, bool status)
        {
            switch (panel) {
                case PlayerBookPanel.Book:
                    BookPanel.SetActive(status);
                    break;
                case PlayerBookPanel.Parents:
                    ParentsPanel.SetActive(status);
                    break;
                case PlayerBookPanel.Player:
                    PlayerPanel.SetActive(status);
                    break;
                case PlayerBookPanel.MiniGames:
                    GamesPanel.SetActive(status);
                    break;
            }
        }

        void HideAllPanels()
        {
            BookPanel.SetActive(false);
            PlayerPanel.SetActive(false);
            ParentsPanel.SetActive(false);
            GamesPanel.SetActive(false);
        }

        void ResetMenuButtons()
        {
            BtnBook.Lock(currentPanel == PlayerBookPanel.Book);
            BtnPlayer.Lock(currentPanel == PlayerBookPanel.Player);
            BtnParents.Lock(currentPanel == PlayerBookPanel.Parents);
            BtnGames.Lock(currentPanel == PlayerBookPanel.MiniGames);
        }

        public void BtnOpenBook()
        {
            OpenPanel(PlayerBookPanel.Book);
        }

        public void BtnOpenPlayer()
        {
            OpenPanel(PlayerBookPanel.Player);
        }

        public void BtnOpenParents()
        {
            OpenPanel(PlayerBookPanel.Parents);
        }

        public void BtnOpenGames()
        {
            OpenPanel(PlayerBookPanel.MiniGames);
        }

        public void ExitThisScene()
        {
            LogManager.I.LogInfo(InfoEvent.Book, "exit");
            AppManager.I.NavigationManager.GoBack();
        }
    }
}