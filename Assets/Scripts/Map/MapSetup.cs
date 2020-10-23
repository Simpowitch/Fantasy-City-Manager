using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapSetup : MonoBehaviour
{
    [Header("Systems")]
    [SerializeField] HexGrid hexGrid = null;

    [Header("Visuals")]
    [SerializeField] TileBase[] edgeTiles = null;
    [SerializeField] TileBase[] landTiles = null;
    [SerializeField] Tilemap terrainTilemap = null;

    [SerializeField] SetupData defaultSetup = new SetupData();


    private void Awake()
    {
        HexMetrics.InitializeHashGrid(defaultSetup.Seed);
        hexGrid.CreateMap(defaultSetup.mapSize.xSize, defaultSetup.mapSize.ySize, true, true, true);
        CreateMap(hexGrid, defaultSetup);
    }

    public void CreateMap(HexGrid hexGrid, SetupData setupData)
    {
        //LAND OR OCEAN ITERATED
        foreach (HexCell cell in hexGrid.Cells)
        {
            cell.IsLand = (1 - setupData.waterBaseStrength) > HexMetrics.SampleHashGrid(cell.Position).a;
        }

        //LAND OR OCEAN ITERATED
        foreach (HexCell cell in hexGrid.Cells)
        {
            List<HexCell> neighborLandTiles = new List<HexCell>();
            neighborLandTiles.PopulateListWithMatchingConditions(cell.Neighbors, (c) => c.IsLand == true);

            float landChance = (setupData.tileTypeStrength * neighborLandTiles.Count) - setupData.waterBaseStrength;

            cell.IsLand = landChance > HexMetrics.SampleHashGrid(cell.Position).b;
        }

        //BITMASK AND VISUALS
        foreach (HexCell cell in hexGrid.Cells)
        {
            cell.CalculateBitmask();
            SetTerrainCellVisual(cell);
        }
    }

    /// <summary>
    /// Paints terrain on the tilemap
    /// </summary>
    /// <param name="cell"></param>
    private void SetTerrainCellVisual(HexCell cell)
    {
        Vector3Int tilemapPosition = HexCoordinates.CoordinatesToTilemapCoordinates(cell.coordinates);

        TileBase tile;

        if (cell.IsShore) //Edge of islands
        {
            tile = edgeTiles[cell.Bitmask];
        }
        else if (cell.IsOcean) //Ocean tile
        {
            tile = null; //Ignore ocean tiles for now
        }
        else //If over 62 (full land tile with all neighbors also landtiles)
        {
            tile = Utility.ReturnRandom(landTiles);
        }

        terrainTilemap.SetTile(tilemapPosition, tile);
    }
}

[System.Serializable]
public class SetupData
{
    const int SEEDMAXCHARS = 10;
    public int Seed
    {
        get
        {
            int seed = 0;
            char[] chars = stringSeed.ToCharArray();
            for (int i = 0; i < chars.Length && i < SEEDMAXCHARS; i++)
            {
                seed += chars[i].GetHashCode();
            }
            return seed;
        }
    }
    public string stringSeed;
    public MapSize mapSize;
    public float tileTypeStrength = 0.1f;
    public float waterBaseStrength = 0.1f;
}

[System.Serializable]
public class MapSize
{
    public string name;
    public int xSize, ySize;

    public MapSize(string name, int xSize, int ySize)
    {
        this.name = name;
        this.xSize = xSize;
        this.ySize = ySize;
    }
}
