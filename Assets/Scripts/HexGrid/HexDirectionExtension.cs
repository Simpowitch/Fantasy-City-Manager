using UnityEngine;

public static class HexDirectionExtension
{
    public static HexDirection Opposite(this HexDirection direction)
    {
        return (int)direction < 3 ? (direction + 3) : (direction - 3);
    }

    public static HexDirection Previous(this HexDirection direction)
    {
        return direction == HexDirection.NE ? HexDirection.NW : (direction - 1);
    }

    public static HexDirection Next(this HexDirection direction)
    {
        return direction == HexDirection.NW ? HexDirection.NE : (direction + 1);
    }

    public static HexDirection Previous2(this HexDirection direction)
    {
        direction -= 2;
        return direction >= HexDirection.NE ? direction : (direction + 6);
    }

    public static HexDirection Next2(this HexDirection direction)
    {
        direction += 2;
        return direction <= HexDirection.NW ? direction : (direction - 6);
    }

    public static HexDirection GetDirectionToNeighbor(HexCell fromCell, HexCell toCell)
    {
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            if (fromCell.GetNeighbor(d) == toCell)
            {
                return d;
            }
        }
        Debug.Log("Could not find direction to cell");
        return HexDirection.NE;
    }

    /// <summary>
    /// Returns a direction from a cell to any other cell. Requires all cells to be flat and the distance to each neighboring cell to be equal to each other
    /// </summary>
    /// <param name="fromCell"></param>
    /// <param name="toCell"></param>
    /// <returns></returns>
    public static HexDirection GetDirectionTo(HexCell fromCell, HexCell toCell)
    {
        HexCoordinates change = new HexCoordinates(toCell.coordinates.X - fromCell.coordinates.X, toCell.coordinates.Y - fromCell.coordinates.Y);
        int deltaX = change.X;
        int deltaY = change.Y;

        if (deltaX == 0 && deltaY == 0)
        {
            Debug.LogWarning("No change detected. No direction can be found between specified cells");
            return HexDirection.NE;
        }

        //Straight lines on grid
        if (deltaX == 0)
        {
            if (deltaY > 0)
            {
                return HexDirection.NE;
            }
            else
            {
                return HexDirection.SW;
            }
        }
        if (deltaY == 0)
        {
            if (deltaX > 0)
            {
                return HexDirection.E;
            }
            else
            {
                return HexDirection.W;
            }
        }
        if (deltaX < 0)
        {
            if (Mathf.Abs(deltaX) == deltaY)
            {
                return HexDirection.NW;
            }
        }
        if (deltaX > -deltaY)
        {
            if (deltaX == -deltaY)
            {
                return HexDirection.SE;
            }
        }

        //Line not straight (traversing in multiple directions)
        if (deltaX > 0)
        {
            if (deltaY > 0)
            {
                if (deltaX >= deltaY)
                {
                    return HexDirection.E;
                }
                else
                {
                    return HexDirection.NE;
                }
            }
            else
            {
                if (deltaX >= Mathf.Abs(deltaY) * 2)
                {
                    return HexDirection.E;
                }
                else if (Mathf.Abs(deltaY) > deltaX * 2)
                {
                    return HexDirection.SW;
                }
                else
                {
                    return HexDirection.SE;
                }
            }
        }
        else
        {
            if (deltaY < 0)
            {
                if (Mathf.Abs(deltaX) >= Mathf.Abs(deltaY))
                {
                    return HexDirection.W;
                }
                else
                {
                    return HexDirection.SW;
                }
            }
            else
            {
                if (Mathf.Abs(deltaX) >= deltaY * 2)
                {
                    return HexDirection.W;
                }
                else if (Mathf.Abs(deltaY) > Mathf.Abs(deltaX) * 2)
                {
                    return HexDirection.NE;
                }
                else
                {
                    return HexDirection.NW;
                }
            }
        }
    }

    public static HexDirection ReturnRandomDirection()
    {
        return (HexDirection)Random.Range((int)HexDirection.NE, (int)HexDirection.NW + 1);
    }
}
