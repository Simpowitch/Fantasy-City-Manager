using System.Collections.Generic;
using UnityEngine;

public class GoodsProducer : Workplace
{
    [Header("Goods producer")]
    [SerializeField] CityResource producedByWorker = null;

    public override Task GetWorkTask(Citizen citizen)
    {
        if (citizen.inventory.HasResourceType(producedByWorker.type)) // Citizen is carrying the type produced at this workplace - Create work to leave at this building
        {
            ActionTimer leaveTimer = new ActionTimer(5f, () =>
            {
                citizen.inventory.RemoveAllOfType(producedByWorker.type, out CityResource resourcesRemoved);
                city.cityStats.AddResource(resourcesRemoved);
            }
            , false);
            return new Task(workTaskDescription, ThoughtFileReader.GetText(citizen.UnitPersonality, workTaskThoughtHeader), leaveTimer, Utility.ReturnRandom(workTaskTiles).position);
        }
        else //Create work to collect resource
        {
            ActionTimer collectTimer = new ActionTimer(5f, () => citizen.inventory.Add(producedByWorker), false);
            List<ResourceObject> harvestableResources = city.ResourceObjectNetwork.GetHarvestableObjects(producedByWorker.type);
            if (harvestableResources.Count > 0)
            {
                ResourceObject closestObject = Utility.GetClosest(harvestableResources, citizen.transform.position);
                closestObject.workOccupiedBy = citizen;
                collectTimer = new ActionTimer(5f, () =>
                {
                    citizen.inventory.Add(producedByWorker);
                    closestObject.Despawn();
                },
                    false);
                return new Task(workTaskDescription, ThoughtFileReader.GetText(citizen.UnitPersonality, workTaskThoughtHeader), collectTimer, closestObject.transform.position);
            }
            else
                return GetIdleTask(citizen);
        }
    }

    private Task GetIdleTask(Citizen citizen)
    {
        ActionTimer collectTimer = new ActionTimer(2f, null, false);
        return new Task("Idle", "I have nothing to do!", collectTimer, Utility.ReturnRandom(workTaskTiles).position);
    }
}
