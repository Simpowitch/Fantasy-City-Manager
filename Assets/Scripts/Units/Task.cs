using System;
using UnityEngine;

public class Task
{
    const float DISTANCETOARRIVED = 1f;
    public string Description { get; private set; }
    public Action OnArrival { get; private set; }
    public ActionTimer ActionTimer { get; private set; }
    public Vector3 Position { get; private set; }

    public bool TaskAssigned { get; private set; }

    public bool TaskCompleted => ActionTimer.IsFinished;
   
    public Task(string description, ActionTimer onTimerEndedMethod, Vector3 positionOfTask, Action onArrivalMethod = null)
    {
        Description = description;
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

    public void AssignTask()
    {
        if (TaskAssigned)
            Debug.LogError("Assigned task trying to be assigned again");
        else
            TaskAssigned = true;
    }

    public void AbortTask()
    {
        if (TaskAssigned)
            TaskAssigned = false;
        else
            Debug.LogError("Not Assigned task trying to be aborted");
    }

    //public Queue<SubTask> SubTasks { private set; get; }

    //public SubTask GetNextSubTask()
    //{
    //    if (SubTasks.Count > 0)
    //        return SubTasks.Dequeue();
    //    else
    //        return null;
    //}

    //public void CreateAndAddSubTask(Unit unit, string description, Vector3 position, float timeAtPosition, Action onTimerEndMethod) => CreateAndAddSubTask(unit, description, position, timeAtPosition, onTimerEndMethod, null);

    //public void CreateAndAddSubTask(Unit unit, string description, Vector3 position, float timeAtPosition, Action onTimerEndMethod, Action onTimerStart)
    //{
    //    if (SubTasks == null)
    //        SubTasks = new Queue<SubTask>();
    //    Action newAction = () =>
    //    {
    //        onTimerEndMethod?.Invoke();
    //        unit.FindNewSubTask();
    //    };
    //    SubTasks.Enqueue(new SubTask(position, description, new ActionTimer(timeAtPosition, newAction, false), onTimerStart));
    //}

    //[System.Serializable]
    //public class SubTask
    //{
    //    public Vector3 Position { private set; get; }
    //    public string Description { private set; get; }
    //    public ActionTimer actionTimer { private set; get; }
    //    public Action actionOnArrival;

    //    public SubTask(Vector3 position, string description, ActionTimer actionTimer, Action actionOnArrival)
    //    {
    //        this.Position = position;
    //        this.actionTimer = actionTimer;
    //        Description = description;
    //        this.actionOnArrival = actionOnArrival;
    //    }

    //    public bool Arrived(Unit unit) => Vector3.Distance(unit.transform.position, Position) < 1;
    //    public void StartTimer()
    //    {
    //        actionOnArrival?.Invoke();
    //        actionTimer.PlayPause(true);
    //    }
    //}
}

//[System.Serializable]
//public class TaskCreator
//{
//    [SerializeField] SubTaskInfo[] subTasks = null;

//    public Task CreateTask(Unit unit, bool pickRandom, params Vector3[] positions)
//    {
//        Task newTask = new Task();
//        for (int i = 0; i < subTasks.Length; i++)
//        {
//            SubTaskInfo subTask = subTasks[i];
//            Vector3 pos = Vector3.zero;
//            if (pickRandom)
//                pos = Utility.ReturnRandom(positions);
//            else
//            {
//                if (i < positions.Length)
//                    pos = positions[i];
//                else
//                    pos = positions[positions.Length - 1];
//            }
//            newTask.CreateAndAddSubTask(unit, subTask.description, pos, subTask.actionTimer, subTask.DoTimerEndEvent, subTask.actionOnTimerStart);
//        }
//        return newTask;
//    }

//    public Task CreateTask(Unit unit, Func<Vector3> positionSetter)
//    {
//        Task newTask = new Task();
//        for (int i = 0; i < subTasks.Length; i++)
//        {
//            SubTaskInfo subTask = subTasks[i];
//            Vector3 pos = positionSetter();
//            newTask.CreateAndAddSubTask(unit, subTask.description, pos, subTask.actionTimer, subTask.DoTimerEndEvent, subTask.actionOnTimerStart);
//        }
//        return newTask;
//    }

//    [System.Serializable]
//    private class SubTaskInfo
//    {
//        public string description = "";
//        public float actionTimer = 0;

//        public void DoTimerEndEvent() => actionOnTimerEnd?.Invoke();
//    }
//}