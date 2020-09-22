using System.Collections.Generic;
using UnityEngine;

public class GoodsProducer : Workplace
{
    [Header("Goods producer")]
    [SerializeField] CityResource producedByWorker = null;

    List<Vector3> collectLocations;

    protected override void Constructed(City city, bool addToCityList)
    {
        base.Constructed(city, addToCityList);

        collectLocations = new List<Vector3>();

        //Debug
        foreach (var item in ObjectTiles)
        {
            collectLocations.Add(item.CenteredWorldPosition);
        }
    }

    public override Task GetWorkTask(Citizen citizen)
    {
        ActionTimer collectTimer = new ActionTimer(5f, () => citizen.inventory.Add(producedByWorker), false);
        return new Task(workTaskDescription, ThoughtFileReader.GetText(citizen.UnitPersonality, workTaskThoughtHeader), collectTimer, Utility.ReturnRandom(collectLocations));
    }
}
