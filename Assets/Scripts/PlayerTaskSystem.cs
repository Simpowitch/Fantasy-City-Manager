using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerTaskSystem : MonoBehaviour
{
    Action onTaskCompleted;

    public void StartTask(Action actionOnComplete)
    {
        this.gameObject.SetActive(true);
        onTaskCompleted = actionOnComplete;
    }

    public void CompleteTask()
    {
        onTaskCompleted?.Invoke();
        this.gameObject.SetActive(false);
    }
}
