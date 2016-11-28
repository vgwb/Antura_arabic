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
    Player,
    Parents,
    MiniGames
}

public class PlayerBookManager : MonoBehaviour
{

    [Header("Scene Setup")]
    public Music SceneMusic;

    [Header("References")]
    public GameObject BookPanel;
    public GameObject PlayerPanel;
    public GameObject ParentsPanel;
    public GameObject GamesPanel;

    PlayerBookPanel currentPanel = PlayerBookPanel.None;

    void Start()
    {
        GlobalUI.ShowPauseMenu(false);
        AudioManager.I.PlayMusic(SceneMusic);
        SceneTransitioner.Close();

        AudioManager.I.PlayDialog("Book_Intro");

        OpenPanel(PlayerBookPanel.Book);
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

    public void BtnOpenMap()
    {
        GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("app_Map");
    }
}
