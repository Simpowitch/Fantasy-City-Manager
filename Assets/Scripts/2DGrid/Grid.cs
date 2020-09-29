using System;
using System.Collections.Generic;
using UnityEngine;

public class Grid<TGridObject>
{
    public event EventHandler<OnGridValueChangedEventArgs> OnGridvalueChanged;
    public class OnGridValueChangedEventArgs : EventArgs
    {
        public int x, y;
    }

    public int Width
    {
        private set;
        get;
    }
    public int Height
    {
        private set;
        get;
    }
    public readonly float cellSize;
    Vector3 originPosition;
    public readonly TGridObject[,] gridArray;

    public Grid(int width, int height, float cellSize, Vector3 originPosition, Func<Grid<TGridObject>, int, int, TGridObject> createGridObject)
    {
        this.Width = width;
        this.Height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;
        gridArray = new TGridObject[width, height];
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                gridArray[x, y] = createGridObject(this, x, y);
            }
        }
    }

    public bool IsWithinGrid(int x, int y)
    {
        return (x >= 0 && y >= 0 && x < Width && y < Height);
    }

    public Vector3 GetWorldPosition(int x, int y, bool center)
    {
        if (center)
        {
            return new Vector3(x, y) * cellSize + originPosition + new Vector3(cellSize / 2, cellSize / 2);
        }
        else
        {
            return new Vector3(x, y) * cellSize + originPosition;
        }
    }

    public Vector3 GetWorldPosition(Vector3 worldPosition, bool center)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        return GetWorldPosition(x, y, center);
    }

    public Vector3 GetTopRightCornerWorldPosition(Vector3 worldPosition)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        return new Vector3(x, y) * cellSize + originPosition + new Vector3(cellSize, cellSize);
    }


    public void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
    }

    public TGridObject GetGridObject(Vector3 worldPosition)
    {
        GetXY(worldPosition, out int x, out int y);
        if (IsWithinGrid(x, y))
        {
            return gridArray[x, y];
        }
        else
        {
            return default;
        }
    }

    public TGridObject GetGridObject(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < Width && y < Height)
        {
            return gridArray[x, y];
        }
        else
        {
            return default;
        }
    }

    public List<TGridObject> GetGridObjects(Vector3 lowerLeft, Vector3 upperRight)
    {
        int aX, aY;
        GetXY(lowerLeft, out aX, out aY);
        int bX, bY;
        GetXY(upperRight, out bX, out bY);
        List<TGridObject> foundGridObjects = new List<TGridObject>();
        for (int x = aX; x < bX; x++)
        {
            for (int y = aY; y < bY; y++)
            {
                TGridObject obj = GetGridObject(x, y);
                if (obj != null)
                {
                    foundGridObjects.Add(obj);
                }
            }
        }
        return foundGridObjects;
    }

    public TGridObject GetGridObjectInDirection(Vector3 fromPosition, Direction2D direction)
    {
        GetXY(fromPosition, out int x, out int y);
        Vector2Int delta = Vector2Int.zero;
        switch (direction)
        {
            case Direction2D.N:
                delta = new Vector2Int(0, 1);
                break;
            case Direction2D.NE:
                delta = new Vector2Int(1, 1);
                break;
            case Direction2D.E:
                delta = new Vector2Int(1, 0);
                break;
            case Direction2D.SE:
                delta = new Vector2Int(1, -1);
                break;
            case Direction2D.S:
                delta = new Vector2Int(0, -1);
                break;
            case Direction2D.SW:
                delta = new Vector2Int(-1, -1);
                break;
            case Direction2D.W:
                delta = new Vector2Int(-1, 0);
                break;
            case Direction2D.NW:
                delta = new Vector2Int(-1, 1);
                break;
        }
        x += delta.x;
        y += delta.y;
        return GetGridObject(x, y);
    }

    public List<TGridObject> GetGridObjectsInLine(Vector3 fromPosition, Direction2D direction, int numberOfCells)
    {
        List<TGridObject> objectsInLine = new List<TGridObject>();
        GetXY(fromPosition, out int x, out int y);
        Vector2Int delta = Vector2Int.zero;
        switch (direction)
        {
            case Direction2D.N:
                delta = new Vector2Int(0, 1);
                break;
            case Direction2D.NE:
                delta = new Vector2Int(1, 1);
                break;
            case Direction2D.E:
                delta = new Vector2Int(1, 0);
                break;
            case Direction2D.SE:
                delta = new Vector2Int(1, -1);
                break;
            case Direction2D.S:
                delta = new Vector2Int(0, -1);
                break;
            case Direction2D.SW:
                delta = new Vector2Int(-1, -1);
                break;
            case Direction2D.W:
                delta = new Vector2Int(-1, 0);
                break;
            case Direction2D.NW:
                delta = new Vector2Int(-1, 1);
                break;
        }
        for (int i = 0; i < numberOfCells; i++)
        {
            x += delta.x;
            y += delta.y;
            TGridObject gridObject = GetGridObject(x, y);
            if (gridObject != null)
            {
                objectsInLine.Add(gridObject);
            }
        }
        return objectsInLine;
    }

    public List<TGridObject> GetGridObjectsInAllLines(Vector3 fromPosition, int numberOfCells)
    {
        List<TGridObject> objectsInLines = new List<TGridObject>();
        for (Direction2D direction = Direction2D.N; direction <= Direction2D.NW; direction++)
        {
            objectsInLines.AddRange(GetGridObjectsInLine(fromPosition, direction, numberOfCells));
        }
        return objectsInLines;
    }

    public List<TGridObject> GetNeighbourList(int currentX, int currentY)
    {
        List<TGridObject> neighbourList = new List<TGridObject>();
        if (currentX - 1 >= 0)
        {
            //Left
            neighbourList.Add(GetGridObject(currentX - 1, currentY));
            //Left down
            if (currentY - 1 >= 0) neighbourList.Add(GetGridObject(currentX - 1, currentY - 1));
            //Left up
            if (currentY + 1 < Height) neighbourList.Add(GetGridObject(currentX - 1, currentY + 1));
        }
        if (currentX + 1 < Width)
        {
            //Right
            neighbourList.Add(GetGridObject(currentX + 1, currentY));
            //Right down
            if (currentY - 1 >= 0) neighbourList.Add(GetGridObject(currentX + 1, currentY - 1));
            //Right up
            if (currentY + 1 < Height) neighbourList.Add(GetGridObject(currentX + 1, currentY + 1));
        }
        //Down
        if (currentY - 1 >= 0) neighbourList.Add(GetGridObject(currentX, currentY - 1));
        //Up
        if (currentY + 1 < Height) neighbourList.Add(GetGridObject(currentX, currentY + 1));

        return neighbourList;
    }

    public void SetGridObject(int x, int y, TGridObject gridObject)
    {
        if (IsWithinGrid(x, y))
        {
            gridArray[x, y] = gridObject;
            TriggerGridObjectChanged(x, y);
        }
        else
        {
            Debug.LogError("Index out of range");
        }
    }

    public void TriggerGridObjectChanged(int x, int y)
    {
        OnGridvalueChanged?.Invoke(this, new OnGridValueChangedEventArgs { x = x, y = y });
    }
}
