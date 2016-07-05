using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TimerManager : MonoBehaviour {

    [Range(10, 300)]
    public float time;
    public Text timerText;

    private bool isRunning;
    private float timeRemaining;

    void Update()
    {
        if (isRunning && timeRemaining > 0f)
        {
            timeRemaining -= Time.deltaTime;
            DisplayTime();
        }
        if (timeRemaining < 1f)
        {
            BalloonsGameManager.instance.OnTimeUp();
        }
    }

    public void StartTimer()
    {
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void ResetTimer()
    {
        timeRemaining = time;
    }

    public void DisplayTime()
    {
        var text = Mathf.Floor(timeRemaining).ToString();
        timerText.text = text;
    }

}
