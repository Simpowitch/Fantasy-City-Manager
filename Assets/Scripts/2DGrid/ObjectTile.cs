using System.Collections.Generic;
using UnityEngine;

public class ObjectTile
{
    public readonly Grid<ObjectTile> grid;
    public readonly int x, y;

    public bool HasRoad { get; set; }

    public Structure Structure { get; set; }

    public ResourceObject ResourceObject {get; set;}

    public FarmTile FarmTile { get; set; }

    public Vector3 CenteredWorldPosition
    {
        get => grid.GetWorldPosition(x, y, true);
    }

    public CanvasTile CanvasTileObject { get; set; }

    public bool IsFree
    {
        get
        {
            if (Structure != null)
                return false;
            if (HasRoad)
                return false;
            if (ResourceObject)
                return false;
            if (FarmTile != null)
                return false;
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

    public List<ObjectTile> GetNeighbors() => grid.GetNeighbourList(x, y);

    public void PrintStateToCanvasObject()
    {
        string text = "";
        if (IsFree)
            text = "F";
        if (Structure)
            text = "S";
        if (HasRoad)
            text = "R";
        CanvasTileObject.SetText(text);
    }

    public override string ToString()
    {
        return $"X: {x}. Y: {y}";
    }
}
