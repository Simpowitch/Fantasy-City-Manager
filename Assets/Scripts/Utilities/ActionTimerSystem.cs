using System.Collections.Generic;
using UnityEngine;
using System;

public class ActionTimerSystem : MonoBehaviour
{
    static List<ActionTimer> actionTimers = new List<ActionTimer>();
    public static void AddActionTimer(ActionTimer newTimer) => actionTimers.Add(newTimer);
    public static void RemoveActionTimer(ActionTimer finishedTimer) => actionTimers.Remove(finishedTimer);

    private void Update()
    {
        if (actionTimers.Count <= 0)
        {
            return;
        }
        float time = Time.deltaTime;
        for (int i = 0; i < actionTimers.Count; i++)
        {
            if (actionTimers[i] != null)
                actionTimers[i].Update(time);
            else
            {
                actionTimers.RemoveAt(i);
                i--;
            }
        }
        for (int i = 0; i < actionTimers.Count; i++)
        {
            if (actionTimers[i].IsFinished)
            {
                RemoveActionTimer(actionTimers[i]);
                i--;
            }
        }
    }
}

public class ActionTimer
{
    Action actionOnTimerEnd;
    float remainingTime;
    bool running;
    public bool IsFinished { get => remainingTime <= 0; }
    public ActionTimer(float time, Action actionOnTimerEnd, bool startRunning)
    {
        remainingTime = time;
        this.actionOnTimerEnd = actionOnTimerEnd;
        this.running = startRunning;
        ActionTimerSystem.AddActionTimer(this);
    }

    public void PlayPause(bool play) => running = play;

    public void Update(float timeChange)
    {
        if (!running)
            return;
        remainingTime -= timeChange;
        if (IsFinished)
        {
            actionOnTimerEnd?.Invoke();
            actionOnTimerEnd = null;
        }
    }
}
