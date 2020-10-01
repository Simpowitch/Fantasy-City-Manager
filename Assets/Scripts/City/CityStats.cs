
public class CityStats
{
    public delegate void StatsHandler(int newStat);
    public event StatsHandler OnPopulationChanged;
    public event StatsHandler OnVisitorsChanged;

    public Inventory Inventory { get; private set; }

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

    public void Setup(int startGold, int startWood, int startStone, int startIron, int startFood)
    {
        OnPopulationChanged?.Invoke(population);
        OnVisitorsChanged?.Invoke(visitors);

        Inventory = new Inventory();
        Inventory.Add(new CityResource(CityResource.Type.Gold, startGold));
        Inventory.Add(new CityResource(CityResource.Type.Wood, startWood));
        Inventory.Add(new CityResource(CityResource.Type.Stone, startStone));
        Inventory.Add(new CityResource(CityResource.Type.Iron, startIron));
        Inventory.Add(new CityResource(CityResource.Type.Food, startFood));
    }

    public void SendValuesToListeners() => Inventory.SendValuesToListeners();
}