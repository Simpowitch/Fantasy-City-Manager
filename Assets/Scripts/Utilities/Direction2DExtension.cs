using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Direction2DExtension
{
    public static Direction2D Opposite(this Direction2D direction)
    {
        return (int)direction < 4 ? (direction + 4) : (direction - 4);
    }

    public static Direction2D Previous(this Direction2D direction)
    {
        return direction == Direction2D.N ? Direction2D.NW : (direction - 1);
    }

    public static Direction2D Next(this Direction2D direction)
    {
        return direction == Direction2D.NW ? Direction2D.N : (direction + 1);
    }
}
