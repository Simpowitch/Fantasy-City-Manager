using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light.Shadow {

    public class Tile : Base
    {
        static public void Draw(LightingBuffer2D buffer, LightingTile tile, Vector2 position, LightingTilemapCollider2D tilemap, float lightSizeSquared, float z) {
            List<Polygon2D> polygons = tile.GetShapePolygons();

            if (polygons.Count < 1) {
                return;
            }

            Shadow.Main.Draw(buffer, polygons, lightSizeSquared, z, position, tilemap.transform.lossyScale, 0);
        }  
    }
}