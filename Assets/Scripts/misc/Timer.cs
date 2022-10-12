using System;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float timeRemaining;
    public bool isRunning = false;

    Action onTimerFinish;

    // STARTS THE TIMER
    public void StartTimer(float maxTime, Action onTimerFinish) {
        timeRemaining = maxTime;
        this.onTimerFinish = onTimerFinish;
        isRunning = true;
    }

    public void PauseTimer() {
        isRunning = false;
    }

    void Update()
    {
        if (isRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
            }
            else
            {
                timeRemaining = 0;
                isRunning = false;
                onTimerFinish();
            }
        }
    }

    public static string DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;
        float minutes = Mathf.FloorToInt(timeToDisplay / 60); 
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}