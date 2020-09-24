using UnityEngine;

public class CanvasGrid : MonoBehaviour
{
    public RectTransform worldCanvasRect;
    public Transform tileParent;
    public CanvasTile tilePrefab;

    CanvasTile[,] canvasTiles;
    public void Setup(Grid<ObjectTile> objectGrid, Grid<PathNode> pathGrid)
    {
        if (canvasTiles != null)
        {
            foreach (var tile in canvasTiles)
            {
                Destroy(tile.gameObject);
            }
        }

        worldCanvasRect.sizeDelta = new Vector2(objectGrid.Width, objectGrid.Height);
        worldCanvasRect.position = new Vector3((float)objectGrid.Width / 2 * objectGrid.cellSize, (float)objectGrid.Height / 2 * objectGrid.cellSize);

        canvasTiles = new CanvasTile[objectGrid.Width, objectGrid.Height];

        for (int y = 0; y < canvasTiles.GetLength(1); y++)
        {
            for (int x = 0; x < canvasTiles.GetLength(0); x++)
            {
                CanvasTile canvasTile = Instantiate(tilePrefab, tileParent);
                canvasTiles[x, y] = canvasTile;
                ObjectTile objectTile = objectGrid.GetGridObject(x, y);
                objectTile.CanvasTileObject = canvasTile;
                PathNode pathNode = pathGrid.GetGridObject(x, y);
                pathNode.CanvasTileObject = canvasTile;
            }
        }
    }

    public void SetGridOutlines(bool status)
    {
        foreach (var tile in canvasTiles)
        {
            tile.SetGridOutline(status);
        }
    }
}
