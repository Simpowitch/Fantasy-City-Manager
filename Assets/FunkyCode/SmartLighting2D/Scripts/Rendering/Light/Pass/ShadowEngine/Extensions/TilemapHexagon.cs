using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light.Shadow {

    public class TilemapHexagon {
        
        static public void Draw(LightingBuffer2D buffer, LightingTilemapCollider2D id) {
            if (id.mapType != LightingTilemapCollider2D.MapType.UnityEngineTilemapHexagon) {
                return;
            }

            if (id.hexagon.colliderType == LightingTilemapCollider.Hexagon.ColliderType.None) {
                return;
            }

            Vector2 lightPosition = -buffer.lightSource.transform.position;
            Vector2 scale = Hexagon.GetScale(id);

            foreach(LightingTile tile in id.hexagon.mapTiles) {
                List<Polygon2D> polygons = tile.GetPolygons(id);

                if (polygons == null || polygons.Count < 1) {
                    continue;
                }

                Vector2 tilePosition = Hexagon.GetTilePosition(tile, id);

                ShadowEngine.objectOffset = tilePosition;
            
                tilePosition += lightPosition;

                if (Vector2.Distance(Vector2.zero, tilePosition) > buffer.lightSource.size * 1.5f) {
					continue;
				}

                ShadowEngine.Draw(buffer, polygons,scale, 0);

                ShadowEngine.objectOffset = Vector2.zero;
            }
        }
    }
}