using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Residence : Structure, INeedProvider
{
    [Header("Residence")]

    public int maxResidents = 2;
    public List<Citizen> Residents { get; private set; } = new List<Citizen>();
    public int NumberOfUnfilledResidenceSpots { get => maxResidents - Residents.Count; }

    public List<Need.NeedType> NeedTypes
    {
        get
        {
            List<Need.NeedType> types = new List<Need.NeedType>();
            types.Add(Need.NeedType.Energy);
            return types;
        }
    }

    protected override void Constructed(City city, bool addToCityList)
    {
        base.Constructed(city, addToCityList);
        if (addToCityList)
            city.residentialBuildings.Add(this);

    }
    public bool IsFunctional() => Residents.Count > 0;

    public bool CanMoveIn() => Residents.Count < maxResidents;
    public void MoveIn(Citizen citizen) => Residents.Add(citizen);
    public void MoveOut(Citizen citizen) => Residents.Remove(citizen);

    public override void Despawn()
    {
        base.city.residentialBuildings.Remove(this);
        foreach (var resident in Residents)
        {
            resident.LeaveHome();
        }
        base.Despawn();
    }

    public Task CreateSatisfyNeedTask(Unit unit, Need.NeedType type)
    {
        if (!NeedTypes.Contains(type))
        {
            Debug.LogError("Need asked for cannot be provided by this building");
            return null;
        }
        return CreateSleepTask(unit);
    }

    private Task CreateSleepTask(Unit unit)
    {
        ActionTimer onTaskEnd = new ActionTimer(3f, () =>
        {
            unit.Energy.Satisfy();
        }, false);
        return new Task("Going to bed", ThoughtFileReader.GetText(unit.UnitPersonality, "Sleeping"), onTaskEnd, Utility.ReturnRandom(ObjectTiles).CenteredWorldPosition);
    }
}
