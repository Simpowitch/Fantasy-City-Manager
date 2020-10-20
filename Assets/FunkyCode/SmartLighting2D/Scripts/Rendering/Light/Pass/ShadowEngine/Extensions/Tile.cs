using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light.Shadow {

    public class Tile {
        
        static public void Draw(LightingBuffer2D buffer, LightingTile tile, Vector2 position, LightingTilemapCollider2D tilemap) {
            List<Polygon2D> polygons = tile.GetShapePolygons();

            if (polygons.Count < 1) {
                return;
            }

            ShadowEngine.objectOffset = position;

            ShadowEngine.Draw(buffer, polygons, tilemap.transform.lossyScale, 0);

            ShadowEngine.objectOffset = Vector2.zero;
        }  
    }
}