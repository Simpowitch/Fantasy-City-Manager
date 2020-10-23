using UnityEngine;
using System.Collections.Generic;
using System;

public class HexGrid : MonoBehaviour
{
    public int CellCountX { private set; get; }
    public int CellCountY { private set; get; }

    [Header("References")]
    public HexCell cellPrefab;
    public InGameCamera cameraController;

    public HexCell[] Cells { private set; get; }

    static bool gameGridStatus = true;

    private void OnEnable()
    {
        ShowGameGrid(gameGridStatus);
    }

    public bool CreateMap(int x, int y, bool newMap, bool defaultTraversable, bool defaultLand)
    {
        ClearCells();

        CellCountX = x;
        CellCountY = y;
        CreateCells(newMap, defaultTraversable, defaultLand);

        ShowGameGrid(gameGridStatus);

        return true;
    }

    void CreateCells(bool newMap, bool defaultTraversable, bool defaultLand)
    {
        Cells = new HexCell[CellCountY * CellCountX];

        for (int y = 0, i = 0; y < CellCountY; y++)
        {
            for (int x = 0; x < CellCountX; x++)
            {
                CreateCell(x, y, i++, newMap, defaultTraversable, defaultLand);
            }
        }

        SetCameraBoundriesToMatchHexGrid();
    }

    void CreateCell(int x, int y, int i, bool newMap, bool defaultTraversable, bool defaultLand)
    {
        Vector3 position;
        position.x = (x + y * 0.5f - y / 2) * (HexMetrics.innerRadius * 2f);
        position.z = 0f;
        position.y = y * (HexMetrics.outerRadius * 1.5f);

        HexCell cell = Cells[i] = Instantiate<HexCell>(cellPrefab);
        cell.transform.SetParent(this.transform);
        cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, y);

        if (newMap)
        {
            //Connect hex neighbors to the west
            if (x > 0)
            {
                cell.SetNeighbor(HexDirection.W, Cells[i - 1]);
            }
            if (y > 0)
            {
                if ((y & 1) == 0) //If even row (with X = 0 to the leftmost position)
                {
                    cell.SetNeighbor(HexDirection.SE, Cells[i - CellCountX]);
                    if (x > 0)
                    {
                        cell.SetNeighbor(HexDirection.SW, Cells[i - CellCountX - 1]);
                    }
                }
                else //Un-even row (with X = 0 with incline into the row)
                {
                    cell.SetNeighbor(HexDirection.SW, Cells[i - CellCountX]);
                    if (x < CellCountX - 1)
                    {
                        cell.SetNeighbor(HexDirection.SE, Cells[i - CellCountX + 1]);
                    }
                }
            }
            cell.Traversable = defaultTraversable;
            cell.IsLand = defaultLand;
        }

        cell.myGrid = this;
    }

    public void SetCameraBoundriesToMatchHexGrid()
    {
        Vector3 minPos = Cells[0].transform.position;
        minPos.x -= HexMetrics.innerRadius;
        minPos.y -= HexMetrics.outerRadius;

        Vector3 maxPos = Cells[Cells.Length - 1].transform.position;

        maxPos.x = Mathf.Max(maxPos.x, Cells[CellCountX * 2 - 1].transform.position.x); //Allow camera movement to the rightmost position (even rows goes further to the right than un-even rows)

        maxPos.x += HexMetrics.innerRadius;
        maxPos.y += HexMetrics.outerRadius;

        cameraController.SetBoundries(minPos, maxPos);
    }

    public HexCell GetCell(HexCoordinates coordinates)
    {
        int y = coordinates.Y;
        int x = coordinates.X + y / 2;

        if (y < 0 || y >= CellCountY)
        {
            return null;
        }
        if (x < 0 || x >= CellCountX)
        {
            return null;
        }

        return Cells[x + y * CellCountX];
    }

    public HexCell GetCell()
    {
        RaycastHit2D hit;
        hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit.collider)
        {
            HexCell foundCell = hit.collider.GetComponent<HexCell>();
            return foundCell;
        }
        else
        {
            return null;
        }
    }

    public List<HexCell> GetAllCellsWithCondition(params Func<HexCell, bool>[] conditions)
    {
        List<HexCell> foundCells = new List<HexCell>();
        foreach (var cell in Cells)
        {
            bool canBeAdded = true;
            foreach (var condition in conditions)
            {
                if (!condition.Invoke(cell))
                {
                    canBeAdded = false;
                    break;
                }
            }
            if (canBeAdded)
            {
                foundCells.Add(cell);
            }
        }
        return foundCells;
    }

    #region UI and Grid
    public void ShowUI(bool visible)
    {
        foreach (var item in Cells)
        {
            item.ShowUI(visible);
        }
    }

    public void ShowGameGrid(bool status)
    {
        if (Cells == null)
        {
            return;
        }
        gameGridStatus = status;
        foreach (var item in Cells)
        {
            item.ShowGameOutline(status);
        }
    }
    #endregion

    void ClearCells()
    {
        if (Cells != null)
        {
            for (int i = 0; i < Cells.Length; i++)
            {
                GameObject.Destroy(Cells[i].gameObject);
            }
        }
    }

    public void ClearSearchHeuristics()
    {
        foreach (var item in Cells)
        {
            item.ClearPathfinding();
        }
    }

    #region Save and Load
    public HexCell[] Save()
    {
        foreach (var item in Cells)
        {
            item.CalculateBitmask();
        }
        Debug.Log("Map saved");
        return Cells;
    }

    public void Load(MapData map)
    {
        CreateMap(map.cellCountX, map.cellCountY, false, false, false);
        for (int i = 0; i < map.cells.Length; i++)
        {
            Cells[i].Load(map.cells[i], this);
        }
        ShowGameGrid(gameGridStatus);
        Debug.Log("Map loaded");
    }
    #endregion
}
