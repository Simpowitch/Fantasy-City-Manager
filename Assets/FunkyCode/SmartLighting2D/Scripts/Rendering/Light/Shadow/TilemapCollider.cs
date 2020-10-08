using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light.Shadow {

    public class TilemapCollider : Base {

        static public void Draw(LightingBuffer2D buffer, LightingTilemapCollider2D id, float lightSizeSquared, float z) {
            if (id.colliderType != LightingTilemapCollider2D.ColliderType.Collider) {
                return;
            }

            Vector2 position = -buffer.lightSource.transform.position;

            DrawEdge(buffer, id,  position, lightSizeSquared, z);
            DrawPolygon(buffer, id, position, lightSizeSquared, z);
        }

        static public void DrawPolygon(LightingBuffer2D buffer, LightingTilemapCollider2D id, Vector2 position, float lightSizeSquared, float z) {
            Polygon.Draw(buffer, id.polygonColliders, lightSizeSquared, z, position, Vector2.one);
        }

        static public void DrawEdge(LightingBuffer2D buffer, LightingTilemapCollider2D id, Vector2 position, float lightSizeSquared, float z) {
            Polygon.Draw(buffer, id.edgeColliders, lightSizeSquared, z, position, Vector2.one);
        }
    }
}