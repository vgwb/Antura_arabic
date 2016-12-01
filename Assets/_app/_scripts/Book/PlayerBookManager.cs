using UnityEngine;
using UnityEngine.UI;
using EA4S;
using ModularFramework.Core;
using System;
using System.Text;

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

    PlayerBookPanel currentPanel = PlayerBookPanel.None;

    void Start()
    {
        AppManager.I.GameSettings.CheatSuperDogMode = false;
        Debug.Log("Setting super dog mode (by default) to: " + AppManager.I.GameSettings.CheatSuperDogMode);

        GlobalUI.ShowPauseMenu(false);
        GlobalUI.ShowBackButton(true, ExitThisScene);
        AudioManager.I.PlayMusic(SceneMusic);
        SceneTransitioner.Close();

        AudioManager.I.PlayDialog("Book_Intro");

        HideAllPanels();
        OpenPanel(OpeningPanel);
    }

    void OpenPanel(PlayerBookPanel newPanel)
    {
        if (newPanel != currentPanel) {
            activatePanel(currentPanel, false);
            currentPanel = newPanel;
            activatePanel(currentPanel, true);
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
        NavigationManager.I.GoToScene(AppScene.Map);
    }
}
