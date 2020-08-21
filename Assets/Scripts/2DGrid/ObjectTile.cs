using System.Collections.Generic;
using UnityEngine;

public class ObjectTile
{
    public readonly Grid<ObjectTile> grid;
    public readonly int x, y;


    bool hasRoad;
    public bool HasRoad
    {
        get => hasRoad;
        set
        {
            hasRoad = value;
            //ResidentialZoned = false;
        }
    }

    List<ObjectTile> adjacentRoadTiles = new List<ObjectTile>();
    public List<Direction> AdjacentRoadDirections
    {
        get
        {
            if (adjacentRoadTiles.Count > 0)
            {
                List<Direction> directions = new List<Direction>();
                foreach (var tile in adjacentRoadTiles)
                {
                    Direction newDirection = ObjectTile.GetDirection(this, tile);
                    if (newDirection != Direction.Invalid)
                        directions.Add(newDirection);
                }
                return directions;
            }
            else
            {
                return null;
            }
        }
    }

    Structure structure;
    public Structure Structure
    {
        get => structure;
        set
        {
            structure = value;
            //ResidentialZoned = false;
        }
    }


    //public bool IsZonable
    //{
    //    get
    //    {
    //        if (!IsBuildable)
    //        {
    //            return false;
    //        }
    //        if (adjacentRoadTiles.Count <= 0)
    //        {
    //            return false;
    //        }
    //        return true;
    //    }
    //}

    //public bool ResidentialZoned
    //{
    //    set;
    //    get;
    //}

    public Vector3 CenteredWorldPosition
    {
        get => grid.GetWorldPosition(x, y, true);
    }

    public CanvasTile CanvasTileObject
    {
        get;
        set;
    }

    public bool IsBuildable
    {
        get
        {
            if (Structure != null)
            {
                return false;
            }
            if (HasRoad)
            {
                return false;
            }
            if (x == 0 || x == grid.Width - 1 || y == 0 || y == grid.Height - 1)
            {
                return false;
            }
            return true;
        }
    }

    public ObjectTile(Grid<ObjectTile> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
    }

    public void AddAdjacentRoadTile(ObjectTile tileWithRoad) => adjacentRoadTiles.Add(tileWithRoad);

    public void RemoveAdjacentRoadTile(ObjectTile tileWithRemovedRoad)
    {
        if (adjacentRoadTiles.Contains(tileWithRemovedRoad))
        {
            adjacentRoadTiles.Remove(tileWithRemovedRoad);
        }
    }

    public static Direction GetDirection(ObjectTile fromTile, ObjectTile toTile)
    {
        int deltaX, deltaY;
        deltaX = toTile.x - fromTile.x;
        deltaY = toTile.y - fromTile.y;

        if (Mathf.Abs(deltaX) > 0 && Mathf.Abs(deltaY) > 0)
        {
            return Direction.Invalid;
        }
        else
        {
            if (deltaX > 0)
            {
                return Direction.Right;
            }
            else if (deltaX < 0)
            {
                return Direction.Left;
            }
            else if (deltaY > 0)
            {
                return Direction.Up;
            }
            else if (deltaY < 0)
            {
                return Direction.Down;
            }
        }
        throw new System.ArgumentException();
    }
}
