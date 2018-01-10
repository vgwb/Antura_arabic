using Antura.UI;
using Antura.Core;
using UnityEngine;

namespace Antura.Book
{
    public enum BookArea
    {
        None,
        Vocabulary,
        Player,
        Journey,
        MiniGames
    }

    public class BookScene : MonoBehaviour
    {
        public static BookArea OverridenOpeningArea;

        [Header("Scene Setup")]
        public Music SceneMusic;
        public BookArea OpeningArea;

        public GameObject[] HideTheseAtStartup;

        [Header("References")]
        public GameObject VocabularyPanel;
        public GameObject PlayerPanel;
        public GameObject JourneyPanel;
        public GameObject GamesPanel;

        public UIButton BtnBook;
        public UIButton BtnPlayer;
        public UIButton BtnJourney;
        public UIButton BtnGames;

        BookArea currentPanel = BookArea.None;

        void Start()
        {
            GlobalUI.ShowPauseMenu(false);
            foreach (var go in HideTheseAtStartup) {
                go.SetActive(false);
            }
            //GlobalUI.ShowBackButton(true, GoBackCustom);
            //AudioManager.I.PlayMusic(SceneMusic);
            //AudioManager.I.PlayDialogue("Book_Intro");

            //HideAllPanels();
            if (OverridenOpeningArea != BookArea.None) {
                OpenArea(OverridenOpeningArea);
            } else {
                OpenArea(OpeningArea);
            }

            // Debug.Log("PREV SCENE IS RESERVED AREA: " + AppManager.I.NavigationManager.PrevSceneIsReservedArea());
        }

        void OpenArea(BookArea newPanel)
        {
            switch (newPanel) {
                case BookArea.Vocabulary:
                    Instantiate(VocabularyPanel);
                    break;
                case BookArea.Journey:
                    Instantiate(JourneyPanel);
                    break;
                case BookArea.Player:
                    Instantiate(PlayerPanel);
                    break;
                case BookArea.MiniGames:
                    Instantiate(GamesPanel);
                    break;
            }
            // var panel = Instantiate(VocabularyPanel);
            //panel.transform.SetParent(UICanvas.transform, false);
            //if (newPanel != currentPanel) {
            //    activatePanel(currentPanel, false);
            //    currentPanel = newPanel;
            //    activatePanel(currentPanel, true);
            //    ResetMenuButtons();
            //}
        }

        void activatePanel(BookArea panel, bool status)
        {
            switch (panel) {
                case BookArea.Vocabulary:
                    VocabularyPanel.SetActive(status);
                    break;
                case BookArea.Journey:
                    JourneyPanel.SetActive(status);
                    break;
                case BookArea.Player:
                    PlayerPanel.SetActive(status);
                    break;
                case BookArea.MiniGames:
                    GamesPanel.SetActive(status);
                    break;
            }
        }

        void HideAllPanels()
        {
            VocabularyPanel.SetActive(false);
            PlayerPanel.SetActive(false);
            JourneyPanel.SetActive(false);
            GamesPanel.SetActive(false);
        }

        void ResetMenuButtons()
        {
            BtnBook.Lock(currentPanel == BookArea.Vocabulary);
            BtnPlayer.Lock(currentPanel == BookArea.Player);
            BtnJourney.Lock(currentPanel == BookArea.Journey);
            BtnGames.Lock(currentPanel == BookArea.MiniGames);
        }

        public void BtnOpenBook()
        {
            OpenArea(BookArea.Vocabulary);
        }

        public void BtnOpenPlayer()
        {
            OpenArea(BookArea.Player);
        }

        public void BtnOpenJourney()
        {
            OpenArea(BookArea.Journey);
        }

        public void BtnOpenGames()
        {
            OpenArea(BookArea.MiniGames);
        }

        public void GoBackCustom()
        {
            AppManager.I.NavigationManager.GoBack();
        }
    }
}