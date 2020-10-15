using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light.Shadow {

    public class TilemapCollider {

        public class Rectangle {
            static public void Draw(LightingBuffer2D buffer, LightingTilemapCollider2D id, float lightSizeSquared, float z) {
                Vector2 position = -buffer.lightSource.transform.position + id.transform.position;

                switch(id.rectangle.colliderType) {
                    case LightingTilemapCollider.Rectangle.ColliderType.CompositeCollider:
                        Shadow.Main.Draw(buffer, id.rectangle.compositeColliders, lightSizeSquared, z, position, Vector2.one, 0);
                    break;
                }
            }
        }
    }
}