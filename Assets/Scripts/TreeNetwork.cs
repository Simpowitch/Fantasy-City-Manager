using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeNetwork : MonoBehaviour
{
    [SerializeField] Tree treeBlueprint = null;
    [SerializeField] Transform treeParent = null;
    Grid<ObjectTile> objectGrid;
    Grid<PathNode> pathGrid;

    List<Tree> spawnedTrees = new List<Tree>();

    const float MINSPAWNTIME = 1f;
    const float MAXSPAWNTIME = 10f;
    float timeLeftToNextSpawn = 0;

    private void Start()
    {
        timeLeftToNextSpawn = Random.Range(MINSPAWNTIME, MAXSPAWNTIME);
    }

    private void Update()
    {
        timeLeftToNextSpawn -= Time.deltaTime;
        if (timeLeftToNextSpawn <= 0)
        {
            ObjectTile objectTile = FindNextObjectTile();
            if (objectTile != null)
                SpawnTree(objectTile);
            timeLeftToNextSpawn = Random.Range(MINSPAWNTIME, MAXSPAWNTIME);
        }
    }

    private ObjectTile FindNextObjectTile()
    {
        if (spawnedTrees.Count > 0 && Utility.RandomizeBool(25)) //Get Nearby Another Tree
        {
            List<ObjectTile> freeTilesAroundTrees = new List<ObjectTile>();
            foreach (var tree in spawnedTrees)
            {
                ObjectTile tile = tree.ObjectTile;
                foreach (var neighbor in tile.GetNeighbors())
                {
                    if (neighbor.IsFree)
                        freeTilesAroundTrees.Add(neighbor);
                    //We intentionally don't check if it has already been added, this increases spawnrate for spots with multiple neighbor tiles with trees
                }
            }
            return Utility.ReturnRandom(freeTilesAroundTrees);
        }
        else //Get Random free position on map
        {
            List<ObjectTile> freeTiles = new List<ObjectTile>();
            foreach (var tile in objectGrid.gridArray)
            {
                if (tile.IsFree)
                    freeTiles.Add(tile);
            }
            return Utility.ReturnRandom(freeTiles);
        }
    }

    public void SpawnTree(ObjectTile objectTile)
    {
        Tree tree = Instantiate(treeBlueprint, objectTile.CenteredWorldPosition, Quaternion.identity, treeParent);
        tree.Spawned(this, objectTile);
        spawnedTrees.Add(tree);
    }

    public void RemoveTree(Tree treeToRemove) => spawnedTrees.Remove(treeToRemove);

    public void Setup(Grid<ObjectTile> objectGrid, Grid<PathNode> pathGrid)
    {
        this.objectGrid = objectGrid;
        this.pathGrid = pathGrid;
    }

    public List<Tree> GetHarvestableTrees()
    {
        List<Tree> treesForChopping = new List<Tree>();
        treesForChopping.PopulateListWithMatchingConditions(spawnedTrees, (tree) => tree.markedForChopping && tree.workOccupiedBy == null);
        if (treesForChopping.Count == 0)
            treesForChopping.PopulateListWithMatchingConditions(spawnedTrees, (tree) => tree.workOccupiedBy == null);
        return treesForChopping;
    }
}
