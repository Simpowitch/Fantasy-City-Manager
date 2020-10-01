using System.Collections.Generic;

public class Inventory
{
    CityResource[] existingResources
    {
        get
        {
            return new CityResource[] { gold, wood, stone, iron, food };
        }
    }
    CityResource gold = new CityResource(CityResource.Type.Gold, 0);
    CityResource wood = new CityResource(CityResource.Type.Wood, 0);
    CityResource stone = new CityResource(CityResource.Type.Stone, 0);
    CityResource iron = new CityResource(CityResource.Type.Iron, 0);
    CityResource food = new CityResource(CityResource.Type.Food, 0);

    public delegate void InventoryHandler(Inventory inventory);
    public event InventoryHandler OnInventoryChanged;

    public void SendValuesToListeners()
    {
        foreach (var resource in existingResources)
        {
            resource.OnValueChanged?.Invoke(resource.Value);
        }
        OnInventoryChanged?.Invoke(this);
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
        OnInventoryChanged?.Invoke(this);
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
            OnInventoryChanged?.Invoke(this);
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
