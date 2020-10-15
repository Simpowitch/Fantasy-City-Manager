using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light.Shadow {

    public class TilemapIsometric : Base {
        
        static public void Draw(LightingBuffer2D buffer, LightingTilemapCollider2D id, float lightSizeSquared, float z) {
            if (id.mapType != LightingTilemapCollider2D.MapType.UnityEngineTilemapIsometric) {
                return;
            }

            if (id.isometric.colliderType == LightingTilemapCollider.Isometric.ColliderType.None) {
                return;
            }

            Vector2 lightPosition = -buffer.lightSource.transform.position;
            Vector2 scale = Isometric.GetScale(id);

            foreach(LightingTile tile in id.isometric.mapTiles) {
                List<Polygon2D> polygons = tile.GetPolygons(id);

                if (polygons == null || polygons.Count < 1) {
                    continue;
                }

                Vector2 tilePosition = Isometric.GetTilePosition(tile, id);	

                if (id.isometric.colliderType == LightingTilemapCollider.Isometric.ColliderType.Grid) {
					tilePosition.y -= 0.25f;
				}
            
                tilePosition += lightPosition;

                if (Vector2.Distance(Vector2.zero, tilePosition) > buffer.lightSource.size * 1.5f) {
					continue;
				}

                Shadow.Main.Draw(buffer, polygons, lightSizeSquared, z, tilePosition, scale, 0);
            }
        }
    }
}