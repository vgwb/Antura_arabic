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
        SceneManager.LoadScene("Wheel");
    }
}
