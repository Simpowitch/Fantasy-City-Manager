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
    [SerializeField] Rotation rotation = null;
    [SerializeField] PathSeeker seeker = null; public PathSeeker Seeker { get => seeker; private set => seeker = value; }
    [SerializeField] protected MessageDisplay messageDisplay = null;
    [SerializeField] protected UnitDetector unitDetector = null;

    string unitName;
    public string UnitName
    {
        get => unitName;
        set
        {
            unitName = value;
            OnUnitInfoChanged?.Invoke(this);
        }
    }

    public delegate void InfoChangeHandler(Unit unitChanged);
    public event InfoChangeHandler OnUnitInfoChanged;
    public float Happiness { private set; get; }
    //Needs
    public Need Social { private set; get; }
    public Need Hunger { private set; get; }
    public Need Energy { private set; get; }
    public Need Recreation { private set; get; }

    public Task currentTask;
    //public Task.SubTask currentSubTask;

    public Inventory inventory = new Inventory();

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
        UnitName = NameGenerator.GetName();

        //Needs
        Energy = new Need("Energy", 0.05f, 1);
        Energy.OnNeedValuesChanged += CalculateHappiness;
        Hunger = new Need("Hunger", 0.05f, 1);
        Hunger.OnNeedValuesChanged += CalculateHappiness;
        Recreation = new Need("Recreation", 0.05f, 1);
        Recreation.OnNeedValuesChanged += CalculateHappiness;
        Social = new Need("Social", 0.05f, 1);
        Social.OnNeedValuesChanged += CalculateHappiness;

        ChangeState(new IdleState(this));
    }

    protected virtual void NewHour(int hour) { }

    protected void GoToNextNode()
    {
        if (seeker.HasPath)
        {
            PathNode nextNode = seeker.Path[0];
            rotation.RotateTowards(nextNode.WorldPosition);
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
            rotation.RotateTowards(newNode.WorldPosition);
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

    public abstract string GetProfession();

    public void SendToViewer(UnitViewer unitViewer) => unitViewer.ShowUnit(this);

    protected void CalculateHappiness()
    {
        float average = 0;
        average += Energy.CurrentValue;
        average += Hunger.CurrentValue;
        average += Recreation.CurrentValue;
        average += Social.CurrentValue;
        average /= 4;
        Happiness = average;
        OnUnitInfoChanged?.Invoke(this);
    }

    //public virtual void FindNewSubTask()
    //{
    //    currentSubTask = currentTask.GetNextSubTask();
    //    if (currentSubTask == null)
    //        FindNewTask();
    //    else
    //        ChangeState(new MoveState(this));
    //}

    protected virtual void FindNewTask()
    {
        if (currentTask != null)
            ChangeState(new MoveState(this));
        else
            ChangeState(new IdleState(this));
    }
    //{
    //    ////currentSubTask = currentTask.GetNextSubTask();
    //    ////if (currentSubTask != null)
    //    ////    ChangeState(new MoveState(this));
    //    ////else
    //    ////    ChangeState(new IdleState(this));
    //}

    protected void FindNeedFullfillTask()
    {
        //Debug
        currentTask = CreateHungerTask();

        //Find method to chose lowest or random low need
    }

    protected abstract Task CreateEnergyTask();
    private Task CreateHungerTask()
    {
        Commercial foodSource = Utility.ReturnRandom(City.taverns); //EXCHANGE FOR FOOD SOURCES
        return foodSource.GetPatreonTask(this);

        //Task newTask = new Task();
        //newTask.CreateAndAddSubTask(this, "Eating at the tavern", Utility.ReturnRandom(City.taverns).GetRandomLocation(), 5f, null);
        //return newTask;
    }
    private Task CreateRecreationTask()
    {
        throw new System.NotImplementedException();
    }
    private Task CreateSocialTask()
    {
        throw new System.NotImplementedException();
    }

    public abstract class State
    {
        protected Unit unit;
        public virtual void EnterState()
        {
            unit.GoToNextNode();
            unit.OnUnitInfoChanged?.Invoke(unit);
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
