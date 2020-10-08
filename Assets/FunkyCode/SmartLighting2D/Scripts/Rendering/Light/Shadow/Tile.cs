using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light.Shadow {

    public class Tile : Base
    {
        static public void Draw(LightingBuffer2D buffer, LightingTile tile, Vector2 position, LightingTilemapCollider2D tilemap, float lightSizeSquared, float z) {
            polygons = tile.GetShapePolygons();

            if (polygons.Count < 1) {
                return;
            }

            Rendering.Light.Shadow.Polygon.Draw(buffer, polygons, lightSizeSquared, z, position, tilemap.transform.lossyScale);
        }  
    }
}