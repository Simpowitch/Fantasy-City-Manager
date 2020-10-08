using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light.Shadow {

    public class TilemapIsometric : Base {
        
        static public void Draw(LightingBuffer2D buffer, LightingTilemapCollider2D id, float lightSizeSquared, float z) {
            if (id.mapType != LightingTilemapCollider2D.MapType.UnityEngineTilemapIsometric) {
                return;
            }

            if (id.colliderType == LightingTilemapCollider2D.ColliderType.None) {
                return;
            }

            if (id.colliderType == LightingTilemapCollider2D.ColliderType.Collider) {
                return;
            }
            
            if (id.isometricMap == null) {
                return;
            }

            Vector2 offset = -buffer.lightSource.transform.position;
            Vector2 polyOffset;

            foreach(LightingTilemapCollider2D.IsometricTile tile in id.isometricMap.mapTiles) {
                polygons = tile.tile.GetPolygons(id);

                if (polygons == null || polygons.Count < 1) {
                    continue;
                }

                Vector2 tilePosition = Vector2.zero;

                tilePosition.x += tile.position.x * 0.5f;
                tilePosition.y += tile.position.x * 0.5f * id.properties.cellSize.y;

                tilePosition.x += tile.position.y * -0.5f;
                tilePosition.y += tile.position.y * 0.5f * id.properties.cellSize.y;

                tilePosition.x *= id.properties.cellSize.x;

                polyOffset.x = offset.x + tilePosition.x;
                polyOffset.y = offset.y + tilePosition.y;

                if (id.colliderType == LightingTilemapCollider2D.ColliderType.SpriteCustomPhysicsShape) {
                    polyOffset.y += 0.25f;
                }

                Rendering.Light.Shadow.Polygon.Draw(buffer, polygons, lightSizeSquared, z, polyOffset, Vector3.one);
            }
        }
    }
}