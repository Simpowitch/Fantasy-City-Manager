using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

[System.Serializable]
public class TaskCreator
{
    [SerializeField] SubTaskInfo[] subTasks = null;

    public Task CreateTask(Unit unit, bool pickRandom, params Vector3[] positions)
    {
        Task newTask = new Task();
        for (int i = 0; i < subTasks.Length; i++)
        {
            SubTaskInfo subTask = subTasks[i];
            Vector3 pos = Vector3.zero;
            if (pickRandom)
                pos = Utility.ReturnRandom(positions);
            else
            {
                if (i < positions.Length)
                    pos = positions[i];
                else
                    pos = positions[positions.Length - 1];
            }
            newTask.CreateAndAddSubTask(unit, subTask.description, pos, subTask.actionTimer, subTask.DoEvent);
        }
        return newTask;
    }

    public Task CreateTask(Unit unit, Func<Vector3> positionSetter)
    {
        Task newTask = new Task();
        for (int i = 0; i < subTasks.Length; i++)
        {
            SubTaskInfo subTask = subTasks[i];
            Vector3 pos = positionSetter();
            newTask.CreateAndAddSubTask(unit, subTask.description, pos, subTask.actionTimer, subTask.DoEvent);
        }
        return newTask;
    }

    [System.Serializable]
    private class SubTaskInfo
    {
        public string description;
        public float actionTimer;
        public UnityEvent actionOnTimerEnd;

        public void DoEvent() => actionOnTimerEnd?.Invoke();
    }
}