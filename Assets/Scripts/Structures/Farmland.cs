using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Farmland : Structure
{
    [SerializeField] GrowingResource cropBlueprint = null;
    List<FarmTile> farmTiles;
    public List<FarmTile> PlantableFarmTiles
    {
        get
        {
            List<FarmTile> empty = new List<FarmTile>();
            empty.PopulateListWithMatchingConditions(farmTiles, (tile) => !tile.Filled && tile.plantingSeedOccupiedBy == null);
            return empty;
        }
    }

    public List<FarmTile> HarvestableFarmTiles
    {
        get
        {
            List<FarmTile> harvestable = new List<FarmTile>();
            harvestable.PopulateListWithMatchingConditions(farmTiles, (tile) => tile.Harvestable);
            return harvestable;
        }
    }

    public List<ResourceObject> GetCropsToHarvest()
    {
        List<ResourceObject> harvestableCrops = new List<ResourceObject>();
        foreach (var farmTile in HarvestableFarmTiles)
        {
            if (farmTile.Crop.workOccupiedBy == null)
                harvestableCrops.Add(farmTile.Crop);
        }
        return harvestableCrops;
    }

    public void PlantSeed(FarmTile farmTile)
    {
        GrowingResource seed = Instantiate(cropBlueprint, farmTile.ObjectTile.CenteredWorldPosition, Quaternion.identity, this.transform);
        farmTile.Crop = seed;
    }

    protected override void Constructed(City city, bool addToCityList)
    {
        base.Constructed(city, addToCityList);

        city.farmlands.Add(this);

        farmTiles = new List<FarmTile>();
        foreach (var objectTile in ObjectTiles)
        {
            farmTiles.Add(new FarmTile(objectTile));
        }
    }

    public override void Despawn()
    {
        foreach (var farmTile in farmTiles)
        {
            farmTile.DeSpawn();
        }
        city.farmlands.Remove(this);
        base.Despawn();
    }
}


public class FarmTile
{
    public ObjectTile ObjectTile { get; private set; }
    public Citizen plantingSeedOccupiedBy;

    public GrowingResource Crop { get; set; }

    public bool Filled { get => Crop != null; }
    public bool Harvestable { get => Crop != null && Crop.CanBeHarvested && Crop.MarkedForHarvest; }

    public FarmTile(ObjectTile objectTile)
    {
        ObjectTile = objectTile;
        ObjectTile.FarmTile = this;
    }

    public void DeSpawn() => ObjectTile.FarmTile = null;
}
