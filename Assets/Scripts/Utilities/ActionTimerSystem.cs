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
        foreach (var actionTimer in actionTimers)
        {
            actionTimer.Update(time);
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
    public bool IsFinished { get => remainingTime <= 0; }
    public ActionTimer(float time, Action actionOnTimerEnd)
    {
        remainingTime = time;
        this.actionOnTimerEnd = actionOnTimerEnd;
        ActionTimerSystem.AddActionTimer(this);
    }

    public void Update(float timeChange)
    {
        remainingTime -= timeChange;
        if (IsFinished)
        {
            actionOnTimerEnd?.Invoke();
            actionOnTimerEnd = null;
        }
    }
}
