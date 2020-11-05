using System.Collections.Generic;
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
        seed.Spawned(null, farmTile.ObjectTile);
    }

    protected override void Constructed(City city, bool addToCityList)
    {
        base.Constructed(city, addToCityList);

        city.farmlands.Add(this);

        farmTiles = new List<FarmTile>();
        foreach (var objectTile in StructureTiles)
        {
            farmTiles.Add(new FarmTile(objectTile, this));
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
    Farmland farmland;

    public GrowingResource Crop { get; set; }

    public bool Filled { get => Crop != null; }
    public bool Harvestable { get => Crop != null && Crop.CanBeHarvested && Crop.MarkedForHarvest; }

    public FarmTile(ObjectTile objectTile, Farmland farmland)
    {
        ObjectTile = objectTile;
        ObjectTile.FarmTile = this;
        this.farmland = farmland;
    }

    // Player Interaction with the farmtile to plant a seed - Returns true if possible
    public bool PlayerInteraction(PlayerCharacter playerCharacter, PlayerInput playerInput, PlayerTaskSystem playerTaskSystem)
    {
        if (Crop != null)
            return false;

        playerCharacter.MoveTo(ObjectTile.CenteredWorldPosition, UnitAnimator.ActionAnimation.PlantSeed, true);

        //Start Task
        playerTaskSystem.StartTask(() =>
        {
            farmland.PlantSeed(this);
            playerCharacter.UnitAnimator.PlayActionAnimation(UnitAnimator.ActionAnimation.Idle);
            playerInput.inputEnabled = true;
            playerCharacter.SetColliderState(true);
        });

        return true;
    }

    public void DeSpawn() => ObjectTile.FarmTile = null;
}
