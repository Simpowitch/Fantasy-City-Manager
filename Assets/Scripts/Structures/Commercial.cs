using UnityEngine;
using System.Collections.Generic;

public class Commercial : Workplace, INeedProvider
{
    [Header("Commercial")]
    [SerializeField] CityResource consumedOfPatreon = null;
    [SerializeField] List<Need.NeedType> providedNeeds = null;

    [Header("Patreon Task setup")]
    [SerializeField] protected string patreonTaskThoughtHeader = "";
    [SerializeField] protected string patreonTaskDescription = "";
    [SerializeField] protected Transform[] patreonTaskTiles = null;

    public List<Need.NeedType> NeedTypes => providedNeeds;

    protected override void Constructed(City city, bool addToCityList)
    {
        base.Constructed(city, addToCityList);
        if (addToCityList)
            city.commercialBuidlings.Add(this);

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
        return new Task(workTaskDescription, ThoughtFileReader.GetText(citizen.UnitPersonality, workTaskThoughtHeader), onTaskEnd, Utility.ReturnRandom(workTaskTiles).position);
    }

    public Task CreateSatisfyNeedTask(Unit unit, Need needToSatisfy)
    {
        if (!NeedTypes.Contains(needToSatisfy.Type))
        {
            Debug.LogError("Need asked for cannot be provided by this building");
            return null;
        }

        ActionTimer onTaskEnd = new ActionTimer(3f, () =>
        {
            needToSatisfy.Satisfy();
            city.cityStats.Inventory.TryToRemove(consumedOfPatreon);
        }, false);
        return new Task(patreonTaskDescription, ThoughtFileReader.GetText(unit.UnitPersonality, patreonTaskThoughtHeader), onTaskEnd, Utility.ReturnRandom(patreonTaskTiles).position);
    }
}