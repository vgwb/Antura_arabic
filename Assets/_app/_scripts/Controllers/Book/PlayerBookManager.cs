using UnityEngine;
using UnityEngine.UI;
using EA4S;
using EA4S.Db;
using EA4S.Teacher;
using ModularFramework.Core;
using System;
using System.Text;
using TMPro;
using System.Globalization;

public class PlayerBookManager : MonoBehaviour
{

    [Header("Scene Setup")]
    public Music SceneMusic;

    void Start()
    {
        GlobalUI.ShowPauseMenu(false);
        AudioManager.I.PlayMusic(SceneMusic);
        SceneTransitioner.Close();

    }

    public void OpenMap()
    {
        GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("app_Map");
    }
}
