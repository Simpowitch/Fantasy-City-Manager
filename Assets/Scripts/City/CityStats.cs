using System.Collections.Generic;
using System.Diagnostics;

public class CityStats
{
    public delegate void StatsHandler(int newStat);
    public event StatsHandler OnPopulationChanged;
    public event StatsHandler OnVisitorsChanged;

    public CityResource Gold = new CityResource("Gold", CityResource.Type.Gold, 0);
    public CityResource Wood = new CityResource("Wood", CityResource.Type.Wood, 0);
    public CityResource Stone = new CityResource("Stone", CityResource.Type.Stone, 0);
    public CityResource Food = new CityResource("Food", CityResource.Type.Food, 0);

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

    public void Setup()
    {
        OnPopulationChanged?.Invoke(population);
        OnVisitorsChanged?.Invoke(visitors);
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

    public void RemoveResources(List<CityResource> cityResources)
    {
        foreach (var resource in cityResources)
        {
            GetResource(resource.type).Value -= resource.Value;
        }
    }

    public void AddResources(List<CityResource> cityResources)
    {
        foreach (var resource in cityResources)
        {
            GetResource(resource.type).Value += resource.Value;
        }
    }
}