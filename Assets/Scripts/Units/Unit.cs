using System.Collections.Generic;
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
    public enum Personality
    {
        Grumpy,
        Charming,
        Arrogant,
        Patriotic
    }
    public Personality UnitPersonality { get; private set; }

    public delegate void InfoChangeHandler(Unit unitChanged);
    public event InfoChangeHandler OnUnitInfoChanged;
    public float Happiness { private set; get; }
    //Needs
    public Need Hunger { private set; get; }
    public Need Energy { private set; get; }
    public Need Recreation { private set; get; }
    public Need Social { private set; get; }

    public Need[] Needs
    {
        get => new Need[] { Hunger, Energy, Recreation, Social };
    }

    public Task currentTask;

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
        Energy = new Need(Need.NeedType.Energy, 0.05f, 1);
        Energy.OnNeedValuesChanged += CalculateHappiness;
        Hunger = new Need(Need.NeedType.Hunger, 0.05f, 1);
        Hunger.OnNeedValuesChanged += CalculateHappiness;
        Recreation = new Need(Need.NeedType.Recreation, 0.05f, 1);
        Recreation.OnNeedValuesChanged += CalculateHappiness;
        Social = new Need(Need.NeedType.Social, 0.05f, 1);
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

    public void SendThought(string thought) => StartCoroutine(messageDisplay.ShowMessage(3, thought, MessageDisplay.MessageType.Chatbubble));

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

    protected virtual void FindNewTask()
    {
        if (currentTask != null)
            ChangeState(new MoveState(this));
        else
            ChangeState(new IdleState(this));
    }


    protected void FindNeedFullfillTask()
    {
        List<Need> lowNeeds = new List<Need>();
        List<Need> mediumNeeds = new List<Need>();

        foreach (Need need in Needs)
        {
            if (need.State == Need.NeedState.Low)
                lowNeeds.Add(need);
            else if (need.State == Need.NeedState.Medium)
                mediumNeeds.Add(need);
        }

        Need chosenNeed;
        if (lowNeeds.Count > 0)
            chosenNeed = Utility.ReturnRandom(lowNeeds);
        else if (mediumNeeds.Count > 0)
            chosenNeed = Utility.ReturnRandom(mediumNeeds);
        else
            chosenNeed = Utility.ReturnRandom(Needs);

        currentTask = GetTask(chosenNeed);
    }


    private Task GetTask(Need need)
    {
        switch (need.Type)
        {
            case Need.NeedType.Energy:
                return CreateEnergyTask();
            case Need.NeedType.Hunger:
                return CreateHungerTask();
            case Need.NeedType.Recreation:
                return CreateRecreationTask();
            case Need.NeedType.Social:
                return CreateSocialTask();
            default:
                Debug.LogError("Need not defined");
                return null;
        }
    }

    protected abstract Task CreateEnergyTask();
    private Task CreateHungerTask()
    {
        INeedProvider source = Utility.ReturnRandom(City.HungerProviders);
        return source.CreateSatisfyNeedTask(this, Need.NeedType.Hunger);
    }
    private Task CreateRecreationTask()
    {
        INeedProvider source = Utility.ReturnRandom(City.RecreationProviders);
        return source.CreateSatisfyNeedTask(this, Need.NeedType.Recreation);
    }
    private Task CreateSocialTask()
    {
        INeedProvider source = Utility.ReturnRandom(City.SocialProviders);
        return source.CreateSatisfyNeedTask(this, Need.NeedType.Social);
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
