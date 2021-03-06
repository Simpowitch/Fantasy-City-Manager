﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class GoodsProducer : Workplace
{
    [Header("Goods producer")]
    [SerializeField] CityResource.Type acceptedResourceType = CityResource.Type.Gold;

    public override Task GetWorkTask(Citizen citizen)
    {
        if (citizen.ResourceCarried != null && citizen.ResourceCarried.type == acceptedResourceType) // Citizen is carrying the type produced at this workplace - Create work to leave at this building
            return GetLeaveResourceTask(citizen);
        else //Create work to collect resource
        {
            List<ResourceObject> harvestableResources = city.ResourceObjectNetwork.GetHarvestable(acceptedResourceType);
            if (harvestableResources.Count > 0)
                return GetHarvestTask(citizen, harvestableResources);
            else
                return GetIdleTask(citizen);
        }
    }

    protected Task GetLeaveResourceTask(Citizen citizen)
    {
        ActionTimer leaveResource = new ActionTimer(2f, () =>
        {
            city.cityStats.Inventory.Add(citizen.ResourceCarried);
            citizen.ResourceCarried = null;
        }
            , false);
        Vector3 pos = Utility.ReturnRandom(WorkplaceTaskTiles).ObjectTile.CenteredWorldPosition;
        Vector3 dir = pos - citizen.transform.position;
        return new Task(workTaskDescription, ThoughtFileReader.GetText(citizen.UnitPersonality, workTaskThoughtHeader), leaveResource, pos, UnitAnimator.ActionAnimation.Idle);
    }

    private Task GetHarvestTask(Citizen citizen, List<ResourceObject> harvestableResources)
    {
        ResourceObject closestObject = Utility.GetClosest(harvestableResources, citizen.transform.position);
        closestObject.workOccupiedBy = citizen;
        ActionTimer collectTimer = new ActionTimer(5f, () =>
        {
            citizen.ResourceCarried = closestObject.Harvest();
        },
            false);
        Vector3 pos = closestObject.transform.position;
        Vector3 dir = pos - citizen.transform.position;
        Action onArrivalMethod = null;
        UnitAnimator.ActionAnimation taskAnimation = UnitAnimator.ActionAnimation.Idle;
        switch (acceptedResourceType)
        {
            case CityResource.Type.Gold:
            case CityResource.Type.Iron:
            case CityResource.Type.Stone:
                onArrivalMethod = () =>
                {
                    closestObject.StartHarvesting();
                    taskAnimation = UnitAnimator.ActionAnimation.Mine;
                };
                break;
            case CityResource.Type.Wood:
                onArrivalMethod = () =>
                    {
                        closestObject.StartHarvesting();
                        taskAnimation = UnitAnimator.ActionAnimation.ChopWood;
                    };
                break;
            case CityResource.Type.Food:
                Debug.LogError("Not implemented");
                break;
        }
        return new Task(workTaskDescription, ThoughtFileReader.GetText(citizen.UnitPersonality, workTaskThoughtHeader), collectTimer, pos, taskAnimation);
    }
}
