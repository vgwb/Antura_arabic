using UnityEngine;
using UnityEngine.UI;
using EA4S;
using EA4S.Db;
using EA4S.Teacher;
using ModularFramework.Core;
using System;
using System.Text;


public class PlayerBookManager : MonoBehaviour
{

    enum PlayerBookPanel
    {
        None,
        Book,
        Player,
        Parents
    }

    [Header("Scene Setup")]
    public Music SceneMusic;

    [Header("References")]
    public GameObject BookPanel;
    public GameObject PlayerPanel;
    public GameObject ParentsPanel;

    PlayerBookPanel currentPanel = PlayerBookPanel.None;

    void Start()
    {
        GlobalUI.ShowPauseMenu(false);
        AudioManager.I.PlayMusic(SceneMusic);
        SceneTransitioner.Close();

        AudioManager.I.PlayDialog("Book_Intro");

        OpenPanel(PlayerBookPanel.Player);
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

    public void BtnOpenMap()
    {
        GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("app_Map");
    }
}
