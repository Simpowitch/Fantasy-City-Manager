using System.Collections.Generic;
using UnityEngine;

public abstract class Workplace : Structure
{
    [Header("Workplace - Time")]
    public int startShiftHour = 8;
    public int endShiftHour = 18;
    public bool ShiftActive { get; private set; }

    [Header("Workplace - Employments")]
    [SerializeField] List<Employment> employments = new List<Employment>();

    [Header("Patreon Task setup")]
    [SerializeField] protected string workTaskThoughtHeader = "";
    [SerializeField] protected string workTaskDescription = "";
    [SerializeField] Transform workTaskTileParent = null;
    protected WorkplaceTaskTile[] WorkplaceTaskTiles { get; private set; } = null;


    public List<Employment> Employments { get => employments; private set => employments = value; }
    public List<Citizen> CitizensEmployed
    {
        get
        {
            List<Citizen> citizens = new List<Citizen>();
            foreach (var employment in Employments)
            {
                if (employment.PositionFilled)
                    citizens.Add(employment.employee);
            }
            return citizens;
        }
    }
    public List<Employment> UnfilledPositions
    {
        get
        {
            List<Employment> free = new List<Employment>();
            free.PopulateListWithMatchingConditions(Employments, (employment) => employment.PositionFilled == false);
            return free;
        }
    }
    public List<Citizen> EmployeesAtSite { get; private set; } = new List<Citizen>();
    public int NumberOfUnfilledPositions { get => UnfilledPositions.Count; }


    protected override void Constructed(City city, bool addToCityList)
    {
        base.Constructed(city, addToCityList);
        if (addToCityList)
            city.workplaces.Add(this);

        foreach (var employment in employments)
        {
            employment.workplace = this;
        }
        Clock.OnHourChanged += NewHour;

        //Set up work task tiles
        WorkplaceTaskTiles = new WorkplaceTaskTile[workTaskTileParent != null ? workTaskTileParent.childCount : 0];
        for (int i = 0; i < WorkplaceTaskTiles.Length; i++)
        {
            WorkplaceTaskTiles[i] = new WorkplaceTaskTile(city.ObjectGrid.GetGridObject(workTaskTileParent.GetChild(i).position), workTaskTileParent.GetChild(i));
        }
    }

    protected override void UnitVisiting(Unit unitVisiting)
    {
        base.UnitVisiting(unitVisiting);
        if (CitizensEmployed.Contains(unitVisiting as Citizen))
        {
            EmployeesAtSite.Add(unitVisiting as Citizen);
        }
    }

    private void NewHour(int hour) => ShiftActive = Clock.IsTimeBetween(startShiftHour, endShiftHour, hour);

    public bool CanEmploy() => UnfilledPositions.Count > 0;
    public bool IsFunctional() => Employments.Count > 0;

    public override void Despawn()
    {
        base.city.workplaces.Remove(this);
        Clock.OnHourChanged -= NewHour;
        base.Despawn();
    }

    public virtual Task GetWorkTask(Citizen citizen)
    {
        WorkplaceTaskTile freeTile = Utility.ReturnRandomElementWithCondition(WorkplaceTaskTiles, (tile) => !tile.Occupied);
        if (freeTile != null)
        {
            freeTile.Occupied = true;
        ActionTimer onTaskEnd = new ActionTimer(3f, () =>
        {
            freeTile.Occupied = false;
        }, false);
        return new Task(workTaskDescription, ThoughtFileReader.GetText(citizen.UnitPersonality, workTaskThoughtHeader), onTaskEnd, freeTile.ObjectTile.CenteredWorldPosition, UnitAnimator.ActionAnimation.Idle);
        }
        else
        {
            Debug.LogError("No free worktiles, is the number of workers higher than the available tiles?");
            return null;
        }
    }

    protected Task GetIdleTask(Citizen citizen)
    {
        ActionTimer collectTimer = new ActionTimer(2f, null, false);

        WorkplaceTaskTile freeTile = Utility.ReturnRandomElementWithCondition(WorkplaceTaskTiles, (tile) => !tile.Occupied);
        if (freeTile != null)
        {
            freeTile.Occupied = true;
            return new Task("Idle", "I have nothing to do!", new ActionTimer(1f, () =>
            {
                freeTile.Occupied = false;
            }, false), freeTile.ObjectTile.CenteredWorldPosition, UnitAnimator.ActionAnimation.Idle);
        }
        else
        {
            return Task.CreateIdleTask("Idle", "I have nothing to do!", citizen.transform.position);
        }
    }

    protected class WorkplaceTaskTile
    {
        public ObjectTile ObjectTile { get; private set; }
        public bool Occupied { get; set; }
        public Direction2D ForwardDirection { get; private set; }

        public WorkplaceTaskTile(ObjectTile objectTile, Transform transform)
        {
            ObjectTile = objectTile;
            ForwardDirection = Utility.GetDirection(transform);
        }
    }
}
