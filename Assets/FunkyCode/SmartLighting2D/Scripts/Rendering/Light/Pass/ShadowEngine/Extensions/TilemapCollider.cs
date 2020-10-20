using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light.Shadow {

    public class TilemapCollider {

        public class Rectangle {
            static public void Draw(LightingBuffer2D buffer, LightingTilemapCollider2D id) {
                Vector2 position = -buffer.lightSource.transform.position;

                switch(id.rectangle.colliderType) {
                    case LightingTilemapCollider.Rectangle.ColliderType.CompositeCollider:
                        ShadowEngine.objectOffset = id.transform.position;

                        ShadowEngine.Draw(buffer, id.rectangle.compositeColliders, Vector2.one, 0);

                        ShadowEngine.objectOffset = Vector2.zero;
                    break;
                }
            }
        }
    }
}