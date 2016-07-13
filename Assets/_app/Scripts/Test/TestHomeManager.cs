using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TestHomeManager : MonoBehaviour
{

    void Start() {
	
    }

    public void StartTest() {
        SceneManager.LoadScene("Wheel");
    }
}
