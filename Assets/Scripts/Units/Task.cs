using System;
using UnityEngine;

public class Task
{
    const float DISTANCETOARRIVED = 0.1f;
    public string Description { get; private set; }
    public string Thought { get; private set; }
    public Action OnArrival { get; private set; }
    public ActionTimer ActionTimer { get; private set; }
    public Vector3 Position { get; private set; }

    public bool TaskCompleted => ActionTimer.IsFinished;

    public Task(string description, string thought, ActionTimer onTimerEndedMethod, Vector3 positionOfTask, Action onArrivalMethod = null)
    {
        Description = description;
        Thought = thought;
        ActionTimer = onTimerEndedMethod;
        Position = positionOfTask;
        OnArrival = onArrivalMethod;
    }

    public bool HasArrived(Vector3 objectPosition) => Vector2.Distance(objectPosition, Position) <= DISTANCETOARRIVED;

    public void Arrived()
    {
        OnArrival?.Invoke();
        ActionTimer.PlayPause(true);
    }
}