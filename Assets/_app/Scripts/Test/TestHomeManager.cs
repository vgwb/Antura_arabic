using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using EA4S;

public class TestHomeManager : MonoBehaviour
{

    void Start() {
	
    }

    public void StartTest() {
        EA4S.LoggerEA4S.SessionID = Random.Range(10000000, 99999999).ToString();
        LoggerEA4S.Log("app", "appversion", "info", AppManager.AppVersion);
        LoggerEA4S.Log("app", "platform", "info", string.Format("{0} | (sys mem) {1} | (video mem) {2} | {3} |", SystemInfo.operatingSystem, SystemInfo.systemMemorySize, SystemInfo.graphicsMemorySize, Screen.width + "x" + Screen.height));
        LoggerEA4S.Log("app", "user", "info", LoggerEA4S.SessionID);
        SceneManager.LoadScene("Wheel");
    }
}
