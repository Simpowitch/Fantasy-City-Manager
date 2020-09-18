using System.Collections.Generic;

public class CityStats
{
    public delegate void StatsHandler(int newStat);
    public event StatsHandler OnPopulationChanged;
    public event StatsHandler OnVisitorsChanged;

    public CityResource Gold { get; private set; }
    public CityResource Wood { get; private set; }
    public CityResource Stone { get; private set; }
    public CityResource Food { get; private set; }

    int population;
    public int Population
    {
        get => population;
        set
        {
            population = value;
            OnPopulationChanged?.Invoke(value);
        }
    }

    int visitors;
    public int Visitors
    {
        get => visitors;
        set
        {
            visitors = value;
            OnVisitorsChanged?.Invoke(value);
        }
    }

    public void Setup(int startGold, int startWood, int startStone, int startFood)
    {
        OnPopulationChanged?.Invoke(population);
        OnVisitorsChanged?.Invoke(visitors);

        Gold = new CityResource(CityResource.Type.Gold, startGold);
        Wood = new CityResource(CityResource.Type.Wood, startWood);
        Stone = new CityResource(CityResource.Type.Stone, startStone);
        Food = new CityResource(CityResource.Type.Food, startFood);
    }

    public void SendValuesToListeners()
    {
        Gold.OnValueChanged?.Invoke(Gold.Value);
        Wood.OnValueChanged?.Invoke(Wood.Value);
        Stone.OnValueChanged?.Invoke(Stone.Value);
        Food.OnValueChanged?.Invoke(Food.Value);
    }

    public CityResource GetResource(CityResource.Type type)
    {
        switch (type)
        {
            case CityResource.Type.Gold:
                return Gold;
            case CityResource.Type.Wood:
                return Wood;
            case CityResource.Type.Stone:
                return Stone;
            case CityResource.Type.Food:
                return Food;
        }
        return null;
    }
    public void RemoveResource(CityResource resourceToRemove) => GetResource(resourceToRemove.type).Value -= resourceToRemove.Value;
    public void AddResource(CityResource resourceToAdd) => GetResource(resourceToAdd.type).Value += resourceToAdd.Value;

    public void RemoveResources(List<CityResource> resourcesToRemove)
    {
        foreach (var resource in resourcesToRemove)
        {
            RemoveResource(resource);
        }
    }


    public void AddResources(List<CityResource> resourcesToAdd)
    {
        foreach (var resource in resourcesToAdd)
        {
            AddResource(resource);
        }
    }
}