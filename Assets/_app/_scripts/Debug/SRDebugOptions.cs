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
        GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("_Start");
        SRDebug.Instance.HideDebugPanel();
    }

    [Category("Scenes")]
    public void Assessment()
    {
        WidgetPopupWindow.I.Close();
        GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("game_Assessment");
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
    public void ColorTickle()
    {
        WidgetPopupWindow.I.Close();
        GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("game_ColorTickle");
        SRDebug.Instance.HideDebugPanel();
    }

    [Category("Minigame")]
    public void DancingDots()
    {
        WidgetPopupWindow.I.Close();
        GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("game_DancingDots");
        SRDebug.Instance.HideDebugPanel();
    }

    [Category("Minigame")]
    public void Egg()
    {
        WidgetPopupWindow.I.Close();
        GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("game_Egg");
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
    public void HideAndSeek()
    {
        WidgetPopupWindow.I.Close();
        GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("game_HideAndSeek");
        SRDebug.Instance.HideDebugPanel();
    }


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
    public void MissingLetter()
    {
        WidgetPopupWindow.I.Close();
        GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("game_MissingLetter");
        SRDebug.Instance.HideDebugPanel();
    }

    [Category("Minigame")]
    public void MixedLetters()
    {
        WidgetPopupWindow.I.Close();
        GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("game_MixedLetters");
        SRDebug.Instance.HideDebugPanel();
    }

    [Category("Minigame")]
    public void Scanner()
    {
        WidgetPopupWindow.I.Close();
        GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("game_Scanner");
        SRDebug.Instance.HideDebugPanel();
    }

    [Category("Minigame")]
    public void TakeMeHome()
    {
        WidgetPopupWindow.I.Close();
        GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("game_TakeMeHome");
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
        //      EA4S.FastCrowd.FastCrowd.Instance.DebugForceEndGame();
        SRDebug.Instance.HideDebugPanel();
    }

    [Category("Options")]
    public void ToggleQuality()
    {
        AppManager.Instance.ToggleQualitygfx();
        SRDebug.Instance.HideDebugPanel();
    }

    /// MakeFriends
    [Category("MakeFriends")]
    public bool MakeFriendsUseDifficulty { get; set; }

    [Category("MakeFriends")]
    public EA4S.MakeFriends.MakeFriendsVariation MakeFriendsDifficulty { get; set; }

    /// ThrowBalls
    private bool ThrowBallsShowProjection = true;
    [Category("ThrowBalls")]
    public bool ShowProjection {
        get { return ThrowBallsShowProjection; }
        set { ThrowBallsShowProjection = value; }
    }

    private float ThrowBallselasticity = 19f;
    [Category("ThrowBalls")]
    public float Elasticity {
        get { return ThrowBallselasticity; }
        set { ThrowBallselasticity = value; }
    }
}
