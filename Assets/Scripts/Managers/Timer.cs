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

    /// Stops timer and returns how much time is left
    public int StopTimer()
    {
        isRunning = false;
        StopAllCoroutines();
        return currentTime;
    }

    protected virtual IEnumerator PassTime()
    {
        yield return _waitForSeconds1;
        currentTime--;
        if (currentTime <= 0)
        {   
            wasStarted = false;
            isRunning = false;
            TimerEnded.Invoke();
            StopTimer();
        }
        else if (isRunning) StartCoroutine(PassTime());
    }

    protected int GetCurrentTime()
    {
        return currentTime;
    }
}
