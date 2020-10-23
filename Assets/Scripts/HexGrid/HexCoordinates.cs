using UnityEngine;

[System.Serializable]
public struct HexCoordinates
{
    [SerializeField]
    private int x, y;
    public int X
    {
        get
        {
            return x;
        }
    }
    public int Z
    {
        get
        {
            return -X - Y;
        }
    }
    public int Y
    {
        get
        {
            return y;
        }
    }


    public HexCoordinates(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public static HexCoordinates FromOffsetCoordinates(int x, int y)
    {
        return new HexCoordinates(x - y / 2, y);
    }

    public static Vector3Int CoordinatesToTilemapCoordinates(HexCoordinates coordinates)
    {
        return new Vector3Int(coordinates.x + coordinates.y / 2, coordinates.y, 0);
    }

    public override string ToString()
    {
        return "(" +
        X.ToString() + ", " + Y.ToString() + ", " + Z.ToString() + ")";
    }

    public string ToStringOnSeparateLines()
    {
        return X.ToString() + "\n" + Y.ToString() + "\n" + Z.ToString();
    }

    public static HexCoordinates FromPosition(Vector3 position)
    {
        float x = position.x / (HexMetrics.innerRadius * 2f);
        //float y = -x;
        float z = -x;

        //float offset = position.z / (HexMetrics.outerRadius * 3f);
        float offset = position.y / (HexMetrics.outerRadius * 3f);
        x -= offset;
        //y -= offset;
        z -= offset;

        int iX = Mathf.RoundToInt(x);
        //int iY = Mathf.RoundToInt(y);
        int iZ = Mathf.RoundToInt(z);
        //int iZ = Mathf.RoundToInt(-x - y);
        int iY = Mathf.RoundToInt(-x - z);

        //if (iX + iY + iZ != 0)
        //{
        //    float dX = Mathf.Abs(x - iX);
        //    float dY = Mathf.Abs(y - iY);
        //    float dZ = Mathf.Abs(-x - y - iZ);

        //    if (dX > dY && dX > dZ)
        //    {
        //        iX = -iY - iZ;
        //    }
        //    else if (dZ > dY)
        //    {
        //        iZ = -iX - iY;
        //    }
        //}

        if (iX + iY + iZ != 0)
        {
            float dX = Mathf.Abs(x - iX);
            float dY = Mathf.Abs(-x - z - iY);
            float dZ = Mathf.Abs(z - iZ);

            if (dX > dZ && dX > dY)
            {
                iX = -iZ - iY;
            }
            else if (dY > dZ)
            {
                iY = -iX - iZ;
            }
        }

        //return new HexCoordinates(iX, iZ);
        return new HexCoordinates(iX, iY);
    }

    public int DistanceTo(HexCoordinates other)
    {
        //return
        //    ((x < other.x ? other.x - x : x - other.x) +
        //    (Y < other.Y ? other.Y - Y : Y - other.Y) +
        //    (z < other.z ? other.z - z : z - other.z)) / 2;
        return
            ((x < other.x ? other.x - x : x - other.x) +
            (Z < other.Z ? other.Z - Z : Z - other.Z) +
            (y < other.y ? other.y - y : y - other.y)) / 2;
    }
}
