using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Task
{
    public Queue<SubTask> SubTasks { private set; get; }

    public SubTask GetNextSubTask()
    {
        if (SubTasks.Count > 0)
            return SubTasks.Dequeue();
        else
            return null;
    }

    public void CreateAndAddSubTask(Unit unit, string description, Vector3 position, float timeAtPosition, Action onTimerEndMethod)
    {
        if (SubTasks == null)
            SubTasks = new Queue<SubTask>();
        Action newAction = () =>
        {
            onTimerEndMethod?.Invoke();
            unit.FindNewSubTask();
        };
        SubTasks.Enqueue(new SubTask(position, description, new ActionTimer(timeAtPosition, newAction, false)));
    }

    [System.Serializable]
    public class SubTask
    {
        public Vector3 Position { private set; get; }
        public string Description { private set; get; }
        public ActionTimer actionTimer { private set; get; }


        public SubTask(Vector3 position, string description, ActionTimer action)
        {
            this.Position = position;
            this.actionTimer = action;
            Description = description;
        }

        public bool Arrived(Unit unit) => Vector3.Distance(unit.transform.position, Position) < 1;
    }
}
