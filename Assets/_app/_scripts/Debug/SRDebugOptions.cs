using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;
using EA4S;
using ModularFramework.Core;

public partial class SROptions
{
    [Category("Open Scene")]
    public void Home()
    {
        WidgetPopupWindow.Close();
        GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("app_Start");
        SRDebug.Instance.HideDebugPanel();
    }

    [Category("Open Scene")]
    public void FastCrowd()
    {
        WidgetPopupWindow.Close();
        GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("game_FastCrowd");
        SRDebug.Instance.HideDebugPanel();
    }

    [Category("Open Scene")]
    public void DontWakeUp()
    {
        WidgetPopupWindow.Close();
        GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("game_DontWakeUp");
        SRDebug.Instance.HideDebugPanel();
    }

    [Category("Open Scene")]
    public void Balloons()
    {
        WidgetPopupWindow.Close();
        GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("game_Balloons");
        SRDebug.Instance.HideDebugPanel();
    }

    [Category("Quality")]
    public void ToggleQuality()
    {
        AppManager.Instance.ToggleQualitygfx();
        SRDebug.Instance.HideDebugPanel();
    }
}
