using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public event Action TimerEnded = delegate { };

    public string timerName;
    bool wasStarted = false;
    private static WaitForSeconds _waitForSeconds1 = new WaitForSeconds(1);
    int maxTime = 0, currentTime = 0;
    bool isRunning = false;

    public bool GetwasStarted(){ return wasStarted; }
    public void SetTimer(int time)
    {
        wasStarted = false;
        maxTime = time;
        currentTime = maxTime;
    }

    public virtual void StartTimer()
    {
        wasStarted = true;
        ResumeTimer();
    }

    public void ResumeTimer()
    {
        isRunning = true;
        StartCoroutine(PassTime());
    }

    /// <summary>
    /// Stops timer and returns how much time is left
    /// </summary>
    public int StopTimer()
    {
        isRunning = false;
        wasStarted = false;
        return currentTime;
    }

    protected virtual IEnumerator PassTime()
    {
        yield return _waitForSeconds1;
        currentTime--;
        if (currentTime <= 0)
        {
            isRunning = false;
            TimerEnded.Invoke();
        }
        else if (isRunning) StartCoroutine(PassTime());
    }

    protected int GetCurrentTime()
    {
        return currentTime;
    }
}
