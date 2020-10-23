using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HexDirection
{
    NE, E, SE, SW, W, NW
}


public static class HexMetrics
{
    #region Basics
    public const float outerToInner = 0.866025404f;
    public const float innerToOuter = 1f / outerToInner;
    public const float innerRadius = 0.5f;
    public const float outerRadius = innerRadius * innerToOuter;
    static Vector3[] corners = {
        new Vector3(0f, outerRadius),
        new Vector3(innerRadius, 0.5f * outerRadius),
        new Vector3(innerRadius, -0.5f * outerRadius),
        new Vector3(0f, -outerRadius),
        new Vector3(-innerRadius, -0.5f * outerRadius),
        new Vector3(-innerRadius, 0.5f * outerRadius),
        new Vector3(0f, outerRadius) //7th corner is same as first
    };
    #endregion


    #region Randomizers
    public const float noiseScale = 0.003f;
    public static Texture2D noiseSource;

    public const int hashGridSize = 256;
    public const float hashGridScale = 2.5f;
    static HexHash[] hashGrid;
    #endregion

    //Creates a hexhash grid, containing hexhashes with randomized values
    public static void InitializeHashGrid(int seed)
    {
        hashGrid = new HexHash[hashGridSize * hashGridSize];
        Random.State previousState = Random.state;
        Random.InitState(seed);
        for (int i = 0; i < hashGrid.Length; i++)
        {
            hashGrid[i] = HexHash.Create();
        }
        Random.state = previousState;
    }

    /// <summary>
    /// Returns an hexhash containing randomized values
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public static HexHash SampleHashGrid(Vector3 position)
    {
        int x = (int)(position.x *hashGridScale) % hashGridSize;
        if (x < 0)
        {
            x += hashGridSize;
        }
        int y = (int)(position.y * hashGridScale) % hashGridSize;
        if (y < 0)
        {
            y += hashGridSize;
        }
        return hashGrid[x + y * hashGridSize];
    }
}
