using System;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float TimeRemaining { get; set; }
    public bool IsRunning { get; set; } = false;

    Action onTimerFinish;

    Action<int> onTimerFinishWithIntParam;

    int intParam = -1;

    // STARTS THE TIMER
    public void StartTimer(float maxTime, Action onTimerFinish) {
        intParam = -1;
        TimeRemaining = maxTime;
        this.onTimerFinish = onTimerFinish;
        IsRunning = true;
    }
    public void StartTimerWithIntParameter(float maxTime, Action<int> onTimerFinishWithIntParam, int intParam) {
        this.intParam = intParam;
        TimeRemaining = maxTime;
        this.onTimerFinishWithIntParam = onTimerFinishWithIntParam;
        IsRunning = true;
    }

    public void PauseTimer() {
        IsRunning = false;
    }

    void Update()
    {
        if (IsRunning)
        {
            if (TimeRemaining > 0)
            {
                TimeRemaining -= Time.deltaTime;
            }
            else
            {
                TimeRemaining = 0;
                IsRunning = false;
                if(intParam != -1) onTimerFinishWithIntParam(intParam);
                else onTimerFinish();
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