using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class RoadNetwork : MonoBehaviour
{
    [SerializeField] Tilemap roadTilemap = null;

    Grid<ObjectTile> objectGrid;
    Grid<PathNode> pathGrid;

    [Header("Road/AI Travelling costs")]
    [SerializeField] int[] roadTravelCosts = null;
    [SerializeField] int grassTravelCost = 10;
    [SerializeField] int startingRoadIndex = 0;

    [Header("Zones around roads")]
    [SerializeField] int tilesZonableAroundRoad = 1;

    public void SetUp(Grid<ObjectTile> objectGrid, Grid<PathNode> pathgrid)
    {
        this.objectGrid = objectGrid;
        this.pathGrid = pathgrid;
        ConfirmStartRoadPositions();
    }

    public void ConfirmStartRoadPositions()
    {
        foreach (var tile in objectGrid.gridArray)
        {
            Vector3Int cellTilemapPosition = roadTilemap.WorldToCell(tile.CenteredWorldPosition);
            if (roadTilemap.HasTile(cellTilemapPosition))
            {
                if (!tile.HasRoad)
                {
                    AddRoad(tile.CenteredWorldPosition, startingRoadIndex);
                }
            }
            else
            {
                pathGrid.GetGridObject(tile.x, tile.y).ChangeMovementPenalty(grassTravelCost);
                if (tile.HasRoad)
                {
                    RemoveRoad(tile.CenteredWorldPosition);
                }
            }
        }
    }

    public void AddRoad(Vector3 worldPosition, int index)
    {
        ObjectTile roadTile = objectGrid.GetGridObject(worldPosition);
        roadTile.HasRoad = true;
        pathGrid.GetGridObject(worldPosition).ChangeMovementPenalty(roadTravelCosts[index]);
        List<ObjectTile> newResidentialPossibleTiles = objectGrid.GetGridObjectsInAllLines(worldPosition, tilesZonableAroundRoad);
        foreach (var tile in newResidentialPossibleTiles)
        {
            tile.AddAdjacentRoadTile(roadTile);
        }
    }

    public void RemoveRoad(Vector3 worldPosition)
    {
        ObjectTile noRoadTile = objectGrid.GetGridObject(worldPosition);
        noRoadTile.HasRoad = false;
        pathGrid.GetGridObject(worldPosition).ChangeMovementPenalty(grassTravelCost);
        List<ObjectTile> newResidentialPossibleTiles = objectGrid.GetGridObjectsInAllLines(worldPosition, tilesZonableAroundRoad);
        foreach (var tile in newResidentialPossibleTiles)
        {
            tile.RemoveAdjacentRoadTile(noRoadTile);
        }
    }
}
