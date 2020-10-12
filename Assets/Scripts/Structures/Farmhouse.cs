using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farmhouse : GoodsProducer
{
    List<Farmland> Farmlands { get => city.farmlands; }

    public override Task GetWorkTask(Citizen citizen)
    {
        if (citizen.inventory.HasResourceType(CityResource.Type.Food)) //If citizen is carrying any food
            return GetLeaveResourceTask(citizen);

        List<Farmland> farmlandsWithHarvestableCrops = new List<Farmland>();
        farmlandsWithHarvestableCrops.PopulateListWithMatchingConditions(Farmlands, (farmland) => farmland.HarvestableFarmTiles.Count > 0);

        if (farmlandsWithHarvestableCrops.Count > 0) //If farmlands have harvestable crops
            return GetHarvestTask(citizen, farmlandsWithHarvestableCrops);

        List<Farmland> farmlandsWithEmptyTiles = new List<Farmland>();
        farmlandsWithEmptyTiles.PopulateListWithMatchingConditions(Farmlands, (farmland) => farmland.PlantableFarmTiles.Count > 0);

        if (farmlandsWithEmptyTiles.Count > 0) //If any farmlands have empty farmtiles
            return GetPlantSeedTask(citizen, farmlandsWithEmptyTiles);
        else
            return GetIdleTask(citizen);
    }

    private Task GetPlantSeedTask(Citizen citizen, List<Farmland> farmlands)
    {
        Farmland closestFarmland = Utility.GetClosest(farmlands, citizen.transform.position);
        FarmTile freeFarmTile = Utility.ReturnRandom(closestFarmland.PlantableFarmTiles);

        if (freeFarmTile != null)
        {
            freeFarmTile.plantingSeedOccupiedBy = citizen;
            ActionTimer plantSeed = new ActionTimer(2f, () =>
            {
                closestFarmland.PlantSeed(freeFarmTile);
                freeFarmTile.plantingSeedOccupiedBy = null;
            }, false);
            Vector3 pos = freeFarmTile.ObjectTile.CenteredWorldPosition;
            Vector3 dir = pos - citizen.transform.position;
            return new Task(workTaskDescription, ThoughtFileReader.GetText(citizen.UnitPersonality, workTaskThoughtHeader), plantSeed, pos, () => citizen.UnitAnimator.PlayPlantSeedAnimation(dir));
        }
        else
        {
            Debug.LogError("No task could be created, no empty farm tiles found");
            return null;
        }
    }

    private Task GetHarvestTask(Citizen citizen, List<Farmland> farmlands)
    {
        List<ResourceObject> crops = new List<ResourceObject>();

        foreach (var farmland in farmlands)
        {
            crops.AddRange(farmland.GetCropsToHarvest());
        }

        if (crops.Count > 0)
        {
            ResourceObject nearestCrop = Utility.GetClosest(crops, citizen.transform.position);
            nearestCrop.workOccupiedBy = citizen;
            ActionTimer collectTimer = new ActionTimer(5f, () =>
                {
                    CityResource cityResource = nearestCrop.Harvest();
                    citizen.inventory.Add(cityResource);
                }
            , false);

            Vector3 pos = nearestCrop.transform.position;
            Vector3 dir = pos - citizen.transform.position;
            return new Task(workTaskDescription, ThoughtFileReader.GetText(citizen.UnitPersonality, workTaskThoughtHeader), collectTimer, pos, () => citizen.UnitAnimator.PlayHarvestFoodAnimation(dir));
        }
        else
        {
            Debug.LogError("No task could be created, no crops found");
            return null;
        }
    }

    private Task GetIdleTask(Citizen citizen)
    {
        ActionTimer collectTimer = new ActionTimer(2f, null, false);
        return new Task("Idle", "I have nothing to do!", collectTimer, Utility.ReturnRandom(workTaskTiles).position);
    }
}

