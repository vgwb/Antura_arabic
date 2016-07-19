using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;
using EA4S;

public partial class SROptions
{
    [Category("Open Minigames")]
    public void FastCrowd() {
        //Debug.Log("Clearing PlayerPrefs");
        SceneManager.LoadScene("game_FastCrowd");
        SRDebug.Instance.HideDebugPanel();
    }

    [Category("Open Minigames")]
    public void DontWakeUp() {
        //Debug.Log("Clearing PlayerPrefs");
        SceneManager.LoadScene("game_DontWakeUp");
        SRDebug.Instance.HideDebugPanel();
    }

    [Category("Open Minigames")]
    public void Balloons() {
        //Debug.Log("Clearing PlayerPrefs");
        SceneManager.LoadScene("game_Balloons");
        SRDebug.Instance.HideDebugPanel();
    }

    [Category("Quality")]
    public void ToggleQuality() {
        //Debug.Log("Clearing PlayerPrefs");
        AppManager.Instance.ToggleQualitygfx();
        SRDebug.Instance.HideDebugPanel();
    }
}
