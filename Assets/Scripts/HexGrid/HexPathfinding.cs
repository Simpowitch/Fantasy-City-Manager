using System.Collections.Generic;
using UnityEngine;
using System;

public static class HexPathfinding
{
    static int searchFrontierPhase = 0;
    static HexCellPriorityQueue searchFrontier = null;


    static HexUnit searcherUnit;
    static HexCell currentPathFrom, currentPathTo;
    public static bool HasPath
    {
        set;
        get;
    }

    public static void FindPath(HexCell fromCell, HexCell toCell, HexUnit unit, bool displayPath)
    {
        ClearPath();
        //New unit == Clear old pathfinding
        if (searcherUnit != unit)
        {
            unit.myGrid.ClearSearchHeuristics();
            searchFrontierPhase = 0;
            searchFrontier = null;
        }
        searcherUnit = unit;
        currentPathFrom = fromCell;
        currentPathTo = toCell;
        HasPath = Search(fromCell, toCell, unit);
    }

    //Pathfinding search
    static bool Search(HexCell fromCell, HexCell toCell, HexUnit unit)
    {
        searchFrontierPhase += 2;
        if (searchFrontier == null)
        {
            searchFrontier = new HexCellPriorityQueue();
        }
        else
        {
            searchFrontier.Clear();
        }

        fromCell.SearchPhase = searchFrontierPhase;
        fromCell.MovementCost = 0;
        searchFrontier.Enqueue(fromCell);

        while (searchFrontier.Count > 0)
        {
            HexCell current = searchFrontier.Dequeue();
            current.SearchPhase += 1;

            if (current == toCell)
            {
                return true;
            }

            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                HexCell neighbor = current.GetNeighbor(d);
                if (neighbor == null || neighbor.SearchPhase > searchFrontierPhase)
                {
                    continue;
                }

                //Hexes forbidden to move to
                if (!neighbor.Traversable)
                {
                    continue;
                }

                int hexEnterCost = 0;

                //Special condition costs here
                hexEnterCost += neighbor.BaseEnterModifier;

                int combinedCost = current.MovementCost + hexEnterCost;

                if (neighbor.SearchPhase < searchFrontierPhase) //Has not been set before
                {
                    neighbor.SearchPhase = searchFrontierPhase;
                    neighbor.MovementCost = combinedCost;
                    neighbor.PathFrom = current;
                    neighbor.SearchHeuristic = neighbor.coordinates.DistanceTo(toCell.coordinates);
                    searchFrontier.Enqueue(neighbor);
                }
                else if (combinedCost < neighbor.MovementCost) //Update cost
                {
                    int oldPriority = neighbor.SearchPriority;
                    neighbor.MovementCost = combinedCost;
                    neighbor.PathFrom = current;
                    searchFrontier.Change(neighbor, oldPriority);
                }
            }
        }
        return false;
    }

    public static void ClearPath()
    {
        if (HasPath)
        {
            HexCell current = currentPathTo;
            while (current != currentPathFrom)
            {
                current.SetNumberLabel(null);
                current = current.PathFrom;
            }
            HasPath = false;
        }
        else if (currentPathFrom)
        {
        }
        currentPathFrom = currentPathTo = null;
    }
}
