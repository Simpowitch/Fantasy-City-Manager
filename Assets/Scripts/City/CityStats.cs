using System.Collections.Generic;

public class CityStats
{
    public delegate void CityStatsHandler(CityStats cityStats);
    public CityStatsHandler OnCityStatsChanged;

    public Inventory Inventory { get; private set; } = new Inventory();

    public List<Citizen> Citizens { get; private set; } = new List<Citizen>();

    public List<District> Districts { get; private set; } = new List<District>();

    public int Population => Citizens.Count;

    public int PopulationCapacity
    {
        get
        {
            int populationSpace = 0;
            foreach (var district in Districts)
            {
                populationSpace += district.PopulationCapacity;
            }
            return populationSpace;
        }
    }

    int visitors;
    public int Visitors
    {
        get => visitors;
        set
        {
            visitors = value;
            OnCityStatsChanged?.Invoke(this);
        }
    }

    public void Setup(int startGold, int startWood, int startStone, int startIron, int startFood)
    {
        Inventory.Add(new CityResource(CityResource.Type.Gold, startGold));
        Inventory.Add(new CityResource(CityResource.Type.Wood, startWood));
        Inventory.Add(new CityResource(CityResource.Type.Stone, startStone));
        Inventory.Add(new CityResource(CityResource.Type.Iron, startIron));
        Inventory.Add(new CityResource(CityResource.Type.Food, startFood));

        Inventory.OnInventoryChanged += SendValuesToListeners;
        SendValuesToListeners(Inventory);
    }

    public void AddCitizen(Citizen newCitizen)
    {
        Citizens.Add(newCitizen);
        OnCityStatsChanged?.Invoke(this);
    }

    public void RemoveCitizen(Citizen citizenToRemove)
    {
        Citizens.Remove(citizenToRemove);
        OnCityStatsChanged?.Invoke(this);
    }

    public void AddDistrict(District newDistrict)
    {
        Districts.Add(newDistrict);
        OnCityStatsChanged?.Invoke(this);
    }

    public void RemoveDistrict(District districtToRemove)
    {
        Districts.Remove(districtToRemove);
        OnCityStatsChanged?.Invoke(this);
    }

    public void SendValuesToListeners(Inventory inventory)
    {
        OnCityStatsChanged?.Invoke(this);
    }
}