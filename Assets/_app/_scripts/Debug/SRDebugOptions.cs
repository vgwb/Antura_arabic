using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;
using EA4S;
using ModularFramework.Core;

public partial class SROptions
{
    [Category("Scenes")]
    public void Home()
    {
        WidgetPopupWindow.I.Close();
        GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("app_Start");
        SRDebug.Instance.HideDebugPanel();
    }

    [Category("Scenes")]
    public void Assessment()
    {
        WidgetPopupWindow.I.Close();
        GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("app_Assessment");
        SRDebug.Instance.HideDebugPanel();
    }

    [Category("Minigame")]
    public void Balloons()
    {
        WidgetPopupWindow.I.Close();
        GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("game_Balloons");
        SRDebug.Instance.HideDebugPanel();
    }

    [Category("Minigame")]
    public void FastCrowd()
    {
        WidgetPopupWindow.I.Close();
        GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("game_FastCrowd");
        SRDebug.Instance.HideDebugPanel();
    }

    [Category("Minigame")]
    public void DancingDots()
    {
        WidgetPopupWindow.I.Close();
        GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("game_DancingDots");
        SRDebug.Instance.HideDebugPanel();
    }

    //[Category("Minigame")]
    //public void Egg()
    //{
    //    WidgetPopupWindow.I.Close();
    //    GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("game_Egg");
    //    SRDebug.Instance.HideDebugPanel();
    //}

    [Category("Minigame")]
    public void MakeFriends()
    {
        WidgetPopupWindow.I.Close();
        GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("game_MakeFriends");
        SRDebug.Instance.HideDebugPanel();
    }

    [Category("Minigame")]
    public void Maze()
    {
        WidgetPopupWindow.I.Close();
        GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("game_Maze");
        SRDebug.Instance.HideDebugPanel();
    }

    [Category("Minigame")]
    public void ThrowBalls()
    {
        WidgetPopupWindow.I.Close();
        GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("game_ThrowBalls");
        SRDebug.Instance.HideDebugPanel();
    }

    [Category("Minigame")]
    public void Tobogan()
    {
        WidgetPopupWindow.I.Close();
        GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("game_Tobogan");
        SRDebug.Instance.HideDebugPanel();
    }

    [Category("Manage")]
    public void Database()
    {
        WidgetPopupWindow.I.Close();
        GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("manage_Database");
        SRDebug.Instance.HideDebugPanel();
    }

    //[Category("Minigame")]
    //public void DontWakeUp()
    //{
    //    WidgetPopupWindow.I.Close();
    //    GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("game_DontWakeUp");
    //    SRDebug.Instance.HideDebugPanel();
    //}

    [Category("Shortcuts")]
    public void EndFastCrowdGame()
    {
        EA4S.FastCrowd.FastCrowd.Instance.DebugForceEndGame();
        SRDebug.Instance.HideDebugPanel();
    }

    [Category("Options")]
    public void ToggleQuality()
    {
        AppManager.Instance.ToggleQualitygfx();
        SRDebug.Instance.HideDebugPanel();
    }
}
