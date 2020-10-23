using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class CityResourceGroup
{
    //CityResource[] existingResources
    //{
    //    get
    //    {
    //        return new CityResource[] { gold, wood, stone, iron, food };
    //    }
    //}
    [SerializeField] CityResource gold = new CityResource(CityResource.Type.Gold, 0);
    [SerializeField] CityResource wood = new CityResource(CityResource.Type.Wood, 0);
    [SerializeField] CityResource stone = new CityResource(CityResource.Type.Stone, 0);
    [SerializeField] CityResource iron = new CityResource(CityResource.Type.Iron, 0);
    [SerializeField] CityResource food = new CityResource(CityResource.Type.Food, 0);

    public delegate void GroupChangeHandler(CityResourceGroup cityResourceGroup);
    public event GroupChangeHandler OnValuesChanged;

    //public void SendValuesToListeners()
    //{
    //    foreach (var resource in existingResources)
    //    {
    //        resource.OnValueChanged?.Invoke(resource.Value);
    //    }
    //    OnValuesChanged?.Invoke(this);
    //}

    public static CityResourceGroup CombineResourceGroups(CityResourceGroup[] groups)
    {
        CityResourceGroup combinedGroup = new CityResourceGroup();
        foreach (var cityResourceGroup in groups)
        {
            combinedGroup.Add(cityResourceGroup.gold);
            combinedGroup.Add(cityResourceGroup.wood);
            combinedGroup.Add(cityResourceGroup.stone);
            combinedGroup.Add(cityResourceGroup.iron);
            combinedGroup.Add(cityResourceGroup.food);
        }
        return combinedGroup;
    }

    public static CityResourceGroup CombineIncomes<T1>(IList<T1> groups) where T1 : IHasIncome
    {
        CityResourceGroup combinedGroup = new CityResourceGroup();
        foreach (var group in groups)
        {
            CityResourceGroup iterationGroup = group.Income;
            combinedGroup.Add(iterationGroup.gold);
            combinedGroup.Add(iterationGroup.wood);
            combinedGroup.Add(iterationGroup.stone);
            combinedGroup.Add(iterationGroup.iron);
            combinedGroup.Add(iterationGroup.food);
        }
        return combinedGroup;
    }

    public static CityResourceGroup CombineUpkeeps<T1>(IList<T1> groups) where T1 : IHasUpkeep
    {
        CityResourceGroup combinedGroup = new CityResourceGroup();
        foreach (var group in groups)
        {
            CityResourceGroup iterationGroup = group.Upkeep;
            combinedGroup.Add(iterationGroup.gold);
            combinedGroup.Add(iterationGroup.wood);
            combinedGroup.Add(iterationGroup.stone);
            combinedGroup.Add(iterationGroup.iron);
            combinedGroup.Add(iterationGroup.food);
        }
        return combinedGroup;
    }

    public void Add(CityResource resourceToAdd)
    {
        switch (resourceToAdd.type)
        {
            case CityResource.Type.Gold:
                gold.Value += resourceToAdd.Value;
                break;
            case CityResource.Type.Wood:
                wood.Value += resourceToAdd.Value;
                break;
            case CityResource.Type.Stone:
                stone.Value += resourceToAdd.Value;
                break;
            case CityResource.Type.Iron:
                iron.Value += resourceToAdd.Value;
                break;
            case CityResource.Type.Food:
                food.Value += resourceToAdd.Value;
                break;
        }
        OnValuesChanged?.Invoke(this);
    }

    public void Add(CityResourceGroup otherGroup)
    {
        gold.Value += otherGroup.gold.Value;
        wood.Value += otherGroup.wood.Value;
        stone.Value += otherGroup.stone.Value;
        iron.Value += otherGroup.iron.Value;
        food.Value += otherGroup.food.Value;
        OnValuesChanged?.Invoke(this);
    }

    public void Remove(CityResourceGroup otherGroup)
    {
        gold.Value -= otherGroup.gold.Value;
        wood.Value -= otherGroup.wood.Value;
        stone.Value -= otherGroup.stone.Value;
        iron.Value -= otherGroup.iron.Value;
        food.Value -= otherGroup.food.Value;
        OnValuesChanged?.Invoke(this);
    }

    public bool TryToRemove(List<CityResource> resourcesToRemove)
    {
        foreach (var askedFor in resourcesToRemove)
        {
            if (HasResourceWithValue(askedFor, out CityResource foundResource))
                continue;
            else
                return false;
        }
        foreach (var askedFor in resourcesToRemove)
        {
            TryToRemove(askedFor);
        }
        return true;
    }

    public bool TryToRemove(CityResource resourceToRemove)
    {
        if (HasResourceWithValue(resourceToRemove, out CityResource foundResource))
        {
            foundResource.Value -= resourceToRemove.Value;
            OnValuesChanged?.Invoke(this);
            return true;
        }
        else
            return false;
    }

    public void RemoveAllOfType(CityResource.Type type, out CityResource resourcesRemoved)
    {
        CityResource foundResource = GetCityResourceOfType(type);
        resourcesRemoved = new CityResource(foundResource.type, foundResource.Value);
        TryToRemove(foundResource);
    }

    public CityResource GetCityResourceOfType(CityResource.Type askedFor)
    {
        switch (askedFor)
        {
            case CityResource.Type.Gold:
                return gold;
            case CityResource.Type.Wood:
                return wood;
            case CityResource.Type.Stone:
                return stone;
            case CityResource.Type.Iron:
                return iron;
            case CityResource.Type.Food:
                return food;
            default:
                return null;
        }
    }

    public bool HasResourceType(CityResource.Type type)
    {
        CityResource cityResource = GetCityResourceOfType(type);
        return cityResource != null && cityResource.Value > 0;
    }

    public bool HasResourceWithValue(CityResource askedFor, out CityResource foundResource)
    {
        foundResource = GetCityResourceOfType(askedFor.type);
        if (foundResource != null)
        {
            return foundResource.Value >= askedFor.Value;
        }
        return false;
    }
}
