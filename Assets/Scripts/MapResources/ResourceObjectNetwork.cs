using System.Collections.Generic;
using UnityEngine;

public class ResourceObjectNetwork : MonoBehaviour
{
    [Header("Spawning")]
    [SerializeField] Transform objectParent = null;
    [SerializeField] ResourceObject stoneBlueprint = null;
    [SerializeField] GrowingResource treeBlueprint = null;

    Grid<ObjectTile> objectGrid;

    List<ResourceObject> harvestableStones = new List<ResourceObject>();
    List<GrowingResource> harvestableTrees = new List<GrowingResource>();

    [Header("Setup")]
    [SerializeField] int startStone = 15;
    [SerializeField] int startTrees = 10;


    #region SpawnTimers
    const float TREEMINSPAWNTIME = 5f;
    const float TREEMAXSPAWNTIME = 20f;
    float treeSpawnTimer = 0;
    #endregion


    private void Start()
    {
        treeSpawnTimer = Random.Range(TREEMINSPAWNTIME, TREEMAXSPAWNTIME);
        for (int i = 0; i < startStone; i++)
        {
            SpawnObject(FindFreeObjectTile(CityResource.Type.Stone), stoneBlueprint);
        }
        for (int i = 0; i < startTrees; i++)
        {
            SpawnObject(FindFreeObjectTile(CityResource.Type.Wood), treeBlueprint);
        }
    }

    private void Update()
    {
        UpdateSpawnTimer(ref treeSpawnTimer, CityResource.Type.Wood);
    }

    private void UpdateSpawnTimer(ref float timer, CityResource.Type type)
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            ObjectTile freeTile = FindFreeObjectTile(type);
            if (freeTile != null)
            {
                ResourceObject blueprint = null;
                switch (type)
                {
                    case CityResource.Type.Wood:
                        blueprint = treeBlueprint;
                        break;
                    case CityResource.Type.Stone:
                        blueprint = stoneBlueprint;
                        break;
                    case CityResource.Type.Gold:
                    case CityResource.Type.Food:
                    default:
                        Debug.LogError("Type not found");
                        break;
                }
                SpawnObject(freeTile, blueprint);
            }

            switch (type)
            {
                case CityResource.Type.Wood:
                    timer = Random.Range(TREEMINSPAWNTIME, TREEMAXSPAWNTIME);
                    break;
                case CityResource.Type.Food:
                case CityResource.Type.Gold:
                case CityResource.Type.Stone:
                default:
                    Debug.LogError("Type not found");
                    break;
            }

        }
    }

    //Setup from the city at start of the game
    public void Setup(Grid<ObjectTile> objectGrid)
    {
        this.objectGrid = objectGrid;
    }

    //Finds a free tile
    private ObjectTile FindFreeObjectTile(CityResource.Type type)
    {
        List<ObjectTile> freeTiles = new List<ObjectTile>();
        if (type == CityResource.Type.Wood && harvestableTrees.Count > 0 && Utility.RandomizeBool(25)) //Get Tile Near Other Trees
        {
            foreach (var tree in harvestableTrees)
            {
                ObjectTile tile = tree.ObjectTile;
                foreach (var neighbor in tile.GetNeighbors())
                {
                    if (neighbor.IsFree)
                        freeTiles.Add(neighbor);
                    //We intentionally don't check if it has already been added, this increases spawnrate for spots with multiple neighbor tiles with trees
                }
            }
        }
        else //Get Random free position on map
        {
            foreach (var tile in objectGrid.gridArray)
            {
                if (tile.IsFree)
                    freeTiles.Add(tile);
            }
        }
        return Utility.ReturnRandom(freeTiles);
    }

    //Spawns an object in the world
    private void SpawnObject(ObjectTile objectTile, ResourceObject blueprint)
    {
        switch (blueprint.Type)
        {
            case CityResource.Type.Stone:
                ResourceObject spawnedStone = Instantiate(blueprint, objectTile.CenteredWorldPosition, Quaternion.identity, objectParent);
                spawnedStone.Spawned(this, objectTile);
                harvestableStones.Add(spawnedStone);
                break;
            case CityResource.Type.Wood:
                GrowingResource spawnedTree = Instantiate(treeBlueprint, objectTile.CenteredWorldPosition, Quaternion.identity, objectParent);
                spawnedTree.Spawned(this, objectTile);
                harvestableTrees.Add(spawnedTree);
                break;
            case CityResource.Type.Food:
            case CityResource.Type.Gold:
            default:
                Debug.LogError("Type not found to add to list");
                break;
        }
    }

    //Removes an object from the lists
    public void RemoveStatic(ResourceObject resourceObject)
    {
        switch (resourceObject.Type)
        {
            case CityResource.Type.Stone:
                harvestableStones.Remove(resourceObject);
                break;
            case CityResource.Type.Wood:
            case CityResource.Type.Food:
            case CityResource.Type.Gold:
            default:
                Debug.LogError("Type not found to remove");
                break;
        }
    }

    public void RemoveGrowable(GrowingResource growingResource)
    {
        switch (growingResource.Type)
        {
            case CityResource.Type.Gold:
            case CityResource.Type.Stone:
            case CityResource.Type.Food:
                Debug.LogError("Type not found to remove");
                break;
            case CityResource.Type.Wood:
                harvestableTrees.Remove(growingResource);
                break;
        }
    }

    public List<ResourceObject> GetHarvestable(CityResource.Type type)
    {
        List<ResourceObject> list = new List<ResourceObject>();
        switch (type)
        {
            case CityResource.Type.Stone:
                list = harvestableStones;
                break;
            case CityResource.Type.Wood:
                List<GrowingResource> trees = GetHarvestableGrowables(CityResource.Type.Wood);
                list.AddRange(trees);
                return list;
            case CityResource.Type.Food:
            case CityResource.Type.Gold:
            default:
                Debug.LogError("List not found");
                return null;
        }

        List<ResourceObject> harvestable = new List<ResourceObject>();
        harvestable.PopulateListWithMatchingConditions(list, (obj) => obj.MarkedForHarvest && obj.workOccupiedBy == null);
        if (harvestable.Count == 0)
            harvestable.PopulateListWithMatchingConditions(list, (obj) => obj.workOccupiedBy == null);
        return harvestable;
    }

    private List<GrowingResource> GetHarvestableGrowables(CityResource.Type type)
    {
        List<GrowingResource> list = new List<GrowingResource>();
        switch (type)
        {
            case CityResource.Type.Wood:
                list = harvestableTrees;
                break;
            case CityResource.Type.Stone:
            case CityResource.Type.Food:
            case CityResource.Type.Gold:
            default:
                Debug.LogError("List not found");
                return null;
        }

        List<GrowingResource> harvestable = new List<GrowingResource>();
        harvestable.PopulateListWithMatchingConditions(list, (obj) => obj.MarkedForHarvest && obj.workOccupiedBy == null && obj.CanBeHarvested);
        if (harvestable.Count == 0)
            harvestable.PopulateListWithMatchingConditions(list, (obj) => obj.workOccupiedBy == null && obj.CanBeHarvested);
        return harvestable;
    }
}
