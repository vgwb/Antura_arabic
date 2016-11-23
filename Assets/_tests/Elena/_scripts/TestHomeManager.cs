using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using EA4S;

public class TestHomeManager : MonoBehaviour
{
    void Start()
    {
        AudioManager.I.PlayMusic(Music.Theme3);
    }

    public void StartTest()
    {
        SceneManager.LoadScene("Wheel");
    }
}
