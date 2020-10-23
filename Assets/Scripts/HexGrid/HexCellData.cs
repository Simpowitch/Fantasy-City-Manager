using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HexCellData
{
    public bool traversable;
    public bool isLand;
    public int bitmask;
    public bool[] connected;
    public HexCoordinates[] connectedCoordinates;

    public HexCellData(HexCell hexCell)
    {
        this.traversable = hexCell.Traversable;
        this.isLand = hexCell.IsLand;
        bitmask = hexCell.Bitmask;
        this.connected = new bool[6];
        this.connectedCoordinates = new HexCoordinates[6];
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            HexCell neighbor = hexCell.GetNeighbor(d);
            if (neighbor)
            {
                connectedCoordinates[(int)d] = neighbor.coordinates;
            }
            connected[(int)d] = (neighbor != null);
        }
    }
}
