using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Light.Shadow {

    public class Shape {

        public static void Draw(LightingBuffer2D buffer, LightingCollider2D id) {
            if (id.InLightSource(buffer) == false) {
                return;
            }

            foreach(LightingColliderShape shape in id.shapes) {
                List<Polygon2D> polygons = shape.GetPolygonsWorld();

                if (polygons.Count < 1) {
                    return;
                }
                
                ShadowEngine.Draw(buffer, polygons, Vector2.one, shape.shadowDistance);
            }
        }
    }
}