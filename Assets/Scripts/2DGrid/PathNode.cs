using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PathNode
{
    public readonly Grid<PathNode> grid;
    public readonly int x, y;

    public int gCost, hCost, fCost; //WALKING COST FROM THE STARTNODE / HEURISTIC COST TO REACH END NODE / G + H

    private int movementPenalty = 0;
    public int MovementPenalty
    {
        private set
        {
            movementPenalty = value;
            grid.TriggerGridObjectChanged(x, y);
        }
        get => movementPenalty;
    }

    public CanvasTile CanvasTileObject
    {
        get;
        set;
    }

    private bool isWalkable = true;
    public bool IsWalkable
    {
        private set
        {
            isWalkable = value;
            grid.TriggerGridObjectChanged(x, y);
        }
        get => isWalkable;
    }

    public PathNode cameFromNode;

    public PathNode(Grid<PathNode> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
    }

    public Vector3 WorldPosition
    {
        get => grid.GetWorldPosition(x, y, true);
    }

    public void CalculateFCost() => fCost = gCost + hCost;

    public void ChangeIsWalkable(bool newState) => IsWalkable = newState;

    public void ChangeMovementPenalty(int newValue) => MovementPenalty = newValue;

    public override string ToString()
    {
        return x + "," + y;
    }

    public static bool CanTravelToNeighbor(PathNode to, PathNode from, List<PathNode> fromNeighbors)
    {
        if (!to.IsWalkable)
            return false;
        if (!fromNeighbors.Contains(to))
            return false;
        Direction2D direction = Utility.GetDirection(from.WorldPosition, to.WorldPosition);
        if (Utility.IsDiagonal(direction))
        {
            PathNode next = from.grid.GetGridObjectInDirection(from.WorldPosition, direction.Next());
            PathNode previous = from.grid.GetGridObjectInDirection(from.WorldPosition, direction.Previous());
            return (next.IsWalkable && previous.IsWalkable);
        }
        else
            return true;
    }
}
