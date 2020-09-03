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
        }
    }

    //Needs
    public delegate void MoodHandler(Unit unitChanged);
    public event MoodHandler MoodRecalculated;
    public Mood Mood { protected set; get; }
    public Need Health { protected set; get; }
    public Need Food { protected set; get; }
    public Need Employment { protected set; get; }
    public Need Recreation { protected set; get; }
    public Need Faith { protected set; get; }
    public Need Hygiene { protected set; get; }


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
        if (building) 
            VisitingBuilding(building);
    }

    protected void BaseSetup(City city)
    {
        this.City = city;
        this.seeker.City = city;
        UnitName = NameGenerator.GetName();
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

    public void SendToViewer(UnitViewer unitViewer) => unitViewer.ShowUnit(this);

    protected void CalculateMoodBuffs()
    {
        List<MoodBuff> newBuffs = new List<MoodBuff>();

        if (Health != null && Health.ActiveMoodBuff != null)
            newBuffs.Add(Health.ActiveMoodBuff);
        if (Food != null && Food.ActiveMoodBuff != null)
            newBuffs.Add(Food.ActiveMoodBuff);
        if (Employment != null && Employment.ActiveMoodBuff != null)
            newBuffs.Add(Employment.ActiveMoodBuff);
        if (Recreation != null && Recreation.ActiveMoodBuff != null)
            newBuffs.Add(Recreation.ActiveMoodBuff);
        if (Faith != null && Faith.ActiveMoodBuff != null)
            newBuffs.Add(Faith.ActiveMoodBuff);
        if (Hygiene != null && Hygiene.ActiveMoodBuff != null)
            newBuffs.Add(Hygiene.ActiveMoodBuff);

        Mood.SetMoodBuffs(newBuffs);

        MoodRecalculated?.Invoke(this);
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
