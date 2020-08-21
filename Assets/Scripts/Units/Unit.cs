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

    public Need Mood { private set; get; }
    public Need Food { private set; get; }
    public Need Recreation { private set; get; }

    int foodMoodModifier;
    int recreationMoodModifier;


    string unitName;
    public string UnitName
    {
        get => unitName;
        set
        {
            unitName = value;
        }
    }

    private void OnEnable()
    {
        DayNightSystem.OnPartOfTheDayChanged += PartOfDayChange;
        Clock.OnHourChanged += NewHour;
        seeker.OnPathUpdated += NewNodeReached;
    }

    private void OnDisable()
    {
        DayNightSystem.OnPartOfTheDayChanged -= PartOfDayChange;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Structure building = collision.GetComponent<Structure>();
        if (building) VisitingBuilding(building);
    }

    protected void BaseSetup(City city)
    {
        this.City = city;
        this.seeker.City = city;
        UnitName = NameGenerator.GetName();

        //Needs
        Mood = new Need("Mood", 0, 0.1f, 0.25f, 0.75f, 0.5f);
        Mood.OnStateChanged += HandleMoodState;
        Food = new Need("Food", 0.1f, 0, 0.25f, 0.75f, 1);
        Food.OnStateChanged += HandleFoodState;
        Recreation = new Need("Recreation", 0.05f, 0, 0.25f, 0.75f, 1);
        Recreation.OnStateChanged += HandleRecreationState;
    }


    public abstract void VisitingBuilding(Structure building);

    protected abstract void PartOfDayChange(DayNightSystem.PartOfTheDay partOfDay);

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

    protected virtual void HandleMoodState(Need.State newState)
    {

    }
    protected virtual void HandleFoodState(Need.State newState)
    {
        switch (newState)
        {
            case Need.State.Critical:
                foodMoodModifier = -100;
                break;
            case Need.State.Low:
                foodMoodModifier = -25;
                break;
            case Need.State.Normal:
                foodMoodModifier = 0;
                break;
            case Need.State.High:
                foodMoodModifier = 10;
                break;
        }
        CalculateMood();
    }
    protected virtual void HandleRecreationState(Need.State newState)
    {
        switch (newState)
        {
            case Need.State.Critical:
                recreationMoodModifier = -50;
                break;
            case Need.State.Low:
                foodMoodModifier = -20;
                break;
            case Need.State.Normal:
                foodMoodModifier = 0;
                break;
            case Need.State.High:
                foodMoodModifier = 20;
                break;
        }
        CalculateMood();
    }

    //-100 will be 0, +100 will be 1
    private void CalculateMood()
    {
        int lowBase = 100;
        int sum = foodMoodModifier + recreationMoodModifier + lowBase;
        float factor = sum / 100f;
        factor /= 2;
        Mood.CurrentValue = factor;
    }

    public abstract class State
    {
        protected Unit unit;
        public virtual void EnterState()
        {
            unit.GoToNextNode();
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
}
