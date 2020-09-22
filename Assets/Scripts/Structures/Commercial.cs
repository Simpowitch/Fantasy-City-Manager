using UnityEngine;
using System.Collections.Generic;

public class Commercial : Workplace, INeedProvider
{
    [Header("Commercial")]
    List<Vector3> patreonLocations;
    List<Vector3> workPositions;
    [SerializeField] protected string patreonTaskThoughtHeader = "";
    [SerializeField] protected string patreonTaskDescription = "";
    [SerializeField] CityResource consumedOfPatreon = null;

    [SerializeField] List<Need.NeedType> providedNeeds = null;
    public List<Need.NeedType> NeedTypes => providedNeeds;

    protected override void Constructed(City city, bool addToCityList)
    {
        base.Constructed(city, addToCityList);
        if (addToCityList)
            city.commercialBuidlings.Add(this);

        patreonLocations = new List<Vector3>();
        workPositions = new List<Vector3>();

        //Debug
        foreach (var item in ObjectTiles)
        {
            patreonLocations.Add(item.CenteredWorldPosition);
            workPositions.Add(item.CenteredWorldPosition);
        }

        foreach (var needType in NeedTypes)
        {
            switch (needType)
            {
                case Need.NeedType.Energy:
                    city.EnergyProviders.Add(this);
                    break;
                case Need.NeedType.Hunger:
                    city.HungerProviders.Add(this);
                    break;
                case Need.NeedType.Recreation:
                    city.RecreationProviders.Add(this);
                    break;
                case Need.NeedType.Social:
                    city.SocialProviders.Add(this);
                    break;
            }
        }
    }

    public override void Despawn()
    {
        base.city.commercialBuidlings.Remove(this);
        base.Despawn();
    }

    public override Task GetWorkTask(Citizen citizen)
    {
        ActionTimer onTaskEnd = new ActionTimer(3f, null, false);
        return new Task(workTaskDescription, ThoughtFileReader.GetText(citizen.UnitPersonality, workTaskThoughtHeader), onTaskEnd, Utility.ReturnRandom(workPositions));
    }

    public Task CreateSatisfyNeedTask(Unit unit, Need.NeedType type)
    {
        if (!NeedTypes.Contains(type))
        {
            Debug.LogError("Need asked for cannot be provided by this building");
            return null;
        }

        ActionTimer onTaskEnd = new ActionTimer(3f, () =>
        {
            switch (type)
            {
                case Need.NeedType.Energy:
                    unit.Energy.Satisfy();
                    break;
                case Need.NeedType.Hunger:
                    unit.Hunger.Satisfy();
                    break;
                case Need.NeedType.Recreation:
                    unit.Recreation.Satisfy();
                    break;
                case Need.NeedType.Social:
                    unit.Social.Satisfy();
                    break;
            }
            city.cityStats.RemoveResource(consumedOfPatreon);
        }, false);
        return new Task(patreonTaskDescription, ThoughtFileReader.GetText(unit.UnitPersonality, patreonTaskThoughtHeader), onTaskEnd, Utility.ReturnRandom(patreonLocations));
    }
}