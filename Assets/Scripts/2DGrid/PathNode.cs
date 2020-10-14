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

    public enum MovementAllowance { Free, Forbidden, OnlyFinal }
    private MovementAllowance movementAllowanceMode = MovementAllowance.Free;
    public MovementAllowance MovementAllowanceMode
    {
        set
        {
            movementAllowanceMode = value;
            grid.TriggerGridObjectChanged(x, y);
        }
        get => movementAllowanceMode;
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

    public void ChangeMovementPenalty(int newValue) => MovementPenalty = newValue;

    public override string ToString()
    {
        return x + "," + y;
    }

    public static bool CanTravelToNeighbor(PathNode to, PathNode from, List<PathNode> fromNeighbors, bool toNodeIsEndNode)
    {
        if (toNodeIsEndNode)
        {
            if (to.movementAllowanceMode == MovementAllowance.Forbidden)
                return false;
        }
        else
        {
            if (to.MovementAllowanceMode != MovementAllowance.Free)
                return false;
        }
        if (!fromNeighbors.Contains(to))
            return false;
        Direction2D direction = Utility.GetDirection(from.WorldPosition, to.WorldPosition);
        if (Utility.IsDiagonal(direction))
        {
            PathNode next = from.grid.GetGridObjectInDirection(from.WorldPosition, direction.Next());
            PathNode previous = from.grid.GetGridObjectInDirection(from.WorldPosition, direction.Previous());
            return (next.MovementAllowanceMode == MovementAllowance.Free && previous.MovementAllowanceMode == MovementAllowance.Free);
        }
        else
            return true;
    }
}
