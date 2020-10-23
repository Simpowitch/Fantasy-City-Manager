using System.Collections.Generic;

public abstract class District : IHasIncome, IHasUpkeep
{
    public enum DistrictCategory
    {
        Residential,
        Culture,
        Services,
        Security,
        Producer,
        Trade
    }
    public abstract int DistrictCategoryIndex { get; }
    public abstract int DistrictSubType { get; }
    public string Name { get; set; }

    public int HouseCapacity { protected set; get; }
    public int PopulationCapacity { get; set; }

    public CityResourceGroup Upkeep => CityResourceGroup.CombineUpkeeps(Structures);
    public CityResourceGroup Income => CityResourceGroup.CombineIncomes(Structures);

    public HexCell HexCell { get; private set; }

    public List<Structure> Structures { get; private set; } = new List<Structure>();

    public District(string name, HexCell hexCell)
    {
        Name = name;
        HexCell = HexCell;
        Structures = new List<Structure>();
    }

    public abstract string GetCurrentInformation();

    public override string ToString()
    {
        return Name;
    }
}