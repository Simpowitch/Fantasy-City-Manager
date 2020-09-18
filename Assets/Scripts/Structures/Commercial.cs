using UnityEngine;
using System.Collections.Generic;

public class Commercial : Workplace
{
    [Header("Commercial")]
    List<Vector3> patreonLocations;
    List<Vector3> workPositions;

    [SerializeField] CityResource consumedOfPatreon = null;

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
    }

    public override void Despawn()
    {
        base.city.commercialBuidlings.Remove(this);
        base.Despawn();
    }

    public Task GetPatreonTask(Unit unit)
    {
        ActionTimer onTaskEnd = new ActionTimer(3f, () =>
        {
            switch (consumedOfPatreon.type)
            {
                case CityResource.Type.Gold:
                case CityResource.Type.Wood:
                case CityResource.Type.Stone:
                    break;
                case CityResource.Type.Food:
                    unit.Hunger.CurrentValue += 1;
                    break;
            }
            city.cityStats.RemoveResource(consumedOfPatreon);
        }, false);
        return new Task("Eat at the tavern", onTaskEnd, Utility.ReturnRandom(patreonLocations));
    }

    public override Task GetWorkTask(Citizen citizen)
    {
        ActionTimer onTaskEnd = new ActionTimer(3f, null, false);
        return new Task("Serving patreon", onTaskEnd, Utility.ReturnRandom(workPositions));
    }
}