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
    [SerializeField] UnitAnimator unitAnimator = null; public UnitAnimator UnitAnimator { get => unitAnimator; }
    [SerializeField] PathSeeker seeker = null;
    [SerializeField] protected UnitDetector unitDetector = null;
    [SerializeField] UnitCanvasController canvasController = null;
    private string unitName;
    protected string UnitName
    {
        get => unitName;
        set
        {
            unitName = value;
            canvasController.ShowNameTag(value);
        }
    }
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
        seeker.OnPathUpdated += NextNodeChanged;
    }

    private void OnDisable()
    {
        Clock.OnHourChanged -= NewHour;
        seeker.OnPathUpdated -= NextNodeChanged;
    }

    private void Awake()
    {
        movementSystem = GetComponent<IMoveVelocity>();
    }

    int updateInterval = 5;
    private void Update()
    {
        if (FSM != null && Time.frameCount % updateInterval == 0)
        {
            FSM.DuringState();
        }
    }

    protected void BaseSetup(City city)
    {
        this.City = city;
        this.seeker.City = city;
        UnitName = NameGenerator.GetName();

        ChangeState(new IdleState(this));
    }

    protected virtual void NewHour(int hour) { }

    protected abstract void InfoChanged();

    protected void GoToCurrentNode()
    {
        if (seeker.HasPath)
            NextNodeChanged(seeker.Path[0]);
        else
            NextNodeChanged(null);
    }

    protected virtual void NextNodeChanged(PathNode nextNode)
    {
        if (nextNode != null)
        {
            movementSystem.MoveTowards(nextNode.WorldPosition);
            unitAnimator.PlayWalkAnimation(nextNode.WorldPosition - transform.position);
            Debug.DrawLine(transform.position, nextNode.WorldPosition, Color.white, 1f);
        }
        else
        {
            movementSystem.SetVelocity(Vector3.zero);
            unitAnimator.PlayIdleAnimation(Vector3.zero);
            if (currentTask != null && currentTask.HasArrived(transform.position))
                ChangeState(new TaskState(this));
            else
                ChangeState(new IdleState(this));
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

    public void SendThought(string thought) => canvasController.ShowChatbubble(3, thought);

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
            unit.InfoChanged();
        }

        public virtual void DuringState()
        {
            unit.seeker.CheckIfNodeArrived();
            if (unit.seeker.IsLost()) unit.GoToCurrentNode();
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

            unit.seeker.FindPathTo(targetPosition);
            base.EnterState();
        }
    }

    public class TaskState : State
    {
        public TaskState(Unit unit) : base(unit) { }

        public override void EnterState()
        {
            unit.canvasController.ShowProgressbar(true);
            unit.currentTask.Arrived();
            unit.SendThought(unit.currentTask.Thought);
            base.EnterState();
        }

        public override void DuringState()
        {
            if (unit.currentTask.TaskCompleted)
                unit.ChangeState(new IdleState(unit));
            else
                unit.canvasController.UpdateProgressbar(unit.currentTask.ActionTimer.Progress); //Show progressbar of task
            base.DuringState();
        }

        public override void ExitState()
        {
            unit.canvasController.ShowProgressbar(false);
            base.ExitState();
        }
    }
}
