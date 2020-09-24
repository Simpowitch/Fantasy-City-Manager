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
            switch (producedByWorker.type)
            {
                case CityResource.Type.Wood:
                    List<Tree> trees = city.TreeNetwork.GetHarvestableTrees();
                    if (trees.Count <= 0)
                        return GetIdleTask(citizen);
                    else
                    {
                        Tree closestTree = Utility.GetClosest(trees, citizen.transform.position);
                        closestTree.workOccupiedBy = citizen;
                        collectTimer = new ActionTimer(5f, () =>
                        {
                            citizen.inventory.Add(producedByWorker);
                            closestTree.Despawn();
                        },
                            false);
                        return new Task(workTaskDescription, ThoughtFileReader.GetText(citizen.UnitPersonality, workTaskThoughtHeader), collectTimer, closestTree.transform.position);
                    }
                case CityResource.Type.Gold: //REPLACE WITH LOCATIONS TO MINE
                case CityResource.Type.Stone: //REPLACE WITH LOCATIONS TO MINE
                case CityResource.Type.Food: //REPLACE WITH LOCATIONS TO FARM
                    return new Task(workTaskDescription, ThoughtFileReader.GetText(citizen.UnitPersonality, workTaskThoughtHeader), collectTimer, Utility.ReturnRandom(workTaskTiles).position);
                default:
                    Debug.LogError("Type of resource not found");
                    return null;
            }

        }
    }

    private Task GetIdleTask(Citizen citizen)
    {
        ActionTimer collectTimer = new ActionTimer(2f, null, false);
        return new Task("Idle", "I have nothing to do!", collectTimer, Utility.ReturnRandom(workTaskTiles).position);
    }
}
