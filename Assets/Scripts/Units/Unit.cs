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

    public delegate void InfoChangeHandler(Unit unitChanged);
    public event InfoChangeHandler OnUnitInfoChanged;
    public float Happiness { private set; get; }
    //Needs
    public Need Social { private set; get; }
    public Need Hunger { private set; get; }
    public Need Energy { private set; get; }
    public Need Recreation { private set; get; }


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
    }

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
