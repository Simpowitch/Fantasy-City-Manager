using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Task
{
    public Queue<SubTask> SubTasks { private set; get; }

    public SubTask GetNextSubTask() => SubTasks.Peek();
    public void CompleteSubTask()
    {
        SubTask completedTask = SubTasks.Dequeue();
        completedTask.OnTaskCompleted?.Invoke();
    }

    public Task(SubTask[] newTasks)
    {
        this.SubTasks = new Queue<SubTask>();
        foreach (SubTask task in newTasks)
        {
            SubTasks.Enqueue(task);
        }
    }

    [System.Serializable]
    public class SubTask
    {
        public Vector3 Position { private set; get; }
        public float TimeAtPosition { private set; get; }
        public UnityEvent OnTaskBeginning { private set; get; }
        public UnityEvent OnTaskCompleted { private set; get; }


        public SubTask(Vector3 position, float time, UnityEvent beginningEvent, UnityEvent endEvent)
        {
            this.Position = position;
            this.TimeAtPosition = time;
            OnTaskBeginning = beginningEvent;
            OnTaskCompleted = endEvent;
        }
    }
}
