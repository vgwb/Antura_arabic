using Antura.Audio;
using Antura.Core;
using System.Collections;
using System.Collections.Generic;
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

    public class Book : MonoBehaviour
    {
        [Header("References")]
        public static Book I;
        public GameObject VocabularyPanel;
        public GameObject PlayerPanel;
        public GameObject JourneyPanel;
        public GameObject GamesPanel;

        BookArea currentPanel = BookArea.None;
        BookArea previousPanel = BookArea.None;

        void Awake()
        {
            I = this;
            HideAllPanels();
        }

        void Start()
        {

        }

        public void OpenArea(BookArea newPanel, bool navigationHistory = false)
        {
            //switch (newPanel) {
            //    case BookArea.Vocabulary:
            //        Instantiate(VocabularyPanel);
            //        break;
            //    case BookArea.Journey:
            //        Instantiate(JourneyPanel);
            //        break;
            //    case BookArea.Player:
            //        Instantiate(PlayerPanel);
            //        break;
            //    case BookArea.MiniGames:
            //        Instantiate(GamesPanel);
            //        break;
            //}
            //var panel = Instantiate(VocabularyPanel);
            //panel.transform.SetParent(UICanvas.transform, false);
            if (newPanel != currentPanel) {
                if (navigationHistory) {
                    previousPanel = currentPanel;
                } else {
                    previousPanel = BookArea.None;
                }

                activatePanel(currentPanel, false);
                currentPanel = newPanel;
                activatePanel(currentPanel, true);
                //ResetMenuButtons();
            }

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

        public void OnBtnClose()
        {
            if (previousPanel == BookArea.None) {
                BookManager.I.CloseBook();
            } else {
                OpenArea(previousPanel);
            }
        }

        public void BtnOpenVocabulary()
        {
            OpenArea(BookArea.Vocabulary);
        }

        public void BtnOpenMinigGames()
        {
            OpenArea(BookArea.MiniGames);
        }

        public void BtnOpenVocabularyWithBack()
        {
            OpenArea(BookArea.Vocabulary, true);
        }

        public void BtnOpenMinigGamesWithBack()
        {
            OpenArea(BookArea.MiniGames, true);
        }

        public void BtnOpenPlayerProfile()
        {
            OpenArea(BookArea.Player);
        }

        public void BtnOpenJourney()
        {
            OpenArea(BookArea.Journey);
        }

        public void BtnOpenPhotoGallery()
        {
            AudioManager.I.PlayDialogue(Database.LocalizationDataId.AnturaSpace_Photo_Gallery);
        }
    }
}