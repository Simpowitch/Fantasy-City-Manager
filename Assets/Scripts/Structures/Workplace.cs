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
    [SerializeField] protected string workTaskThoughtHeader = "";
    [SerializeField] protected string workTaskDescription = "";


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

        List<Vector3> structurePositions = new List<Vector3>();
        foreach (var tile in ObjectTiles)
        {
            structurePositions.Add(tile.CenteredWorldPosition);
        }
        foreach (var employment in employments)
        {
            //employment.workingPositions = structurePositions;
            employment.workplace = this;
        }
        Clock.OnHourChanged += NewHour;
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

    public abstract Task GetWorkTask(Citizen citizen);
}
