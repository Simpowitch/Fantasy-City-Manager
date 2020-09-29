using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    protected City City { get; set; }

    protected State FSM
    {
        private set;
        get;
    }

    IMoveVelocity movementSystem = null;
    [SerializeField] PathSeeker seeker = null; public PathSeeker Seeker { get => seeker; private set => seeker = value; }
    [SerializeField] protected MessageDisplay messageDisplay = null;
    [SerializeField] protected UnitDetector unitDetector = null;

    protected string unitName;
    public enum Personality
    {
        Grumpy,
        Charming,
        Arrogant,
        Patriotic
    }
    public Personality UnitPersonality { get; private set; }

    public Task currentTask;

    public Inventory inventory = new Inventory();
    public float Happiness { protected set; get; }

    private void OnEnable()
    {
        Clock.OnHourChanged += NewHour;
        seeker.OnPathUpdated += NewNodeReached;
    }

    private void OnDisable()
    {
        Clock.OnHourChanged -= NewHour;
        seeker.OnPathUpdated -= NewNodeReached;
    }

    private void Awake()
    {
        movementSystem = GetComponent<IMoveVelocity>();
    }

    private void Update()
    {
        if (FSM != null)
        {
            FSM.DuringState();
        }
    }

    protected void BaseSetup(City city)
    {
        this.City = city;
        this.seeker.City = city;
        unitName = NameGenerator.GetName();

        ChangeState(new IdleState(this));
    }

    protected virtual void NewHour(int hour) { }

    protected abstract void InfoChanged();

    protected void GoToNextNode()
    {
        if (seeker.HasPath)
        {
            PathNode nextNode = seeker.Path[0];
            movementSystem.MoveTowards(nextNode.WorldPosition);
            Debug.DrawLine(transform.position, nextNode.WorldPosition, Color.white, 1f);
        }
        else
        {
            movementSystem.SetVelocity(Vector3.zero);
        }
    }

    protected virtual void NewNodeReached(PathNode newNode)
    {
        if (newNode != null)
        {
            movementSystem.MoveTowards(newNode.WorldPosition);
            Debug.DrawLine(transform.position, newNode.WorldPosition, Color.white, 1f);
        }
        else
        {
            movementSystem.SetVelocity(Vector3.zero);
        }
    }

    protected void ChangeState(State newState)
    {
        if (FSM != null)
        {
            FSM.ExitState();
        }
        FSM = newState;
        FSM.EnterState();
    }

    public void SendThought(string thought) => StartCoroutine(messageDisplay.ShowMessage(3, thought, MessageDisplay.MessageType.Chatbubble));

    protected virtual void FindNewTask()
    {
        if (currentTask != null)
            ChangeState(new MoveState(this));
        else
            ChangeState(new IdleState(this));
    }

    public abstract class State
    {
        protected Unit unit;
        public virtual void EnterState()
        {
            unit.GoToNextNode();
            unit.InfoChanged();
        }

        public virtual void DuringState()
        {
            unit.seeker.CheckIfNodeArrived();
            if (unit.seeker.IsLost()) unit.GoToNextNode();
        }
        public virtual void ExitState() { }

        public State(Unit unit)
        {
            this.unit = unit;
        }
    }

    public class IdleState : State
    {
        public IdleState(Unit unit) : base(unit) { }

        public override void DuringState()
        {
            unit.FindNewTask();
        }
    }

    public class MoveState : State
    {
        public MoveState(Unit unit) : base(unit) { }

        public override void EnterState()
        {
            Vector3 targetPosition = unit.currentTask.Position;

            unit.Seeker.FindPathTo(targetPosition);
            base.EnterState();
        }

        public override void DuringState()
        {
            if (unit.currentTask.HasArrived(unit.transform.position))
                unit.ChangeState(new TaskState(unit));
            base.DuringState();
        }
    }

    public class TaskState : State
    {
        public TaskState(Unit unit) : base(unit) { }

        public override void EnterState()
        {
            unit.currentTask.Arrived();
            unit.SendThought(unit.currentTask.Thought);
            base.EnterState();
        }

        public override void DuringState()
        {
            if (unit.currentTask.TaskCompleted)
                unit.ChangeState(new IdleState(unit));
            base.DuringState();
        }
    }
}
