using UnityEngine;
using UnityEngine.Tilemaps;

public class RoadNetwork : MonoBehaviour
{
    public enum GroundType { Grass, GravelRoad, StoneRoad, Indoor }

    [SerializeField] Tilemap roadTilemap = null;

    Grid<ObjectTile> objectGrid;
    Grid<PathNode> pathGrid;

    [Header("Road/AI Travelling costs")]
    [SerializeField] int grassTravelCost = 100;
    [SerializeField] int gravelTravelCost = 20;
    [SerializeField] int stoneTravelCost = 10;
    [SerializeField] int indoorTravelCost = 1;

    public bool PathfindingPenaltiesShown { get; private set; } = false;

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
                    AddRoad(tile.CenteredWorldPosition, GroundType.GravelRoad);
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

    public void AddRoad(Vector3 worldPosition, GroundType groundType)
    {
        ObjectTile roadTile = objectGrid.GetGridObject(worldPosition);
        roadTile.HasRoad = true;
        pathGrid.GetGridObject(worldPosition).ChangeMovementPenalty(GetTravellingCost(groundType));
    }

    public void RemoveRoad(Vector3 worldPosition)
    {
        ObjectTile noRoadTile = objectGrid.GetGridObject(worldPosition);
        noRoadTile.HasRoad = false;
        pathGrid.GetGridObject(worldPosition).ChangeMovementPenalty(grassTravelCost);
    }

    private int GetTravellingCost(GroundType groundType)
    {
        switch (groundType)
        {
            case GroundType.Grass:
                return grassTravelCost;
            case GroundType.GravelRoad:
                return gravelTravelCost;
            case GroundType.StoneRoad:
                return stoneTravelCost;
            case GroundType.Indoor:
                return indoorTravelCost;
            default:
                Debug.LogError("Missing road type");
                return grassTravelCost;
        }
    }

    public void ChangeWalkable(Vector3 worldPosition, bool walkable)
    {
        PathNode node = pathGrid.GetGridObject(worldPosition);
        node.ChangeIsWalkable(walkable);
    }

    public void ShowPathfindingPenalties(bool enabled)
    {
        if (pathGrid == null)
            return;
        foreach (var pathNode in pathGrid.gridArray)
        {
            string text = enabled ? pathNode.MovementPenalty.ToString() : "";
            pathNode.CanvasTileObject.SetText(text);
        }
        PathfindingPenaltiesShown = enabled;
    }
}
